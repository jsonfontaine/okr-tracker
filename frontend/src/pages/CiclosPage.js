import React, { useState, useEffect, useCallback } from 'react';
import { Container, Row, Col, Table } from 'react-bootstrap';
import { Card as DSCard, CardContent, Button as DSButton, Input, Modal as DSModal, ModalHeader, ModalBody, Snackbar } from '@genial/design-system';
import { listarCiclos, criarCiclo, atualizarCiclo, excluirCiclo } from '../services/api';

export default function CiclosPage() {
  const [ciclos, setCiclos] = useState([]);
  const [nome, setNome] = useState('');
  const [dataInicio, setDataInicio] = useState('');
  const [dataFim, setDataFim] = useState('');
  const [error, setError] = useState(null);

  // Modal de edição
  const [showEdit, setShowEdit] = useState(false);
  const [editId, setEditId] = useState('');
  const [editNome, setEditNome] = useState('');
  const [editDataInicio, setEditDataInicio] = useState('');
  const [editDataFim, setEditDataFim] = useState('');

  const normalizeDateInput = (value) => {
    const digits = value.replace(/\D/g, '').slice(0, 8);
    if (digits.length <= 2) return digits;
    if (digits.length <= 4) return `${digits.slice(0, 2)}/${digits.slice(2)}`;
    return `${digits.slice(0, 2)}/${digits.slice(2, 4)}/${digits.slice(4)}`;
  };

  const parseBrDateToIso = (value) => {
    const match = value.match(/^(\d{2})\/(\d{2})\/(\d{4})$/);
    if (!match) return null;

    const day = Number(match[1]);
    const month = Number(match[2]);
    const year = Number(match[3]);
    const date = new Date(Date.UTC(year, month - 1, day));

    if (
      date.getUTCFullYear() !== year ||
      date.getUTCMonth() !== month - 1 ||
      date.getUTCDate() !== day
    ) {
      return null;
    }

    return date.toISOString();
  };

  const formatIsoToBrDate = (value) => {
    if (!value) return '';
    const datePart = String(value).split('T')[0];
    const match = datePart.match(/^(\d{4})-(\d{2})-(\d{2})$/);
    if (!match) return '';
    return `${match[3]}/${match[2]}/${match[1]}`;
  };

  const getSortDate = (ciclo) => ciclo.dataInicio || ciclo.dataCriacao || '';

  const carregarCiclos = useCallback(async () => {
    const result = await listarCiclos();
    if (result.success) {
      const ciclosOrdenados = [...(result.data || [])]
        .sort((a, b) => getSortDate(a).localeCompare(getSortDate(b)));
      setCiclos(ciclosOrdenados);
    }
  }, []);

  useEffect(() => {
    carregarCiclos();
  }, [carregarCiclos]);

  const handleCriar = async (e) => {
    e.preventDefault();
    setError(null);

    const dataInicioIso = dataInicio ? parseBrDateToIso(dataInicio) : null;
    const dataFimIso = dataFim ? parseBrDateToIso(dataFim) : null;

    if (dataInicio && !dataInicioIso) {
      setError('Data Início inválida. Use o formato dd/MM/aaaa.');
      return;
    }

    if (dataFim && !dataFimIso) {
      setError('Data Fim inválida. Use o formato dd/MM/aaaa.');
      return;
    }

    if (dataInicioIso && dataFimIso && dataInicioIso > dataFimIso) {
      setError('Data Início não pode ser maior que Data Fim.');
      return;
    }
    
    const payload = {
      nome,
      ...(dataInicioIso && { dataInicio: dataInicioIso }),
      ...(dataFimIso && { dataFim: dataFimIso })
    };
    
    const result = await criarCiclo(payload);
    if (result.success) {
      setNome('');
      setDataInicio('');
      setDataFim('');
      carregarCiclos();
    } else {
      setError(result.message);
    }
  };

  const handleEditar = (ciclo) => {
    setEditId(ciclo.id);
    setEditNome(ciclo.nome);
    setEditDataInicio(formatIsoToBrDate(ciclo.dataInicio));
    setEditDataFim(formatIsoToBrDate(ciclo.dataFim));
    setShowEdit(true);
  };

  const handleSalvarEdicao = async () => {
    setError(null);

    const dataInicioIso = editDataInicio ? parseBrDateToIso(editDataInicio) : null;
    const dataFimIso = editDataFim ? parseBrDateToIso(editDataFim) : null;

    if (editDataInicio && !dataInicioIso) {
      setError('Data Início inválida. Use o formato dd/MM/aaaa.');
      return;
    }

    if (editDataFim && !dataFimIso) {
      setError('Data Fim inválida. Use o formato dd/MM/aaaa.');
      return;
    }

    if (dataInicioIso && dataFimIso && dataInicioIso > dataFimIso) {
      setError('Data Início não pode ser maior que Data Fim.');
      return;
    }
    
    const payload = {
      nome: editNome,
      ...(dataInicioIso && { dataInicio: dataInicioIso }),
      ...(dataFimIso && { dataFim: dataFimIso })
    };
    
    const result = await atualizarCiclo(editId, payload);
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

  const formatarPeriodo = (dataInicio, dataFim) => {
    if (!dataInicio && !dataFim) return '—';
    const inicio = dataInicio ? formatIsoToBrDate(dataInicio) : '?';
    const fim = dataFim ? formatIsoToBrDate(dataFim) : '?';
    return `${inicio} a ${fim}`;
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
              <Col md={3}>
                <Input
                  data-testid="ds-input-ciclo-nome"
                  label="Nome do ciclo"
                  placeholder="Ex: 2026-Q1"
                  value={nome}
                  onChange={(e) => setNome(e.target.value)}
                />
              </Col>
              <Col md={2}>
                <label htmlFor="ciclo-data-inicio" style={{ fontWeight: 600, marginBottom: '4px', display: 'block' }}>
                  Data Início
                </label>
                <Input
                  id="ciclo-data-inicio"
                  data-testid="ds-input-ciclo-data-inicio"
                  type="text"
                  placeholder="dd/MM/aaaa"
                  inputMode="numeric"
                  value={dataInicio}
                  onChange={(e) => setDataInicio(normalizeDateInput(e.target.value))}
                />
              </Col>
              <Col md={2}>
                <label htmlFor="ciclo-data-fim" style={{ fontWeight: 600, marginBottom: '4px', display: 'block' }}>
                  Data Fim
                </label>
                <Input
                  id="ciclo-data-fim"
                  data-testid="ds-input-ciclo-data-fim"
                  type="text"
                  placeholder="dd/MM/aaaa"
                  inputMode="numeric"
                  value={dataFim}
                  onChange={(e) => setDataFim(normalizeDateInput(e.target.value))}
                />
              </Col>
              <Col md={3}>
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
            <th>Período</th>
            <th>Data Criação</th>
            <th>Última Atualização</th>
            <th>Ações</th>
          </tr>
        </thead>
        <tbody>
          {ciclos.map((c) => (
            <tr key={c.id}>
              <td>{c.nome}</td>
              <td>{formatarPeriodo(c.dataInicio, c.dataFim)}</td>
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
              <td colSpan="5" className="text-muted text-center">
                Nenhum ciclo cadastrado.
              </td>
            </tr>
          )}
        </tbody>
      </Table>

      {/* Modal de Edição */}
      <DSModal data-testid="ds-modal-edit-ciclo" open={showEdit} onClose={() => setShowEdit(false)} size="md">
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
          <div style={{ marginBottom: '16px' }}>
            <label htmlFor="edit-ciclo-data-inicio" style={{ fontWeight: 600, marginBottom: '4px', display: 'block' }}>
              Data Início
            </label>
            <Input
              id="edit-ciclo-data-inicio"
              data-testid="ds-input-edit-ciclo-data-inicio"
              type="text"
              placeholder="dd/MM/aaaa"
              inputMode="numeric"
              value={editDataInicio}
              onChange={(e) => setEditDataInicio(normalizeDateInput(e.target.value))}
            />
          </div>
          <div style={{ marginBottom: '16px' }}>
            <label htmlFor="edit-ciclo-data-fim" style={{ fontWeight: 600, marginBottom: '4px', display: 'block' }}>
              Data Fim
            </label>
            <Input
              id="edit-ciclo-data-fim"
              data-testid="ds-input-edit-ciclo-data-fim"
              type="text"
              placeholder="dd/MM/aaaa"
              inputMode="numeric"
              value={editDataFim}
              onChange={(e) => setEditDataFim(normalizeDateInput(e.target.value))}
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
