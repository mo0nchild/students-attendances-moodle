<?php
namespace local_myplugin\external;

defined('MOODLE_INTERNAL') || die();

require_once($CFG->libdir . '/externallib.php');
require_once($CFG->dirroot . '/login/lib.php');
require_once($CFG->dirroot . '/course/lib.php');

use core_external\external_api;
use core_external\external_function_parameters;
use core_external\external_value;
use core_external\external_single_structure;
use core_external\external_multiple_structure;
use \moodle_exception;
use \context_course;

class get_teacher_info extends external_api {
    
    public static function execute_parameters() {
        return new external_function_parameters([
            'username' => new external_value(PARAM_USERNAME, 'Логин учителя'),
            'password' => new external_value(PARAM_RAW, 'Пароль учителя')
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

        // Проверка, является ли пользователь преподавателем
        $isteacher = false;
        $courses = enrol_get_users_courses($user->id, true);
        
        foreach ($courses as $course) {
            $context = context_course::instance($course->id);
            if (has_capability('moodle/course:update', $context, $user)) {
                $isteacher = true;
                break;
            }
        }

        if (!$isteacher) {
            throw new moodle_exception('notteacher', 'local_myplugin');
        }

        // Получаем информацию о пользователе
        $userinfo = $DB->get_record('user', ['id' => $user->id], '*', MUST_EXIST);
        return [
            'id' => $userinfo->id,
            'username' => $userinfo->username,
            'email' => $userinfo->email,
            'firstname' => $userinfo->firstname,
            'lastname' => $userinfo->lastname,
            'teacher' => $isteacher
        ];
    }

    public static function execute_returns() {
        return new external_single_structure([
            'id' => new external_value(PARAM_INT, 'ID учителя'),
            'username' => new external_value(PARAM_RAW, 'Логин'),
            'email' => new external_value(PARAM_EMAIL, 'Email'),
            'firstname' => new external_value(PARAM_TEXT, 'Имя'),
            'lastname' => new external_value(PARAM_TEXT, 'Фамилия'),
            'teacher' => new external_value(PARAM_BOOL, 'Является преподавателем')
        ]);
    }
    
}