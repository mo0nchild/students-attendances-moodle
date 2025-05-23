import { flip, offset, shift, useFloating } from "@floating-ui/react-dom";
import { useEffect, useRef, useState } from "react";
import { Button, ListGroup } from "react-bootstrap";

export default function ContextMenuButton() {
  const [isOpen, setIsOpen] = useState(false);
  const buttonRef = useRef<HTMLButtonElement | null>(null);
  const menuRef = useRef<HTMLDivElement | null>(null);

  const { x, y, strategy, refs, update } = useFloating({
    placement: 'bottom-start',
    middleware: [offset(), flip(), shift()],
  });

  useEffect(() => {
    if (buttonRef.current) {
      refs.setReference(buttonRef.current);
    }
    if (menuRef.current) {
      refs.setFloating(menuRef.current);
    }
  }, [refs]);

  useEffect(() => {
    if (isOpen) {
      update();
    }
  }, [isOpen, update]);

  const toggleMenu = () => setIsOpen((prev) => !prev);

  return (
    <div>
      <Button ref={buttonRef} onClick={toggleMenu}>
        Открыть меню
      </Button>

      {isOpen && (
        <ListGroup
          ref={menuRef}
          style={{
            position: strategy,
            top: y ?? 0,
            left: x ?? 0,
            minWidth: buttonRef.current?.offsetWidth ?? 0,
            zIndex: 9999,
          }}
        >
          <ListGroup.Item action onClick={() => alert('Первый пункт')}>
            Первый пункт
          </ListGroup.Item>
          <ListGroup.Item action onClick={() => alert('Второй пункт')}>
            Второй пункт
          </ListGroup.Item>
          <ListGroup.Item action onClick={() => alert('Третий пункт')}>
            Третий пункт
          </ListGroup.Item>
        </ListGroup>
      )}
    </div>
  );
}