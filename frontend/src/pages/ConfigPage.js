import React, { useState } from 'react';
import { Container } from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';
import { Card as DSCard, CardContent, Button as DSButton, Input, Snackbar } from '@genial/design-system';
import { configurarDatabase } from '../services/api';

export default function ConfigPage({ onConfigured }) {
  const [path, setPath] = useState(() => localStorage.getItem('okr-tracker-db-path') || '');
  const [message, setMessage] = useState(null);
  const [snackType, setSnackType] = useState('success');
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
      setSnackType('success');
      setMessage('Base de dados configurada com sucesso!');
      if (onConfigured) await onConfigured();
      setTimeout(() => navigate('/'), 1000);
    } else {
      setSnackType('error');
      setMessage(result.message || 'Erro ao configurar a base de dados.');
    }
  };

  return (
    <Container>
      <h2>⚙️ Configuração</h2>
      <p className="text-muted">
        Configure o caminho do arquivo LiteDB (.db) antes de utilizar a aplicação.
      </p>

      <DSCard data-testid="ds-card-config">
        <CardContent data-testid="ds-card-config-content">
          <form onSubmit={handleSubmit}>
            <div style={{ marginBottom: '16px' }}>
              <Input
                data-testid="ds-input-db-path"
                label="Caminho do arquivo .db"
                placeholder="Ex: C:\\Users\\...\\database\\jason-okr-tracker.db"
                value={path}
                onChange={(e) => setPath(e.target.value)}
                helperText="Informe o caminho absoluto do arquivo LiteDB."
              />
            </div>
            <DSButton
              data-testid="ds-button-config-submit"
              type="submit"
              variant="primary"
              disabled={loading || !path.trim()}
              loading={loading}
            >
              Configurar
            </DSButton>
          </form>
        </CardContent>
      </DSCard>

      {message && (
        <Snackbar
          data-testid="ds-snackbar-config"
          type={snackType}
          message={message}
          duration={5000}
          onClose={() => setMessage(null)}
          open={!!message}
          onOpenChange={(open) => { if (!open) setMessage(null); }}
        />
      )}
    </Container>
  );
}
