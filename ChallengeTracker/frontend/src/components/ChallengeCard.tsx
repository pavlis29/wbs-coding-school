import { Link } from 'react-router-dom';
import { CalendarDays, ChevronRight, LockKeyhole } from 'lucide-react';
import type { Challenge } from '../types';
import { formatDay } from '../utils/dates';
import { formatAmount } from '../utils/numbers';

const symbols: Record<string, string> = { Fitness: '🏃', Learning: '📚', Wellness: '🌿', Creativity: '🎨', Productivity: '⚡', Other: '✨' };

export function ChallengeCard({ challenge }: { challenge: Challenge }) {
  const percent = Math.min(100, Math.round(challenge.total / challenge.target * 100));
  return <Link className="challenge-card" to={`/challenges/${challenge.id}`}>
    <div className="card-top"><span className="category-bubble">{symbols[challenge.category] || symbols.Other}</span><span className={`status status-${challenge.status.toLowerCase()}`}>{challenge.status}</span></div>
    <div><div className="card-category">{challenge.category}{challenge.visibility === 'Private' && <LockKeyhole size={12} aria-label="Private" />}</div><h3>{challenge.title}</h3><p className="card-description">{challenge.description || 'Set a goal. Show up every day. Make it happen.'}</p></div>
    <div className="card-bottom">
      {challenge.membership?.status === 'Pending' && <p className="pending-label">Awaiting approval</p>}
      <div className="progress-label"><span>Team progress</span><strong>{formatAmount(challenge.total)} / {formatAmount(challenge.target)} {challenge.unit}</strong></div>
      <div className="progress-track"><div style={{ width: `${percent}%` }} /></div>
      <div className="card-footer"><span><CalendarDays size={14} /> Until {formatDay(challenge.endDate)}</span><span>{percent}% <ChevronRight size={15} /></span></div>
    </div>
  </Link>;
}
