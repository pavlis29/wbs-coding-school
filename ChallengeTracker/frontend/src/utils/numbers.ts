// Significant digits keep small fractions visible instead of displaying them as 0.
export function formatAmount(value: number): string {
  return value.toLocaleString('en-GB', { maximumSignificantDigits: 15 });
}
