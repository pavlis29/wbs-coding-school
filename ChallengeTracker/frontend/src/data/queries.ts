import { useQuery } from '@tanstack/react-query';
import { useAuth } from '../contexts/AuthContext';
import { request } from '../utils/api';
import type { Challenge, Leader, Membership, ProgressEntry } from '../types';

export function useChallenges(filters: Record<string, string>) {
  const { token } = useAuth();
  return useQuery({
    queryKey: ['challenges', filters],
    queryFn: () => request<Challenge[]>(`/challenges?${new URLSearchParams(filters)}`, token),
  });
}

export function useChallenge(id: string) {
  const { token } = useAuth();
  return useQuery({ queryKey: ['challenge', id], queryFn: () => request<Challenge>(`/challenges/${id}`, token) });
}

export function useProgress(id: string, enabled: boolean) {
  const { token } = useAuth();
  return useQuery({ queryKey: ['progress', id], queryFn: () => request<ProgressEntry[]>(`/challenges/${id}/progress-entries`, token), enabled });
}

export function useMemberships(id: string, enabled: boolean) {
  const { token } = useAuth();
  return useQuery({ queryKey: ['memberships', id], queryFn: () => request<Membership[]>(`/challenges/${id}/memberships`, token), enabled });
}

export function useLeaderboard(id: string, enabled: boolean) {
  const { token } = useAuth();
  return useQuery({ queryKey: ['leaderboard', id], queryFn: () => request<Leader[]>(`/leaderboards/challenges/${id}?period=total`, token), enabled });
}
