import React, { useState } from 'react';
import { Container, Card, Form, Button, Alert } from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';
import { configurarDatabase } from '../services/api';

export default function ConfigPage({ onConfigured }) {
  const [path, setPath] = useState(() => localStorage.getItem('okr-tracker-db-path') || '');
  const [message, setMessage] = useState(null);
  const [variant, setVariant] = useState('success');
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setMessage(null);

    const result = await configurarDatabase(path);
    setLoading(false);

    if (result.success) {
      localStorage.setItem('okr-tracker-db-path', path);
      setVariant('success');
      setMessage('Base de dados configurada com sucesso!');
      if (onConfigured) await onConfigured();
      setTimeout(() => navigate('/'), 1000);
    } else {
      setVariant('danger');
      setMessage(result.message || 'Erro ao configurar a base de dados.');
    }
  };

  return (
    <Container>
      <h2>⚙️ Configuração</h2>
      <p className="text-muted">
        Configure o caminho do arquivo LiteDB (.db) antes de utilizar a aplicação.
      </p>

      <Card className="shadow-sm">
        <Card.Body>
          <Form onSubmit={handleSubmit}>
            <Form.Group className="mb-3">
              <Form.Label>Caminho do arquivo .db</Form.Label>
              <Form.Control
                type="text"
                placeholder="Ex: C:\Users\jason.fontaine\Source\Repos\okr-tracker\database\jason-okr-tracker.db"
                value={path}
                onChange={(e) => setPath(e.target.value)}
                required
              />
              <Form.Text className="text-muted">
                Informe o caminho absoluto do arquivo LiteDB.
              </Form.Text>
            </Form.Group>
            <Button type="submit" variant="primary" disabled={loading}>
              {loading ? 'Configurando...' : 'Configurar'}
            </Button>
          </Form>

          {message && (
            <Alert variant={variant} className="mt-3">
              {message}
            </Alert>
          )}
        </Card.Body>
      </Card>
    </Container>
  );
}
