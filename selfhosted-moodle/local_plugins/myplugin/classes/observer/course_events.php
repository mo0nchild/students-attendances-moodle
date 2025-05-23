<?php
namespace local_myplugin\observer;

defined('MOODLE_INTERNAL') || die();

class course_events {

    public static function course_created(\core\event\course_created $event) {
        global $DB;

        $courseid = $event->objectid;
        $course = $DB->get_record('course', ['id' => $courseid]);

        $message_data = [
            'record_id' => $courseid,
            'course_id' => $event->courseid,
            'fullname' => $course->fullname,
            'shortname' => $course->shortname,
        ];
        
        $event_info = new \local_myplugin\local\models\event_info('course_created', $courseid);
        \local_myplugin\local\event_handler::handle($message_data, $event_info);
error_log(print_r($event, true));
    }

    public static function course_updated(\core\event\course_updated $event) {
        global $DB;

        $courseid = $event->objectid;
        $course = $DB->get_record('course', ['id' => $courseid]);

        $message_data = [
            'record_id' => $courseid,
            'course_id' => $event->courseid,
            'fullname' => $course->fullname,
            'shortname' => $course->shortname,
        ];
        
        $event_info = new \local_myplugin\local\models\event_info('course_updated', $courseid);
        \local_myplugin\local\event_handler::handle($message_data, $event_info);
error_log(print_r($event, true));
    }

    public static function course_deleted(\core\event\course_deleted $event) {
        global $DB;

        $courseid = $event->objectid;
        $message_data = [
            'record_id' => $courseid, 
            'course_id' => $event->courseid,
        ];
        
        $event_info = new \local_myplugin\local\models\event_info('course_deleted', $courseid);
        \local_myplugin\local\event_handler::handle($message_data, $event_info);
error_log(print_r($event, true));
    }

    public static function user_enrolment_created(\core\event\user_enrolment_created $event) {
        global $DB;
        
        $enrolmentid = $event->objectid;
        $enrolment = $DB->get_record('user_enrolments', ['id' => $enrolmentid]);

        $userid = $event->relateduserid;
        $courseid = $event->courseid;

        $message_data = [
            'record_id' => $enrolmentid,
            'user_id' => $userid,
            'course_id' => $courseid,
        ];
        
        $event_info = new \local_myplugin\local\models\event_info('user_enrolment_created', $enrolmentid);
        \local_myplugin\local\event_handler::handle($message_data, $event_info);
error_log(print_r($event, true));
    }

    public static function user_enrolment_updated(\core\event\user_enrolment_updated $event) {
        global $DB;
        
        $enrolmentid = $event->objectid;
        $enrolment = $DB->get_record('user_enrolments', ['id' => $enrolmentid]);

        $userid = $event->relateduserid;
        $courseid = $event->courseid;

        $message_data = [
            'record_id' => $enrolmentid,
            'user_id' => $userid,
            'course_id' => $courseid,
        ];
        
        $event_info = new \local_myplugin\local\models\event_info('user_enrolment_updated', $enrolmentid);
        \local_myplugin\local\event_handler::handle($message_data, $event_info);
error_log(print_r($event, true));
    }

    public static function user_enrolment_deleted(\core\event\user_enrolment_deleted $event) {
        global $DB;
        
        $enrolmentid = $event->objectid;
        $message_data = [
            'record_id' => $enrolmentid,
            'user_id' => $event->relateduserid,
            'course_id' => $event->courseid,
        ]; 
        $event_info = new \local_myplugin\local\models\event_info('user_enrolment_deleted', $enrolmentid);
        \local_myplugin\local\event_handler::handle($message_data, $event_info);
error_log(print_r($event, true));
    }

    public static function user_role_assigned(\core\event\role_assigned $event) {
        global $DB;
        
        $roleid = $event->objectid;
        $role = $DB->get_record('role', ['id' => $roleid]);
        $message_data = [
            'record_id' => $roleid,
            'user_id' => $event->relateduserid,
            'course_id' => $event->courseid,
            'role' => $role->archetype,
            'description' => $role->description,
            'shortname' => $role->shortname,
        ]; 
        $event_info = new \local_myplugin\local\models\event_info('user_role_assigned', $roleid);
        \local_myplugin\local\event_handler::handle($message_data, $event_info);
error_log(print_r($event, true));
    }

    public static function user_role_unassigned(\core\event\role_unassigned $event) {
        global $DB;
        
        $roleid = $event->objectid;
        $role = $DB->get_record('role', ['id' => $roleid]);
        $message_data = [
            'record_id' => $roleid,
            'user_id' => $event->relateduserid,
            'course_id' => $event->courseid,
            'role' => $role->archetype
        ]; 
        $event_info = new \local_myplugin\local\models\event_info('user_role_unassigned', $roleid);
        \local_myplugin\local\event_handler::handle($message_data, $event_info);
error_log(print_r($event, true));
    }
}