import React, { useState, useEffect, useCallback } from 'react';
import { Container, Row, Col, Form } from 'react-bootstrap';
import {
  Card as DSCard, CardContent, Button as DSButton, InputText, InputSelect, InputCheckbox,
  Modal as DSModal, ModalHeader, ModalContent, Snackbar, Loading,
} from '@genial/design-system';
import ObjetivoCard from '../components/ObjetivoCard';
import {
  listarCiclos, listarTimes, listarOKRs,
  criarObjetivo, criarKR, exportarResumoExecutivo,
} from '../services/api';

export default function DashboardPage() {
  const [ciclos, setCiclos] = useState([]);
  const [times, setTimes] = useState([]);
  const [cicloId, setCicloId] = useState('');
  const [timeId, setTimeId] = useState('');
  const [objetivos, setObjetivos] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  // Modal criar objetivo
  const [showObjetivoModal, setShowObjetivoModal] = useState(false);
  const [objForm, setObjForm] = useState({
    titulo: '', descricao: '', prioridade: 'Media', farol: 'Verde',
    intruder: false, descobertaTardia: false, valor: '',
  });

  // Modal criar KR
  const [showKRModal, setShowKRModal] = useState(false);
  const [krObjetivoId, setKrObjetivoId] = useState('');
  const [krForm, setKrForm] = useState({
    titulo: '', descricao: '', tipo: 'Quantitativo', progresso: 0,
    farol: 'Verde', intruder: false, descobertaTardia: false,
  });

  // Modal Resumo Executivo
  const [showCardModal, setShowCardModal] = useState(false);
  const [resumoTexto, setResumoTexto] = useState('');
  const [copiado, setCopiado] = useState(false);

  const carregarFiltros = useCallback(async () => {
    const [ciclosRes, timesRes] = await Promise.all([listarCiclos(), listarTimes()]);
    if (ciclosRes.success) setCiclos(ciclosRes.data || []);
    if (timesRes.success) setTimes(timesRes.data || []);
  }, []);

  useEffect(() => {
    carregarFiltros();
  }, [carregarFiltros]);

  const carregarOKRs = useCallback(async () => {
    if (!cicloId || !timeId) return;
    setLoading(true);
    setError(null);
    const result = await listarOKRs(cicloId, timeId);
    setLoading(false);
    if (result.success) {
      setObjetivos(result.data || []);
    } else {
      setError(result.message);
    }
  }, [cicloId, timeId]);

  useEffect(() => {
    carregarOKRs();
  }, [carregarOKRs]);

  // Criar Objetivo
  const handleCriarObjetivo = async (e) => {
    e.preventDefault();
    setError(null);
    const result = await criarObjetivo({
      ...objForm, cicloId, timeId,
    });
    if (result.success) {
      setShowObjetivoModal(false);
      setObjForm({ titulo: '', descricao: '', prioridade: 'Media', farol: 'Verde', intruder: false, descobertaTardia: false, valor: '' });
      carregarOKRs();
    } else {
      setError(result.message);
    }
  };

  // Criar KR
  const handleCriarKR = async (e) => {
    e.preventDefault();
    setError(null);
    const result = await criarKR({
      ...krForm, objetivoId: krObjetivoId,
    });
    if (result.success) {
      setShowKRModal(false);
      setKrForm({ titulo: '', descricao: '', tipo: 'Quantitativo', progresso: 0, farol: 'Verde', intruder: false, descobertaTardia: false });
      carregarOKRs();
    } else {
      setError(result.message);
    }
  };

  const handleExportarCard = async () => {
    const result = await exportarResumoExecutivo(cicloId, timeId);
    if (result.success) {
      setResumoTexto(result.data);
      setCopiado(false);
      setShowCardModal(true);
    } else {
      setError(result.message);
    }
  };

  const abrirCriarKR = (objetivoId) => {
    setKrObjetivoId(objetivoId);
    setShowKRModal(true);
  };

  return (
    <Container>
      <h2>🎯 Dashboard de OKRs</h2>
        <br />

      {/* Filtros */}
      <DSCard data-testid="ds-card-filtros" className="mb-4">
        <CardContent data-testid="ds-card-filtros-content">
          <Row className="align-items-end">
            <Col md={4}>
              <InputSelect
                data-testid="ds-select-ciclo"
                label="Ciclo"
                value={cicloId}
                options={[
                  { label: 'Selecione um ciclo', value: '' },
                  ...ciclos.map((c) => ({ label: c.nome, value: c.id })),
                ]}
                onChange={(e) => setCicloId(e.target.value)}
              />
            </Col>
            <Col md={4}>
              <InputSelect
                data-testid="ds-select-time"
                label="Time"
                value={timeId}
                options={[
                  { label: 'Selecione um time', value: '' },
                  ...times.map((t) => ({ label: t.nome, value: t.id })),
                ]}
                onChange={(e) => setTimeId(e.target.value)}
              />
            </Col>
            <Col md={4} className="d-flex gap-2">
              <DSButton
                data-testid="ds-button-novo-objetivo"
                variant="success"
                disabled={!cicloId || !timeId}
                onClick={() => setShowObjetivoModal(true)}
              >
                + Objetivo
              </DSButton>
              <DSButton
                data-testid="ds-button-resumo"
                variant="outline-secondary"
                disabled={!cicloId || !timeId}
                onClick={handleExportarCard}
              >
                📊 Resumo Executivo
              </DSButton>
            </Col>
          </Row>
        </CardContent>
      </DSCard>

      {error && (
        <Snackbar
          data-testid="ds-snackbar-dashboard-error"
          type="error"
          message={error}
          duration={5000}
          onClose={() => setError(null)}
          open={!!error}
          onOpenChange={(open) => { if (!open) setError(null); }}
        />
      )}

      {loading && (
        <div className="text-center py-4">
          <Loading data-testid="ds-loading-dashboard" variant="spinner" size="lg" />
        </div>
      )}

      {!loading && cicloId && timeId && objetivos.length === 0 && (
        <p className="text-muted text-center py-3">Nenhum objetivo cadastrado para este ciclo/time.</p>
      )}

      {/* Lista de Objetivos */}
      {[...objetivos]
        .sort((a, b) => {
          const ordem = { Alta: 0, Media: 1, Baixa: 2 };
          return (ordem[a.prioridade] ?? 3) - (ordem[b.prioridade] ?? 3);
        })
        .map((obj) => (
        <div key={obj.id}>
          <ObjetivoCard objetivo={obj} onUpdated={carregarOKRs} onAddKr={abrirCriarKR} />
        </div>
      ))}

      {/* Modal: Criar Objetivo */}
      <DSModal data-testid="ds-modal-criar-objetivo" opened={showObjetivoModal} onClose={() => setShowObjetivoModal(false)} width="700px">
        <ModalHeader title="Novo Objetivo" onClose={() => setShowObjetivoModal(false)} />
        <ModalContent>
          <form onSubmit={handleCriarObjetivo}>
            <div style={{ marginBottom: '16px' }}>
              <InputText
                data-testid="ds-input-obj-titulo"
                label="Título"
                value={objForm.titulo}
                onChange={(e) => setObjForm({ ...objForm, titulo: e.target.value })}
              />
            </div>
            <div style={{ marginBottom: '16px' }}>
              <InputText
                data-testid="ds-input-obj-descricao"
                label="Descrição"
                value={objForm.descricao}
                onChange={(e) => setObjForm({ ...objForm, descricao: e.target.value })}
              />
            </div>
            <Row>
              <Col md={4}>
                <div style={{ marginBottom: '16px' }}>
                  <InputSelect
                    data-testid="ds-select-obj-prioridade"
                    label="Prioridade"
                    value={objForm.prioridade}
                    options={[
                      { label: 'Baixa', value: 'Baixa' },
                      { label: 'Média', value: 'Media' },
                      { label: 'Alta', value: 'Alta' },
                    ]}
                    onChange={(e) => setObjForm({ ...objForm, prioridade: e.target.value })}
                    hideEmptyOption
                  />
                </div>
              </Col>
              <Col md={4}>
                <div style={{ marginBottom: '16px' }}>
                  <InputSelect
                    data-testid="ds-select-obj-farol"
                    label="Farol"
                    value={objForm.farol}
                    options={[
                      { label: '🟢 Verde', value: 'Verde' },
                      { label: '🟡 Amarelo', value: 'Amarelo' },
                      { label: '🔴 Vermelho', value: 'Vermelho' },
                    ]}
                    onChange={(e) => setObjForm({ ...objForm, farol: e.target.value })}
                    hideEmptyOption
                  />
                </div>
              </Col>
              <Col md={4}>
                <div style={{ marginTop: '8px' }}>
                  <InputCheckbox
                    data-testid="ds-checkbox-obj-intruder"
                    label="Intruder"
                    checked={objForm.intruder}
                    onChange={(e) => setObjForm({ ...objForm, intruder: e.target.checked })}
                  />
                  <InputCheckbox
                    data-testid="ds-checkbox-obj-descoberta"
                    label="Descoberta Tardia"
                    checked={objForm.descobertaTardia}
                    onChange={(e) => setObjForm({ ...objForm, descobertaTardia: e.target.checked })}
                  />
                </div>
              </Col>
            </Row>
            <div style={{ marginBottom: '16px' }}>
              <InputText
                data-testid="ds-input-obj-valor"
                label="💎 Valor para o negócio"
                placeholder="Descreva o valor que esse objetivo entrega para o negócio..."
                value={objForm.valor}
                onChange={(e) => setObjForm({ ...objForm, valor: e.target.value })}
              />
            </div>
            <div style={{ display: 'flex', gap: '8px', justifyContent: 'flex-end' }}>
              <DSButton data-testid="ds-button-cancel-obj" variant="secondary" onClick={() => setShowObjetivoModal(false)}>
                Cancelar
              </DSButton>
              <DSButton data-testid="ds-button-submit-obj" type="submit" variant="primary">
                Criar
              </DSButton>
            </div>
          </form>
        </ModalContent>
      </DSModal>

      {/* Modal: Criar KR */}
      <DSModal data-testid="ds-modal-criar-kr" opened={showKRModal} onClose={() => setShowKRModal(false)} width="700px">
        <ModalHeader title="Novo Key Result" onClose={() => setShowKRModal(false)} />
        <ModalContent>
          <form onSubmit={handleCriarKR}>
            <div style={{ marginBottom: '16px' }}>
              <InputText
                data-testid="ds-input-kr-titulo"
                label="Título"
                value={krForm.titulo}
                onChange={(e) => setKrForm({ ...krForm, titulo: e.target.value })}
              />
            </div>
            <div style={{ marginBottom: '16px' }}>
              <InputText
                data-testid="ds-input-kr-descricao"
                label="Descrição"
                value={krForm.descricao}
                onChange={(e) => setKrForm({ ...krForm, descricao: e.target.value })}
              />
            </div>
            <Row>
              <Col md={4}>
                <div style={{ marginBottom: '16px' }}>
                  <InputSelect
                    data-testid="ds-select-kr-tipo"
                    label="Tipo"
                    value={krForm.tipo}
                    options={[
                      { label: 'Quantitativo', value: 'Quantitativo' },
                      { label: 'Qualitativo', value: 'Qualitativo' },
                      { label: 'Requisito', value: 'Requisito' },
                    ]}
                    onChange={(e) => setKrForm({ ...krForm, tipo: e.target.value })}
                    hideEmptyOption
                  />
                </div>
              </Col>
              <Col md={4}>
                <div style={{ marginBottom: '16px' }}>
                  <InputText
                    data-testid="ds-input-kr-progresso"
                    label="Progresso Inicial (%)"
                    type="number"
                    value={String(krForm.progresso)}
                    onChange={(e) => setKrForm({ ...krForm, progresso: Number(e.target.value) })}
                  />
                </div>
              </Col>
              <Col md={4}>
                <div style={{ marginBottom: '16px' }}>
                  <InputSelect
                    data-testid="ds-select-kr-farol"
                    label="Farol"
                    value={krForm.farol}
                    options={[
                      { label: '🟢 Verde', value: 'Verde' },
                      { label: '🟡 Amarelo', value: 'Amarelo' },
                      { label: '🔴 Vermelho', value: 'Vermelho' },
                    ]}
                    onChange={(e) => setKrForm({ ...krForm, farol: e.target.value })}
                    hideEmptyOption
                  />
                </div>
              </Col>
            </Row>
            <InputCheckbox
              data-testid="ds-checkbox-kr-intruder"
              label="Intruder"
              checked={krForm.intruder}
              onChange={(e) => setKrForm({ ...krForm, intruder: e.target.checked })}
            />
            <InputCheckbox
              data-testid="ds-checkbox-kr-descoberta"
              label="Descoberta Tardia"
              checked={krForm.descobertaTardia}
              onChange={(e) => setKrForm({ ...krForm, descobertaTardia: e.target.checked })}
            />
            <div style={{ display: 'flex', gap: '8px', justifyContent: 'flex-end', marginTop: '16px' }}>
              <DSButton data-testid="ds-button-cancel-kr" variant="secondary" onClick={() => setShowKRModal(false)}>
                Cancelar
              </DSButton>
              <DSButton data-testid="ds-button-submit-kr" type="submit" variant="primary">
                Criar
              </DSButton>
            </div>
          </form>
        </ModalContent>
      </DSModal>

      {/* Modal: Resumo Executivo */}
      <DSModal data-testid="ds-modal-resumo" opened={showCardModal} onClose={() => { setShowCardModal(false); setCopiado(false); }} width="800px">
        <ModalHeader title="📊 Resumo Executivo" onClose={() => { setShowCardModal(false); setCopiado(false); }} />
        <ModalContent>
          <Form.Control
            as="textarea"
            rows={18}
            value={resumoTexto}
            readOnly
            style={{ fontFamily: 'monospace', fontSize: '0.85rem', whiteSpace: 'pre' }}
          />
          <div style={{ display: 'flex', gap: '8px', justifyContent: 'flex-end', marginTop: '16px' }}>
            <DSButton
              data-testid="ds-button-copiar-resumo"
              variant="primary"
              onClick={() => {
                navigator.clipboard.writeText(resumoTexto);
                setCopiado(true);
                setTimeout(() => setCopiado(false), 3000);
              }}
            >
              📋 Copiar
            </DSButton>
            <DSButton data-testid="ds-button-fechar-resumo" variant="secondary" onClick={() => { setShowCardModal(false); setCopiado(false); }}>
              Fechar
            </DSButton>
          </div>
        </ModalContent>
      </DSModal>

      {copiado && (
        <Snackbar
          data-testid="ds-snackbar-copiado"
          type="success"
          message="✅ Resumo copiado para a área de transferência!"
          duration={3000}
          onClose={() => setCopiado(false)}
          open={copiado}
          onOpenChange={(open) => { if (!open) setCopiado(false); }}
        />
      )}
    </Container>
  );
}
