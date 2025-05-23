<?php
defined('MOODLE_INTERNAL') || die();

function xmldb_local_myplugin_install() {
    // Установка настроек по умолчанию
    // set_config('rabbit_host', 'localhost', 'local_myplugin');
    // set_config('rabbit_port', 5672, 'local_myplugin');
    
    // Создание начальных данных
    // global $DB;
    // $DB->insert_record('myplugin_messages', [
    //     'payload' => json_encode(['test' => 'initial']),
    //     'attempts' => 0
    // ]);
    
    return true;
}