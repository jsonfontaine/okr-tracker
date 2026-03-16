import React from 'react';
import { Tag } from '@genial/design-system';

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
  // Cores baseadas no progresso, usando tokens do DS/Tailwind
  let bgColor = '#e5e7eb'; // neutro (gray-200)
  let fillColor = '#22c55e'; // verde padrão
  if (progresso >= 100) fillColor = 'var(--brand-accent, #22c55e)';
  else if (progresso >= 50) fillColor = 'var(--brand-primary, #0ea5e9)';
  else if (progresso > 0) fillColor = '#fbbf24'; // amarelo
  else fillColor = '#e5e7eb';

  return (
    <div style={{ minWidth: 100, display: 'flex', alignItems: 'center', gap: 6 }}>
      <div
        data-testid="ds-progress"
        style={{
          width: 80,
          height: 8,
          background: bgColor,
          borderRadius: 4,
          overflow: 'hidden',
          flexShrink: 0,
        }}
      >
        <div
          style={{
            width: `${Math.max(0, Math.min(100, progresso))}%`,
            height: '100%',
            background: fillColor,
            transition: 'width 0.3s',
          }}
        />
      </div>
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
