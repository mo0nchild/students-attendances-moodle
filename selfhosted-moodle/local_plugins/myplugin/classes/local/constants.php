<?php
namespace local_myplugin\local;

defined('MOODLE_INTERNAL') || die();

class constants {
    /** Название таблицы с сообщениями */
    public const MESSAGES_TABLE = 'myplugin_messages';

    /** Статуса обработки сообщений */
    public const MESSAGE_PENDING = 'pending';

    public const MESSAGE_PROCESSING = 'processing';

    public const MESSAGE_COMPLETE = 'complete';
    
    public const MESSAGE_FAILED = 'failed';
}