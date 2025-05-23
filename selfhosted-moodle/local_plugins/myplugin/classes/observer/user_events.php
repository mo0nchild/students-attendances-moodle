<?php
namespace local_myplugin\observer;

defined('MOODLE_INTERNAL') || die();

class user_events {

    public static function user_updated(\core\event\user_updated $event) {
        global $DB;
    
        $userid = $event->objectid;
        $user = $DB->get_record('user', ['id' => $userid]);

        $message_data = [
            'record_id' => $userid,
            'username' => $user->username,
            'firstname' => $user->firstname,
            'lastname' => $user->lastname,
            'address' => $user->address,
            'city' => $user->city,
            'country' => $user->city
        ];
        $event_info = new \local_myplugin\local\models\event_info('user_updated', $userid);
        \local_myplugin\local\event_handler::handle($message_data, $event_info);
error_log(print_r($event, true));
    }

    public static function user_created(\core\event\user_created $event) {
        global $DB;
        
        $userid = $event->objectid;
        $user = $DB->get_record('user', ['id' => $userid]);

        $message_data = [
            'record_id' => $userid,
            'username' => $user->username,
            'firstname' => $user->firstname,
            'lastname' => $user->lastname,
            'address' => $user->address,
            'city' => $user->city,
            'country' => $user->city
        ];
        
        $event_info = new \local_myplugin\local\models\event_info('user_created', $userid);
        \local_myplugin\local\event_handler::handle($message_data, $event_info);
error_log(print_r($event, true));
    } 

    public static function user_deleted(\core\event\user_deleted $event) {
        global $DB;
        
        $userid = $event->objectid;
        $message_data = [
            'record_id' => $userid,
            'username' => $event->other['username'] ?? '',
            'email' => $event->other['email'] ?? '',
        ];
        $event_info = new \local_myplugin\local\models\event_info('user_deleted', $userid);
        \local_myplugin\local\event_handler::handle($message_data, $event_info);
error_log(print_r($event, true));
    }

    public static function user_password_updated(\core\event\user_password_updated $event) {
        global $DB;
        
        $userid = $event->relateduserid;
        $message_data = [
            'record_id' => $userid,
        ];
        $event_info = new \local_myplugin\local\models\event_info('user_password_updated', $userid);
        \local_myplugin\local\event_handler::handle($message_data, $event_info);
error_log(print_r($event, true));
    }
}