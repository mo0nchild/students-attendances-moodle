<?php  // Moodle configuration file

unset($CFG);
global $CFG;
$CFG = new stdClass();

$CFG->dbtype    = 'mariadb';
$CFG->dblibrary = 'native';
$CFG->dbhost    = 'db';
$CFG->dbname    = 'moodle';
$CFG->dbuser    = 'moodle';
$CFG->dbpass    = 'moodle';
$CFG->prefix    = 'mdl_';
$CFG->dboptions = array (
  'dbpersist' => 0,
  'dbport' => '',
  'dbsocket' => '',
  'dbcollation' => 'utf8mb4_general_ci',
);

$CFG->wwwroot   = 'http://localhost';
$CFG->dataroot  = '/var/www/moodledata';
$CFG->admin     = 'admin';
$CFG->directorypermissions = 0777;

$logdir = $CFG->dataroot . '/log';
// Debug-настройки
@error_reporting(E_ALL | E_STRICT);   // NOT FOR PRODUCTION SERVERS!
@ini_set('display_errors', '1');         // NOT FOR PRODUCTION SERVERS!
@ini_set('log_errors', '1');
@ini_set('error_log', $logdir . '/php_errors.log');

$CFG->debug = (E_ALL | E_STRICT);   // === DEBUG_DEVELOPER - NOT FOR PRODUCTION SERVERS!
$CFG->log_errors = true;
//
// You can specify a comma separated list of user ids that that always see
// debug messages, this overrides the debug flag in $CFG->debug and $CFG->debugdisplay
// for these users only.

$CFG->debugdisplay = 0;    // Показывать ошибки на экране (1 - вкл, 0 - выкл)
$CFG->debugusers = '2';     // ID пользователей, которые увидят debug (2 - обычно admin)

$CFG->perfdebug = 0;      // Отключить отладку производительности
$CFG->perfdebug = 0;      // Отключить отладку производительности

// Настройки логирования
$CFG->logging = 16777215;             // Включить логирование всех уровней
$CFG->loglifetime = 0;                // Хранить логи бессрочно

// Создаем поддиректорию для логов (если её нет)
if (!is_dir($logdir)) {
  mkdir($logdir, $CFG->directorypermissions, true);
}

$CFG->forced_plugin_settings['local_myplugin'] = [
  'rabbit_host' => 'rabbitmq', // Хост
  'rabbit_port' => 5672,                  // Порт
  'rabbit_user' => 'admin',         // Логин
  'rabbit_pass' => '1234567890'      // Пароль
];

require_once(__DIR__ . '/lib/setup.php');