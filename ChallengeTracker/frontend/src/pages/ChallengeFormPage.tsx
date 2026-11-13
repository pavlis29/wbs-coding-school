import { Link, useNavigate, useParams } from 'react-router-dom';
import { ArrowLeft } from 'lucide-react';
import { useSaveChallenge } from '../actions/mutations';
import { ChallengeForm } from '../components/ChallengeForm';
import { useChallenge } from '../data/queries';
import { ErrorNotice } from '../components/ErrorNotice';
import { useAuth } from '../contexts/AuthContext';
import type { ChallengeInput } from '../types';

export function CreateChallengePage() {
  const navigate = useNavigate();
  const save = useSaveChallenge();
  async function submit(input: ChallengeInput) {
    const challenge = await save.mutateAsync(input);
    navigate(`/challenges/${challenge.id}`);
  }
  return <><Link className="back-link" to="/"><ArrowLeft size={16} /> Back to overview</Link><div className="page-head"><div><span className="eyebrow">CHALLENGE STUDIO</span><h1>Create a challenge</h1><p>Great things start with a clear goal.</p></div></div><section className="panel form-panel"><ChallengeForm saving={save.isPending} onSubmit={submit} onCancel={() => navigate('/')} /></section></>;
}

export function EditChallengePage() {
  const { id = '' } = useParams();
  const { user } = useAuth();
  const navigate = useNavigate();
  const query = useChallenge(id);
  const save = useSaveChallenge(id);
  if (query.isPending) return <p className="loading">Loading challenge…</p>;
  if (query.error) return <ErrorNotice error={query.error} retry={() => { void query.refetch(); }} />;
  if (query.data.ownerId !== user!.id) return <section className="panel"><h1>Only the owner can edit this challenge</h1><Link to={`/challenges/${id}`}>Back to challenge</Link></section>;
  async function submit(input: ChallengeInput) {
    await save.mutateAsync(input);
    navigate(`/challenges/${id}`);
  }
  return <><Link className="back-link" to={`/challenges/${id}`}><ArrowLeft size={16} /> Back to challenge</Link><div className="page-head"><div><span className="eyebrow">CHALLENGE STUDIO</span><h1>Edit your challenge</h1><p>Keep your goal clear and achievable.</p></div></div><section className="panel form-panel"><ChallengeForm key={id} initial={query.data} saving={save.isPending} onSubmit={submit} onCancel={() => navigate(`/challenges/${id}`)} /></section></>;
}
