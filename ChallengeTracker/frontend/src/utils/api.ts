import type { FieldErrors } from '../types';

const baseUrl = (import.meta.env.VITE_API_BASE_URL || 'http://localhost:5080').replace(/\/$/, '');

export class ApiError extends Error {
  constructor(
    public status: number,
    message: string,
    public errors: FieldErrors = {},
    public retryAfter?: string,
  ) {
    super(message);
    this.name = 'ApiError';
  }
}

export async function request<T>(path: string, token: string | null, options: RequestInit = {}): Promise<T> {
  const headers = new Headers(options.headers);
  if (options.body) headers.set('Content-Type', 'application/json');
  if (token) headers.set('Authorization', `Bearer ${token}`);

  let response: Response;
  try {
    response = await fetch(`${baseUrl}${path}`, { ...options, headers });
  } catch {
    throw new ApiError(0, 'Cannot connect right now. Check your connection and try again.');
  }

  if (!response.ok) {
    const problem = await response.json().catch(() => ({}));
    if (response.status === 401 && token) window.dispatchEvent(new Event('session-expired'));
    const errors: FieldErrors = {};
    for (const [key, values] of Object.entries(problem.errors || {})) {
      const field = key.split('.').pop() || key;
      errors[field.charAt(0).toLowerCase() + field.slice(1)] = Array.isArray(values)
        ? values.map(String) : [String(values)];
    }
    const retryAfter = response.headers.get('Retry-After') || undefined;
    const message = response.status === 429
      ? `Too many requests. ${retryAfter && /^\d+$/.test(retryAfter) ? `Try again in ${retryAfter} seconds.` : 'Please wait a moment and try again.'}`
      : problem.detail || problem.title || 'The request could not be completed.';
    throw new ApiError(response.status, message, errors, retryAfter);
  }
  if (response.status === 204) return undefined as T;
  return response.json() as Promise<T>;
}

export function errorMessage(error: unknown): string {
  return error instanceof Error ? error.message : 'Something went wrong. Please try again.';
}
