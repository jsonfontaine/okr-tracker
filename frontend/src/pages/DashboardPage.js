import React, { useState, useEffect, useCallback } from 'react';
import {
  Container, Row, Col, Form, Button, Card, Modal, Alert, Spinner,
} from 'react-bootstrap';
import ObjetivoCard from '../components/ObjetivoCard';
import {
  listarCiclos, listarTimes, listarOKRs,
  criarObjetivo, criarKR, exportarAdaptiveCard,
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
    intruder: false, descobertaTardia: false,
  });

  // Modal criar KR
  const [showKRModal, setShowKRModal] = useState(false);
  const [krObjetivoId, setKrObjetivoId] = useState('');
  const [krForm, setKrForm] = useState({
    titulo: '', descricao: '', tipo: 'Quantitativo', progresso: 0,
    farol: 'Verde', intruder: false, descobertaTardia: false,
  });

  // Modal Adaptive Card
  const [showCardModal, setShowCardModal] = useState(false);
  const [cardJson, setCardJson] = useState('');

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
      setObjForm({ titulo: '', descricao: '', prioridade: 'Media', farol: 'Verde', intruder: false, descobertaTardia: false });
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
    const result = await exportarAdaptiveCard(cicloId, timeId);
    if (result.success) {
      setCardJson(JSON.stringify(result.data, null, 2));
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

      {/* Filtros */}
      <Card className="shadow-sm mb-4">
        <Card.Body>
          <Row className="align-items-end">
            <Col md={4}>
              <Form.Group>
                <Form.Label>Ciclo</Form.Label>
                <Form.Select value={cicloId} onChange={(e) => setCicloId(e.target.value)}>
                  <option value="">Selecione um ciclo</option>
                  {ciclos.map((c) => (
                    <option key={c.id} value={c.id}>{c.nome}</option>
                  ))}
                </Form.Select>
              </Form.Group>
            </Col>
            <Col md={4}>
              <Form.Group>
                <Form.Label>Time</Form.Label>
                <Form.Select value={timeId} onChange={(e) => setTimeId(e.target.value)}>
                  <option value="">Selecione um time</option>
                  {times.map((t) => (
                    <option key={t.id} value={t.id}>{t.nome}</option>
                  ))}
                </Form.Select>
              </Form.Group>
            </Col>
            <Col md={4} className="d-flex gap-2">
              <Button
                variant="success"
                disabled={!cicloId || !timeId}
                onClick={() => setShowObjetivoModal(true)}
              >
                + Objetivo
              </Button>
              <Button
                variant="outline-secondary"
                disabled={!cicloId || !timeId}
                onClick={handleExportarCard}
              >
                📋 Adaptive Card
              </Button>
            </Col>
          </Row>
        </Card.Body>
      </Card>

      {error && <Alert variant="danger">{error}</Alert>}

      {loading && (
        <div className="text-center py-4">
          <Spinner animation="border" />
        </div>
      )}

      {!loading && cicloId && timeId && objetivos.length === 0 && (
        <Alert variant="info">Nenhum objetivo cadastrado para este ciclo/time.</Alert>
      )}

      {/* Lista de Objetivos */}
      {objetivos.map((obj) => (
        <div key={obj.id}>
          <ObjetivoCard objetivo={obj} onUpdated={carregarOKRs} />
          <div className="mb-3 ms-3">
            <Button
              variant="outline-primary"
              size="sm"
              onClick={() => abrirCriarKR(obj.id)}
            >
              + Adicionar KR
            </Button>
          </div>
        </div>
      ))}

      {/* Modal: Criar Objetivo */}
      <Modal show={showObjetivoModal} onHide={() => setShowObjetivoModal(false)} size="lg">
        <Modal.Header closeButton>
          <Modal.Title>Novo Objetivo</Modal.Title>
        </Modal.Header>
        <Form onSubmit={handleCriarObjetivo}>
          <Modal.Body>
            <Form.Group className="mb-3">
              <Form.Label>Título</Form.Label>
              <Form.Control
                required
                value={objForm.titulo}
                onChange={(e) => setObjForm({ ...objForm, titulo: e.target.value })}
              />
            </Form.Group>
            <Form.Group className="mb-3">
              <Form.Label>Descrição</Form.Label>
              <Form.Control
                as="textarea"
                rows={2}
                required
                value={objForm.descricao}
                onChange={(e) => setObjForm({ ...objForm, descricao: e.target.value })}
              />
            </Form.Group>
            <Row>
              <Col md={4}>
                <Form.Group className="mb-3">
                  <Form.Label>Prioridade</Form.Label>
                  <Form.Select
                    value={objForm.prioridade}
                    onChange={(e) => setObjForm({ ...objForm, prioridade: e.target.value })}
                  >
                    <option value="Baixa">Baixa</option>
                    <option value="Media">Média</option>
                    <option value="Alta">Alta</option>
                  </Form.Select>
                </Form.Group>
              </Col>
              <Col md={4}>
                <Form.Group className="mb-3">
                  <Form.Label>Farol</Form.Label>
                  <Form.Select
                    value={objForm.farol}
                    onChange={(e) => setObjForm({ ...objForm, farol: e.target.value })}
                  >
                    <option value="Verde">🟢 Verde</option>
                    <option value="Amarelo">🟡 Amarelo</option>
                    <option value="Vermelho">🔴 Vermelho</option>
                  </Form.Select>
                </Form.Group>
              </Col>
              <Col md={4}>
                <Form.Check
                  className="mt-4"
                  label="Intruder"
                  checked={objForm.intruder}
                  onChange={(e) => setObjForm({ ...objForm, intruder: e.target.checked })}
                />
                <Form.Check
                  label="Descoberta Tardia"
                  checked={objForm.descobertaTardia}
                  onChange={(e) => setObjForm({ ...objForm, descobertaTardia: e.target.checked })}
                />
              </Col>
            </Row>
          </Modal.Body>
          <Modal.Footer>
            <Button variant="secondary" onClick={() => setShowObjetivoModal(false)}>Cancelar</Button>
            <Button type="submit" variant="primary">Criar</Button>
          </Modal.Footer>
        </Form>
      </Modal>

      {/* Modal: Criar KR */}
      <Modal show={showKRModal} onHide={() => setShowKRModal(false)} size="lg">
        <Modal.Header closeButton>
          <Modal.Title>Novo Key Result</Modal.Title>
        </Modal.Header>
        <Form onSubmit={handleCriarKR}>
          <Modal.Body>
            <Form.Group className="mb-3">
              <Form.Label>Título</Form.Label>
              <Form.Control
                required
                value={krForm.titulo}
                onChange={(e) => setKrForm({ ...krForm, titulo: e.target.value })}
              />
            </Form.Group>
            <Form.Group className="mb-3">
              <Form.Label>Descrição</Form.Label>
              <Form.Control
                as="textarea"
                rows={2}
                required
                value={krForm.descricao}
                onChange={(e) => setKrForm({ ...krForm, descricao: e.target.value })}
              />
            </Form.Group>
            <Row>
              <Col md={4}>
                <Form.Group className="mb-3">
                  <Form.Label>Tipo</Form.Label>
                  <Form.Select
                    value={krForm.tipo}
                    onChange={(e) => setKrForm({ ...krForm, tipo: e.target.value })}
                  >
                    <option value="Quantitativo">Quantitativo</option>
                    <option value="Qualitativo">Qualitativo</option>
                    <option value="Requisito">Requisito</option>
                  </Form.Select>
                </Form.Group>
              </Col>
              <Col md={4}>
                <Form.Group className="mb-3">
                  <Form.Label>Progresso Inicial (%)</Form.Label>
                  <Form.Control
                    type="number"
                    min={0}
                    max={100}
                    value={krForm.progresso}
                    onChange={(e) => setKrForm({ ...krForm, progresso: Number(e.target.value) })}
                  />
                </Form.Group>
              </Col>
              <Col md={4}>
                <Form.Group className="mb-3">
                  <Form.Label>Farol</Form.Label>
                  <Form.Select
                    value={krForm.farol}
                    onChange={(e) => setKrForm({ ...krForm, farol: e.target.value })}
                  >
                    <option value="Verde">🟢 Verde</option>
                    <option value="Amarelo">🟡 Amarelo</option>
                    <option value="Vermelho">🔴 Vermelho</option>
                  </Form.Select>
                </Form.Group>
              </Col>
            </Row>
            <Form.Check
              label="Intruder"
              checked={krForm.intruder}
              onChange={(e) => setKrForm({ ...krForm, intruder: e.target.checked })}
            />
            <Form.Check
              label="Descoberta Tardia"
              checked={krForm.descobertaTardia}
              onChange={(e) => setKrForm({ ...krForm, descobertaTardia: e.target.checked })}
            />
          </Modal.Body>
          <Modal.Footer>
            <Button variant="secondary" onClick={() => setShowKRModal(false)}>Cancelar</Button>
            <Button type="submit" variant="primary">Criar</Button>
          </Modal.Footer>
        </Form>
      </Modal>

      {/* Modal: Adaptive Card JSON */}
      <Modal show={showCardModal} onHide={() => setShowCardModal(false)} size="lg">
        <Modal.Header closeButton>
          <Modal.Title>📋 Adaptive Card JSON</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form.Control
            as="textarea"
            rows={15}
            value={cardJson}
            readOnly
            style={{ fontFamily: 'monospace', fontSize: '0.85rem' }}
          />
        </Modal.Body>
        <Modal.Footer>
          <Button
            variant="primary"
            onClick={() => {
              navigator.clipboard.writeText(cardJson);
            }}
          >
            📋 Copiar
          </Button>
          <Button variant="secondary" onClick={() => setShowCardModal(false)}>
            Fechar
          </Button>
        </Modal.Footer>
      </Modal>
    </Container>
  );
}
