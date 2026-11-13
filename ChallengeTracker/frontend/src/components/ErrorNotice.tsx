import { errorMessage, ApiError } from '../utils/api';

export function ErrorNotice({ error, retry }: { error: unknown; retry?: () => void }) {
  if (!error) return null;
  const details = error instanceof ApiError ? Object.values(error.errors).flat() : [];
  return <div className="toast error" role="alert">
    <div><strong>{errorMessage(error)}</strong>{details.length > 0 && <ul>{details.map((message, i) => <li key={i}>{message}</li>)}</ul>}</div>
    {retry && <button className="button secondary" onClick={retry}>Try again</button>}
  </div>;
}
