import { useMutation, useQueryClient } from '@tanstack/react-query';
import { useAuth } from '../contexts/AuthContext';
import { request } from '../utils/api';
import type { Challenge, ChallengeInput, Membership, ProgressEntry, ProgressInput } from '../types';
import { utcDay } from '../utils/dates';

// All successful writes refresh both the detail and the dashboard summaries.
export function useChallengeAction<T>(action: (token: string | null) => Promise<T>, id?: string) {
  const { token } = useAuth();
  const client = useQueryClient();
  return useMutation({
    mutationFn: () => action(token),
    onSuccess: async () => {
      await Promise.all([
        client.invalidateQueries({ queryKey: ['challenges'] }),
        client.invalidateQueries({ queryKey: ['challenge', id] }),
        client.invalidateQueries({ queryKey: ['memberships', id] }),
        client.invalidateQueries({ queryKey: ['progress', id] }),
        client.invalidateQueries({ queryKey: ['leaderboard', id] }),
      ]);
    },
  });
}

export function useSaveChallenge(id?: string) {
  const { token } = useAuth();
  const client = useQueryClient();
  return useMutation({
    mutationFn: (input: ChallengeInput) => request<Challenge>(id ? `/challenges/${id}` : '/challenges', token, {
      method: id ? 'PUT' : 'POST', body: JSON.stringify(input),
    }),
    onSuccess: async value => {
      client.setQueryData(['challenge', value.id], value);
      await client.invalidateQueries({ queryKey: ['challenges'] });
    },
  });
}

export function useJoinChallenge(id: string) {
  return useChallengeAction(token => request<Membership>('/memberships', token, {
    method: 'POST', body: JSON.stringify({ challengeId: id }),
  }), id);
}

export function useSaveProgress(challengeId: string, entry?: ProgressEntry) {
  const { token, user } = useAuth();
  const client = useQueryClient();
  return useMutation({
    mutationFn: (input: ProgressInput) => request<ProgressEntry>(entry ? `/progress-entries/${entry.id}` : '/progress-entries', token, {
      method: entry ? 'PUT' : 'POST',
      body: JSON.stringify(entry ? input : { ...input, challengeId }),
    }),
    onMutate: async input => {
      await Promise.all([
        client.cancelQueries({ queryKey: ['progress', challengeId] }),
        client.cancelQueries({ queryKey: ['challenge', challengeId] }),
      ]);
      const previousEntries = client.getQueryData<ProgressEntry[]>(['progress', challengeId]);
      const previousChallenge = client.getQueryData<Challenge>(['challenge', challengeId]);
      const optimistic: ProgressEntry = {
        ...input, id: entry?.id || `pending-${crypto.randomUUID()}`, challengeId,
        userId: user!.id, displayName: user!.displayName,
        day: utcDay(new Date(input.loggedAt)), createdAt: entry?.createdAt || new Date().toISOString(),
      };
      client.setQueryData<ProgressEntry[]>(['progress', challengeId], current => entry
        ? (current || []).map(item => item.id === entry.id ? optimistic : item)
        : [...(current || []), optimistic]);
      if (previousChallenge) client.setQueryData(['challenge', challengeId], {
        ...previousChallenge, total: previousChallenge.total + input.amount - (entry?.amount || 0),
      });
      return { previousEntries, previousChallenge };
    },
    onError: (_error, _input, snapshot) => {
      client.setQueryData(['progress', challengeId], snapshot?.previousEntries || []);
      if (snapshot?.previousChallenge) client.setQueryData(['challenge', challengeId], snapshot.previousChallenge);
    },
    onSettled: async () => {
      await Promise.all([
        client.invalidateQueries({ queryKey: ['progress', challengeId] }),
        client.invalidateQueries({ queryKey: ['challenge', challengeId] }),
        client.invalidateQueries({ queryKey: ['leaderboard', challengeId] }),
        client.invalidateQueries({ queryKey: ['challenges'] }),
      ]);
    },
  });
}
