import mockApi from './mockApi';
const useMock = (import.meta.env.VITE_USE_MOCK_API || 'true') === 'true';

async function fetchJson(path: string, opts?: RequestInit) {
  const base = import.meta.env.VITE_API_BASE_URL || '';
  const res = await fetch(base + path, opts);
  if (!res.ok) throw new Error(await res.text());
  return res.json();
}

export const api = {
  getTickets: (params?: any) => (useMock ? mockApi.getTickets(params) : fetchJson(`/tickets${buildQuery(params)}`)),
  getTicket: (id: number) => (useMock ? mockApi.getTicket(id) : fetchJson(`/tickets/${id}`)),
  createTicket: (payload: any) => (useMock ? mockApi.createTicket(payload) : fetchJson('/tickets', { method: 'POST', body: JSON.stringify(payload), headers: { 'Content-Type': 'application/json' } })),
  updateTicket: (id: number, payload: any) => (useMock ? mockApi.updateTicket(id, payload) : fetchJson(`/tickets/${id}`, { method: 'PUT', body: JSON.stringify(payload), headers: { 'Content-Type': 'application/json' } })),
  deleteTicket: (id: number) => (useMock ? mockApi.deleteTicket(id) : fetchJson(`/tickets/${id}`, { method: 'DELETE' })),
  addComment: (ticketId: number, authorId: number, authorName: string, content: string) => (useMock ? mockApi.addComment(ticketId, authorId, authorName, content) : fetchJson(`/tickets/${ticketId}/comments`, { method: 'POST', body: JSON.stringify({ authorId, authorName, content }), headers: { 'Content-Type': 'application/json' } })),
  getTeams: () => (useMock ? mockApi.getTeams() : fetchJson('/teams')),
  getUsers: () => (useMock ? mockApi.getUsers() : fetchJson('/users')),
  getDashboardStats: () => (useMock ? mockApi.getDashboardStats() : fetchJson('/dashboard/statistics'))
};

function buildQuery(params?: any) {
  if (!params) return '';
  const q = new URLSearchParams();
  Object.keys(params).forEach((k) => {
    const v = params[k];
    if (v !== undefined && v !== null) q.set(k, String(v));
  });
  const s = q.toString();
  return s ? `?${s}` : '';
}
