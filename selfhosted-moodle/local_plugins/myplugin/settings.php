<?php
defined('MOODLE_INTERNAL') || die();

if ($hassiteconfig) {
    // Создаем категорию в меню "Плагины"
    if (!$ADMIN->locate('local_myplugin')) {
        $ADMIN->add('root', new admin_category('local_myplugin', get_string('pluginname', 'local_myplugin')));
    }
   // Создаем страницу настроек
    $settings = new admin_settingpage('local_myplugin_settings', 
        get_string('settings', 'local_myplugin'),
        'moodle/site:config');

    // Настройки RabbitMQ
    $settings->add(new admin_setting_configtext(
        'local_myplugin/rabbit_host',
        get_string('rabbit_host', 'local_myplugin'),
        get_string('rabbit_host_desc', 'local_myplugin'),
        'localhost'
    ));

    $settings->add(new admin_setting_configtext(
        'local_myplugin/rabbit_port',
        get_string('rabbit_port', 'local_myplugin'),
        get_string('rabbit_port_desc', 'local_myplugin'),
        '5672',
        PARAM_INT
    ));

    $settings->add(new admin_setting_configtext(
        'local_myplugin/rabbit_user',
        get_string('rabbit_user', 'local_myplugin'),
        get_string('rabbit_user_desc', 'local_myplugin'),
        'guest'
    ));

    $settings->add(new admin_setting_configpasswordunmask(
        'local_myplugin/rabbit_pass',
        get_string('rabbit_pass', 'local_myplugin'),
        get_string('rabbit_pass_desc', 'local_myplugin'),
        'guest'
    ));

    // Настройки обмена
    $settings->add(new admin_setting_configtext(
        'local_myplugin/rabbit_exchange',
        get_string('rabbit_exchange', 'local_myplugin'),
        get_string('rabbit_exchange_desc', 'local_myplugin'),
        'moodle_events'
    ));

    $ADMIN->add('local_myplugin', $settings);
}