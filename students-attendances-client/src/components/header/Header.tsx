import { useEffect, useState, type JSX } from "react";
import { User } from "lucide-react";
import { useUserStore } from "@core/store/store";
import { Button, Image, Offcanvas } from "react-bootstrap";

import './Header.css'
import { appStorage } from "@core/utils/localstorage";
import { roleKey } from "@core/utils/api";
import { tokenManager } from "@core/utils/token-manager";

export default function Header(): JSX.Element {
  const { user, logout } = useUserStore()
  const [ show, setShow ] = useState<boolean>();

  const handleClose = () => setShow(false);
  const handleShow = () => {
    if (user) setShow(true)
  };

  useEffect(() => {
    console.log(user)
    if (!user) handleClose()
  }, [user])

  const handleLogout = () => {
    logout()
    tokenManager.removeAuth()
    window.location.reload()
  }

  const isAdmin = appStorage.getItem(roleKey)?.toLocaleLowerCase() == 'admin'

  return (
  <div className="d-flex align-items-center justify-content-between p-3 shadow-sm bg-white bg-opacity-75 position-relative" style={{ zIndex: 10 }}>
    <h1 className="fs-5 text-dark mb-0">
      <a style={{color: 'black', textDecoration: 'none'}} href='/dashboard'>Посещения</a>
    </h1>
    {
      isAdmin
        ? <button className="btn btn-outline-danger d-flex align-items-center gap-2" onClick={handleLogout}>
            <User size={20} />
            Выйти
          </button>
        : <button className="btn btn-outline-dark d-flex align-items-center gap-2" onClick={handleShow}>
            <User size={20} />
            {
              user == null ? 'Профиль' : `${user.firstName} ${user.lastName}`
            }
          </button>
    }
    <Offcanvas show={show} onHide={handleClose} placement="end">
      <Offcanvas.Header closeButton>
        <Offcanvas.Title>Информация о пользователе</Offcanvas.Title>
      </Offcanvas.Header>
      <Offcanvas.Body className="d-flex w-100 flex-column align-items-center gap-3">
        <div className="profile-block d-flex flex-column align-items-center shadow-sm">
          <Image src={user?.imageUrl} roundedCircle width={120} height={120} />
          <div className="mb-3">
            <p className="fs-4 text-center m-0">{user?.firstName} {user?.lastName}</p>
            <p className="fs-5 text-center m-0">({user?.username})</p>
          </div>
        </div>
        <div className="profile-block d-flex flex-column align-items-center shadow-sm">
          <div className="text-start w-100 mb-4">
            <h5>Контактные данные:</h5>
            <p className="m-0">Email: {user?.email}</p>
            <p className="m-0">г. {user?.city}, Регион: {user?.country}</p>
          </div>
          <div className="gradient-input-wrapper mb-3">
            <Button style={{
              backgroundColor: 'transparent', 
              border: '0'
            }}
            onClick={handleLogout}>
              Выйти из профиля
            </Button>
          </div>
        </div>
      </Offcanvas.Body>
    </Offcanvas>
  </div>
  )
}