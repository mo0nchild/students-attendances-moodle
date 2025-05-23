<?php
defined('MOODLE_INTERNAL') || die();

$callbacks = [
    [
        'hook' => \core_user\hook\before_user_deleted::class,
        'callback' => \local_myplugin\hook\before_user_deleted_handler::class . '::handle',
    ],
    [
        'hook' => \core_user\hook\before_user_updated::class,
        'callback' => \local_myplugin\hook\before_user_updated_handler::class . '::handle',
    ]
];