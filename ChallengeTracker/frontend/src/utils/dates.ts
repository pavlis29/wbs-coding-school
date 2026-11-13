// The API uses UTC calendar days for its one-entry-per-day rule.
export function utcDay(date = new Date()): string {
  return date.toISOString().slice(0, 10);
}

export function formatDay(day: string): string {
  return new Date(`${day.slice(0, 10)}T12:00:00Z`).toLocaleDateString('en-GB', {
    day: 'numeric', month: 'short', year: 'numeric', timeZone: 'UTC',
  });
}

export function canEditProgress(createdAt: string): boolean {
  return Date.now() - Date.parse(createdAt) < 24 * 60 * 60 * 1000;
}
