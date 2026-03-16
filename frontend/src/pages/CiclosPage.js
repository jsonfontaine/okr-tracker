import React, { useState, useEffect, useCallback } from 'react';
import { Container, Row, Col, Table } from 'react-bootstrap';
import { Card as DSCard, CardContent, Button as DSButton, Input, Modal as DSModal, ModalHeader, ModalBody, Snackbar } from '@genial/design-system';
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

      {error && (
        <Snackbar
          data-testid="ds-snackbar-ciclos-error"
          type="error"
          message={error}
          duration={5000}
          onClose={() => setError(null)}
          open={!!error}
          onOpenChange={(open) => { if (!open) setError(null); }}
        />
      )}

      <DSCard data-testid="ds-card-ciclos-form" className="mb-4">
        <CardContent data-testid="ds-card-ciclos-form-content">
          <form onSubmit={handleCriar}>
            <Row className="align-items-end">
              <Col md={8}>
                <Input
                  data-testid="ds-input-ciclo-nome"
                  label="Nome do ciclo"
                  placeholder="Ex: 2026-Q1"
                  value={nome}
                  onChange={(e) => setNome(e.target.value)}
                />
              </Col>
              <Col md={4}>
                <DSButton data-testid="ds-button-criar-ciclo" type="submit" variant="primary" fullWidth>
                  Criar Ciclo
                </DSButton>
              </Col>
            </Row>
          </form>
        </CardContent>
      </DSCard>

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
              <td>{new Date(c.dataCriacao).toLocaleDateString('pt-BR')}</td>
              <td>{new Date(c.ultimaAtualizacao).toLocaleDateString('pt-BR')}</td>
              <td>
                <div style={{ display: 'flex', gap: '4px' }}>
                  <DSButton data-testid="ds-button-edit-ciclo" variant="outline" size="sm" onClick={() => handleEditar(c)}>
                    ✏️
                  </DSButton>
                  <DSButton data-testid="ds-button-delete-ciclo" variant="destructive" size="sm" onClick={() => handleExcluir(c.id)}>
                    🗑️
                  </DSButton>
                </div>
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
      <DSModal data-testid="ds-modal-edit-ciclo" opened={showEdit} onClose={() => setShowEdit(false)} width="500px">
        <ModalHeader title="Editar Ciclo" onClose={() => setShowEdit(false)} />
        <ModalBody>
          <div style={{ marginBottom: '16px' }}>
            <Input
              data-testid="ds-input-edit-ciclo-nome"
              label="Nome"
              value={editNome}
              onChange={(e) => setEditNome(e.target.value)}
            />
          </div>
          <div style={{ display: 'flex', gap: '8px', justifyContent: 'flex-end' }}>
            <DSButton data-testid="ds-button-cancel-edit-ciclo" variant="secondary" onClick={() => setShowEdit(false)}>
              Cancelar
            </DSButton>
            <DSButton data-testid="ds-button-save-edit-ciclo" variant="primary" onClick={handleSalvarEdicao}>
              Salvar
            </DSButton>
          </div>
        </ModalBody>
      </DSModal>
    </Container>
  );
}
