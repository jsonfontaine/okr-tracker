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
export const criarCiclo = (payload) => {
  // Se for string (compatibilidade), converter para objeto
  const body = typeof payload === 'string' ? { nome: payload } : payload;
  return request('/ciclos', { method: 'POST', body });
};
export const atualizarCiclo = (id, payload) => {
  // Se for string (compatibilidade), converter para objeto
  const body = typeof payload === 'string' ? { nome: payload } : payload;
  return request(`/ciclos/${id}`, { method: 'PUT', body });
};
export const excluirCiclo = (id) =>
  request(`/ciclos/${id}`, { method: 'DELETE' });

// --- Projetos ---
export const listarProjetos = () => request('/projetos');
export const criarProjeto = (nome, descricao) =>
  request('/projetos', { method: 'POST', body: { nome, descricao } });
export const atualizarProjeto = (id, nome, descricao) =>
  request(`/projetos/${id}`, { method: 'PUT', body: { nome, descricao } });
export const excluirProjeto = (id) =>
  request(`/projetos/${id}`, { method: 'DELETE' });

// --- OKRs ---
export const listarOKRs = (cicloId, projetoId) =>
  request(`/okr?cicloId=${cicloId}&projetoId=${projetoId}`);

// --- Objetivos ---
export const criarObjetivo = (dados) =>
  request('/objetivos', { method: 'POST', body: dados });
export const atualizarObjetivo = (id, dados) =>
  request(`/objetivos/${id}`, { method: 'PUT', body: dados });
export const excluirObjetivo = (id) =>
  request(`/objetivos/${id}`, { method: 'DELETE' });

// --- Key Results ---
export const criarKR = (dados) =>
  request('/krs', { method: 'POST', body: dados });
export const atualizarKR = (id, dados) =>
  request(`/krs/${id}`, { method: 'PUT', body: dados });
export const atualizarProgressoKR = (id, progresso) =>
  request(`/krs/${id}/progresso`, { method: 'PUT', body: { progresso } });

// --- Comentários ---
export const criarComentario = (dados) =>
  request('/comentarios', { method: 'POST', body: dados });

// --- Fatos Relevantes ---
export const criarFatoRelevante = (dados) =>
  request('/fatos-relevantes', { method: 'POST', body: dados });

// --- Riscos ---
export const criarRisco = (dados) =>
  request('/riscos', { method: 'POST', body: dados });

