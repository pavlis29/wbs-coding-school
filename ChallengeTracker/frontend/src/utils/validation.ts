import { z } from 'zod';

export const challengeSchema = z.object({
  title: z.string().trim().min(3, 'Use at least 3 characters.').max(90),
  description: z.string().trim().max(500),
  category: z.string().trim().min(1, 'Choose a category.').max(40),
  target: z.number().positive('The target must be greater than zero.').max(1_000_000_000),
  unit: z.string().trim().min(1, 'Enter a unit.').max(24),
  startDate: z.iso.date('Enter a valid start date.'),
  endDate: z.iso.date('Enter a valid end date.'),
  visibility: z.enum(['Public', 'Private']),
}).refine(value => value.endDate >= value.startDate, {
  path: ['endDate'], message: 'End date must be on or after start date.',
});

export const progressSchema = z.object({
  amount: z.number().positive('Enter an amount greater than zero.').max(1_000_000_000),
  note: z.string().trim().max(250),
  day: z.iso.date('Enter a valid date.'),
});
