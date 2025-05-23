import Loading from "@components/loading/Loading";
import { useUserStore } from "@core/store/store";
import { EyeOff, Eye } from "lucide-react";
import { useState, type CSSProperties, type JSX } from "react";
import { Container, Row, Col, Alert, Button, Form } from "react-bootstrap";
import { useNavigate } from "react-router-dom";

export default function LoginPage(): JSX.Element {
	const [ username, setUsername ] = useState('');
  const [ password, setPassword ] = useState('');
  const [ showPassword, setShowPassword ] = useState(false);

  const { error, login, isLoading } = useUserStore()
  const navigator = useNavigate()

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    if (!username || !password) return

    if (await login({ username, password })) {
      console.log('Авторизация', { username, password });
      navigator('/dashboard')
    }
  };
  return (
    <Container fluid={'sm'} className="d-flex align-items-center justify-content-center">
      <Row className="w-100 justify-content-center" >
        <Col xs={12} sm={8} md={8} lg={5} xl={4}>
          <Form onSubmit={handleSubmit} className="shadow p-4 rounded bg-white bg-opacity-75">
            <h2 className="mb-4 text-center">Вход в аккаунт</h2>
            {error && <Alert variant="danger">{error}</Alert>}
            <Form.Group controlId="formBasicLogin" className="mb-3">
              <Form.Label className="w-100 mx-2 text-start">Логин:</Form.Label>
              <div className="gradient-input-wrapper mb-3">
                <Form.Control
                  type="text"
                  placeholder="Введите логин"
                  value={username}
                  onChange={e => setUsername(e.target.value)}
                  className="gradient-input"
                />
              </div>
            </Form.Group>

            <Form.Group controlId="formBasicPassword" className="mb-5">
              <Form.Label className="w-100 mx-2 text-start">Пароль:</Form.Label>
              <div className="position-relative gradient-input-wrapper mb-3">
                <Form.Control
                  type={showPassword ? 'text' : 'password'}
                  placeholder="Введите пароль"
                  value={password}
                  onChange={e => setPassword(e.target.value)}
                  className="gradient-input pe-5" // отступ справа для иконки
                />
                <span onClick={() => setShowPassword(!showPassword)}
                  style={{
                    position: 'absolute',
                    top: '50%',
                    right: '1rem',
                    transform: 'translateY(-50%)',
                    cursor: 'pointer',
                    color: '#666'
                  }}
                >{ !showPassword ? <EyeOff size={20} /> : <Eye size={20} /> }</span>
              </div>
            </Form.Group>
            <Loading isLoading={isLoading}>
              <Button type="submit" className="w-100 text-white" style={loginButtonStyle}>Войти</Button>
            </Loading>
          </Form>
        </Col>
      </Row>
    </Container>
  );
}

const loginButtonStyle: CSSProperties = {
  background: 'linear-gradient(135deg,rgb(203, 91, 17) 0%,rgb(173, 37, 252) 100%)',
  border: 'none'
}