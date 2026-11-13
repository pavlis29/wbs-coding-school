import { useState } from 'react';
import { Link } from 'react-router-dom';
import { Flame, Medal, Plus, Search, Target } from 'lucide-react';
import { useChallenges } from '../data/queries';
import { ChallengeCard } from '../components/ChallengeCard';
import { ErrorNotice } from '../components/ErrorNotice';
import { useAuth } from '../contexts/AuthContext';
import type { Challenge } from '../types';

export function DashboardPage() {
  const { user } = useAuth();
  const [search, setSearch] = useState('');
  const [status, setStatus] = useState('Open');
  const mine = useChallenges({ mine: 'true' });
  const discover = useChallenges({ visibility: 'public', ...(status ? { status: status.toLowerCase() } : {}) });
  const myItems = mine.data || [];
  const match = (challenge: Challenge) => `${challenge.title} ${challenge.category}`.toLowerCase().includes(search.toLowerCase().trim());
  const filteredMine = myItems.filter(match);
  const explore = (discover.data || []).filter(match).filter(challenge => !challenge.membership);
  const today = new Date().toLocaleDateString('en-GB', { weekday: 'long', day: 'numeric', month: 'long' }).toUpperCase();

  return <>
    <div className="page-head"><div><span className="eyebrow">{today}</span><h1>Your challenges<span className="heading-dot">.</span></h1><p>Hi {user!.displayName}. Keep your momentum going.</p></div><Link className="button primary create-top" to="/create"><Plus size={18} /> New challenge</Link></div>
    <div className="stats-grid">
      <div className="stat-card"><span><Target size={19} /> MY CHALLENGES</span><div><strong>{myItems.length}</strong><span>Goals you're part of</span></div></div>
      <div className="stat-card"><span><Flame size={19} /> IN PROGRESS</span><div><strong>{myItems.filter(item => item.status === 'Running').length}</strong><span>Keep up the streak</span></div></div>
      <div className="stat-card"><span><Medal size={19} /> COMPLETED</span><div><strong>{myItems.filter(item => item.status === 'Completed').length}</strong><span>Every win counts</span></div></div>
    </div>
    <div className="section-heading"><div><h2>My challenges</h2><p>The goals you've joined, including requests awaiting approval.</p></div><div className="search"><Search size={17} /><input aria-label="Search challenges" placeholder="Search challenges…" value={search} onChange={event => setSearch(event.target.value)} /></div></div>
    <ErrorNotice error={mine.error} retry={() => { void mine.refetch(); }} />
    {mine.isPending ? <p className="loading" role="status">Loading your challenges…</p> : <div className="cards-grid">{filteredMine.map(challenge => <ChallengeCard key={challenge.id} challenge={challenge} />)}<Link className="new-card" to="/create"><span><Plus size={27} /></span><strong>Start something new</strong><small>Set a fresh goal and challenge yourself.</small></Link></div>}
    {!mine.isPending && !mine.error && filteredMine.length === 0 && <p className="empty-results">{search ? 'No joined challenges match your search.' : 'Create a challenge or join one below to get started.'}</p>}
    <div className="section-heading"><div><h2>Discover</h2><p>Find a public challenge and make progress together.</p></div><div className="filters"><select aria-label="Discover status" value={status} onChange={event => setStatus(event.target.value)}><option value="Open">Open to join</option><option value="Running">Running</option><option value="Completed">Completed</option><option value="">All statuses</option></select></div></div>
    <ErrorNotice error={discover.error} retry={() => { void discover.refetch(); }} />
    {discover.isPending ? <p className="loading" role="status">Discovering challenges…</p> : <div className="cards-grid">{explore.map(challenge => <ChallengeCard key={challenge.id} challenge={challenge} />)}</div>}
    {!discover.isPending && !discover.error && explore.length === 0 && <section className="panel empty-journal"><Target size={28} /><strong>No new challenges here yet</strong><p>Try another filter or create one for your friends.</p></section>}
    <div className="bottom-banner"><div><span className="eyebrow">A LITTLE MOTIVATION</span><h3>Great things are done by a series of small things brought together.</h3><p>Stay consistent. You've got this.</p></div><div className="banner-art" aria-hidden="true">✦</div></div>
  </>;
}
