<?php
namespace local_myplugin\local\logging;

defined('MOODLE_INTERNAL') || die();

class service {
    const DEFAULT_LOG_FILENAME = 'local_myplugin.log';
    const MAX_PATH_LENGTH = 1024;

    /**
     * Логирует сообщение в файл с валидацией пути
     */
    public static function log_to_file(string $message, ?string $logfile = null): void {
        $logfile_path = self::resolve_log_path($logfile);  
        try {
            self::ensure_log_directory_exists($logfile_path);
            file_put_contents($logfile_path, self::format_log_entry($message), FILE_APPEND | LOCK_EX);
        } catch (\Exception $e) {
            error_log("Logging failed: {$e->getMessage()}");
            self::fallback_logging($message);
        }
    }

    /**
     * Определяет корректный путь к лог-файлу
     */
    protected static function resolve_log_path(?string $custom_path = null): string {
        global $CFG;
        // Если передан явный путь, используем его
        if ($custom_path !== null) {
            return self::validate_log_path($custom_path);
        }

        // Стандартный путь: moodledata/log/local_myplugin.log
        $default_path = $CFG->dataroot . '/log/' . self::DEFAULT_LOG_FILENAME;
        return self::validate_log_path($default_path);
    }

    /**
     * Валидация и безопасная нормализация пути
     */
    protected static function validate_log_path(string $path): string {
        global $CFG;
        // Проверка длины
        if (strlen($path) > self::MAX_PATH_LENGTH) {
            throw new \invalid_parameter_exception("Log path is too long");
        }
        // Нормализация
        $dir = dirname($path);
        $normalized_dir = realpath($dir) ?: $dir; // Используем realpath или исходный путь
        return rtrim($normalized_dir, '/') . '/' . basename($path);
    }

    /**
     * Создает директорию для логов при необходимости
     */
    protected static function ensure_log_directory_exists(string $path): void {
        global $CFG;

        $dir = dirname($path);
        if (!is_dir($dir)) {
            if (!mkdir($dir, 0777, true)) {
                error_log("Failed to create log directory - " . $dir);
                throw new \moodle_exception("Failed to create log directory - " . $dir);
            }
            // Устанавливаем правильные права
            chmod($dir, 0777);
        }
        // Проверка прав на запись
        if (!is_writable($dir)) {
            error_log("Log directory is not writable - " . $dir); 
            throw new \moodle_exception("Log directory is not writable - " . $dir);
        }
    }

    /**
     * Форматирование записи лога
     */
    protected static function format_log_entry(string $message): string {
        return sprintf("[%s] %s\n", date('Y-m-d H:i:s'), $message);
    }

    /**
     * Резервное логирование при ошибках
     */
    protected static function fallback_logging(string $message): void {
        // 1. Пытаемся в системный лог Moodle
        error_log("FALLBACK LOG: " . $message);
        
        // 2. В крайнем случае - в временный файл
        $tmp_file = sys_get_temp_dir() . '/moodle_fallback.log';
        @file_put_contents($tmp_file, self::format_log_entry($message), FILE_APPEND);
    }
}