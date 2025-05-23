<?php
namespace local_myplugin\local\models;

defined('MOODLE_INTERNAL') || die();

final class event_info {
    /** @var string Тип события (например, 'user_updated') */
    private string $event_type;

    /** @var int ID записи, связанной с событием */
    private int $record_id;

    /** @var array Изменения (например, ['email' => 'old@test.com → new@test.com']) */
    private array $changes;

    public function __construct(string $event_type, int $record_id, ?array $changes = null) {
        $this->event_type = $event_type;
        $this->record_id = $record_id;
        $this->changes = $changes ?? [];
    }

    public function get_event_type(): string { 
        return $this->event_type; 
    }

    public function get_record_id(): int {
        return $this->record_id;
    }
    public function get_changes(): ?array {
        return $this->changes;
    }
    /**
     * Преобразует объект DTO в массив 
     */
    public function to_array(): array {
        return [
            'record_id' => $this->user_id,
            'event_type' => $this->event_type,
            'changes' => $this->changes,
        ];
    }
}