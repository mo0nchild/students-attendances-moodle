<?php
namespace local_myplugin\observer;

defined('MOODLE_INTERNAL') || die();

class cohort_events {
    
    public static function cohort_created(\core\event\cohort_created $event) {
        global $DB;
        
        $cohortid = $event->objectid;
        $cohort = $DB->get_record('cohort', ['id' => $cohortid]);

        $message_data = [
            'record_id' => $cohortid,
            'cohort_name' => $cohort->name,
            'description' => $cohort->description,
        ];
        $event_info = new \local_myplugin\local\models\event_info('cohort_created', $cohortid);
        \local_myplugin\local\event_handler::handle($message_data, $event_info);
error_log(print_r($event, true));
    }

    public static function cohort_updated(\core\event\cohort_updated $event) {
        global $DB;

        $cohortid = $event->objectid;
        $cohort = $DB->get_record('cohort', ['id' => $cohortid]);

        $message_data = [
            'record_id' => $cohortid,
            'cohort_name' => $cohort->name,
            'description' => $cohort->description,
        ];
        $event_info = new \local_myplugin\local\models\event_info('cohort_updated', $cohortid);
        \local_myplugin\local\event_handler::handle($message_data, $event_info);
error_log(print_r($event, true));
    }

    public static function cohort_deleted(\core\event\cohort_deleted $event) {
        global $DB;

        $cohortid = $event->objectid;
        $message_data = ['record_id' => $cohortid];

        $event_info = new \local_myplugin\local\models\event_info('cohort_deleted', $cohortid);
        \local_myplugin\local\event_handler::handle($message_data, $event_info);
error_log(print_r($event, true));
    }

    public static function cohort_member_added(\core\event\cohort_member_added $event) {
        global $DB;
        
        $cohortid = $event->objectid;
        $message_data = [
            'record_id' => $cohortid,
            'user_id' => $event->relateduserid,
        ];
        $event_info = new \local_myplugin\local\models\event_info('cohort_member_added', $cohortid);
        \local_myplugin\local\event_handler::handle($message_data, $event_info);
error_log(print_r($event, true));
    }

    public static function cohort_member_removed(\core\event\cohort_member_removed $event) {
        global $DB;

        $cohortid = $event->objectid;
        $message_data = [
            'record_id' => $cohortid,
            'user_id' => $event->relateduserid,
        ];
        $event_info = new \local_myplugin\local\models\event_info('cohort_member_removed', $cohortid);
        \local_myplugin\local\event_handler::handle($message_data, $event_info);
error_log(print_r($event, true));
    }
}