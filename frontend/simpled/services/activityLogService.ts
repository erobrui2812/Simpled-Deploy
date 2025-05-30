// src/services/activityLogService.ts

import type { ActivityLog } from '@/types';

const API_URL = process.env.NEXT_PUBLIC_API_URL ?? 'http://54.226.33.124:5193';

export async function fetchActivityLogs(itemId: string, token: string): Promise<ActivityLog[]> {
  const res = await fetch(`${API_URL}/api/items/${itemId}/activity`, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });

  if (!res.ok) {
    throw new Error('Error al obtener el historial de actividad');
  }

  return res.json();
}
