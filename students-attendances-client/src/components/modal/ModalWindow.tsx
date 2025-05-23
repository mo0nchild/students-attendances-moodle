import { createRef, useEffect, type JSX } from "react";
import { CloseButton } from "react-bootstrap";

import './ModalWindow.css'

export interface ModalWindowProps {
    isOpen: boolean,
    children: React.ReactNode,
    onClose?: () => void,
}
export default function ModalWindow(props: ModalWindowProps): JSX.Element {
    const { isOpen, children, onClose } = props
    const dialogRef = createRef<HTMLDialogElement>()
    useEffect(() => {
			if(isOpen) {
				if (dialogRef.current?.open) dialogRef.current?.close()
				dialogRef.current?.showModal()
			}
			else dialogRef.current?.close()

			return () => {
				// eslint-disable-next-line react-hooks/exhaustive-deps
				dialogRef.current?.close()
			}
    }, [dialogRef, isOpen])
    return (
    <dialog ref={dialogRef} className='modal-window'>
        <div className='modal-window-header'>
            <CloseButton style={{
                width: '1.1rem',
                height: '1.1rem',
                backgroundSize: '100% 100%',
            }} onMouseDown={onClose}/>
        </div>
        <div style={{width: '100%', height: '1px', backgroundColor: 'grey'}}></div>
        <div style={{marginTop: '20px'}}>{ children }</div>
    </dialog>
    )
}