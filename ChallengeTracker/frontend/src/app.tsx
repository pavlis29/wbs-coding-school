import { BrowserRouter, Link, Route, Routes } from 'react-router-dom';
import { QueryClientProvider } from '@tanstack/react-query';
import { queryClient } from './data/queryClient';
import { AuthProvider } from './contexts/AuthContext';
import { ProtectedRoute } from './components/ProtectedRoute';
import { AppLayout } from './layouts/AppLayout';
import { AuthPage } from './pages/AuthPage';
import { DashboardPage } from './pages/DashboardPage';
import { ChallengeDetailPage } from './pages/ChallengeDetailPage';
import { CreateChallengePage, EditChallengePage } from './pages/ChallengeFormPage';

export default function App() {
  return <QueryClientProvider client={queryClient}><BrowserRouter><AuthProvider><Routes>
    <Route path="/login" element={<AuthPage key="login" />} />
    <Route path="/register" element={<AuthPage key="register" register />} />
    <Route element={<ProtectedRoute />}><Route element={<AppLayout />}>
      <Route index element={<DashboardPage />} />
      <Route path="create" element={<CreateChallengePage />} />
      <Route path="challenges/:id" element={<ChallengeDetailPage />} />
      <Route path="challenges/:id/edit" element={<EditChallengePage />} />
      <Route path="*" element={<section className="panel"><h1>Page not found</h1><Link className="button secondary" to="/">Back to overview</Link></section>} />
    </Route></Route>
  </Routes></AuthProvider></BrowserRouter></QueryClientProvider>;
}
