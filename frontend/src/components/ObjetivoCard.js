import React, { useState } from 'react';
import { Card, Collapse, Button, Form, ListGroup } from 'react-bootstrap';
import { FarolBadge, PrioridadeBadge, StatusBadge, ProgressoBar, TagBadge } from './Badges';
import KeyResultCard from './KeyResultCard';
import { criarComentario, criarFatoRelevante, criarRisco } from '../services/api';

export default function ObjetivoCard({ objetivo, onUpdated }) {
  const [expanded, setExpanded] = useState(false);

  // Formulários para eventos no nível do objetivo
  const [comentario, setComentario] = useState('');
  const [fato, setFato] = useState('');
  const [riscoDesc, setRiscoDesc] = useState('');
  const [riscoImpacto, setRiscoImpacto] = useState('');

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
    <Card className="mb-3 shadow-sm">
      <Card.Header
        className="d-flex justify-content-between align-items-center"
        style={{ cursor: 'pointer' }}
        onClick={() => setExpanded(!expanded)}
      >
        <div>
          <strong>🎯 {objetivo.titulo}</strong>{' '}
          <PrioridadeBadge prioridade={objetivo.prioridade} />
          <FarolBadge farol={objetivo.farol} />
          <StatusBadge status={objetivo.status} />
          <TagBadge label="Intruder" show={objetivo.intruder} />
          <TagBadge label="Descoberta Tardia" show={objetivo.descobertaTardia} />
        </div>
        <div className="d-flex align-items-center gap-2">
          <ProgressoBar progresso={objetivo.progresso} />
          <span className="text-muted">{expanded ? '▲' : '▼'}</span>
        </div>
      </Card.Header>

      <Collapse in={expanded}>
        <div>
          <Card.Body>
            <p className="text-muted">{objetivo.descricao}</p>

            {/* Key Results */}
            <h5 className="mt-3">Key Results</h5>
            {objetivo.keyResults?.length > 0 ? (
              objetivo.keyResults.map((kr) => (
                <KeyResultCard key={kr.id} kr={kr} onUpdated={onUpdated} />
              ))
            ) : (
              <p className="text-muted">Nenhum KR cadastrado.</p>
            )}

            <hr />

            {/* Comentários do Objetivo */}
            <h6>💬 Check-ins do Objetivo</h6>
            {objetivo.comentarios?.length > 0 ? (
              <ListGroup variant="flush" className="mb-2">
                {objetivo.comentarios.map((c) => (
                  <ListGroup.Item key={c.id} className="py-1 small">
                    {c.texto}{' '}
                    <span className="text-muted">
                      ({new Date(c.dataCriacao).toLocaleDateString()})
                    </span>
                  </ListGroup.Item>
                ))}
              </ListGroup>
            ) : (
              <p className="text-muted small">Nenhum check-in.</p>
            )}
            <Form onSubmit={handleAddComentario} className="d-flex gap-1 mb-3">
              <Form.Control
                size="sm"
                placeholder="Novo check-in..."
                value={comentario}
                onChange={(e) => setComentario(e.target.value)}
              />
              <Button type="submit" variant="outline-secondary" size="sm">+</Button>
            </Form>

            {/* Fatos Relevantes do Objetivo */}
            <h6>📝 Fatos Relevantes</h6>
            {objetivo.fatosRelevantes?.length > 0 ? (
              <ListGroup variant="flush" className="mb-2">
                {objetivo.fatosRelevantes.map((f) => (
                  <ListGroup.Item key={f.id} className="py-1 small">
                    {f.texto}{' '}
                    <span className="text-muted">
                      ({new Date(f.dataCriacao).toLocaleDateString()})
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

            {/* Riscos do Objetivo */}
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
            <Form onSubmit={handleAddRisco} className="d-flex gap-1">
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
          </Card.Body>
        </div>
      </Collapse>
    </Card>
  );
}
