import React, { useState } from 'react';
import { Card, Collapse, Button, Form, ListGroup, Row, Col } from 'react-bootstrap';
import { ProgressoBar, TagBadge } from './Badges';
import KeyResultCard from './KeyResultCard';
import { criarComentario, criarFatoRelevante, criarRisco, atualizarObjetivo } from '../services/api';

export default function ObjetivoCard({ objetivo, onUpdated }) {
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

  return (
    <Card className="mb-3 border-start border-3 border-primary">
      <Card.Header
        className="d-flex justify-content-between align-items-center"
      >
        <div className="d-flex align-items-center flex-wrap gap-1">
          <strong>🎯 {objetivo.titulo}</strong>{' '}
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
          <Button
            variant="link"
            size="sm"
            onClick={() => setExpanded(!expanded)}
          >
            {expanded ? '▲' : '▼'}
          </Button>
        </div>
      </Card.Header>

      <Collapse in={expanded}>
        <div>
          <Card.Body>
            {/* 1. Descrição */}
            <p className="text-muted">{objetivo.descricao}</p>

            {/* 2. Valor para o negócio */}
            {objetivo.valor && (
              <p className="mb-2">
                <strong>💎 Valor para o negócio:</strong> {objetivo.valor}
              </p>
            )}

            <hr />

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
                    size="sm"
                    placeholder="Descrição do risco..."
                    value={riscoDesc}
                    onChange={(e) => setRiscoDesc(e.target.value)}
                  />
                  <Form.Control
                    size="sm"
                    placeholder="Impacto"
                    value={riscoImpacto}
                    onChange={(e) => setRiscoImpacto(e.target.value)}
                    style={{ width: 120 }}
                  />
                  <Button type="submit" variant="outline-secondary" size="sm">+</Button>
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
                    size="sm"
                    placeholder="Novo fato relevante..."
                    value={fato}
                    onChange={(e) => setFato(e.target.value)}
                  />
                  <Button type="submit" variant="outline-secondary" size="sm">+</Button>
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
                      size="sm"
                      placeholder="Escrever comentário..."
                      value={comentario}
                      onChange={(e) => setComentario(e.target.value)}
                    />
                    <Button type="submit" variant="outline-primary" size="sm">Enviar</Button>
                  </Form>
                </div>
              </Col>
            </Row>

            <hr />

            {/* 6. Key Results */}
            <h5 className="mt-3">Key Results</h5>
            {objetivo.keyResults?.length > 0 ? (
              objetivo.keyResults.map((kr) => (
                <KeyResultCard key={kr.id} kr={kr} onUpdated={onUpdated} />
              ))
            ) : (
              <p className="text-muted">Nenhum KR cadastrado.</p>
            )}
          </Card.Body>
        </div>
      </Collapse>
    </Card>
  );
}
