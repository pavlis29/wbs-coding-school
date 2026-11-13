import { useEffect, useRef, useState } from 'react';
import type { FormEvent } from 'react';
import { CheckCircle2, X } from 'lucide-react';
import type { Challenge, FieldErrors, ProgressEntry } from '../types';
import { useSaveProgress } from '../actions/mutations';
import { ApiError, errorMessage } from '../utils/api';
import { progressSchema } from '../utils/validation';
import { utcDay } from '../utils/dates';

export function ProgressDialog({ challenge, entry, onClose, onSaved }: {
  challenge: Challenge;
  entry?: ProgressEntry;
  onClose: () => void;
  onSaved: () => void;
}) {
  const [form, setForm] = useState({ amount: entry?.amount || 1, note: entry?.note || '', day: entry?.day || utcDay() });
  const [errors, setErrors] = useState<FieldErrors>({});
  const [message, setMessage] = useState('');
  const save = useSaveProgress(challenge.id, entry);
  const panel = useRef<HTMLElement>(null);
  const closeRef = useRef(onClose);
  closeRef.current = onClose;
  const pendingRef = useRef(false);
  pendingRef.current = save.isPending;

  useEffect(() => {
    const previous = document.activeElement as HTMLElement | null;
    const overflow = document.body.style.overflow;
    document.body.style.overflow = 'hidden';
    panel.current?.querySelector<HTMLInputElement>('input')?.focus();
    function keydown(event: KeyboardEvent) {
      if (event.key === 'Escape' && !pendingRef.current) closeRef.current();
      if (event.key !== 'Tab') return;
      const controls = panel.current?.querySelectorAll<HTMLElement>('button:not(:disabled), input, textarea');
      if (!controls?.length) return;
      const first = controls[0];
      const last = controls[controls.length - 1];
      if (event.shiftKey && document.activeElement === first) { event.preventDefault(); last.focus(); }
      else if (!event.shiftKey && document.activeElement === last) { event.preventDefault(); first.focus(); }
    }
    document.addEventListener('keydown', keydown);
    return () => { document.body.style.overflow = overflow; document.removeEventListener('keydown', keydown); previous?.focus(); };
  }, []);

  async function submit(event: FormEvent) {
    event.preventDefault();
    setErrors({});
    setMessage('');
    const result = progressSchema.safeParse(form);
    if (!result.success) {
      const fields: FieldErrors = {};
      result.error.issues.forEach(issue => { fields[String(issue.path[0])] = [issue.message]; });
      setErrors(fields);
      return;
    }
    if (form.day < challenge.startDate || form.day > challenge.endDate) {
      setErrors({ day: ['Choose a date within the challenge period.'] }); return;
    }
    const loggedAt = form.day === utcDay() ? new Date().toISOString() : `${form.day}T12:00:00.000Z`;
    try { await save.mutateAsync({ amount: result.data.amount, note: result.data.note, loggedAt }); onSaved(); }
    catch (error) {
      if (error instanceof ApiError) setErrors(error.errors);
      setMessage(errorMessage(error));
    }
  }
  return <div className="overlay" onClick={event => { if (event.target === event.currentTarget && !save.isPending) onClose(); }}>
    <section className="modal small-modal" role="dialog" aria-modal="true" aria-labelledby="progress-heading" ref={panel}>
      <div className="modal-head"><div><span className="eyebrow">KEEP MOVING</span><h2 id="progress-heading">{entry ? 'Edit progress' : 'Log your progress'}</h2><p>One entry per UTC day. Every small step counts.</p></div><button className="icon-btn" onClick={onClose} disabled={save.isPending} aria-label="Close"><X size={20} /></button></div>
      <form className="form-grid" onSubmit={submit} noValidate>
        <label className="field">Amount ({challenge.unit})<input required type="number" step="any" min="0" value={form.amount} onChange={e => setForm({ ...form, amount: Number(e.target.value) })} aria-invalid={Boolean(errors.amount)} />{errors.amount && <span className="field-error">{errors.amount.join(' ')}</span>}</label>
        <label className="field">Date (UTC)<input required type="date" min={challenge.startDate} max={challenge.endDate} value={form.day} onChange={e => setForm({ ...form, day: e.target.value })} aria-invalid={Boolean(errors.day || errors.loggedAt)} />{(errors.day || errors.loggedAt) && <span className="field-error">{(errors.day || errors.loggedAt).join(' ')}</span>}</label>
        <label className="field full">Note (optional)<textarea rows={3} maxLength={250} value={form.note} onChange={e => setForm({ ...form, note: e.target.value })} placeholder="How did it go?" />{errors.note && <span className="field-error">{errors.note.join(' ')}</span>}</label>
        {message && <p className="form-error full" role="alert">{message}</p>}
        <div className="modal-actions full"><button className="button secondary" type="button" disabled={save.isPending} onClick={onClose}>Cancel</button><button className="button primary" disabled={save.isPending}>{save.isPending ? 'Saving…' : 'Save progress'} <CheckCircle2 size={16} /></button></div>
      </form>
    </section>
  </div>;
}
