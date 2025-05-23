<?php
defined('MOODLE_INTERNAL') || die();

$tasks = [
    [
        'classname' => 'local_myplugin\task\cleanup_task',
        'blocking' => 0,
        'minute' => '0',
        'hour' => '*',      // Каждый час
        'day' => '*',       // Каждый день
        'dayofweek' => '*', // Каждый день недели
        'month' => '*',     // Каждый месяц
    ]
];