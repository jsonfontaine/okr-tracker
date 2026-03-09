import React, { useState } from 'react';
import { Card, Button, Form, ListGroup, Collapse, Row, Col } from 'react-bootstrap';
import { ProgressoBar, TagBadge } from './Badges';
import { atualizarProgressoKR, atualizarKR, criarComentario, criarFatoRelevante, criarRisco } from '../services/api';

export default function KeyResultCard({ kr, onUpdated }) {
  const [showDetails, setShowDetails] = useState(false);
  const [progresso, setProgresso] = useState(kr.progresso);
  const [saving, setSaving] = useState(false);

  // Formulários de eventos
  const [comentario, setComentario] = useState('');
  const [fato, setFato] = useState('');
  const [riscoDesc, setRiscoDesc] = useState('');
  const [riscoImpacto, setRiscoImpacto] = useState('');

  const handleChangeField = async (field, value) => {
    const dados = {
      titulo: kr.titulo,
      descricao: kr.descricao,
      tipo: kr.tipo,
      farol: kr.farol,
      status: kr.status,
      intruder: kr.intruder,
      descobertaTardia: kr.descobertaTardia,
      [field]: value,
    };
    await atualizarKR(kr.id, dados);
    if (onUpdated) onUpdated();
  };

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
        {/* Header com título, badges e botão expandir — sem alteração */}
        <div className="d-flex justify-content-between align-items-center">
          <div className="d-flex align-items-center flex-wrap gap-1">
            <strong>📌 {kr.titulo}</strong>{' '}
            <span className="text-muted small">[{kr.tipo}]</span>{' '}
            <Form.Select
              size="sm"
              value={kr.farol}
              onChange={(e) => handleChangeField('farol', e.target.value)}
              style={{ width: 'auto', display: 'inline-block' }}
            >
              <option value="Verde">✅ Verde</option>
              <option value="Amarelo">⚠️ Amarelo</option>
              <option value="Vermelho">🔴 Vermelho</option>
            </Form.Select>
            <Form.Select
              size="sm"
              value={kr.status}
              onChange={(e) => handleChangeField('status', e.target.value)}
              style={{ width: 'auto', display: 'inline-block' }}
            >
              <option value="NaoIniciado">Não Iniciado</option>
              <option value="EmAndamento">Em Andamento</option>
              <option value="Concluido">Concluído</option>
            </Form.Select>
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

        {/* Barra de progresso e input — sem alteração */}
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
            <Row>
              {/* Coluna esquerda (60%) — Descrição, Fatos Relevantes, Riscos */}
              <Col md={7}>
                <p className="text-muted small mb-2">{kr.descricao}</p>

                {/* Fatos Relevantes */}
                <h6>📝 Fatos Relevantes</h6>
                {kr.fatosRelevantes?.length > 0 ? (
                  <ListGroup variant="flush" className="mb-2">
                    {kr.fatosRelevantes.map((f) => (
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
              </Col>

              {/* Coluna direita (40%) — Comentários estilo chat */}
              <Col md={5}>
                <div className="d-flex flex-column h-100 border-start ps-3">
                  <h6 className="mb-2">💬 Comentários</h6>
                  <div className="flex-grow-1 overflow-auto mb-2" style={{ maxHeight: 250 }}>
                    {kr.comentarios?.length > 0 ? (
                      [...kr.comentarios]
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
          </div>
        </Collapse>
      </Card.Body>
    </Card>
  );
}
