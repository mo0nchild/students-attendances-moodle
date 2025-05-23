<?php
defined('MOODLE_INTERNAL') || die();

$functions = [
    'local_myplugin_get_user_info' => [
        'classname'   => 'local_myplugin\external\get_user_info',
        'methodname'  => 'execute',
        'classpath'   => 'local/myplugin/classes/external/get_user_info.php',
        'description' => 'Получает данные пользователя',
        'type'        => 'read',
        'ajax'        => true,
        'loginrequired' => false, 
        'capabilities' => ''
    ],
    'local_myplugin_get_teacher_info' => [
        'classname'   => 'local_myplugin\external\get_teacher_info',
        'methodname'  => 'execute',
        'classpath'   => 'local/myplugin/classes/external/get_teacher_info.php',
        'description' => 'Получает данные преподавателя (только для учителей)',
        'type'        => 'read',
        'ajax'        => true,
        'loginrequired' => false, 
        'capabilities' => ''
    ],
    'local_myplugin_get_current_time' => [
        'classname'   => 'local_myplugin\external\get_current_time',
        'methodname'  => 'execute',
        'classpath'   => 'local/myplugin/classes/external/get_current_time.php',
        'description' => 'Получает текущее время сервера',
        'type'        => 'read',
        'ajax'        => true,
        'loginrequired' => false, 
        'capabilities' => ''
    ]
];
$services = [
    'MyPlugin Service' => [
        'functions'       => [
            'local_myplugin_get_user_info',
            'local_myplugin_get_teacher_info',
            'local_myplugin_get_current_time'
        ],
        'restrictedusers' => 0,
        'enabled'         => 1,
        'shortname'       => 'local_myplugin_service'
    ]
];