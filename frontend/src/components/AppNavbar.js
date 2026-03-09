import React from 'react';
import { Link, useLocation } from 'react-router-dom';
import { Header, Button, Tag } from '@genial/design-system';

export default function AppNavbar({ dbConfigurado, dbCaminho, onDisconnect }) {
  const location = useLocation();

  const navLinks = [
    { to: '/', label: 'Dashboard', requiresDb: true },
    { to: '/ciclos', label: 'Ciclos', requiresDb: true },
    { to: '/times', label: 'Times', requiresDb: true },
    { to: '/config', label: '⚙️ Configuração', requiresDb: false },
  ];

  return (
    <Header
      data-testid="ds-header-main"
      leading={{
        logo: (
          <Link to="/" style={{ textDecoration: 'none', color: 'inherit' }}>
            <span style={{ fontWeight: 700, fontSize: '1.1rem' }}>🎯 OKR Tracker</span>
          </Link>
        ),
        content: (
          <nav style={{ display: 'flex', gap: '8px', alignItems: 'center' }}>
            {navLinks.map((link) => {
              const isActive = location.pathname === link.to;
              const isDisabled = link.requiresDb && !dbConfigurado;

              if (isDisabled) {
                return (
                  <span
                    key={link.to}
                    style={{ opacity: 0.35, cursor: 'not-allowed', fontSize: '0.9rem', padding: '4px 8px' }}
                  >
                    {link.label}
                  </span>
                );
              }

              return (
                <Link
                  key={link.to}
                  to={link.to}
                  style={{
                    textDecoration: 'none',
                    color: 'inherit',
                    fontSize: '0.9rem',
                    padding: '4px 8px',
                    borderRadius: '4px',
                    fontWeight: isActive ? 600 : 400,
                    opacity: isActive ? 1 : 0.75,
                  }}
                >
                  {link.label}
                </Link>
              );
            })}
          </nav>
        ),
      }}
      trailing={
        <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
          {dbConfigurado ? (
            <>
              <Tag
                data-testid="ds-tag-db-status"
                label="Conectado"
                color="success"
                selected
              />
              <span
                style={{ fontSize: '0.75rem', opacity: 0.7, maxWidth: 250, overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}
                title={dbCaminho}
              >
                {dbCaminho}
              </span>
              <Button
                data-testid="ds-button-disconnect"
                variant="outline-secondary"
                size="sm"
                onClick={onDisconnect}
              >
                ✕ Desconectar
              </Button>
            </>
          ) : (
            <Tag
              data-testid="ds-tag-db-offline"
              label="Nenhuma base carregada"
              color="warning"
              selected
            />
          )}
        </div>
      }
    />
  );
}
