<?php
namespace local_myplugin\observer;

defined('MOODLE_INTERNAL') || die();

class course_module_events {

    public static function module_created(\core\event\course_module_created $event) {
        global $DB;
        if (!isset($event->other['modulename']) || $event->other['modulename'] != 'attendance') {
            return;
        }

        $modulename = $event->other['modulename'];
        $attandanceid = $event->other['instanceid'];
        $attendancename = $event->other['name'];
        
        $message_data = [
            'record_id' => $attandanceid,
            'course_id' => $event->courseid,
            'attendance_name' => $attendancename,
        ];

        $event_info = new \local_myplugin\local\models\event_info('module_created', $attandanceid);
        \local_myplugin\local\event_handler::handle($message_data, $event_info);
error_log(print_r($event, true));
    }

    public static function module_deleted(\core\event\course_module_deleted $event) {
        global $DB;

        if (!isset($event->other['modulename']) || $event->other['modulename'] != 'attendance') {
            return;
        }
        $modulename = $event->other['modulename'];
        $attandanceid = $event->other['instanceid'];
        
        $message_data = [
            'record_id' => $attandanceid,
            'course_id' => $event->courseid,
        ];

        $event_info = new \local_myplugin\local\models\event_info('module_deleted', $attandanceid);
        \local_myplugin\local\event_handler::handle($message_data, $event_info);
error_log(print_r($event, true));
    }
/**
     * (
            [eventname] => \core\event\course_module_updated
            [component] => core
            [action] => updated
            [target] => course_module
            [objecttable] => course_modules
            [objectid] => 36
            [crud] => u
            [edulevel] => 1
            [contextid] => 108
            [contextlevel] => 70
            [contextinstanceid] => 36
            [userid] => 2
            [courseid] => 22
            [relateduserid] =>
            [anonymous] => 0
            [other] => Array
                (
                    [modulename] => attendance
                    [instanceid] => 1
                    [name] => Посещаемость
                )

            [timecreated] => 1746662353
        )
     */
    public static function module_updated(\core\event\course_module_updated $event) {
        global $DB;
        if (!isset($event->other['modulename']) || $event->other['modulename'] != 'attendance') {
            return;
        }

        $modulename = $event->other['modulename'];
        $attandanceid = $event->other['instanceid'];
        $attendancename = $event->other['name'];
        
        $message_data = [
            'record_id' => $attandanceid,
            'course_id' => $event->courseid,
            'attendance_name' => $attendancename,
        ];

        $event_info = new \local_myplugin\local\models\event_info('module_updated', $attandanceid);
        \local_myplugin\local\event_handler::handle($message_data, $event_info);
error_log(print_r($event, true));
    }

}