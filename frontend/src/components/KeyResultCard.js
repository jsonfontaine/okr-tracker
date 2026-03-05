import React, { useState } from 'react';
import { Card, Button, Form, ListGroup, Collapse } from 'react-bootstrap';
import { FarolBadge, StatusBadge, ProgressoBar, TagBadge } from './Badges';
import { atualizarProgressoKR, criarComentario, criarFatoRelevante, criarRisco } from '../services/api';

export default function KeyResultCard({ kr, onUpdated }) {
  const [showDetails, setShowDetails] = useState(false);
  const [progresso, setProgresso] = useState(kr.progresso);
  const [saving, setSaving] = useState(false);

  // Formulários de eventos
  const [comentario, setComentario] = useState('');
  const [fato, setFato] = useState('');
  const [riscoDesc, setRiscoDesc] = useState('');
  const [riscoImpacto, setRiscoImpacto] = useState('');

  const handleProgressoSave = async () => {
    setSaving(true);
    const res = await atualizarProgressoKR(kr.id, progresso);
    setSaving(false);
    if (res.success && onUpdated) onUpdated();
  };

  const handleAddComentario = async (e) => {
    e.preventDefault();
    if (!comentario.trim()) return;
    await criarComentario({ krId: kr.id, texto: comentario });
    setComentario('');
    if (onUpdated) onUpdated();
  };

  const handleAddFato = async (e) => {
    e.preventDefault();
    if (!fato.trim()) return;
    await criarFatoRelevante({ krId: kr.id, texto: fato });
    setFato('');
    if (onUpdated) onUpdated();
  };

  const handleAddRisco = async (e) => {
    e.preventDefault();
    if (!riscoDesc.trim()) return;
    await criarRisco({ krId: kr.id, descricao: riscoDesc, impacto: riscoImpacto || null });
    setRiscoDesc('');
    setRiscoImpacto('');
    if (onUpdated) onUpdated();
  };

  return (
    <Card className="mb-2 border-start border-3 border-primary">
      <Card.Body className="py-2">
        <div className="d-flex justify-content-between align-items-center">
          <div>
            <strong>📌 {kr.titulo}</strong>{' '}
            <span className="text-muted small">[{kr.tipo}]</span>{' '}
            <FarolBadge farol={kr.farol} />
            <StatusBadge status={kr.status} />
            <TagBadge label="Intruder" show={kr.intruder} />
            <TagBadge label="Descoberta Tardia" show={kr.descobertaTardia} />
          </div>
          <Button
            variant="link"
            size="sm"
            onClick={() => setShowDetails(!showDetails)}
          >
            {showDetails ? '▲' : '▼'}
          </Button>
        </div>

        <div className="mt-1 d-flex align-items-center gap-2">
          <ProgressoBar progresso={kr.progresso} />
          <Form.Control
            type="number"
            size="sm"
            min={0}
            max={100}
            value={progresso}
            onChange={(e) => setProgresso(Number(e.target.value))}
            style={{ width: 80 }}
          />
          <Button
            variant="outline-primary"
            size="sm"
            onClick={handleProgressoSave}
            disabled={saving}
          >
            {saving ? '...' : 'Salvar'}
          </Button>
        </div>

        <Collapse in={showDetails}>
          <div className="mt-3">
            <p className="text-muted small mb-2">{kr.descricao}</p>

            {/* Comentários */}
            <h6 className="mt-2">💬 Check-ins</h6>
            {kr.comentarios?.length > 0 ? (
              <ListGroup variant="flush" className="mb-2">
                {kr.comentarios.map((c) => (
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

            {/* Fatos Relevantes */}
            <h6>📝 Fatos Relevantes</h6>
            {kr.fatosRelevantes?.length > 0 ? (
              <ListGroup variant="flush" className="mb-2">
                {kr.fatosRelevantes.map((f) => (
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

            {/* Riscos */}
            <h6>⚠️ Riscos</h6>
            {kr.riscos?.length > 0 ? (
              <ListGroup variant="flush" className="mb-2">
                {kr.riscos.map((r) => (
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
          </div>
        </Collapse>
      </Card.Body>
    </Card>
  );
}
