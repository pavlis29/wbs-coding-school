export type ChallengeStatus = 'Open' | 'Running' | 'Completed';
export type Visibility = 'Public' | 'Private';

export interface User {
  id: string;
  email: string;
  displayName: string;
}

export interface AuthResponse {
  token: string;
  expiresAt: string;
  user: User;
}

export interface Membership {
  id: string;
  userId: string;
  displayName: string;
  challengeId: string;
  status: 'Active' | 'Pending';
}

export interface ChallengeInput {
  title: string;
  description: string;
  category: string;
  target: number;
  unit: string;
  startDate: string;
  endDate: string;
  visibility: Visibility;
}

export interface Challenge extends ChallengeInput {
  id: string;
  ownerId: string;
  ownerName: string;
  status: ChallengeStatus;
  createdAt: string;
  total: number;
  membership: Membership | null;
}

export interface ProgressInput {
  amount: number;
  note: string;
  loggedAt: string;
}

export interface ProgressEntry extends ProgressInput {
  id: string;
  challengeId: string;
  userId: string;
  displayName: string;
  day: string;
  createdAt: string;
}

export interface Leader {
  userId: string;
  displayName: string;
  total: number;
}

export type FieldErrors = Record<string, string[]>;
