import React, { useState, useEffect, useCallback } from 'react';
import { Container, Row, Col, Form } from 'react-bootstrap';
import {
  Card as DSCard, CardContent, Button as DSButton, Input,
  Modal as DSModal, ModalHeader, ModalBody, Snackbar, Loading,
} from '@genial/design-system';
import ObjetivoCard from '../components/ObjetivoCard';
import {
  listarCiclos, listarProjetos, listarOKRs,
  criarObjetivo, criarKR, exportarResumoExecutivo,
} from '../services/api';

export default function DashboardPage() {
  const [ciclos, setCiclos] = useState([]);
  const [projetos, setProjetos] = useState([]);
  const [cicloId, setCicloId] = useState('');
  const [projetoId, setProjetoId] = useState('');
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
    const [ciclosRes, projetosRes] = await Promise.all([listarCiclos(), listarProjetos()]);
    if (ciclosRes.success) setCiclos(ciclosRes.data || []);
    if (projetosRes.success) setProjetos(projetosRes.data || []);
  }, []);

  useEffect(() => {
    carregarFiltros();
  }, [carregarFiltros]);

  const carregarOKRs = useCallback(async () => {
    if (!cicloId || !projetoId) return;
    setLoading(true);
    setError(null);
    const result = await listarOKRs(cicloId, projetoId);
    setLoading(false);
    if (result.success) {
      setObjetivos(result.data || []);
    } else {
      setError(result.message);
    }
  }, [cicloId, projetoId]);

  useEffect(() => {
    carregarOKRs();
  }, [carregarOKRs]);

  // Criar Objetivo
  const handleCriarObjetivo = async (e) => {
    e.preventDefault();
    setError(null);
    const result = await criarObjetivo({
      ...objForm, cicloId, projetoId,
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
    const result = await exportarResumoExecutivo(cicloId, projetoId);
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
              <div className="form-group">
                <label htmlFor="ds-select-ciclo">Ciclo</label>
                <select
                  id="ds-select-ciclo"
                  data-testid="ds-select-ciclo"
                  className="form-control"
                  value={cicloId}
                  onChange={e => setCicloId(e.target.value)}
                >
                  <option value="">Selecione um ciclo</option>
                  {ciclos.map((c) => (
                    <option key={c.id} value={c.id}>{c.nome}</option>
                  ))}
                </select>
              </div>
            </Col>
            <Col md={4}>
              <div className="form-group">
                <label htmlFor="ds-select-projeto">Projeto</label>
                <select
                  id="ds-select-projeto"
                  data-testid="ds-select-projeto"
                  className="form-control"
                  value={projetoId}
                  onChange={e => setProjetoId(e.target.value)}
                >
                  <option value="">Selecione um projeto</option>
                  {projetos.map((t) => (
                    <option key={t.id} value={t.id}>{t.nome}</option>
                  ))}
                </select>
              </div>
            </Col>
            <Col md={4} className="d-flex gap-2">
              <DSButton
                data-testid="ds-button-novo-objetivo"
                variant="success"
                disabled={!cicloId || !projetoId}
                onClick={() => setShowObjetivoModal(true)}
              >
                + Objetivo
              </DSButton>
              <DSButton
                data-testid="ds-button-resumo"
                variant="outline-secondary"
                disabled={!cicloId || !projetoId}
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

      {!loading && cicloId && projetoId && objetivos.length === 0 && (
        <p className="text-muted text-center py-3">Nenhum objetivo cadastrado para este ciclo/projeto.</p>
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
      <DSModal data-testid="ds-modal-criar-objetivo" open={showObjetivoModal} onClose={() => setShowObjetivoModal(false)} size="lg">
        <ModalHeader title="Novo Objetivo" onClose={() => setShowObjetivoModal(false)} />
        <ModalBody>
          <form onSubmit={handleCriarObjetivo}>
            <div style={{ marginBottom: '16px' }}>
              <Input
                data-testid="ds-input-obj-titulo"
                label="Título"
                value={objForm.titulo}
                onChange={(e) => setObjForm({ ...objForm, titulo: e.target.value })}
              />
            </div>
            <div style={{ marginBottom: '16px' }}>
              <Input
                data-testid="ds-input-obj-descricao"
                label="Descrição"
                value={objForm.descricao}
                onChange={(e) => setObjForm({ ...objForm, descricao: e.target.value })}
              />
            </div>
            <Row>
              <Col md={4}>
                <div style={{ marginBottom: '16px' }}>
                  <div className="form-group">
                    <label htmlFor="ds-select-obj-prioridade">Prioridade</label>
                    <select
                      id="ds-select-obj-prioridade"
                      data-testid="ds-select-obj-prioridade"
                      className="form-control"
                      value={objForm.prioridade}
                      onChange={e => setObjForm({ ...objForm, prioridade: e.target.value })}
                    >
                      <option value="Baixa">Baixa</option>
                      <option value="Media">Média</option>
                      <option value="Alta">Alta</option>
                    </select>
                  </div>
                </div>
              </Col>
              <Col md={4}>
                <div style={{ marginBottom: '16px' }}>
                  <div className="form-group">
                    <label htmlFor="ds-select-obj-farol">Farol</label>
                    <select
                      id="ds-select-obj-farol"
                      data-testid="ds-select-obj-farol"
                      className="form-control"
                      value={objForm.farol}
                      onChange={e => setObjForm({ ...objForm, farol: e.target.value })}
                    >
                      <option value="Verde">🟢 Verde</option>
                      <option value="Amarelo">🟡 Amarelo</option>
                      <option value="Vermelho">🔴 Vermelho</option>
                    </select>
                  </div>
                </div>
              </Col>
              <Col md={4}>
                <div style={{ marginTop: '8px' }}>
                   <div className="form-check">
                     <input
                       type="checkbox"
                       id="ds-checkbox-obj-intruder"
                       data-testid="ds-checkbox-obj-intruder"
                       className="form-check-input"
                       checked={objForm.intruder}
                       onChange={e => setObjForm({ ...objForm, intruder: e.target.checked })}
                     />
                     <label className="form-check-label" htmlFor="ds-checkbox-obj-intruder">Intruder</label>
                   </div>
                   <div className="form-check">
                     <input
                       type="checkbox"
                       id="ds-checkbox-obj-descoberta"
                       data-testid="ds-checkbox-obj-descoberta"
                       className="form-check-input"
                       checked={objForm.descobertaTardia}
                       onChange={e => setObjForm({ ...objForm, descobertaTardia: e.target.checked })}
                     />
                     <label className="form-check-label" htmlFor="ds-checkbox-obj-descoberta">Descoberta Tardia</label>
                   </div>
                </div>
              </Col>
            </Row>
            <div style={{ marginBottom: '16px' }}>
              <Input
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
        </ModalBody>
      </DSModal>

      {/* Modal: Criar KR */}
       <DSModal data-testid="ds-modal-criar-kr" open={showKRModal} onClose={() => setShowKRModal(false)} size="lg">
         <ModalHeader title="Novo Key Result" onClose={() => setShowKRModal(false)} />
         <ModalBody>
           {/* Show error inside modal if present and modal is open */}
           {showKRModal && error && (
             <Snackbar
               data-testid="ds-snackbar-kr-modal-error"
               type="error"
               message={error}
               duration={5000}
               onClose={() => setError(null)}
               open={!!error}
               onOpenChange={(open) => { if (!open) setError(null); }}
             />
           )}
           <form onSubmit={handleCriarKR}>
             <div style={{ marginBottom: '16px' }}>
               <Input
                 data-testid="ds-input-kr-titulo"
                 label="Título"
                 value={krForm.titulo}
                 onChange={(e) => setKrForm({ ...krForm, titulo: e.target.value })}
               />
             </div>
             <div style={{ marginBottom: '16px' }}>
               <Input
                 data-testid="ds-input-kr-descricao"
                 label="Descrição"
                 value={krForm.descricao}
                 onChange={(e) => setKrForm({ ...krForm, descricao: e.target.value })}
               />
             </div>
             <Row>
               <Col md={4}>
                 <div style={{ marginBottom: '16px' }}>
                   <div className="form-group">
                     <label htmlFor="ds-select-kr-tipo">Tipo</label>
                     <select
                       id="ds-select-kr-tipo"
                       data-testid="ds-select-kr-tipo"
                       className="form-control"
                       value={krForm.tipo}
                       onChange={e => setKrForm({ ...krForm, tipo: e.target.value })}
                     >
                       <option value="Quantitativo">Quantitativo</option>
                       <option value="Qualitativo">Qualitativo</option>
                       <option value="Requisito">Requisito</option>
                     </select>
                   </div>
                 </div>
               </Col>
               <Col md={4}>
                 <div style={{ marginBottom: '16px' }}>
                   <Input
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
                   <div className="form-group">
                     <label htmlFor="ds-select-kr-farol">Farol</label>
                     <select
                       id="ds-select-kr-farol"
                       data-testid="ds-select-kr-farol"
                       className="form-control"
                       value={krForm.farol}
                       onChange={e => setKrForm({ ...krForm, farol: e.target.value })}
                     >
                       <option value="Verde">🟢 Verde</option>
                       <option value="Amarelo">🟡 Amarelo</option>
                       <option value="Vermelho">🔴 Vermelho</option>
                     </select>
                   </div>
                 </div>
               </Col>
             </Row>
              <div className="form-check">
                <input
                  type="checkbox"
                  id="ds-checkbox-kr-intruder"
                  data-testid="ds-checkbox-kr-intruder"
                  className="form-check-input"
                  checked={krForm.intruder}
                  onChange={e => setKrForm({ ...krForm, intruder: e.target.checked })}
                />
                <label className="form-check-label" htmlFor="ds-checkbox-kr-intruder">Intruder</label>
              </div>
              <div className="form-check">
                <input
                  type="checkbox"
                  id="ds-checkbox-kr-descoberta"
                  data-testid="ds-checkbox-kr-descoberta"
                  className="form-check-input"
                  checked={krForm.descobertaTardia}
                  onChange={e => setKrForm({ ...krForm, descobertaTardia: e.target.checked })}
                />
                <label className="form-check-label" htmlFor="ds-checkbox-kr-descoberta">Descoberta Tardia</label>
              </div>
             <div style={{ display: 'flex', gap: '8px', justifyContent: 'flex-end', marginTop: '16px' }}>
               <DSButton data-testid="ds-button-cancel-kr" variant="secondary" onClick={() => setShowKRModal(false)}>
                 Cancelar
               </DSButton>
               <DSButton data-testid="ds-button-submit-kr" type="submit" variant="primary">
                 Criar
               </DSButton>
             </div>
           </form>
         </ModalBody>
       </DSModal>

      {/* Modal: Resumo Executivo */}
      <DSModal data-testid="ds-modal-resumo" open={showCardModal} onClose={() => { setShowCardModal(false); setCopiado(false); }} size="xl">
        <ModalHeader title="📊 Resumo Executivo" onClose={() => { setShowCardModal(false); setCopiado(false); }} />
        <ModalBody>
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
        </ModalBody>
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
