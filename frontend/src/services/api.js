const API_BASE = '/api';

async function request(url, options = {}) {
  const config = {
    headers: { 'Content-Type': 'application/json' },
    ...options,
  };

  if (config.body && typeof config.body === 'object') {
    config.body = JSON.stringify(config.body);
  }

  const response = await fetch(`${API_BASE}${url}`, config);
  const data = await response.json();
  return data;
}

// --- Configuração ---
export const obterStatusDatabase = () => request('/config/database', { method: 'GET' });
export const configurarDatabase = (databasePath) =>
  request('/config/database', { method: 'POST', body: { databasePath } });
export const desconectarDatabase = () =>
  request('/config/database', { method: 'DELETE' });

// --- Ciclos ---
export const listarCiclos = () => request('/ciclos');
export const criarCiclo = (nome) =>
  request('/ciclos', { method: 'POST', body: { nome } });
export const atualizarCiclo = (id, nome) =>
  request(`/ciclos/${id}`, { method: 'PUT', body: { nome } });
export const excluirCiclo = (id) =>
  request(`/ciclos/${id}`, { method: 'DELETE' });

// --- Times ---
export const listarTimes = () => request('/times');
export const criarTime = (nome, descricao) =>
  request('/times', { method: 'POST', body: { nome, descricao } });
export const atualizarTime = (id, nome, descricao) =>
  request(`/times/${id}`, { method: 'PUT', body: { nome, descricao } });
export const excluirTime = (id) =>
  request(`/times/${id}`, { method: 'DELETE' });

// --- OKRs ---
export const listarOKRs = (cicloId, timeId) =>
  request(`/okr?cicloId=${cicloId}&timeId=${timeId}`);

// --- Objetivos ---
export const criarObjetivo = (dados) =>
  request('/objetivos', { method: 'POST', body: dados });
export const atualizarObjetivo = (id, dados) =>
  request(`/objetivos/${id}`, { method: 'PUT', body: dados });

// --- Key Results ---
export const criarKR = (dados) =>
  request('/krs', { method: 'POST', body: dados });
export const atualizarKR = (id, dados) =>
  request(`/krs/${id}`, { method: 'PUT', body: dados });
export const atualizarProgressoKR = (id, progresso) =>
  request(`/krs/${id}/progresso`, { method: 'PUT', body: { progresso } });
export const excluirKR = (id) =>
  request(`/krs/${id}`, { method: 'DELETE' });

// --- Comentários ---
export const criarComentario = (dados) =>
  request('/comentarios', { method: 'POST', body: dados });

// --- Fatos Relevantes ---
export const criarFatoRelevante = (dados) =>
  request('/fatos-relevantes', { method: 'POST', body: dados });

// --- Riscos ---
export const criarRisco = (dados) =>
  request('/riscos', { method: 'POST', body: dados });

// --- Export ---
export const exportarAdaptiveCard = (cicloId, timeId) =>
  request(`/export/adaptive-card?cicloId=${cicloId}&timeId=${timeId}`);
