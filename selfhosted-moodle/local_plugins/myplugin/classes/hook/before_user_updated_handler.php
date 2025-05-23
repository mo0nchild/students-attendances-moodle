<?php
namespace local_myplugin\hook;

use core_user\hook\before_user_updated;

defined('MOODLE_INTERNAL') || die();

class before_user_updated_handler {
    
    public static function handle(before_user_updated $hook): void {

        $newuser = $hook->user;         
        $currentuser = $hook->currentuserdata; 
        
        if (isset($newuser->deleted) && $newuser->deleted == 1) {
            return;
        }
        // $message = date('Y-m-d H:i:s') . " - Attempt to updated user ID";
        // \core\notification::info($message);
    }
}