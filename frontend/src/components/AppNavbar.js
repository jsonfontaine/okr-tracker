import React from 'react';
import { Navbar, Container, Nav, Badge, Button } from 'react-bootstrap';
import { Link, useLocation } from 'react-router-dom';

export default function AppNavbar({ dbConfigurado, dbCaminho, onDisconnect }) {
  const location = useLocation();

  return (
    <Navbar expand="lg" className="mb-4 navbar-brand-custom" variant="dark">
      <Container>
        <Navbar.Brand as={Link} to="/">🎯 OKR Tracker</Navbar.Brand>
        <Navbar.Toggle aria-controls="main-navbar" />
        <Navbar.Collapse id="main-navbar">
          <Nav className="me-auto">
            <Nav.Link
              as={dbConfigurado ? Link : 'span'}
              to={dbConfigurado ? '/' : undefined}
              active={location.pathname === '/'}
              disabled={!dbConfigurado}
            >
              Dashboard
            </Nav.Link>
            <Nav.Link
              as={dbConfigurado ? Link : 'span'}
              to={dbConfigurado ? '/ciclos' : undefined}
              active={location.pathname === '/ciclos'}
              disabled={!dbConfigurado}
            >
              Ciclos
            </Nav.Link>
            <Nav.Link
              as={dbConfigurado ? Link : 'span'}
              to={dbConfigurado ? '/times' : undefined}
              active={location.pathname === '/times'}
              disabled={!dbConfigurado}
            >
              Times
            </Nav.Link>
            <Nav.Link as={Link} to="/config" active={location.pathname === '/config'}>
              ⚙️ Configuração
            </Nav.Link>
          </Nav>
          <Navbar.Text className="d-flex align-items-center gap-2">
            {dbConfigurado ? (
              <>
                <Badge bg="success" className="me-1">●</Badge>
                <span className="db-status-text" title={dbCaminho}>{dbCaminho}</span>
                <Button
                  variant="outline-light"
                  size="sm"
                  onClick={onDisconnect}
                  title="Desconectar base de dados"
                >
                  ⏏ Desconectar
                </Button>
              </>
            ) : (
              <>
                <Badge bg="danger" className="me-1">●</Badge>
                <span className="text-warning small">Nenhuma base carregada</span>
              </>
            )}
          </Navbar.Text>
        </Navbar.Collapse>
      </Container>
    </Navbar>
  );
}
