import React from 'react';
import { Badge, ProgressBar } from 'react-bootstrap';

const farolColors = {
  Verde: 'success',
  Amarelo: 'warning',
  Vermelho: 'danger',
};

const prioridadeBadge = {
  Alta: 'danger',
  Media: 'warning',
  Baixa: 'secondary',
};

const statusLabels = {
  NaoIniciado: 'Não Iniciado',
  EmAndamento: 'Em Andamento',
  Concluido: 'Concluído',
};

export function FarolBadge({ farol }) {
  return (
    <Badge bg={farolColors[farol] || 'secondary'} className="me-1">
      Farol: {farol}
    </Badge>
  );
}

export function PrioridadeBadge({ prioridade }) {
  return (
    <Badge bg={prioridadeBadge[prioridade] || 'secondary'} className="me-1">
      Prioridade: {prioridade}
    </Badge>
  );
}

export function StatusBadge({ status }) {
  const label = statusLabels[status] || status;
  let bg = 'secondary';
  if (status === 'Concluido') bg = 'success';
  else if (status === 'EmAndamento') bg = 'primary';

  return <Badge bg={bg}>Status: {label}</Badge>;
}

export function ProgressoBar({ progresso }) {
  let variant = 'primary';
  if (progresso >= 100) variant = 'success';
  else if (progresso >= 50) variant = 'info';
  else if (progresso > 0) variant = 'warning';

  return (
    <ProgressBar
      now={progresso}
      label={`${Math.round(progresso)}%`}
      variant={variant}
      style={{ minWidth: 100 }}
    />
  );
}

export function TagBadge({ label, show }) {
  if (!show) return null;
  return (
    <Badge bg="dark" className="me-1">
      {label}
    </Badge>
  );
}
