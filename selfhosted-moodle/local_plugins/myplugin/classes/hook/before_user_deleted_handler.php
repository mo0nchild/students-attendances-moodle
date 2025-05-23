<?php
namespace local_myplugin\hook;

use core_user\hook\before_user_deleted;

defined('MOODLE_INTERNAL') || die();

class before_user_deleted_handler {
    public static function handle(before_user_deleted $hook): void {
        // $logfile = sys_get_temp_dir() . '/moodle_user_deletions.log';
        // $message = date('Y-m-d H:i:s') . " - Attempt to delete user ID";
        // file_put_contents($logfile, $message, FILE_APPEND);
        // \core\notification::info($message);
    }
}