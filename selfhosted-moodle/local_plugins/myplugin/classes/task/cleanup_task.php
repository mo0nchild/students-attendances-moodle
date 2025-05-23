<?php
namespace local_myplugin\task;

defined('MOODLE_INTERNAL') || die();

class cleanup_task extends \core\task\scheduled_task {
    /** @var string Название таблицы для очистки */
    protected static $messages_table = \local_myplugin\local\constants::MESSAGES_TABLE;

    /** @var int Количество секунд для хранения сообщений (1 день) */
    protected static $retention_period = 24 * 3600;

    public function get_name() {
        return get_string('cleanup_task', 'local_myplugin');
    }
    
    public function execute() {
        global $DB;
        try {
            // Проверка существования таблицы (опционально)
            if (!$DB->get_manager()->table_exists(self::$messages_table)) {
                throw new \moodle_exception('table_not_found', 'local_myplugin', '', self::$messages_table);
            }

            $time_threshold = time() - self::$retention_period;
            $count = $DB->count_records_select(self::$messages_table, 'time_created < ?', [$time_threshold]);

            if ($count <= 0) {
                \local_myplugin\local\logging\service::log_to_file(
                    "[Cleanup Task] No old records found in " . self::$messages_table
                );
                return;
            }

            // Удаление записей
            $DB->delete_records_select(self::$messages_table, 'time_created < ?', [$time_threshold]);

            // Логгирование
            $log_message = sprintf(
                "[Cleanup Task] Deleted %d old records from %s (older than %s)",
                $count,
                self::$messages_table,
                userdate($time_threshold)
            );
            \local_myplugin\local\logging\service::log_to_file($log_message);

        } catch (\Exception $e) {
            \local_myplugin\local\logging\service::log_to_file(
                "[Cleanup Task ERROR] " . $e->getMessage()
            );
            throw $e;
        }
    }
}