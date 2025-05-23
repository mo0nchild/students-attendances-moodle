<?php
namespace local_myplugin\external;

defined('MOODLE_INTERNAL') || die();

require_once($CFG->libdir . '/externallib.php');

use core_external\external_api;
use core_external\external_function_parameters;
use core_external\external_value;
use core_external\external_single_structure;
use core_external\external_multiple_structure;
use \moodle_exception;
use \context_system;

class get_current_time extends external_api {

    public static function execute_parameters() {
        return new external_function_parameters([]);
    }

    public static function execute() {
        $context = context_system::instance();
        self::validate_context($context);

        return [
            'timestamp' => time(),
            'datetime' => date('Y-m-d H:i:s'),
            'timezone' => date_default_timezone_get()
        ];
    }

    public static function execute_returns() {
        return new external_single_structure([
            'timestamp' => new external_value(PARAM_INT, 'Unix timestamp'),
            'datetime' => new external_value(PARAM_TEXT, 'Formatted date time'),
            'timezone' => new external_value(PARAM_TEXT, 'Server timezone'),
        ]);
    }
}