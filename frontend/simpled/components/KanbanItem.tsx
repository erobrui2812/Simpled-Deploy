'use client';

import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { Badge } from '@/components/ui/badge';
import { Card, CardContent } from '@/components/ui/card';
import { Progress } from '@/components/ui/progress';
import { listItem } from '@/lib/animation-variants';
import { cn, getInitials, getStatusBadgeClass, getStatusLabel } from '@/lib/utils';
import type { Item, User } from '@/types';
import { useSortable } from '@dnd-kit/sortable';
import { CSS } from '@dnd-kit/utilities';
import { format } from 'date-fns';
import { es } from 'date-fns/locale';
import { motion } from 'framer-motion';
import { CalendarIcon, CheckSquare } from 'lucide-react';

interface KanbanItemProps {
  readonly item: Item;
  readonly users: readonly User[];
  readonly onClick?: () => void;
  readonly isOverlay?: boolean;
}

export default function KanbanItem({ item, users, onClick, isOverlay = false }: KanbanItemProps) {
  const { attributes, listeners, setNodeRef, transform, transition, isDragging } = useSortable({
    id: item.id,
    data: { type: 'item', item },
  });

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
  };

  const formattedDate = item.dueDate
    ? format(new Date(item.dueDate), 'd MMM yyyy', { locale: es })
    : null;
  const statusClass = getStatusBadgeClass(item.status);
  const statusLabel = getStatusLabel(item.status);
  const assignee = item.assigneeId ? users.find((u) => u.id === item.assigneeId) : null;

  const subtasksCount = item.subtasks?.length ?? 0;
  const completedSubtasks = item.subtasks?.filter((st) => st.isCompleted).length ?? 0;
  const progress =
    subtasksCount > 0
      ? Math.round((completedSubtasks / subtasksCount) * 100)
      : (item.progress ?? 0);

  return (
    <motion.div
      ref={setNodeRef}
      style={style}
      {...attributes}
      {...listeners}
      onClick={onClick}
      variants={listItem}
      layout
      initial="hidden"
      animate="visible"
      exit={{ opacity: 0, y: 10, transition: { duration: 0.2 } }}
    >
      <Card
        className={cn(
          'cursor-pointer shadow-sm transition-all duration-200 hover:shadow-md',
          isDragging && 'scale-95 opacity-50',
          isOverlay && 'scale-105 rotate-2 shadow-lg',
        )}
        interactive={!isDragging && !isOverlay}
      >
        <CardContent className="space-y-2 p-3">
          <div className="flex items-center justify-between">
            <h3 className="line-clamp-2 text-base font-medium">{item.title}</h3>
            <Badge className={cn('ml-1 shrink-0', statusClass)}>{statusLabel}</Badge>
          </div>

          {item.description && (
            <p className="text-muted-foreground line-clamp-2 text-sm">{item.description}</p>
          )}

          {subtasksCount > 0 && (
            <div className="space-y-1">
              <div className="text-muted-foreground flex items-center justify-between text-xs">
                <div className="flex items-center gap-1">
                  <CheckSquare className="h-3.5 w-3.5" />
                  <span>
                    {completedSubtasks} de {subtasksCount}
                  </span>
                </div>
                <span>{progress}%</span>
              </div>
              <Progress value={progress} className="h-1.5" />
            </div>
          )}

          <div className="flex items-center justify-between pt-1">
            {assignee && (
              <div className="flex items-center gap-2">
                <Avatar className="h-6 w-6">
                  <AvatarImage src={assignee.imageUrl || '/placeholder.svg'} alt={assignee.name} />
                  <AvatarFallback className="text-xs">{getInitials(assignee.name)}</AvatarFallback>
                </Avatar>
                <span className="text-muted-foreground text-xs">{assignee.name}</span>
              </div>
            )}

            {formattedDate && (
              <div className="text-muted-foreground ml-auto flex items-center text-xs">
                <CalendarIcon className="mr-1 h-3 w-3" />
                {formattedDate}
              </div>
            )}
          </div>
        </CardContent>
      </Card>
    </motion.div>
  );
}
