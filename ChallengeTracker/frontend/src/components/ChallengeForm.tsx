import { useState } from 'react';
import type { FormEvent } from 'react';
import { ArrowRight } from 'lucide-react';
import type { ChallengeInput, FieldErrors } from '../types';
import { ApiError, errorMessage } from '../utils/api';
import { challengeSchema } from '../utils/validation';
import { utcDay } from '../utils/dates';

interface Props {
  initial?: ChallengeInput;
  saving: boolean;
  onSubmit: (input: ChallengeInput) => Promise<unknown>;
  onCancel: () => void;
}

export function ChallengeForm({ initial, saving, onSubmit, onCancel }: Props) {
  const [form, setForm] = useState<ChallengeInput>(initial || {
    title: '', description: '', category: 'Fitness', target: 100, unit: 'km',
    startDate: utcDay(), endDate: utcDay(new Date(Date.now() + 7 * 86_400_000)), visibility: 'Public',
  });
  const [errors, setErrors] = useState<FieldErrors>({});
  const [message, setMessage] = useState('');
  function change<K extends keyof ChallengeInput>(field: K, value: ChallengeInput[K]) {
    setForm(current => ({ ...current, [field]: value }));
  }
  async function submit(event: FormEvent) {
    event.preventDefault();
    setErrors({});
    setMessage('');
    const result = challengeSchema.safeParse(form);
    if (!result.success) {
      const fields: FieldErrors = {};
      result.error.issues.forEach(issue => { fields[String(issue.path[0])] = [issue.message]; });
      setErrors(fields);
      return;
    }
    try { await onSubmit(result.data); }
    catch (error) {
      if (error instanceof ApiError) setErrors(error.errors);
      setMessage(errorMessage(error));
    }
  }
  const fieldError = (field: keyof ChallengeInput) => errors[field] && <span className="field-error" role="alert">{errors[field].join(' ')}</span>;

  return <form onSubmit={submit} className="form-grid" noValidate>
    <label className="field full">Challenge title<input required maxLength={90} placeholder="e.g. Read 100 pages this week" value={form.title} onChange={e => change('title', e.target.value)} aria-invalid={Boolean(errors.title)} />{fieldError('title')}</label>
    <label className="field full">Description<textarea maxLength={500} rows={3} placeholder="What do you want to achieve?" value={form.description} onChange={e => change('description', e.target.value)} />{fieldError('description')}</label>
    <label className="field">Category<select value={form.category} onChange={e => change('category', e.target.value)}>{['Fitness', 'Learning', 'Wellness', 'Creativity', 'Productivity', 'Other'].map(category => <option key={category}>{category}</option>)}</select>{fieldError('category')}</label>
    <label className="field">Unit<input required maxLength={24} placeholder="pages, km, hours" value={form.unit} onChange={e => change('unit', e.target.value)} aria-invalid={Boolean(errors.unit)} />{fieldError('unit')}</label>
    <label className="field">Target amount<input required type="number" step="any" min="0" max="1000000000" value={form.target} onChange={e => change('target', Number(e.target.value))} aria-invalid={Boolean(errors.target)} />{fieldError('target')}</label>
    <label className="field">Visibility<select value={form.visibility} onChange={e => change('visibility', e.target.value as ChallengeInput['visibility'])}><option value="Public">Public — anyone can join</option><option value="Private">Private — approve requests</option></select>{fieldError('visibility')}</label>
    <label className="field">Start date<input required type="date" value={form.startDate} onChange={e => change('startDate', e.target.value)} aria-invalid={Boolean(errors.startDate)} />{fieldError('startDate')}</label>
    <label className="field">End date<input required type="date" value={form.endDate} min={form.startDate} onChange={e => change('endDate', e.target.value)} aria-invalid={Boolean(errors.endDate)} />{fieldError('endDate')}</label>
    {form.visibility === 'Private' && <p className="form-help full">Share the challenge link with friends. You decide whose requests to approve.</p>}
    {message && <p className="form-error full" role="alert">{message}</p>}
    <div className="modal-actions full"><button className="button secondary" type="button" disabled={saving} onClick={onCancel}>Cancel</button><button className="button primary" disabled={saving} type="submit">{saving ? 'Saving…' : initial ? 'Save changes' : 'Create challenge'} <ArrowRight size={16} /></button></div>
  </form>;
}
