import React, { useState, useEffect, useCallback } from 'react';
import { Container, Row, Col, Table } from 'react-bootstrap';
import { Card as DSCard, CardContent, Button as DSButton, InputText, Modal as DSModal, ModalHeader, ModalContent, Snackbar } from '@genial/design-system';
import { listarProjetos, criarProjeto, atualizarProjeto, excluirProjeto } from '../services/api';

export default function TimesPage() {
  const [projetos, setProjetos] = useState([]);
  const [nome, setNome] = useState('');
  const [descricao, setDescricao] = useState('');
  const [error, setError] = useState(null);

  // Modal de edição
  const [showEdit, setShowEdit] = useState(false);
  const [editId, setEditId] = useState('');
  const [editNome, setEditNome] = useState('');
  const [editDescricao, setEditDescricao] = useState('');

  const carregarProjetos = useCallback(async () => {
    const result = await listarProjetos();
    if (result.success) setProjetos(result.data || []);
  }, []);

  useEffect(() => {
    carregarProjetos();
  }, [carregarProjetos]);

  const handleCriar = async (e) => {
    e.preventDefault();
    setError(null);
    const result = await criarProjeto(nome, descricao);
    if (result.success) {
      setNome('');
      setDescricao('');
      carregarProjetos();
    } else {
      setError(result.message);
    }
  };

  const handleEditar = (projeto) => {
    setEditId(projeto.id);
    setEditNome(projeto.nome);
    setEditDescricao(projeto.descricao || '');
    setShowEdit(true);
  };

  const handleSalvarEdicao = async () => {
    setError(null);
    const result = await atualizarProjeto(editId, editNome, editDescricao);
    if (result.success) {
      setShowEdit(false);
      carregarProjetos();
    } else {
      setError(result.message);
    }
  };

  const handleExcluir = async (id) => {
    if (!window.confirm('Tem certeza que deseja excluir este projeto?')) return;
    setError(null);
    const result = await excluirProjeto(id);
    if (result.success) {
      carregarProjetos();
    } else {
      setError(result.message);
    }
  };

  return (
    <Container>
      <h2>📁 Projetos</h2>

      {error && (
        <Snackbar
          data-testid="ds-snackbar-projetos-error"
          type="error"
          message={error}
          duration={5000}
          onClose={() => setError(null)}
          open={!!error}
          onOpenChange={(open) => { if (!open) setError(null); }}
        />
      )}

      <DSCard data-testid="ds-card-projetos-form" className="mb-4">
        <CardContent data-testid="ds-card-projetos-form-content">
          <form onSubmit={handleCriar}>
            <Row className="align-items-end">
              <Col md={4}>
                <InputText
                  data-testid="ds-input-projeto-nome"
                  label="Nome do projeto"
                  placeholder="Ex: Bridge"
                  value={nome}
                  onChange={(e) => setNome(e.target.value)}
                />
              </Col>
              <Col md={5}>
                <InputText
                  data-testid="ds-input-projeto-descricao"
                  label="Descrição"
                  placeholder="Descrição opcional"
                  value={descricao}
                  onChange={(e) => setDescricao(e.target.value)}
                />
              </Col>
              <Col md={3}>
                <DSButton data-testid="ds-button-criar-projeto" type="submit" variant="primary" fullWidth>
                  Criar Projeto
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
            <th>Descrição</th>
            <th>Data Criação</th>
            <th>Ações</th>
          </tr>
        </thead>
        <tbody>
          {projetos.map((t) => (
            <tr key={t.id}>
              <td>{t.nome}</td>
              <td>{t.descricao || '-'}</td>
              <td>{new Date(t.dataCriacao).toLocaleDateString('pt-BR')}</td>
              <td>
                <div style={{ display: 'flex', gap: '4px' }}>
                  <DSButton data-testid="ds-button-edit-projeto" variant="outline" size="sm" onClick={() => handleEditar(t)}>
                    ✏️
                  </DSButton>
                  <DSButton data-testid="ds-button-delete-projeto" variant="destructive" size="sm" onClick={() => handleExcluir(t.id)}>
                    🗑️
                  </DSButton>
                </div>
              </td>
            </tr>
          ))}
          {projetos.length === 0 && (
            <tr>
              <td colSpan="4" className="text-muted text-center">
                Nenhum projeto cadastrado.
              </td>
            </tr>
          )}
        </tbody>
      </Table>

      {/* Modal de Edição */}
      <DSModal data-testid="ds-modal-edit-projeto" opened={showEdit} onClose={() => setShowEdit(false)} width="500px">
        <ModalHeader title="Editar Projeto" onClose={() => setShowEdit(false)} />
        <ModalContent>
          <div style={{ marginBottom: '16px' }}>
            <InputText
              data-testid="ds-input-edit-projeto-nome"
              label="Nome"
              value={editNome}
              onChange={(e) => setEditNome(e.target.value)}
            />
          </div>
          <div style={{ marginBottom: '16px' }}>
            <InputText
              data-testid="ds-input-edit-projeto-descricao"
              label="Descrição"
              value={editDescricao}
              onChange={(e) => setEditDescricao(e.target.value)}
            />
          </div>
          <div style={{ display: 'flex', gap: '8px', justifyContent: 'flex-end' }}>
            <DSButton data-testid="ds-button-cancel-edit-projeto" variant="secondary" onClick={() => setShowEdit(false)}>
              Cancelar
            </DSButton>
            <DSButton data-testid="ds-button-save-edit-projeto" variant="primary" onClick={handleSalvarEdicao}>
              Salvar
            </DSButton>
          </div>
        </ModalContent>
      </DSModal>
    </Container>
  );
}
