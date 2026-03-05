import React, { useState, useEffect, useCallback } from 'react';
import { Container, Row, Col, Form, Button, Card, Table, Modal } from 'react-bootstrap';
import { listarTimes, criarTime, atualizarTime, excluirTime } from '../services/api';

export default function TimesPage() {
  const [times, setTimes] = useState([]);
  const [nome, setNome] = useState('');
  const [descricao, setDescricao] = useState('');
  const [error, setError] = useState(null);

  // Modal de edição
  const [showEdit, setShowEdit] = useState(false);
  const [editId, setEditId] = useState('');
  const [editNome, setEditNome] = useState('');
  const [editDescricao, setEditDescricao] = useState('');

  const carregarTimes = useCallback(async () => {
    const result = await listarTimes();
    if (result.success) setTimes(result.data || []);
  }, []);

  useEffect(() => {
    carregarTimes();
  }, [carregarTimes]);

  const handleCriar = async (e) => {
    e.preventDefault();
    setError(null);
    const result = await criarTime(nome, descricao);
    if (result.success) {
      setNome('');
      setDescricao('');
      carregarTimes();
    } else {
      setError(result.message);
    }
  };

  const handleEditar = (time) => {
    setEditId(time.id);
    setEditNome(time.nome);
    setEditDescricao(time.descricao || '');
    setShowEdit(true);
  };

  const handleSalvarEdicao = async () => {
    setError(null);
    const result = await atualizarTime(editId, editNome, editDescricao);
    if (result.success) {
      setShowEdit(false);
      carregarTimes();
    } else {
      setError(result.message);
    }
  };

  const handleExcluir = async (id) => {
    if (!window.confirm('Tem certeza que deseja excluir este time?')) return;
    setError(null);
    const result = await excluirTime(id);
    if (result.success) {
      carregarTimes();
    } else {
      setError(result.message);
    }
  };

  return (
    <Container>
      <h2>👥 Times</h2>

      {error && <div className="alert alert-danger">{error}</div>}

      <Card className="shadow-sm mb-4">
        <Card.Body>
          <Form onSubmit={handleCriar}>
            <Row className="align-items-end">
              <Col md={4}>
                <Form.Group>
                  <Form.Label>Nome do time</Form.Label>
                  <Form.Control
                    type="text"
                    placeholder="Ex: Bridge"
                    value={nome}
                    onChange={(e) => setNome(e.target.value)}
                    required
                  />
                </Form.Group>
              </Col>
              <Col md={5}>
                <Form.Group>
                  <Form.Label>Descrição</Form.Label>
                  <Form.Control
                    type="text"
                    placeholder="Descrição opcional"
                    value={descricao}
                    onChange={(e) => setDescricao(e.target.value)}
                  />
                </Form.Group>
              </Col>
              <Col md={3}>
                <Button type="submit" variant="primary" className="w-100">
                  Criar Time
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
            <th>Descrição</th>
            <th>Data Criação</th>
            <th>Ações</th>
          </tr>
        </thead>
        <tbody>
          {times.map((t) => (
            <tr key={t.id}>
              <td>{t.nome}</td>
              <td>{t.descricao || '-'}</td>
              <td>{new Date(t.dataCriacao).toLocaleDateString()}</td>
              <td>
                <Button
                  variant="outline-primary"
                  size="sm"
                  className="me-1"
                  onClick={() => handleEditar(t)}
                >
                  ✏️
                </Button>
                <Button
                  variant="outline-danger"
                  size="sm"
                  onClick={() => handleExcluir(t.id)}
                >
                  🗑️
                </Button>
              </td>
            </tr>
          ))}
          {times.length === 0 && (
            <tr>
              <td colSpan="4" className="text-muted text-center">
                Nenhum time cadastrado.
              </td>
            </tr>
          )}
        </tbody>
      </Table>

      {/* Modal de Edição */}
      <Modal show={showEdit} onHide={() => setShowEdit(false)}>
        <Modal.Header closeButton>
          <Modal.Title>Editar Time</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form.Group className="mb-3">
            <Form.Label>Nome</Form.Label>
            <Form.Control
              type="text"
              value={editNome}
              onChange={(e) => setEditNome(e.target.value)}
            />
          </Form.Group>
          <Form.Group>
            <Form.Label>Descrição</Form.Label>
            <Form.Control
              type="text"
              value={editDescricao}
              onChange={(e) => setEditDescricao(e.target.value)}
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
