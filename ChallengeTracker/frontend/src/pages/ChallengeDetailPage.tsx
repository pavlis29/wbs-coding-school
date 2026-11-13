import { useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { Activity, ArrowLeft, ArrowRight, CheckCircle2, ClipboardList, Copy, Edit3, LockKeyhole, Plus, Trash2, Trophy, Users, X } from 'lucide-react';
import { useAuth } from '../contexts/AuthContext';
import { useChallenge, useLeaderboard, useMemberships, useProgress } from '../data/queries';
import { useChallengeAction, useJoinChallenge } from '../actions/mutations';
import { ErrorNotice } from '../components/ErrorNotice';
import { ProgressDialog } from '../components/ProgressDialog';
import { ApiError, request } from '../utils/api';
import { canEditProgress, formatDay, utcDay } from '../utils/dates';
import { formatAmount } from '../utils/numbers';
import type { Membership, ProgressEntry } from '../types';

function MembershipRow({ member, owner, ownerId, onApproved }: { member: Membership; owner: boolean; ownerId: string; onApproved: () => void }) {
  const approve = useChallengeAction(token => request<void>(`/memberships/${member.id}/approve`, token, { method: 'POST' }), member.challengeId);
  return <div>
    <div className="member-row"><span className="avatar">{member.displayName.slice(0, 2).toUpperCase()}</span><div className="member-name"><strong>{member.displayName}</strong><span>{member.userId === ownerId ? 'Owner' : member.status === 'Pending' ? 'Awaiting approval' : 'Member'}</span></div>
      {owner && member.status === 'Pending' && <button className="button secondary" disabled={approve.isPending} onClick={() => approve.mutate(undefined, { onSuccess: onApproved })}>{approve.isPending ? 'Approving…' : 'Approve'}</button>}
    </div><ErrorNotice error={approve.error} />
  </div>;
}

function ProgressRow({ entry, userId, unit, onEdit, onDeleted }: { entry: ProgressEntry; userId: string; unit: string; onEdit: () => void; onDeleted: () => void }) {
  const remove = useChallengeAction(token => request<void>(`/progress-entries/${entry.id}`, token, { method: 'DELETE' }), entry.challengeId);
  const pending = entry.id.startsWith('pending-');
  const own = entry.userId === userId;
  function deleteEntry() {
    if (window.confirm('Delete this progress entry?')) remove.mutate(undefined, { onSuccess: onDeleted });
  }
  return <div>
    <div className={`entry-row${pending ? ' entry-pending' : ''}`}><div className="entry-icon"><CheckCircle2 size={19} /></div><div className="entry-body"><strong>{entry.displayName} <span>logged {formatAmount(entry.amount)} {unit}</span></strong><p>{entry.note || 'Progress logged'}</p><small>{formatDay(entry.day)}{pending ? ' · Saving…' : own && !canEditProgress(entry.createdAt) ? ' · Editing closed after 24 hours' : ''}</small></div>
      {own && !pending && <div className="entry-actions"><button aria-label={`Edit progress from ${entry.day}`} title={canEditProgress(entry.createdAt) ? 'Edit progress' : 'Editing is available for 24 hours after creation'} onClick={onEdit} disabled={!canEditProgress(entry.createdAt) || remove.isPending}><Edit3 size={17} /></button><button aria-label={`Delete progress from ${entry.day}`} title="Delete progress" onClick={deleteEntry} disabled={remove.isPending}><Trash2 size={17} /></button></div>}
    </div><ErrorNotice error={remove.error} />
  </div>;
}

export function ChallengeDetailPage() {
  const { id = '' } = useParams();
  const { user } = useAuth();
  const navigate = useNavigate();
  const challenge = useChallenge(id);
  const item = challenge.data;
  const owner = item?.ownerId === user!.id;
  const active = item?.membership?.status === 'Active';
  const readable = Boolean(item && (item.visibility === 'Public' || owner || active));
  const progress = useProgress(id, readable);
  const leaderboard = useLeaderboard(id, readable);
  const members = useMemberships(id, Boolean(item && (owner || item.membership)));
  const join = useJoinChallenge(id);
  const leave = useChallengeAction(token => request<void>(`/memberships/${item?.membership?.id}`, token, { method: 'DELETE' }), id);
  const remove = useChallengeAction(token => request<void>(`/challenges/${id}`, token, { method: 'DELETE' }), id);
  const start = useChallengeAction(token => request<void>(`/challenges/${id}/start`, token, { method: 'POST' }), id);
  const complete = useChallengeAction(token => request<void>(`/challenges/${id}/complete`, token, { method: 'POST' }), id);
  const [notice, setNotice] = useState('');
  const [dialog, setDialog] = useState<{ entry?: ProgressEntry } | null>(null);

  if (challenge.isPending) return <p className="loading" role="status">Loading challenge…</p>;
  if (challenge.error || !item) {
    const forbidden = challenge.error instanceof ApiError && challenge.error.status === 403;
    return <><Link className="back-link" to="/"><ArrowLeft size={16} /> Back to overview</Link>
      {forbidden ? <section className="panel access-panel"><LockKeyhole size={32} /><h1>This challenge is private</h1><p>Request access and the owner can approve your membership.</p><button className="button primary" disabled={join.isPending} onClick={() => join.mutate()}>{join.isPending ? 'Sending request…' : 'Request access'}</button><ErrorNotice error={join.error} /></section>
        : <ErrorNotice error={challenge.error || new Error('Challenge not found.')} retry={() => { void challenge.refetch(); }} />}
    </>;
  }

  const entries = progress.data || [];
  const todayAmount = entries.filter(entry => entry.userId === user!.id && entry.day === utcDay()).reduce((sum, entry) => sum + entry.amount, 0);
  const percent = Math.min(100, Math.round(item.total / item.target * 100));
  const invite = `${window.location.origin}/challenges/${id}`;
  const busy = start.isPending || complete.isPending || remove.isPending || leave.isPending || join.isPending;

  function deleteChallenge() {
    if (window.confirm(`Delete “${item!.title}” and all its progress entries?`)) remove.mutate(undefined, { onSuccess: () => navigate('/') });
  }
  function leaveChallenge() {
    if (window.confirm(item!.membership?.status === 'Pending' ? 'Cancel your membership request?' : 'Leave this challenge? Your previous progress will remain.')) leave.mutate(undefined, { onSuccess: () => navigate('/') });
  }
  async function copyInvite() {
    try { await navigator.clipboard.writeText(invite); setNotice('Invitation link copied.'); }
    catch { setNotice('Select the invitation link below and copy it.'); }
  }

  return <>
    <Link className="back-link" to="/"><ArrowLeft size={16} /> Back to overview</Link>
    {notice && <div className="toast success" role="status"><CheckCircle2 size={17} /><span>{notice}</span><button onClick={() => setNotice('')} aria-label="Dismiss notification"><X size={15} /></button></div>}
    <ErrorNotice error={join.error || leave.error || remove.error || start.error || complete.error} />
    <div className="detail-head"><div><span className="eyebrow">{item.category.toUpperCase()} · {item.visibility.toUpperCase()} CHALLENGE</span><h1>{item.title}</h1><p>{item.description || 'Every step forward matters.'}</p><p className="owner-caption">Created by {item.ownerName}</p></div><span className={`status status-${item.status.toLowerCase()}`}>{item.status}</span></div>
    <div className="detail-actions">
      {owner && <><Link className="button secondary" to={`/challenges/${id}/edit`}><Edit3 size={16} /> Edit challenge</Link><button className="button danger-outline" disabled={busy} onClick={deleteChallenge}><Trash2 size={16} /> Delete challenge</button></>}
      {!item.membership && item.status === 'Open' && <button className="button primary" disabled={busy} onClick={() => join.mutate(undefined, { onSuccess: member => setNotice(member.status === 'Pending' ? 'Request sent. Waiting for the owner to approve.' : 'You joined the challenge!') })}>{join.isPending ? 'Joining…' : item.visibility === 'Private' ? 'Request to join' : 'Join challenge'} <Plus size={16} /></button>}
      {!item.membership && item.status !== 'Open' && <p className="form-help">Joining is closed for this challenge.</p>}
      {item.membership && !owner && <button className="button secondary" disabled={busy} onClick={leaveChallenge}>{item.membership.status === 'Pending' ? 'Cancel request' : 'Leave challenge'}</button>}
      {owner && item.status === 'Open' && <button className="button primary" disabled={busy} onClick={() => start.mutate(undefined, { onSuccess: () => setNotice('Challenge started!') })}>Start challenge <ArrowRight size={16} /></button>}
      {owner && item.status === 'Running' && <button className="button primary" disabled={busy} onClick={() => complete.mutate(undefined, { onSuccess: () => setNotice('Challenge completed!') })}>Mark completed <CheckCircle2 size={16} /></button>}
    </div>
    {item.membership?.status === 'Pending' && <div className="pending-banner" role="status">Your membership is awaiting approval. You can log progress after the owner approves your request.</div>}
    {owner && item.visibility === 'Private' && <div className="invite-panel"><label className="field">Invitation link<input readOnly value={invite} onFocus={event => event.target.select()} /></label><button className="button secondary" onClick={copyInvite}><Copy size={16} /> Copy invite link</button></div>}
    <div className="detail-grid">
      <section className="panel detail-summary"><span className="eyebrow">{readable ? 'TEAM PROGRESS' : 'CHALLENGE GOAL'}</span><div className="percent-line"><strong>{readable ? `${percent}%` : formatAmount(item.target)}</strong><span>{readable ? `${formatAmount(item.total)} of ${formatAmount(item.target)} ${item.unit}` : item.unit}</span></div>
        {readable && <div className="progress-track large"><div style={{ width: `${percent}%` }} /></div>}
        <div className="meta-grid"><div><span>Starts</span><strong>{formatDay(item.startDate)}</strong></div><div><span>Ends</span><strong>{formatDay(item.endDate)}</strong></div><div><span>Your today (UTC)</span><strong data-testid="today-progress">{active ? `${formatAmount(todayAmount)} ${item.unit}` : 'Join to take part'}</strong></div></div>
      </section>
      <section className="panel leaderboard"><div className="panel-title"><div><Trophy size={19} color="#e8ad61" /><h3>Leaderboard</h3></div></div>
        {!readable ? <p className="empty-small">Available to active members.</p> : <><ErrorNotice error={leaderboard.error} retry={() => { void leaderboard.refetch(); }} />{leaderboard.isPending ? <p className="empty-small">Loading results…</p> : leaderboard.data?.length ? leaderboard.data.map((leader, i) => <div key={leader.userId} className="leader-row"><span className="rank">{i + 1}</span><span className="leader-name">{leader.displayName}</span><strong>{formatAmount(leader.total)} {item.unit}</strong></div>) : !leaderboard.error && <p className="empty-small">No progress yet. Be the first!</p>}</>}
      </section>
    </div>
    {(owner || item.membership) && <section className="panel log-panel"><div className="panel-title"><div><Users size={20} /><h3>Members</h3></div></div><ErrorNotice error={members.error} retry={() => { void members.refetch(); }} />{members.isPending ? <p className="empty-small">Loading members…</p> : (members.data || []).map(member => <MembershipRow key={member.id} member={member} owner={owner} ownerId={item.ownerId} onApproved={() => setNotice('Membership approved.')} />)}</section>}
    {readable && <section className="panel log-panel"><div className="panel-title"><div><Activity size={20} /><h3>Progress journal</h3></div>{active && <button className="button primary" onClick={() => setDialog({})}><Plus size={16} /> Log progress</button>}</div>
      <ErrorNotice error={progress.error} retry={() => { void progress.refetch(); }} />
      {progress.isPending ? <p className="loading">Loading progress…</p> : entries.length ? <div className="entry-list">{[...entries].sort((a, b) => b.day.localeCompare(a.day) || b.createdAt.localeCompare(a.createdAt)).map(entry => <ProgressRow key={entry.id} entry={entry} userId={user!.id} unit={item.unit} onEdit={() => setDialog({ entry })} onDeleted={() => setNotice('Progress entry deleted.')} />)}</div>
        : !progress.error && <div className="empty-journal"><ClipboardList size={31} /><strong>No progress logged yet</strong><p>{active ? 'Add your first entry and start tracking your progress.' : 'Join this challenge to add your own progress.'}</p></div>}
    </section>}
    {dialog && <ProgressDialog challenge={item} entry={dialog.entry} onClose={() => setDialog(null)} onSaved={() => { setDialog(null); setNotice('Progress saved!'); }} />}
  </>;
}
