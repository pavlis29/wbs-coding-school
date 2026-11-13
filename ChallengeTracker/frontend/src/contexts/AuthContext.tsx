import { createContext, useCallback, useContext, useEffect, useState } from 'react';
import type { ReactNode } from 'react';
import type { AuthResponse, User } from '../types';
import { ApiError, errorMessage, request } from '../utils/api';
import { queryClient } from '../data/queryClient';

const storageKey = 'challenge-tracker-token';

interface AuthState {
  user: User | null;
  token: string | null;
  loading: boolean;
  error: string;
  login: (email: string, password: string) => Promise<void>;
  register: (displayName: string, email: string, password: string) => Promise<void>;
  logout: () => void;
  retry: () => void;
}

const AuthContext = createContext<AuthState | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState<string | null>(() => localStorage.getItem(storageKey));
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(Boolean(token));
  const [error, setError] = useState('');
  const [attempt, setAttempt] = useState(0);

  const logout = useCallback(() => {
    localStorage.removeItem(storageKey);
    setToken(null);
    setUser(null);
    setError('');
    queryClient.clear();
  }, []);

  useEffect(() => {
    window.addEventListener('session-expired', logout);
    return () => window.removeEventListener('session-expired', logout);
  }, [logout]);

  useEffect(() => {
    if (!token) { setLoading(false); return; }
    let cancelled = false;
    setLoading(true);
    setError('');
    request<User>('/auth/me', token)
      .then(value => { if (!cancelled) setUser(value); })
      .catch(reason => {
        if (cancelled) return;
        if (reason instanceof ApiError && reason.status === 401) logout();
        else setError(errorMessage(reason));
      })
      .finally(() => { if (!cancelled) setLoading(false); });
    return () => { cancelled = true; };
  }, [token, attempt, logout]);

  function acceptSession(session: AuthResponse) {
    queryClient.clear();
    localStorage.setItem(storageKey, session.token);
    setUser(session.user);
    setToken(session.token);
    setError('');
  }

  async function login(email: string, password: string) {
    acceptSession(await request<AuthResponse>('/auth/login', null, {
      method: 'POST', body: JSON.stringify({ email: email.trim(), password }),
    }));
  }

  async function register(displayName: string, email: string, password: string) {
    acceptSession(await request<AuthResponse>('/auth/register', null, {
      method: 'POST', body: JSON.stringify({ displayName: displayName.trim(), email: email.trim(), password }),
    }));
  }

  return <AuthContext.Provider value={{ user, token, loading, error, login, register, logout, retry: () => setAttempt(value => value + 1) }}>
    {children}
  </AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) throw new Error('AuthProvider is required.');
  return context;
}
