import React, { useState } from 'react';
import { Form, ListGroup, Row, Col } from 'react-bootstrap';
import { Card as DSCard, CardHeader, CardContent, Button as DSButton } from '@genial/design-system';
import { ProgressoBar, TagBadge } from './Badges';
import KeyResultCard from './KeyResultCard';
import { criarComentario, criarFatoRelevante, criarRisco, atualizarObjetivo } from '../services/api';
import '../custom-button-border.css';

export default function ObjetivoCard({ objetivo, onUpdated, onAddKr }) {
  const [expanded, setExpanded] = useState(false);

  // Formulários para eventos no nível do objetivo
  const [comentario, setComentario] = useState('');
  const [fato, setFato] = useState('');
  const [riscoDesc, setRiscoDesc] = useState('');
  const [riscoImpacto, setRiscoImpacto] = useState('');

  const handleChangeField = async (field, value) => {
    const dados = {
      titulo: objetivo.titulo,
      descricao: objetivo.descricao,
      cicloId: objetivo.cicloId,
      timeId: objetivo.timeId,
      prioridade: objetivo.prioridade,
      farol: objetivo.farol,
      status: objetivo.status,
      intruder: objetivo.intruder,
      descobertaTardia: objetivo.descobertaTardia,
      valor: objetivo.valor,
      [field]: value,
    };
    await atualizarObjetivo(objetivo.id, dados);
    if (onUpdated) onUpdated();
  };

  const handleAddComentario = async (e) => {
    e.preventDefault();
    if (!comentario.trim()) return;
    await criarComentario({ objetivoId: objetivo.id, texto: comentario });
    setComentario('');
    if (onUpdated) onUpdated();
  };

  const handleAddFato = async (e) => {
    e.preventDefault();
    if (!fato.trim()) return;
    await criarFatoRelevante({ objetivoId: objetivo.id, texto: fato });
    setFato('');
    if (onUpdated) onUpdated();
  };

  const handleAddRisco = async (e) => {
    e.preventDefault();
    if (!riscoDesc.trim()) return;
    await criarRisco({ objetivoId: objetivo.id, descricao: riscoDesc, impacto: riscoImpacto || null });
    setRiscoDesc('');
    setRiscoImpacto('');
    if (onUpdated) onUpdated();
  };

  const cardBackground = objetivo.status === 'Concluido' ? '#d4f8e8' : (objetivo.farol === 'Vermelho' ? '#fde2e1' : undefined);

  return (
    <div
      style={{
        marginBottom: '12px',
        border: '2px solid var(--brand-primary)',
        borderRadius: '12px',
      }}
    >
      <DSCard data-testid="ds-card-objetivo" background={cardBackground}>
        <CardHeader data-testid="ds-card-objetivo-header" background={cardBackground}>
          <div className="d-flex justify-content-between align-items-center">
            <div className="d-flex align-items-center flex-wrap gap-1">
              <strong>🎯 {objetivo.titulo}</strong>{' '}
              <span className="text-muted small">Prioridade:</span>
              <Form.Select
                size="sm"
                value={objetivo.prioridade}
                onChange={(e) => handleChangeField('prioridade', e.target.value)}
                style={{ width: 'auto', display: 'inline-block' }}
              >
                <option value="Alta">🔴 Alta</option>
                <option value="Media">🟡 Média</option>
                <option value="Baixa">⚪ Baixa</option>
              </Form.Select>
              <span className="text-muted small">Farol:</span>
              <Form.Select
                size="sm"
                value={objetivo.farol}
                onChange={(e) => handleChangeField('farol', e.target.value)}
                style={{ width: 'auto', display: 'inline-block' }}
              >
                <option value="Verde">✅ Verde</option>
                <option value="Amarelo">⚠️ Amarelo</option>
                <option value="Vermelho">🔴 Vermelho</option>
              </Form.Select>
              <span className="text-muted small">Status:</span>
              <Form.Select
                size="sm"
                value={objetivo.status}
                onChange={(e) => handleChangeField('status', e.target.value)}
                style={{ width: 'auto', display: 'inline-block' }}
              >
                <option value="NaoIniciado">Não Iniciado</option>
                <option value="EmAndamento">Em Andamento</option>
                <option value="Concluido">Concluído</option>
              </Form.Select>
              <TagBadge label="Intruder" show={objetivo.intruder} />
              <TagBadge label="Descoberta Tardia" show={objetivo.descobertaTardia} />
            </div>
            <div className="d-flex align-items-center gap-2">
              <ProgressoBar progresso={objetivo.progresso} />
              {onAddKr && (
                <DSButton
                  data-testid="ds-button-add-kr"
                  variant="outline"
                  size="sm"
                  onClick={() => onAddKr(objetivo.id)}
                  className="okr-btn-border"
                >
                  + Adicionar KR
                </DSButton>
              )}
              <DSButton
                data-testid="ds-button-expand-objetivo"
                variant="link-button"
                size="sm"
                onClick={() => setExpanded(!expanded)}
                className="okr-btn-border"
              >
                {expanded ? '▲' : '▼'}
              </DSButton>
            </div>
          </div>
        </CardHeader>

        {expanded && (
          <CardContent data-testid="ds-card-objetivo-content">
            {/* 1. Descrição */}
            <p className="mb-2">
              <strong>📝 Descrição:</strong> {objetivo.descricao}
            </p>

            {/* 2. Valor para o negócio */}
            {objetivo.valor && (
              <p className="mb-2">
                <strong>💎 Valor para o negócio:</strong> {objetivo.valor}
              </p>
            )}

            <br />
            <hr />
            <br />

            <Row>
              {/* Coluna esquerda (60%) — Riscos e Fatos Relevantes */}
              <Col md={7}>
                {/* 3. Riscos */}
                <h6>⚠️ Riscos</h6>
                {objetivo.riscos?.length > 0 ? (
                  <ListGroup variant="flush" className="mb-2">
                    {objetivo.riscos.map((r) => (
                      <ListGroup.Item key={r.id} className="py-1 small">
                        {r.descricao}
                        {r.impacto && <span className="text-muted"> (Impacto: {r.impacto})</span>}
                      </ListGroup.Item>
                    ))}
                  </ListGroup>
                ) : (
                  <p className="text-muted small">Nenhum risco.</p>
                )}
                <Form onSubmit={handleAddRisco} className="d-flex gap-1 mb-3">
                  <Form.Control
                    as="textarea"
                    rows={2}
                    size="sm"
                    placeholder="Descrição do risco..."
                    value={riscoDesc}
                    onChange={(e) => setRiscoDesc(e.target.value)}
                    onKeyDown={(e) => {
                      if (e.key === 'Enter' && !e.shiftKey) {
                        e.preventDefault();
                        handleAddRisco(e);
                      }
                    }}
                  />
                  <Form.Control
                    size="sm"
                    placeholder="Impacto"
                    value={riscoImpacto}
                    onChange={(e) => setRiscoImpacto(e.target.value)}
                    style={{ width: 120 }}
                  />
                  <DSButton data-testid="ds-button-add-risco" type="submit" variant="outline-secondary" size="sm">+</DSButton>
                </Form>

                {/* 4. Fatos Relevantes */}
                <h6>📝 Fatos Relevantes</h6>
                {objetivo.fatosRelevantes?.length > 0 ? (
                  <ListGroup variant="flush" className="mb-2">
                    {objetivo.fatosRelevantes.map((f) => (
                      <ListGroup.Item key={f.id} className="py-1 small">
                        {f.texto}{' '}
                        <span className="text-muted">
                          ({new Date(f.dataCriacao).toLocaleDateString('pt-BR')})
                        </span>
                      </ListGroup.Item>
                    ))}
                  </ListGroup>
                ) : (
                  <p className="text-muted small">Nenhum fato.</p>
                )}
                <Form onSubmit={handleAddFato} className="d-flex gap-1 mb-3">
                  <Form.Control
                    as="textarea"
                    rows={2}
                    size="sm"
                    placeholder="Novo fato relevante..."
                    value={fato}
                    onChange={(e) => setFato(e.target.value)}
                    onKeyDown={(e) => {
                      if (e.key === 'Enter' && !e.shiftKey) {
                        e.preventDefault();
                        handleAddFato(e);
                      }
                    }}
                  />
                  <DSButton data-testid="ds-button-add-fato" type="submit" variant="outline-secondary" size="sm">+</DSButton>
                </Form>
              </Col>

              {/* Coluna direita (40%) — Comentários estilo chat */}
              <Col md={5}>
                <div className="d-flex flex-column h-100 border-start ps-3">
                  <h6 className="mb-2">💬 Comentários do Objetivo</h6>
                  <div className="flex-grow-1 overflow-auto mb-2" style={{ maxHeight: 250 }}>
                    {objetivo.comentarios?.length > 0 ? (
                      [...objetivo.comentarios]
                        .sort((a, b) => new Date(a.dataCriacao) - new Date(b.dataCriacao))
                        .map((c) => (
                          <div key={c.id} className="mb-2 p-2 rounded small" style={{ backgroundColor: '#f0f2f5' }}>
                            <div>{c.texto}</div>
                            <div className="text-muted" style={{ fontSize: '0.7rem' }}>
                              {new Date(c.dataCriacao).toLocaleDateString('pt-BR')}{' '}
                              {new Date(c.dataCriacao).toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit', hour12: false })}
                            </div>
                          </div>
                        ))
                    ) : (
                      <p className="text-muted small">Nenhum comentário.</p>
                    )}
                  </div>
                  <Form onSubmit={handleAddComentario} className="d-flex gap-1">
                    <Form.Control
                      as="textarea"
                      rows={3}
                      size="sm"
                      placeholder="Escrever comentário..."
                      value={comentario}
                      onChange={(e) => setComentario(e.target.value)}
                      onKeyDown={(e) => {
                        if (e.key === 'Enter' && !e.shiftKey) {
                          e.preventDefault();
                          handleAddComentario(e);
                        }
                      }}
                    />
                    <DSButton data-testid="ds-button-send-comment" type="submit" variant="primary" size="sm">Enviar</DSButton>
                  </Form>
                </div>
              </Col>
            </Row>

            <br />
            <hr />
            <br />

            {/* 6. Key Results */}
            <h5 className="mt-3">Key Results</h5>
            <br />

            {objetivo.keyResults?.length > 0 ? (
              objetivo.keyResults.map((kr) => (
                <KeyResultCard key={kr.id} kr={kr} onUpdated={onUpdated} />
              ))
            ) : (
              <p className="text-muted">Nenhum KR cadastrado.</p>
            )}
          </CardContent>
        )}
      </DSCard>
    </div>
  );
}
