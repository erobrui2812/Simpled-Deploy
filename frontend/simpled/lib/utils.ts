import { type ClassValue, clsx } from 'clsx';
import { twMerge } from 'tailwind-merge';
import sanitizeHtml from 'sanitize-html';

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

export const sanitize = (dirty: string): string =>
  sanitizeHtml(dirty, {
    allowedTags: [],
    allowedAttributes: {},
  });

export function getInitials(name: string) {
  if (!name) return '??';
  return name
    .split(' ')
    .map((n) => n[0])
    .join('')
    .toUpperCase()
    .substring(0, 2);
}

export function formatDate(date: string | Date): string {
  if (!date) return '';
  const dateObj = typeof date === 'string' ? new Date(date) : date;
  return new Intl.DateTimeFormat('es-ES', {
    day: 'numeric',
    month: 'short',
    year: 'numeric',
  }).format(dateObj);
}

export function getStatusColor(status?: string) {
  switch (status) {
    case 'completed':
      return 'bg-emerald-500 text-white';
    case 'delayed':
      return 'bg-rose-500 text-white';
    case 'in-progress':
      return 'bg-blue-500 text-white';
    default:
      return 'bg-amber-500 text-white';
  }
}

export function getStatusBadgeClass(status?: string) {
  switch (status) {
    case 'completed':
      return 'bg-emerald-100 text-emerald-800 dark:bg-emerald-900/20 dark:text-emerald-300';
    case 'delayed':
      return 'bg-rose-100 text-rose-800 dark:bg-rose-900/20 dark:text-rose-300';
    case 'in-progress':
      return 'bg-blue-100 text-blue-800 dark:bg-blue-900/20 dark:text-blue-300';
    default:
      return 'bg-amber-100 text-amber-800 dark:bg-amber-900/20 dark:text-amber-300';
  }
}

export function getStatusLabel(status?: string) {
  switch (status) {
    case 'completed':
      return 'Completada';
    case 'delayed':
      return 'Retrasada';
    case 'in-progress':
      return 'En progreso';
    default:
      return 'Pendiente';
  }
}
