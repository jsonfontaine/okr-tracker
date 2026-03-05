import React, { useState, useEffect, useCallback } from 'react';
import { Container, Row, Col, Form, Button, Card, Table, Modal } from 'react-bootstrap';
import { listarCiclos, criarCiclo, atualizarCiclo, excluirCiclo } from '../services/api';

export default function CiclosPage() {
  const [ciclos, setCiclos] = useState([]);
  const [nome, setNome] = useState('');
  const [error, setError] = useState(null);

  // Modal de edição
  const [showEdit, setShowEdit] = useState(false);
  const [editId, setEditId] = useState('');
  const [editNome, setEditNome] = useState('');

  const carregarCiclos = useCallback(async () => {
    const result = await listarCiclos();
    if (result.success) setCiclos(result.data || []);
  }, []);

  useEffect(() => {
    carregarCiclos();
  }, [carregarCiclos]);

  const handleCriar = async (e) => {
    e.preventDefault();
    setError(null);
    const result = await criarCiclo(nome);
    if (result.success) {
      setNome('');
      carregarCiclos();
    } else {
      setError(result.message);
    }
  };

  const handleEditar = (ciclo) => {
    setEditId(ciclo.id);
    setEditNome(ciclo.nome);
    setShowEdit(true);
  };

  const handleSalvarEdicao = async () => {
    setError(null);
    const result = await atualizarCiclo(editId, editNome);
    if (result.success) {
      setShowEdit(false);
      carregarCiclos();
    } else {
      setError(result.message);
    }
  };

  const handleExcluir = async (id) => {
    if (!window.confirm('Tem certeza que deseja excluir este ciclo?')) return;
    setError(null);
    const result = await excluirCiclo(id);
    if (result.success) {
      carregarCiclos();
    } else {
      setError(result.message);
    }
  };

  return (
    <Container>
      <h2>📅 Ciclos</h2>

      {error && <div className="alert alert-danger">{error}</div>}

      <Card className="shadow-sm mb-4">
        <Card.Body>
          <Form onSubmit={handleCriar}>
            <Row className="align-items-end">
              <Col md={8}>
                <Form.Group>
                  <Form.Label>Nome do ciclo</Form.Label>
                  <Form.Control
                    type="text"
                    placeholder="Ex: 2026-Q1"
                    value={nome}
                    onChange={(e) => setNome(e.target.value)}
                    required
                  />
                </Form.Group>
              </Col>
              <Col md={4}>
                <Button type="submit" variant="primary" className="w-100">
                  Criar Ciclo
                </Button>
              </Col>
            </Row>
          </Form>
        </Card.Body>
      </Card>

      <Table striped hover responsive>
        <thead>
          <tr>
            <th>Nome</th>
            <th>Data Criação</th>
            <th>Última Atualização</th>
            <th>Ações</th>
          </tr>
        </thead>
        <tbody>
          {ciclos.map((c) => (
            <tr key={c.id}>
              <td>{c.nome}</td>
              <td>{new Date(c.dataCriacao).toLocaleDateString()}</td>
              <td>{new Date(c.ultimaAtualizacao).toLocaleDateString()}</td>
              <td>
                <Button
                  variant="outline-primary"
                  size="sm"
                  className="me-1"
                  onClick={() => handleEditar(c)}
                >
                  ✏️
                </Button>
                <Button
                  variant="outline-danger"
                  size="sm"
                  onClick={() => handleExcluir(c.id)}
                >
                  🗑️
                </Button>
              </td>
            </tr>
          ))}
          {ciclos.length === 0 && (
            <tr>
              <td colSpan="4" className="text-muted text-center">
                Nenhum ciclo cadastrado.
              </td>
            </tr>
          )}
        </tbody>
      </Table>

      {/* Modal de Edição */}
      <Modal show={showEdit} onHide={() => setShowEdit(false)}>
        <Modal.Header closeButton>
          <Modal.Title>Editar Ciclo</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form.Group>
            <Form.Label>Nome</Form.Label>
            <Form.Control
              type="text"
              value={editNome}
              onChange={(e) => setEditNome(e.target.value)}
            />
          </Form.Group>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={() => setShowEdit(false)}>
            Cancelar
          </Button>
          <Button variant="primary" onClick={handleSalvarEdicao}>
            Salvar
          </Button>
        </Modal.Footer>
      </Modal>
    </Container>
  );
}
