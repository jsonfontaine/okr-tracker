import React from 'react';
import { Navbar, Container, Nav } from 'react-bootstrap';
import { Link, useLocation } from 'react-router-dom';

export default function AppNavbar() {
  const location = useLocation();

  return (
    <Navbar bg="dark" variant="dark" expand="lg" className="mb-4">
      <Container>
        <Navbar.Brand as={Link} to="/">🎯 OKR Tracker</Navbar.Brand>
        <Navbar.Toggle aria-controls="main-navbar" />
        <Navbar.Collapse id="main-navbar">
          <Nav className="me-auto">
            <Nav.Link as={Link} to="/" active={location.pathname === '/'}>
              Dashboard
            </Nav.Link>
            <Nav.Link as={Link} to="/ciclos" active={location.pathname === '/ciclos'}>
              Ciclos
            </Nav.Link>
            <Nav.Link as={Link} to="/times" active={location.pathname === '/times'}>
              Times
            </Nav.Link>
            <Nav.Link as={Link} to="/config" active={location.pathname === '/config'}>
              ⚙️ Configuração
            </Nav.Link>
          </Nav>
        </Navbar.Collapse>
      </Container>
    </Navbar>
  );
}
