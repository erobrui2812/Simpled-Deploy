'use client';

import { Card, CardContent, CardDescription, CardTitle } from '@/components/ui/card';
import { cn } from '@/lib/utils';
import { useSortable } from '@dnd-kit/sortable';
import { CSS } from '@dnd-kit/utilities';
import { format } from 'date-fns';
import { es } from 'date-fns/locale';
import { CalendarIcon } from 'lucide-react';

type User = {
  id: string;
  name: string;
  imageUrl: string;
};

interface KanbanItemProps {
  readonly item: {
    readonly id: string;
    readonly title: string;
    readonly description?: string;
    readonly dueDate?: string;
    readonly status?: 'pending' | 'in-progress' | 'completed' | 'delayed';
    readonly assigneeId?: string | null;
  };
  readonly users: readonly User[];
  readonly onClick?: () => void;
  readonly isOverlay?: boolean;
}

const statusClasses: Record<string, string> = {
  pending: 'bg-amber-200 text-amber-800',
  'in-progress': 'bg-blue-200  text-blue-800',
  completed: 'bg-emerald-200 text-emerald-800',
  delayed: 'bg-rose-200   text-rose-800',
};

const statusLabels: Record<string, string> = {
  pending: 'Pendiente',
  'in-progress': 'En progreso',
  completed: 'Completada',
  delayed: 'Retrasada',
};

export default function KanbanItem({ item, users, onClick, isOverlay = false }: KanbanItemProps) {
  const { attributes, listeners, setNodeRef, transform, transition, isDragging } = useSortable({
    id: item.id,
    data: { type: 'item', item },
  });

  const style = { transform: CSS.Transform.toString(transform), transition };
  const formattedDate = item.dueDate
    ? format(new Date(item.dueDate), 'd MMM yyyy', { locale: es })
    : null;

  const cls = statusClasses[item.status ?? 'pending'] ?? statusClasses.pending;
  const label = statusLabels[item.status ?? 'pending'] ?? statusLabels.pending;

  const assignee = item.assigneeId ? users.find((u) => u.id === item.assigneeId) : null;

  return (
    <Card
      ref={setNodeRef}
      style={style}
      {...attributes}
      {...listeners}
      onClick={onClick}
      className={cn(
        'cursor-pointer border shadow-sm transition-shadow hover:shadow-md',
        isDragging && 'opacity-50',
        isOverlay && 'rotate-3 shadow-lg',
      )}
    >
      <CardContent className="space-y-2 p-3">
        <div className="flex items-center justify-between">
          <CardTitle className="text-base font-medium">{item.title}</CardTitle>
          <span className={cn('rounded-full px-2 py-0.5 text-xs font-semibold', cls)}>{label}</span>
        </div>
        {assignee && (
          <div className="mb-1 flex items-center space-x-2 text-xs">
            <img src={assignee.imageUrl} alt={assignee.name} className="h-5 w-5 rounded-full" />
            <span>{assignee.name}</span>
          </div>
        )}
        {item.description && (
          <CardDescription className="line-clamp-2 text-sm">{item.description}</CardDescription>
        )}
        {formattedDate && (
          <div className="text-muted-foreground flex items-center text-xs">
            <CalendarIcon className="mr-1 h-3 w-3" />
            {formattedDate}
          </div>
        )}
      </CardContent>
    </Card>
  );
}
