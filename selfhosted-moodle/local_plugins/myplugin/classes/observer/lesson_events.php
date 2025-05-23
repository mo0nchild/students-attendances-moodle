<?php
namespace local_myplugin\observer;

defined('MOODLE_INTERNAL') || die();

class lesson_events {
    
    /**
     * (
            [eventname] => \mod_attendance\event\attendance_taken
            [component] => mod_attendance
            [action] => taken
            [target] => attendance
            [objecttable] => attendance_log
            [objectid] => 13
            [crud] => u
            [edulevel] => 1
            [contextid] => 155
            [contextlevel] => 70
            [contextinstanceid] => 65
            [userid] => 2
            [courseid] => 38
            [relateduserid] =>
            [anonymous] => 0
            [other] => Array
                (
                    [sessionid] => 13
                    [grouptype] => 0
                )

            [timecreated] => 1747234228
        )
     */
    public static function attendance_taken(\mod_attendance\event\attendance_taken $event) {
        global $DB;

        $lessonid = $event->other['sessionid'];
        $lesson = $DB->get_record('attendance_sessions', ['id' => $lessonid]);

        $message_data = [
            'record_id' => $lessonid,
            'course_id' => $event->courseid,
            'teacher_id' => $event->userid,
            'datetime' => date('Y-m-d H:i:s', $lesson->sessdate),
            'description' => $lesson->description,
        ];
        $event_info = new \local_myplugin\local\models\event_info('attendance_taken', $lessonid);
        \local_myplugin\local\event_handler::handle($message_data, $event_info);
error_log(print_r($event, true));
    }

    public static function lesson_added(\mod_attendance\event\session_added $event) {
        global $DB;
        $attendanceid = $event->objectid;

        $timeThreshold = time() - 10; // Сессии, добавленные за последние 10 сек
        $lesson = $DB->get_record_sql(
            'SELECT * FROM {attendance_sessions} WHERE attendanceid = ? AND timemodified >= ? 
            ORDER BY id DESC LIMIT 1',
            [$attendanceid, $timeThreshold]
        );
        $message_data = [
            'record_id' => $lesson->id,
            'datetime' => date('Y-m-d H:i:s', $lesson->sessdate),
            'description' => $lesson->description,
            'course_id' => $event->courseid, 
            'group_id' => $lesson->groupid,
            'attendance_id' => $attendanceid,
        ];
        $event_info = new \local_myplugin\local\models\event_info('lesson_added', $lesson->id);
        \local_myplugin\local\event_handler::handle($message_data, $event_info);
        
error_log(print_r($event, true));
// error_log(print_r($_SERVER, true));
// error_log(print_r(WS_SERVER, true));
    }   

    public static function lesson_deleted(\mod_attendance\event\session_deleted $event) {
        global $DB;

        $attendanceid = $event->objectid;
        $lessonid = intval($event->other['info']);

        $message_data = [
            'record_id' => $lessonid, 
            'course_id' => $event->courseid,
        ];
        $event_info = new \local_myplugin\local\models\event_info('lesson_deleted', $lessonid);
        \local_myplugin\local\event_handler::handle($message_data, $event_info);
error_log(print_r($event, true));
    }

    public static function lesson_updated(\mod_attendance\event\session_updated $event) {
        global $DB;

        $attendanceid = $event->objectid;
        $lessonid = intval($event->other['sessionid']);
        $lesson = $DB->get_record('attendance_sessions', ['id' => $lessonid]);
        $message_data = [
            'record_id' => $lessonid,
            'datetime' => date('Y-m-d H:i:s', $lesson->sessdate),
            'description' => $lesson->description,
            'course_id' => $event->courseid, 
            'group_id' => $lesson->groupid,
            'attendance_id' => $attendanceid,
        ];
        $event_info = new \local_myplugin\local\models\event_info('lesson_updated', $lessonid);
        \local_myplugin\local\event_handler::handle($message_data, $event_info);
error_log(print_r($event, true));
    }

}