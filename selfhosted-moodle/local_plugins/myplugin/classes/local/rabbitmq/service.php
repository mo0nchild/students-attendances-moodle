<?php
namespace local_myplugin\local\rabbitmq;

require_once(__DIR__.'/../../../vendor/autoload.php');

use PhpAmqpLib\Connection\AMQPStreamConnection;
use PhpAmqpLib\Exchange\AMQPExchangeType;
use PhpAmqpLib\Message\AMQPMessage;
use PhpAmqpLib\Exception\AMQPExceptionInterface;

defined('MOODLE_INTERNAL') || die();

class service {
    protected $config;
    protected $connection;
    protected $channel;
    protected $is_connected = false;
    
    public function __construct() { $this->load_config(); }
    public function __destruct() { $this->disconnect(); }
    
    protected function load_config() {
        $this->config = [
            'hostname'     => get_config('local_myplugin', 'rabbit_host'),
            'port'     => get_config('local_myplugin', 'rabbit_port'),
            'username'     => get_config('local_myplugin', 'rabbit_user'),
            'password'     => get_config('local_myplugin', 'rabbit_pass'),
            'exchange' => get_config('local_myplugin', 'rabbit_exchange')
        ];
    }
    public function is_enabled(): bool {
        return !empty($this->config['hostname']) && !empty($this->config['exchange']);
    }
    /**
     * Получает статистику по соединению
     */
    public function get_connection_stats(): array {
        return [
            'connected' => $this->is_connected,
            'config' => array_diff_key($this->config, ['password' => null]),
            'server_properties' => $this->is_connected ? $this->connection->getServerProperties() : null
        ];
    }
    
    /**
     * Устанавливает соединение с RabbitMQ
     */
    protected function connect(): void {
        if ($this->is_connected) { 
            error_log("RabbitMQ already has connection");
            return; 
        }
        try {
            $this->connection = new AMQPStreamConnection(
                $this->config['hostname'],
                $this->config['port'],
                $this->config['username'],
                $this->config['password']
            );

            $this->channel = $this->connection->channel();
            $this->is_connected = true;

        } catch (AMQPExceptionInterface $e) {
            $this->handle_amqp_exception($e, 'connection');
            throw $e;
        }
    }
    /**
     * Закрывает соединение с RabbitMQ
     */
    public function disconnect(): void {
        try {
            if ($this->channel) {
                $this->channel->close();
                $this->channel = null;
            }
            
            if ($this->connection) {
                $this->connection->close();
                $this->connection = null;
            }
            
            $this->is_connected = false;
        } catch (\Exception $e) {
            error_log("RabbitMQ disconnect error: " . $e->getMessage());
        }
    }
    /**
     * Отправка сообщения в RabbitMQ
     * @param array $data Данные для отправки
     * @throws \moodle_exception При ошибках подключения
     */
     public function send_message(array $data, string $routing_key = ''): void {
        if (!$this->is_enabled()) {
            throw new \moodle_exception('rabbitmq_not_configured', 'local_myplugin');
        }
        try {
            $this->connect();
            // Объявляем exchange если нужно
            $this->channel->exchange_declare($this->config['exchange'], AMQPExchangeType::FANOUT,
                false, // passive
                true,  // durable
                false  // auto_delete
            );
            $message = new AMQPMessage(
                json_encode($data, JSON_THROW_ON_ERROR | JSON_UNESCAPED_UNICODE),
                [
                    'content_type' => 'application/json',
                    'delivery_mode' => AMQPMessage::DELIVERY_MODE_PERSISTENT,
                ]
            );
            
            $this->channel->basic_publish(
                $message,
                $this->config['exchange'],
                $routing_key
            );

        } catch (AMQPExceptionInterface $e) {
            $this->handle_amqp_exception($e, 'publish');
            throw new \moodle_exception('rabbitmq_send_error', 'local_myplugin', '', $e->getMessage());
        }
    }
    /**
     * Обработчик ошибок AMQP
     */
    protected function handle_amqp_exception(AMQPExceptionInterface $e, string $context): void {
        $error_message = sprintf("RabbitMQ %s error: %s (code: %d)", $context,
            $e->getMessage(),
            $e->getCode()
        );
        error_log($error_message);
        $this->disconnect();
    }
    /**
     * Проверка доступности сервера RabbitMQ
     */
    public function is_available(): bool {
        try {
            $this->connect();
            return true;
        } catch (AMQPExceptionInterface $e) {
            return false;
        } finally {
            $this->disconnect();
        }
    }
}