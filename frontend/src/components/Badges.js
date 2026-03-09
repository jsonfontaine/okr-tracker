import React from 'react';
import { Tag, ProgressLine } from '@genial/design-system';

const farolTagColor = {
  Verde: 'success',
  Amarelo: 'warning',
  Vermelho: 'error',
};

const prioridadeTagColor = {
  Alta: 'error',
  Media: 'warning',
  Baixa: 'secondary',
};

const statusLabels = {
  NaoIniciado: 'Não Iniciado',
  EmAndamento: 'Em Andamento',
  Concluido: 'Concluído',
};

const statusTagColor = {
  NaoIniciado: 'secondary',
  EmAndamento: 'primary',
  Concluido: 'success',
};

export function FarolBadge({ farol }) {
  return (
    <Tag
      data-testid="ds-tag-farol"
      label={`Farol: ${farol}`}
      color={farolTagColor[farol] || 'secondary'}
      selected
    />
  );
}

export function PrioridadeBadge({ prioridade }) {
  return (
    <Tag
      data-testid="ds-tag-prioridade"
      label={`Prioridade: ${prioridade}`}
      color={prioridadeTagColor[prioridade] || 'secondary'}
      selected
    />
  );
}

export function StatusBadge({ status }) {
  const label = statusLabels[status] || status;
  return (
    <Tag
      data-testid="ds-tag-status"
      label={`Status: ${label}`}
      color={statusTagColor[status] || 'secondary'}
      selected
    />
  );
}

export function ProgressoBar({ progresso }) {
  let color = undefined;
  if (progresso >= 100) color = 'var(--brand-accent)';
  else if (progresso >= 50) color = 'var(--brand-primary)';
  else if (progresso > 0) color = '#f0ad4e';

  return (
    <div style={{ minWidth: 100, display: 'flex', alignItems: 'center', gap: 6 }}>
      <ProgressLine
        data-testid="ds-progress"
        progress={progresso}
        color={color}
        height="8px"
      />
      <span style={{ fontSize: '0.75rem', whiteSpace: 'nowrap' }}>{Math.round(progresso)}%</span>
    </div>
  );
}

export function TagBadge({ label, show }) {
  if (!show) return null;
  return (
    <Tag
      data-testid={`ds-tag-${label.toLowerCase().replace(/\s/g, '-')}`}
      label={label}
      color="info"
      selected
    />
  );
}
