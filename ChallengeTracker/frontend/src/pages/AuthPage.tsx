import { useState } from 'react';
import type { FormEvent } from 'react';
import { Link, Navigate, useLocation } from 'react-router-dom';
import { ArrowRight, Flame } from 'lucide-react';
import { useAuth } from '../contexts/AuthContext';
import { ErrorNotice } from '../components/ErrorNotice';

export function AuthPage({ register = false }: { register?: boolean }) {
  const auth = useAuth();
  const location = useLocation();
  const from = typeof location.state?.from === 'string' && location.state.from.startsWith('/') && !location.state.from.startsWith('//')
    ? location.state.from : '/';
  const [displayName, setDisplayName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<unknown>(null);
  if (auth.loading) return <div className="session-screen" role="status">Opening your workspace…</div>;
  if (auth.user) return <Navigate to={from} replace />;

  async function submit(event: FormEvent) {
    event.preventDefault();
    setSaving(true);
    setError(null);
    try {
      if (register) await auth.register(displayName, email, password);
      else await auth.login(email, password);
    } catch (reason) { setError(reason); }
    finally { setSaving(false); }
  }

  return <main className="auth-screen">
    <section className="auth-story"><div className="auth-brand"><Flame size={26} /><strong>challengetracker</strong></div><span className="eyebrow">YOUR GROWTH SPACE</span><h1>Small steps.<br /><span>Real progress.</span></h1><p>Pick a goal, invite your people and make a little progress every day.</p><div className="auth-example"><span>📚</span><div><strong>Read 100 pages</strong><p>A new habit begins with one page.</p><div className="progress-track"><div style={{ width: '62%' }} /></div></div></div></section>
    <section className="auth-form-panel"><span className="eyebrow">LET'S KEEP GROWING</span><h2>{register ? 'Create your account' : 'Welcome back'}</h2><p>{register ? 'Your next challenge starts here.' : 'Sign in to your challenges and daily progress.'}</p>
      <ErrorNotice error={error} />
      <form onSubmit={submit} className="auth-form">
        {register && <label className="field">Display name<input required minLength={2} maxLength={60} autoComplete="nickname" value={displayName} onChange={e => setDisplayName(e.target.value)} /></label>}
        <label className="field">Email<input required type="email" autoComplete="email" value={email} onChange={e => setEmail(e.target.value)} /></label>
        <label className="field">Password<input required type="password" autoComplete={register ? 'new-password' : 'current-password'} minLength={register ? 8 : undefined} value={password} onChange={e => setPassword(e.target.value)} /></label>
        {register && <p className="form-help">At least 8 characters, with uppercase, lowercase, a number and a symbol.</p>}
        <button className="button primary" type="submit" disabled={saving}>{saving ? 'Please wait…' : register ? 'Create account' : 'Sign in'} <ArrowRight size={17} /></button>
      </form>
      <p className="auth-switch">{register ? 'Already have an account?' : 'New here?'} <Link to={register ? '/login' : '/register'} state={{ from }}>{register ? 'Sign in' : 'Create an account'}</Link></p>
    </section>
  </main>;
}
