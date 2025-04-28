'use client';

import { Card, CardContent, CardDescription, CardTitle } from '@/components/ui/card';
import { cn } from '@/lib/utils';
import { useSortable } from '@dnd-kit/sortable';
import { CSS } from '@dnd-kit/utilities';
import { format } from 'date-fns';
import { es } from 'date-fns/locale';
import { CalendarIcon } from 'lucide-react';

interface KanbanItemProps {
  readonly item: {
    readonly id: string;
    readonly title: string;
    readonly description?: string;
    readonly dueDate?: string;
  };
  readonly onClick?: () => void;
  readonly isOverlay?: boolean;
}

export default function KanbanItem({ item, onClick, isOverlay = false }: KanbanItemProps) {
  const { attributes, listeners, setNodeRef, transform, transition, isDragging } = useSortable({
    id: item.id,
    data: {
      type: 'item',
      item,
    },
  });

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
  };

  const formattedDate = item.dueDate
    ? format(new Date(item.dueDate), 'd MMM yyyy', { locale: es })
    : null;

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
      <CardContent className="p-3">
        <CardTitle className="mb-1 text-base font-medium">{item.title}</CardTitle>

        {item.description && (
          <CardDescription className="mb-2 line-clamp-2 text-sm">
            {item.description}
          </CardDescription>
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
