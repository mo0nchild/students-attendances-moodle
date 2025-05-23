<?php
require('../../config.php');

require_login();
$PAGE->set_url(new moodle_url('/local/myplugin/index.php'));
$PAGE->set_context(context_system::instance());
$PAGE->set_title('Hello World');
$PAGE->set_heading('Hello World');

echo $OUTPUT->header();
echo $OUTPUT->heading("Добро пожаловать в local_myplugin!");
echo $OUTPUT->footer();
