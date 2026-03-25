import React, { useState, useEffect, useCallback } from 'react';
import { Container, Row, Col } from 'react-bootstrap';
import {
  Card as DSCard, CardContent, Button as DSButton, Input,
  Modal as DSModal, ModalHeader, ModalBody, Snackbar, Loading,
} from '@genial/design-system';
import ObjetivoCard from '../components/ObjetivoCard';
import ResumoExecutivoModal from '../components/ResumoExecutivoModal';
import {
  listarCiclos, listarProjetos, listarOKRs,
  criarObjetivo, criarKR,
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
  const [copiado, setCopiado] = useState(false);

  const getSortDate = (ciclo) => ciclo.dataInicio || ciclo.dataCriacao || '';

  const carregarFiltros = useCallback(async () => {
    const [ciclosRes, projetosRes] = await Promise.all([listarCiclos(), listarProjetos()]);
    if (ciclosRes.success) {
      const ciclosOrdenados = [...(ciclosRes.data || [])]
        .sort((a, b) => getSortDate(a).localeCompare(getSortDate(b)));
      setCiclos(ciclosOrdenados);
    }
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

  const handleExportarCard = () => {
    setCopiado(false);
    setShowCardModal(true);
  };

  const abrirCriarKR = (objetivoId) => {
    setKrObjetivoId(objetivoId);
    setShowKRModal(true);
  };

  const escaparHtml = (texto) => (texto || '')
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')
    .replace(/"/g, '&quot;')
    .replace(/'/g, '&#39;');

  const exportarHtml = () => {
    try {
      const EMOJI = { Verde: '🟢', Amarelo: '🟡', Vermelho: '🔴' };
      const nomeCiclo = ciclos.find(c => c.id === cicloId)?.nome ?? cicloId;
      const nomeProjeto = projetos.find(p => p.id === projetoId)?.nome ?? projetoId;

      const objetivosHtml = objetivos.map(obj => {
        const objConcluido = obj.status === 'Concluido' || obj.status === 'Concluído';
        const objBg = objConcluido ? '#d4f8e8' : obj.farol === 'Vermelho' ? '#fde2e1' : obj.farol === 'Amarelo' ? '#fff7d6' : '#f0f4ff';

        const krsRows = (obj.keyResults ?? []).map(kr => {
          const krConcluido = kr.status === 'Concluido' || kr.status === 'Concluído';
          const krBg = krConcluido ? '#d4f8e8' : kr.farol === 'Vermelho' ? '#fde2e1' : kr.farol === 'Amarelo' ? '#fff7d6' : 'transparent';
          return `<tr style="background-color:${krBg}">
            <td class="td-kr">${escaparHtml(kr.titulo)}</td>
            <td class="td-center">${Number(kr.progresso ?? 0).toFixed(0)}%</td>
            <td class="td-center">${EMOJI[kr.farol] ?? ''} ${escaparHtml(kr.farol)}</td>
          </tr>`;
        }).join('');

        const krsSection = obj.keyResults && obj.keyResults.length > 0 ? `
          <div class="section">
            <span class="section-title">Key Results</span>
            <table>
              <thead><tr>
                <th>KR</th>
                <th class="th-center" style="width:100px">Progresso</th>
                <th class="th-center" style="width:120px">Farol</th>
              </tr></thead>
              <tbody>${krsRows}</tbody>
            </table>
          </div>` : '';

        const fatosSection = obj.fatosRelevantes && obj.fatosRelevantes.length > 0 ? `
          <div class="section">
            <span class="section-title">📝 Fatos Relevantes</span>
            <ul>${obj.fatosRelevantes.map(f => `<li>${escaparHtml(f.texto)}</li>`).join('')}</ul>
          </div>` : '';

        const riscosSection = obj.riscos && obj.riscos.length > 0 ? `
          <div class="section">
            <span class="section-title">⚠️ Riscos</span>
            <ul>${obj.riscos.map(r => `<li>${escaparHtml(r.descricao)}${r.impacto ? ` <em>(Impacto: ${escaparHtml(r.impacto)})</em>` : ''}</li>`).join('')}</ul>
          </div>` : '';

        return `
          <div class="objetivo">
            <div class="objetivo-header" style="background-color:${objBg}">
              <h2>🎯 ${escaparHtml(obj.titulo)}</h2>
              <div class="obj-meta">
                <span>📊 <strong>${Number(obj.progresso ?? 0).toFixed(0)}%</strong></span>
                <span>${EMOJI[obj.farol] ?? ''} ${escaparHtml(obj.farol)}</span>
                <span>Status: ${escaparHtml(obj.status)}</span>
              </div>
            </div>
            <div class="objetivo-body">
              ${obj.descricao ? `<p><strong>📝 Descrição:</strong> ${escaparHtml(obj.descricao)}</p>` : ''}
              ${obj.valor ? `<p><strong>💎 Valor:</strong> ${escaparHtml(obj.valor)}</p>` : ''}
              ${krsSection}${fatosSection}${riscosSection}
            </div>
          </div>`;
      }).join('');

      const dataGeracao = new Date().toLocaleString('pt-BR');
      const nomeArquivo = `resumo-executivo-${(nomeProjeto + '-' + nomeCiclo).replace(/[^\w-]/g, '-')}.html`;

      const html = `<!DOCTYPE html>
<html lang="pt-BR">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Resumo Executivo \u2014 ${escaparHtml(nomeProjeto)} | ${escaparHtml(nomeCiclo)}</title>
  <style>
    *, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }
    body { font-family: Arial, Helvetica, sans-serif; font-size: 13px; color: #1f2937; background: #f3f4f6; padding: 32px 16px; }
    .container { max-width: 960px; margin: 0 auto; }
    .page-header { margin-bottom: 28px; padding-bottom: 14px; border-bottom: 2px solid #d1d5db; }
    .page-header h1 { font-size: 22px; font-weight: 700; margin-bottom: 6px; }
    .page-header .meta { color: #6b7280; font-size: 12px; }
    .objetivo { margin-bottom: 24px; border-radius: 8px; border: 1px solid #d1d5db; overflow: hidden; background: #fff; box-shadow: 0 1px 3px rgba(0,0,0,.06); }
    .objetivo-header { padding: 12px 16px; border-bottom: 1px solid #e5e7eb; }
    .objetivo-header h2 { font-size: 15px; font-weight: 700; margin-bottom: 6px; }
    .obj-meta { display: flex; gap: 16px; flex-wrap: wrap; color: #374151; font-size: 12px; }
    .objetivo-body { padding: 12px 16px; }
    .objetivo-body p { margin-bottom: 10px; color: #374151; font-size: 13px; line-height: 1.5; }
    .section { margin-bottom: 12px; }
    .section-title { font-size: 12px; font-weight: 700; display: block; margin-bottom: 6px; color: #374151; }
    table { width: 100%; border-collapse: collapse; font-size: 12px; margin-top: 4px; }
    thead tr { background-color: #f3f4f6; }
    th { padding: 6px 10px; text-align: left; border-bottom: 2px solid #d1d5db; font-weight: 700; white-space: nowrap; color: #374151; }
    .th-center { text-align: center; }
    .td-kr { padding: 6px 10px; border-bottom: 1px solid #e5e7eb; vertical-align: top; }
    .td-center { padding: 6px 10px; border-bottom: 1px solid #e5e7eb; text-align: center; vertical-align: middle; white-space: nowrap; }
    ul { margin: 4px 0 0 18px; }
    li { margin-bottom: 4px; color: #374151; line-height: 1.5; }
    .footer { margin-top: 40px; padding-top: 12px; border-top: 1px solid #e5e7eb; font-size: 11px; color: #9ca3af; text-align: right; }
    @media print { body { background: #fff; padding: 0; } .objetivo { box-shadow: none; } }
  </style>
</head>
<body>
  <div class="container">
    <div class="page-header">
      <h1>&#128202; Resumo Executivo</h1>
      <div class="meta">
        <strong>Projeto:</strong> ${escaparHtml(nomeProjeto)} &nbsp;&nbsp;|&nbsp;&nbsp; <strong>Ciclo:</strong> ${escaparHtml(nomeCiclo)}
      </div>
    </div>
    ${objetivos.length === 0
      ? '<p style="color:#9ca3af">Nenhum OKR cadastrado para este ciclo/projeto.</p>'
      : objetivosHtml}
    <div class="footer">Gerado em ${dataGeracao}</div>
  </div>
</body>
</html>`;

      const blob = new Blob([html], { type: 'text/html;charset=utf-8' });
      const url = URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.download = nomeArquivo;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      URL.revokeObjectURL(url);

      setCopiado(true);
      setTimeout(() => setCopiado(false), 3000);
    } catch (err) {
      setError('Não foi possível exportar o relatório HTML.');
    }
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
          <ObjetivoCard objetivo={obj} ciclos={ciclos} onUpdated={carregarOKRs} onAddKr={abrirCriarKR} />
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

      <ResumoExecutivoModal
        open={showCardModal}
        objetivos={objetivos}
        cicloNome={ciclos.find(c => c.id === cicloId)?.nome ?? cicloId}
        projetoNome={projetos.find(p => p.id === projetoId)?.nome ?? projetoId}
        copiado={copiado}
        onClose={() => { setShowCardModal(false); setCopiado(false); }}
        onExport={exportarHtml}
        onCopiedClose={() => setCopiado(false)}
      />
    </Container>
  );
}
