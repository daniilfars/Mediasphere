const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';

let getAccessToken = () => null;

export const setTokenGetter = (getter) => {
  getAccessToken = getter;
};

export async function request(path, options = {}) {
  const token = getAccessToken();
  const headers = { ...options.headers };

  if (options.body && !(options.body instanceof FormData)) {
    headers['Content-Type'] = 'application/json';
  }

  if (token) {
    headers.Authorization = `Bearer ${token}`;
  }

  const response = await fetch(`${API_URL}${path}`, { ...options, headers });

  if (response.status === 401) throw new Error('Unauthorized');
  if (!response.ok) {
    const text = await response.text();
    throw new Error(text || `HTTP ${response.status}`);
  }
  if (response.status === 204) return null;

  const contentType = response.headers.get('content-type');
  if (contentType?.includes('application/json')) return response.json();
  return null;
}