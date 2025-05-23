<?php
namespace local_myplugin\observer;

defined('MOODLE_INTERNAL') || die();

class course_group_events {

    public static function group_created(\core\event\group_created $event) {
        global $DB;

        $groupid = $event->objectid;
        $group = $DB->get_record('groups', ['id' => $groupid]);

        $message_data = [
            'record_id' => $groupid,
            'name' => $group->name,
            'course_id' => $event->courseid,
            'description' => $group->description, 
        ];
        $event_info = new \local_myplugin\local\models\event_info('group_created', $groupid);
        \local_myplugin\local\event_handler::handle($message_data, $event_info);
error_log(print_r($event, true));
    }

    public static function group_deleted(\core\event\group_deleted $event) {
        global $DB;

        $groupid = $event->objectid;
        $message_data = [
            'record_id' => $groupid, 
            'course_id' => $event->courseid,
        ];

        $event_info = new \local_myplugin\local\models\event_info('group_deleted', $groupid);
        \local_myplugin\local\event_handler::handle($message_data, $event_info);
error_log(print_r($event, true));
    }

    public static function group_updated(\core\event\group_updated $event) {
        global $DB;

        $groupid = $event->objectid;
        $group = $DB->get_record('groups', ['id' => $groupid]);

        $message_data = [
            'record_id' => $groupid,
            'name' => $group->name,
            'course_id' => $event->courseid,
            'description' => $group->description, 
        ];
        $event_info = new \local_myplugin\local\models\event_info('group_updated', $groupid);
        \local_myplugin\local\event_handler::handle($message_data, $event_info);
error_log(print_r($event, true));
    }

    public static function group_member_added(\core\event\group_member_added $event) {
        global $DB;

        $groupid = $event->objectid;
        $message_data = [
            'record_id' => $groupid,
            'user_id' => $event->relateduserid,
            'course_id' => $event->courseid,
        ];
        $event_info = new \local_myplugin\local\models\event_info('group_member_added', $groupid);
        \local_myplugin\local\event_handler::handle($message_data, $event_info);
error_log(print_r($event, true));
    }

    public static function group_member_removed(\core\event\group_member_removed $event) {
        global $DB;

        $groupid = $event->objectid;
        $message_data = [
            'record_id' => $groupid,
            'user_id' => $event->relateduserid,
            'course_id' => $event->courseid,
        ];
        $event_info = new \local_myplugin\local\models\event_info('group_member_removed', $groupid);
        \local_myplugin\local\event_handler::handle($message_data, $event_info);
error_log(print_r($event, true));
    }

}