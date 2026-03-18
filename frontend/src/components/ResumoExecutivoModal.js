import React from 'react';
import {
  Modal as DSModal,
  ModalHeader,
  ModalBody,
  Button as DSButton,
  Snackbar,
} from '@genial/design-system';

const FAROL_EMOJI = { Verde: '🟢', Amarelo: '🟡', Vermelho: '🔴' };

const thStyle = {
  padding: '5px 10px',
  textAlign: 'left',
  borderBottom: '2px solid #d1d5db',
  fontWeight: 700,
  backgroundColor: '#f3f4f6',
  whiteSpace: 'nowrap',
};

const tdStyle = {
  padding: '5px 10px',
  borderBottom: '1px solid #e5e7eb',
  verticalAlign: 'top',
};

function KRTable({ krs }) {
  if (!krs || krs.length === 0) return null;
  return (
    <table style={{ width: '100%', borderCollapse: 'collapse', marginTop: '6px', marginBottom: '10px', fontSize: '0.82rem' }}>
      <thead>
        <tr>
          <th style={thStyle}>KR</th>
          <th style={{ ...thStyle, width: '100px', textAlign: 'center' }}>Progresso</th>
          <th style={{ ...thStyle, width: '120px', textAlign: 'center' }}>Farol</th>
        </tr>
      </thead>
      <tbody>
        {krs.map((kr, idx) => {
          const concluido = kr.status === 'Concluido' || kr.status === 'Concluído';
          const rowBg = concluido
            ? '#d4f8e8'
            : kr.farol === 'Vermelho'
            ? '#fde2e1'
            : kr.farol === 'Amarelo'
            ? '#fff7d6'
            : 'transparent';
          return (
            <tr key={kr.id || idx} style={{ backgroundColor: rowBg }}>
              <td style={tdStyle}>{kr.titulo}</td>
              <td style={{ ...tdStyle, textAlign: 'center' }}>{Number(kr.progresso ?? 0).toFixed(0)}%</td>
              <td style={{ ...tdStyle, textAlign: 'center' }}>
                {FAROL_EMOJI[kr.farol] ?? ''} {kr.farol}
              </td>
            </tr>
          );
        })}
      </tbody>
    </table>
  );
}

