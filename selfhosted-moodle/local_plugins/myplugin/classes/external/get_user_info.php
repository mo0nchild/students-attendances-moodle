<?php
namespace local_myplugin\external;

defined('MOODLE_INTERNAL') || die();

require_once($CFG->libdir . '/externallib.php');
require_once($CFG->dirroot . '/login/lib.php');

use core_external\external_api;
use core_external\external_function_parameters;
use core_external\external_value;
use core_external\external_single_structure;
use core_external\external_multiple_structure;
use \moodle_exception;

class get_user_info extends external_api {

    public static function execute_parameters() {
        return new external_function_parameters([
            'username' => new external_value(PARAM_USERNAME, 'Логин пользователя'),
            'password' => new external_value(PARAM_RAW, 'Пароль пользователя')
        ]);
    }

    public static function execute(string $username, string $password) {
        global $DB, $CFG;

        // Валидация параметров
        $params = self::validate_parameters(self::execute_parameters(), [
            'username' => $username,
            'password' => $password
        ]);

        // Аутентификация пользователя
        $user = authenticate_user_login($params['username'], $params['password']);
        if (!$user) {
            throw new moodle_exception('invalidlogin');
        }

        // Получаем полную информацию о пользователе
        $userinfo = $DB->get_record('user', ['id' => $user->id], '*', MUST_EXIST);

        // Возвращаем только необходимые данные
        return [
            'id' => $userinfo->id,
            'username' => $userinfo->username,
            'email' => $userinfo->email,
            'firstname' => $userinfo->firstname,
            'lastname' => $userinfo->lastname,
            'city' => $userinfo->city,
            'country' => $userinfo->country
        ];
    }

    public static function execute_returns() {
        return new external_single_structure([
            'id' => new external_value(PARAM_INT, 'ID пользователя'),
            'username' => new external_value(PARAM_RAW, 'Логин'),
            'email' => new external_value(PARAM_EMAIL, 'Email'),
            'firstname' => new external_value(PARAM_TEXT, 'Имя'),
            'lastname' => new external_value(PARAM_TEXT, 'Фамилия'),
            'city' => new external_value(PARAM_TEXT, 'Город'),
            'country' => new external_value(PARAM_TEXT, 'Страна')
        ]);
    }
}