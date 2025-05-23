<?php
defined('MOODLE_INTERNAL') || die();

use local_myplugin\local\constants;

function xmldb_local_myplugin_uninstall() {
    global $DB;
    $DB->execute("DROP TABLE IF EXISTS {" . constants::MESSAGES_TABLE . "}");

    return true;
}