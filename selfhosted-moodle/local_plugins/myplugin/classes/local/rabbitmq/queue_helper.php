<?php
namespace local_myplugin\local\rabbitmq;

defined('MOODLE_INTERNAL') || die();

class queue_helper {
    const RETRY_DELAYS = [10, 30, 40];
    const MAINTENANCE_THRESHOLD = null; // null = использовать длину RETRY_DELAYS

    /** @var string Название таблицы с сообщениями */
    protected static $messages_table = \local_myplugin\local\constants::MESSAGES_TABLE;

    /** @var string Название класса задачи повторной обработки */
    protected static $retrytask_classname = '\\local_myplugin\\task\\retry_task';
    
    protected static function get_max_attempts(): int {
        return count(self::RETRY_DELAYS);
    }
    /**
     * Планирует повторную попытку отправки
     */
    public static function schedule_retry(int $message_id, int $attempt = 1): void {

        if (self::is_retry_limit_reached($attempt)) {
            self::enable_maintenance_mode($attempt);
            return;
        }

        try {
            $task = new \local_myplugin\task\retry_task();
            $task->set_custom_data([
                'message_id' => $message_id,
                'attempt' => $attempt
            ]);

            $task->set_next_run_time(time() + self::get_delay_for_attempt($attempt));      
            \core\task\manager::queue_adhoc_task($task, true); // Второй параметр для избежания дубликатов
            
        } catch (\Exception $e) {
            error_log("Failed to schedule retry for message {$message_id}: " . $e->getMessage());
            throw new \moodle_exception('retryfailed', 'local_myplugin', '', null, $e->getMessage());
        }
    }

    // Берем задержку из массива или последний элемент, если вышли за границы
    protected static function get_delay_for_attempt(int $attempt): int {
        return self::RETRY_DELAYS[$attempt - 1] ?? end(self::RETRY_DELAYS);
    }

    /**
     * Активирует режим техобслуживания после исчерпания попыток
     */
    protected static function enable_maintenance_mode(int $failed_attempts): void {
        global $CFG, $DB;
        try {
            // Включаем режим техобслуживания
            set_config('maintenance_enabled', 1);
            set_config('maintenance_message', 
                get_string('maintenance_message', 'local_myplugin', userdate(time()))
            );
    
            $tasks_cancelled = self::cleanup_task_queue();
    
            error_log(sprintf(
                "RabbitMQ: Activated maintenance mode after %d failed attempts. Cancelled %d adhoc tasks.",
                $failed_attempts,
                $tasks_cancelled
            ));
        } catch (Exception $e) {
            error_log("Critical error enabling maintenance mode: " . $e->getMessage());
            throw $e;
        }
    }
    /**
     * Очищает очередь задач
     */
    protected static function cleanup_task_queue(): int {

        global $DB, $CFG;

        // Проверка существования таблицы
        if (!$DB->get_manager()->table_exists(self::$messages_table)) {
            error_log("Таблица с сообщениями " . self::$messages_table . " не найдена");
            throw new \moodle_exception('table_not_found', 'local_myplugin', '', self::$messages_table);
        }
        // Останавливаем все adhoc-задачи с обработкой ошибок
        try {
            $tasks = \core\task\manager::get_adhoc_tasks(self::$retrytask_classname);
            $processed = 0;

            foreach ($tasks as $task) {
                error_log("Processing task ID: " . $task->get_id());
                try {
                    $custom_data = $task->get_custom_data();
                    if (empty($custom_data->message_id)) {
                        error_log("Skipping task - no message_id");
                        continue;
                    }
                    $m_id = $custom_data->message_id;
                    $message = $DB->get_record(self::$messages_table, ['id' => $m_id], '*', MUST_EXIST);

                    if (!$message) {
                        error_log("Message not found, skipping");
                        continue;
                    }
                    $table_name = $CFG->prefix . self::$messages_table;  
                    $result = $DB->execute("UPDATE {$table_name} SET status = :status, time_modified = :now WHERE id = :id", [
                        'status' => \local_myplugin\local\constants::MESSAGE_FAILED,
                        'now' => time(),
                        'id' => $m_id
                    ]);              
                    $DB->delete_records('task_adhoc', ['id' => $task->get_id()]);
                    
                    $upd_message = $DB->get_record(self::$messages_table, ['id' => $m_id]);
                    error_log(json_encode($upd_message));
                    $processed++;
                    
                } catch (\Exception $e) {
                    error_log("Error processing task ID {$task->get_id()}: " . $e->getMessage());
                    continue; 
                }
            }
            return $processed;
        } catch (\Exception $e) {

            error_log("Error cleaning up task queue: " . $e->getMessage());
            return 0;
        }
    }

    /**
     * Проверяет, достигнут ли лимит попыток
     */
    public static function is_retry_limit_reached(int $attempt): bool {
        $threshold = self::MAINTENANCE_THRESHOLD ?? self::get_max_attempts();
        return $attempt > $threshold;
    }
}