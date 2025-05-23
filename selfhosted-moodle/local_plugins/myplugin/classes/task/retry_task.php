<?php
namespace local_myplugin\task;

use local_myplugin\local\constants;

defined('MOODLE_INTERNAL') || die();

class retry_task extends \core\task\adhoc_task {
    
    /** @var string Название таблицы с сообщениями */
    protected static $messages_table;

    /**
     * Получает название таблицы сообщений
     */
    protected static function get_messages_table(): string {
        if (!isset(static::$messages_table)) {
            static::$messages_table = constants::MESSAGES_TABLE;
        }
        return static::$messages_table;
    }

    /**
     * Выполняет повторную попытку отправки сообщения
     */
    public function execute() {
        global $DB;

        $data = $this->get_custom_data();
        if (empty($data->message_id)) {
            error_log("Error: Missing message ID in task data");
            return;
        }

        $message = $this->get_message_from_db($data->message_id);
        if (!$message) { return; }
        
        // Проверяем, не превышено ли максимальное количество попыток
        if (\local_myplugin\local\rabbitmq\queue_helper::is_retry_limit_reached($data->attempt)) {
            $this->handle_max_attempts_reached($message);
            return;
        }
        try { 
            $message->status = ($message->status == constants::MESSAGE_FAILED) 
                ? constants::MESSAGE_FAILED 
                : constants::MESSAGE_PROCESSING;
            error_log("retry_php");
            $message->time_modified = time();
            $DB->update_record(self::get_messages_table(), $message);
            $this->process_message($message); 
        } 
        catch (\Exception $e) {
            $this->handle_retry_failure($message, $data->attempt, $e);
        }
    }
    
    /**
     * Получает сообщение из базы данных
     */
    protected function get_message_from_db(int $message_id): ?\stdClass {
        global $DB;

        $message = $DB->get_record(self::get_messages_table(), ['id' => $message_id]);
        if (!$message) {
            error_log("Error: Message {$message_id} not found in database");
            return null;
        }
        return $message;
    }

    /**
     * Обрабатывает сообщение - отправляет в RabbitMQ
     */
    protected function process_message(\stdClass $message): void {
        global $DB;

        $rabbit_service = new \local_myplugin\local\rabbitmq\service();
        try {
            $event_data = json_decode($message->event_data, true);
            
            if (!is_array($event_data)) {
                throw new \moodle_exception('invalidjsondata', 'local_myplugin');
            }
            $rabbit_service->send_message($event_data);

        } finally { 
            $rabbit_service->disconnect(); 
        }
        
        // Успешная отправка - удаляем сообщение
        $DB->delete_records(self::get_messages_table(), ['id' => $message->id]);
        mtrace("Successfully sent message {$message->id} to RabbitMQ");
    }

    /**
     * Обрабатывает ситуацию, когда достигнуто максимальное количество попыток
     */
    protected function handle_max_attempts_reached(\stdClass $message): void {
        global $DB;

        error_log("Max attempts reached for message {$message->id}. Moving to failed state.");
        
        // Обновляем запись в базе данных
        $message->status = constants::MESSAGE_FAILED;
        $message->time_modified = time();
        $DB->update_record(self::get_messages_table(), $message);
        
        // Добавить логику для перемещения в таблицу неудачных сообщений
    }
    /**
     * Обрабатывает неудачную попытку отправки
     */
    protected function handle_retry_failure(\stdClass $message, int $attempt, \Exception $e): void {
        global $DB;

        error_log("Retry attempt {$attempt} failed for message {$message->id}: " . $e->getMessage());

        \local_myplugin\local\rabbitmq\queue_helper::schedule_retry($message->id, $attempt + 1);
        
        $message->last_attempt = time();
        $message->attempts = $attempt;
        $DB->update_record(self::get_messages_table(), $message);
    }
}