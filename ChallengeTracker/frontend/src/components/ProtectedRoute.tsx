import { Navigate, Outlet, useLocation } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

export function ProtectedRoute() {
  const auth = useAuth();
  const location = useLocation();
  if (auth.loading) return <div className="session-screen" role="status">Opening your workspace…</div>;
  if (auth.error) return <div className="session-screen"><section className="panel">
    <h1>Unable to open your workspace</h1><p role="alert">{auth.error}</p>
    <div className="detail-actions"><button className="button primary" onClick={auth.retry}>Try again</button><button className="button secondary" onClick={auth.logout}>Back to sign in</button></div>
  </section></div>;
  return auth.user ? <Outlet /> : <Navigate to="/login" state={{ from: location.pathname }} replace />;
}