export default function ResumoExecutivoModal({
  open,
  objetivos = [],
  cicloNome = '',
  projetoNome = '',
  copiado,
  onClose,
  onExport,
  onCopiedClose,
}) {
  return (
    <>
      <DSModal data-testid="ds-modal-resumo" open={open} onClose={onClose} size="xl">
        <ModalHeader title="📊 Resumo Executivo" onClose={onClose} />
        <ModalBody>
          <div
            style={{
              fontFamily: 'Arial, sans-serif',
              fontSize: '0.85rem',
              maxHeight: '540px',
              overflowY: 'auto',
              border: '1px solid #d5dae3',
              borderRadius: '6px',
              padding: '16px',
              backgroundColor: '#ffffff',
            }}
          >
            {/* Cabeçalho projeto/ciclo */}
            {(projetoNome || cicloNome) && (
              <div style={{ marginBottom: '16px', color: '#6b7280', fontSize: '0.8rem', display: 'flex', gap: '16px', flexWrap: 'wrap' }}>
                {projetoNome && <span><strong>Projeto:</strong> {projetoNome}</span>}
                {cicloNome && <span><strong>Ciclo:</strong> {cicloNome}</span>}
              </div>
            )}

            {objetivos.length === 0 && (
              <p style={{ color: '#9ca3af' }}>Nenhum OKR cadastrado para este ciclo/projeto.</p>
            )}

            {objetivos.map((obj, objIdx) => {
              const objConcluido = obj.status === 'Concluido' || obj.status === 'Concluído';
              const objBg = objConcluido
                ? '#d4f8e8'
                : obj.farol === 'Vermelho'
                ? '#fde2e1'
                : obj.farol === 'Amarelo'
                ? '#fff7d6'
                : '#f9fafb';

              return (
                <div
                  key={obj.id || objIdx}
                  style={{
                    marginBottom: '20px',
                    borderRadius: '8px',
                    border: '1px solid #e5e7eb',
                    overflow: 'hidden',
                  }}
                >
                  {/* Cabeçalho do objetivo */}
                  <div style={{ backgroundColor: objBg, padding: '10px 14px', borderBottom: '1px solid #e5e7eb' }}>
                    <div style={{ fontWeight: 700, fontSize: '0.92rem', marginBottom: '6px' }}>
                      🎯 {obj.titulo}
                    </div>
                    <div style={{ display: 'flex', gap: '14px', flexWrap: 'wrap', color: '#374151', fontSize: '0.8rem' }}>
                      <span>📊 <strong>{Number(obj.progresso ?? 0).toFixed(0)}%</strong></span>
                      <span>{FAROL_EMOJI[obj.farol] ?? ''} {obj.farol}</span>
                      <span>Status: {obj.status}</span>
                    </div>
                  </div>

                  <div style={{ padding: '10px 14px', backgroundColor: '#ffffff' }}>
                    {/* Descrição */}
                    {obj.descricao && (
                      <p style={{ marginBottom: '8px', color: '#374151' }}>
                        <strong>📝 Descrição:</strong> {obj.descricao}
                      </p>
                    )}

                    {/* Valor para o negócio */}
                    {obj.valor && (
                      <p style={{ marginBottom: '8px', color: '#374151' }}>
                        <strong>💎 Valor:</strong> {obj.valor}
                      </p>
                    )}

                    {/* Tabela de KRs */}
                    {obj.keyResults && obj.keyResults.length > 0 && (
                      <div style={{ marginBottom: '10px' }}>
                        <strong style={{ fontSize: '0.82rem', color: '#374151' }}>Key Results</strong>
                        <KRTable krs={obj.keyResults} />
                      </div>
                    )}

                    {/* Fatos Relevantes */}
                    {obj.fatosRelevantes && obj.fatosRelevantes.length > 0 && (
                      <div style={{ marginBottom: '8px' }}>
                        <strong style={{ fontSize: '0.82rem' }}>📝 Fatos Relevantes</strong>
                        <ul style={{ margin: '4px 0 0 16px', padding: 0 }}>
                          {obj.fatosRelevantes.map((f, fi) => (
                            <li key={fi} style={{ marginBottom: '2px', color: '#374151' }}>
                              {f.texto}
                            </li>
                          ))}
                        </ul>
                      </div>
                    )}

                    {/* Riscos */}
                    {obj.riscos && obj.riscos.length > 0 && (
                      <div style={{ marginBottom: '4px' }}>
                        <strong style={{ fontSize: '0.82rem' }}>⚠️ Riscos</strong>
                        <ul style={{ margin: '4px 0 0 16px', padding: 0 }}>
                          {obj.riscos.map((r, ri) => (
                            <li key={ri} style={{ marginBottom: '2px', color: '#374151' }}>
                              {r.descricao}{r.impacto ? ` (Impacto: ${r.impacto})` : ''}
                            </li>
                          ))}
                        </ul>
                      </div>
                    )}
                  </div>
                </div>
              );
            })}
          </div>

          <div style={{ display: 'flex', gap: '8px', justifyContent: 'flex-end', marginTop: '16px' }}>
            <DSButton data-testid="ds-button-copiar-resumo" variant="primary" onClick={onExport}>
              ⬇️ Exportar HTML
            </DSButton>
            <DSButton data-testid="ds-button-fechar-resumo" variant="secondary" onClick={onClose}>
              Fechar
            </DSButton>
          </div>
        </ModalBody>
      </DSModal>

      {copiado && (
        <Snackbar
          data-testid="ds-snackbar-copiado"
          type="success"
          message="✅ Relatório HTML exportado com sucesso!"
          duration={3000}
          onClose={onCopiedClose}
          open={copiado}
          onOpenChange={(isOpen) => {
            if (!isOpen) onCopiedClose();
          }}
        />
      )}
    </>
  );
}
