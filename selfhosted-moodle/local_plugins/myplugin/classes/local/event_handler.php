<?php
namespace local_myplugin\local;

use local_myplugin\local\models\event_info;

defined('MOODLE_INTERNAL') || die();

class event_handler {

    /** @var string Название таблицы с сообщениями */
    protected static $messages_table = \local_myplugin\local\constants::MESSAGES_TABLE;

    /**
     * Функция для обработки событий системы Moodle
     */
    public static function handle(array $base_message, event_info $event): void {

        // Добавляем в сообщение информацию о событии
        $message_data = [
            'event_type' => $event->get_event_type(), 
            'timestamp' => time(),
            'payload' => $base_message,
        ];
        self::log_action($message_data, $event);
        // Проверяем, что это не API-вызов (только для веб-интерфейса)
        if (defined('WS_SERVER') && WS_SERVER) { return; }
        try {
            // Пытаемся отправить в RabbitMQ
            $rabbit_service = new \local_myplugin\local\rabbitmq\service();
            $rabbit_service->send_message($message_data);
            
        } catch (\Exception $e) {
            error_log("[Event error] {$event->get_event_type()}: " . $e->getMessage());
            // При ошибке сохраняем в БД и планируем повтор
            self::retry_execute($message_data);
        }
    } 
    /**
     * Функция для запуска повторной обработки сообщений
     */
    public static function retry_execute(array $message_data): void {
        global $DB;

        $record = (object)[
            'event_data' => json_encode($message_data),
            'attempts' => 0,
            'time_created' => time(),
            'time_modified' => time()
        ];
        $message_id = $DB->insert_record(self::$messages_table, $record);
        \local_myplugin\local\rabbitmq\queue_helper::schedule_retry($message_id);
    }
    /**
     * Логирование процесса обработки
     */
    protected static function log_action(array $base_message, event_info $event): void {
        $logmessage = sprintf("[Event handle] %s / id=%d: %s", 
            $event->get_event_type(),
            $event->get_record_id(),
            json_encode($base_message)
        );
        // debugging($logmessage, DEBUG_DEVELOPER);
        \local_myplugin\local\logging\service::log_to_file($logmessage);
    }
}