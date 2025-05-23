<?php
defined('MOODLE_INTERNAL') || die();

$observers = [
    // Обработчики событий, связанные с пользователем
    [
        'eventname' => '\core\event\user_updated',
        'callback' => 'local_myplugin\observer\user_events::user_updated',
        'includefile' => '/local/myplugin/classes/observer/user_events.php',
    ],
    [
        'eventname' => '\core\event\user_created',
        'callback' => 'local_myplugin\observer\user_events::user_created',
        'includefile' => '/local/myplugin/classes/observer/user_events.php',
    ],
    [
        'eventname' => '\core\event\user_deleted',
        'callback' => 'local_myplugin\observer\user_events::user_deleted',
        'includefile' => '/local/myplugin/classes/observer/user_events.php',
    ],
    [
        'eventname' => '\core\event\user_password_updated',
        'callback' => 'local_myplugin\observer\user_events::user_password_updated',
        'includefile' => '/local/myplugin/classes/observer/user_events.php',
    ],
    
    // Обработчики событий, связанные с курсом

    [
        'eventname' => '\core\event\course_updated',
        'callback' => 'local_myplugin\observer\course_events::course_updated',
        'includefile' => '/local/myplugin/classes/observer/course_events.php',
    ],
    [
        'eventname' => '\core\event\course_created',
        'callback' => 'local_myplugin\observer\course_events::course_created',
        'includefile' => '/local/myplugin/classes/observer/course_events.php',
    ],
    [
        'eventname' => '\core\event\course_deleted',
        'callback' => 'local_myplugin\observer\course_events::course_deleted',
        'includefile' => '/local/myplugin/classes/observer/course_events.php',
    ],

    [
        'eventname' => '\core\event\user_enrolment_created',
        'callback' => 'local_myplugin\observer\course_events::user_enrolment_created',
        'includefile' => '/local/myplugin/classes/observer/course_events.php',
    ],
    [
        'eventname' => '\core\event\user_enrolment_updated',
        'callback' => 'local_myplugin\observer\course_events::user_enrolment_updated',
        'includefile' => '/local/myplugin/classes/observer/course_events.php',
    ],
    [
        'eventname' => '\core\event\user_enrolment_deleted',
        'callback' => 'local_myplugin\observer\course_events::user_enrolment_deleted',
        'includefile' => '/local/myplugin/classes/observer/course_events.php',
    ],

    
    [
        'eventname' => '\core\event\role_assigned',
        'callback' => 'local_myplugin\observer\course_events::user_role_assigned',
        'includefile' => '/local/myplugin/classes/observer/course_events.php',
    ],
    [
        'eventname' => '\core\event\role_unassigned',
        'callback' => 'local_myplugin\observer\course_events::user_role_unassigned',
        'includefile' => '/local/myplugin/classes/observer/course_events.php',
    ],

    // Обработчики событий, связанные с глобальными группами

    [
        'eventname' => '\core\event\cohort_created',
        'callback' => 'local_myplugin\observer\cohort_events::cohort_created',
        'includefile' => '/local/myplugin/classes/observer/cohort_events.php',
    ],
    [
        'eventname' => '\core\event\cohort_updated',
        'callback' => 'local_myplugin\observer\cohort_events::cohort_updated',
        'includefile' => '/local/myplugin/classes/observer/cohort_events.php',
    ],
    [
        'eventname' => '\core\event\cohort_deleted',
        'callback' => 'local_myplugin\observer\cohort_events::cohort_deleted',
        'includefile' => '/local/myplugin/classes/observer/cohort_events.php',
    ],


    [
        'eventname' => '\core\event\cohort_member_added',
        'callback' => 'local_myplugin\observer\cohort_events::cohort_member_added',
        'includefile' => '/local/myplugin/classes/observer/cohort_events.php',
    ],
    [
        'eventname' => '\core\event\cohort_member_removed',
        'callback' => 'local_myplugin\observer\cohort_events::cohort_member_removed',
        'includefile' => '/local/myplugin/classes/observer/cohort_events.php',
    ],

    // Обработчики событий, связанные с посещениями

    [
        'eventname' => '\mod_attendance\event\attendance_taken',
        'callback' => 'local_myplugin\observer\lesson_events::attendance_taken',
        'includefile' => '/local/myplugin/classes/observer/lesson_events.php',
    ],
    [
        'eventname' => '\mod_attendance\event\session_added',
        'callback' => 'local_myplugin\observer\lesson_events::lesson_added',
        'includefile' => '/local/myplugin/classes/observer/lesson_events.php',
    ],
    [
        'eventname' => '\mod_attendance\event\session_deleted',
        'callback' => 'local_myplugin\observer\lesson_events::lesson_deleted',
        'includefile' => '/local/myplugin/classes/observer/lesson_events.php',
    ],
    [
        'eventname' => '\mod_attendance\event\session_updated',
        'callback' => 'local_myplugin\observer\lesson_events::lesson_updated',
        'includefile' => '/local/myplugin/classes/observer/lesson_events.php',
    ],
    // [
    //     'eventname' => '\mod_attendance\event\status_added',
    //     'callback' => 'local_myplugin\observer\lesson_events::attendance_status_added',
    //     'includefile' => '/local/myplugin/classes/observer/lesson_events.php',
    // ],
    // [
    //     'eventname' => '\mod_attendance\event\status_removed',
    //     'callback' => 'local_myplugin\observer\lesson_events::attendance_status_removed',
    //     'includefile' => '/local/myplugin/classes/observer/lesson_events.php',
    // ],

    // Обработчики событий, связанные с группами курса

    [
        'eventname' => '\core\event\group_created',
        'callback' => 'local_myplugin\observer\course_group_events::group_created',
        'includefile' => '/local/myplugin/classes/observer/course_group_events.php',
    ],
    [
        'eventname' => '\core\event\group_deleted',
        'callback' => 'local_myplugin\observer\course_group_events::group_deleted',
        'includefile' => '/local/myplugin/classes/observer/course_group_events.php',
    ],
    [
        'eventname' => '\core\event\group_updated',
        'callback' => 'local_myplugin\observer\course_group_events::group_updated',
        'includefile' => '/local/myplugin/classes/observer/course_group_events.php',
    ],
    [
        'eventname' => '\core\event\group_member_added',
        'callback' => 'local_myplugin\observer\course_group_events::group_member_added',
        'includefile' => '/local/myplugin/classes/observer/course_group_events.php',
    ],
    [
        'eventname' => '\core\event\group_member_removed',
        'callback' => 'local_myplugin\observer\course_group_events::group_member_removed',
        'includefile' => '/local/myplugin/classes/observer/course_group_events.php',
    ],

    // Обработчики событий, связанные с модулями курса

    [
        'eventname' => '\core\event\course_module_created',
        'callback' => 'local_myplugin\observer\course_module_events::module_created',
        'includefile' => '/local/myplugin/classes/observer/course_module_events.php',
    ],
    [
        'eventname' => '\core\event\course_module_deleted',
        'callback' => 'local_myplugin\observer\course_module_events::module_deleted',
        'includefile' => '/local/myplugin/classes/observer/course_module_events.php',
    ],
    [
        'eventname' => '\core\event\course_module_updated',
        'callback' => 'local_myplugin\observer\course_module_events::module_updated',
        'includefile' => '/local/myplugin/classes/observer/course_module_events.php',
    ],
];