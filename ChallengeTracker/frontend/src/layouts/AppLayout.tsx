import { NavLink, Link, Outlet, useLocation } from 'react-router-dom';
import { ChevronRight, CirclePlus, Flame, LayoutGrid, LogOut } from 'lucide-react';
import { useAuth } from '../contexts/AuthContext';

export function AppLayout() {
  const { user, logout } = useAuth();
  const location = useLocation();
  const initials = user!.displayName.slice(0, 2).toUpperCase();
  const page = location.pathname === '/' ? 'Overview' : location.pathname === '/create' ? 'Create challenge' : 'Challenge details';
  return <div className="app">
    <aside className="sidebar">
      <Link className="brand" to="/"><span className="brand-icon"><Flame size={23} fill="currentColor" /></span><span>challenge<span className="brand-accent">tracker</span><small>YOUR GROWTH SPACE</small></span></Link>
      <div className="nav-label">WORKSPACE</div>
      <nav className="main-nav" aria-label="Main navigation">
        <NavLink to="/" end title="Overview"><LayoutGrid size={19} /><span>Overview</span></NavLink>
        <NavLink to="/create" title="Create challenge"><CirclePlus size={19} /><span>Create challenge</span></NavLink>
      </nav>
      <div className="sidebar-bottom">
        <div className="side-motivation"><span>✦ TODAY'S REMINDER</span><strong>Progress, not perfection.</strong><p>One small step today is a big step tomorrow.</p></div>
        <div className="profile"><div className="avatar">{initials}</div><div><strong>{user!.displayName}</strong><span>{user!.email}</span></div></div>
        <button className="logout-button" onClick={logout} title="Sign out"><LogOut size={17} /><span>Sign out</span></button>
      </div>
    </aside>
    <main className="main">
      <header className="topbar"><span className="crumb">Workspace <ChevronRight size={14} /> {page}</span><span className="topbar-right"><span className="demo-pill"><span /> {user!.displayName}</span><span className="top-avatar">{initials}</span></span></header>
      <div className="content"><Outlet /></div>
    </main>
  </div>;
}
