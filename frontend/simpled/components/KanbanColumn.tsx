// KanbanColumn.tsx
'use client';

import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { useDroppable } from '@dnd-kit/core';
import { SortableContext, verticalListSortingStrategy } from '@dnd-kit/sortable';
import { PencilIcon, PlusIcon, Trash2Icon } from 'lucide-react';
import KanbanItem from './KanbanItem';

interface User {
  id: string;
  name: string;
  imageUrl: string;
}

interface KanbanColumnProps {
  readonly column: { id: string; title: string };
  readonly items: any[];
  readonly users: User[];
  readonly canEdit: boolean;
  readonly onAddItem: () => void;
  readonly onEditColumn: () => void;
  readonly onEditItem: (item: any) => void;
  readonly onDeleteColumn: () => void;
}

export default function KanbanColumn({
  column,
  items,
  users,
  canEdit,
  onAddItem,
  onEditColumn,
  onEditItem,
  onDeleteColumn,
}: KanbanColumnProps) {
  const { setNodeRef } = useDroppable({ id: column.id });

  return (
    <Card ref={setNodeRef} className="bg-card flex w-[300px] min-w-[300px] flex-col shadow-md">
      <CardHeader className="flex flex-row items-center justify-between p-3 pb-2">
        <CardTitle className="text-lg font-medium">{column.title}</CardTitle>
        {canEdit && (
          <div className="flex gap-2">
            <Button variant="ghost" size="icon" onClick={onEditColumn} className="h-8 w-8">
              <PencilIcon className="size-4" />
            </Button>
            <Button variant="ghost" size="icon" onClick={onDeleteColumn} className="h-8 w-8">
              <Trash2Icon className="size-4" />
            </Button>
          </div>
        )}
      </CardHeader>
      <CardContent className="flex-grow overflow-y-auto p-3 pt-0">
        <div className="flex flex-col gap-2">
          <SortableContext
            items={items.map((item) => item.id)}
            strategy={verticalListSortingStrategy}
          >
            {items.map((item) => (
              <KanbanItem
                key={item.id}
                item={item}
                users={users} // ← y aquí
                onClick={() => canEdit && onEditItem(item)}
              />
            ))}
          </SortableContext>
        </div>

        {canEdit && (
          <Button
            variant="ghost"
            size="sm"
            onClick={onAddItem}
            className="text-muted-foreground hover:text-foreground mt-2 w-full"
          >
            <PlusIcon className="mr-1 h-4 w-4" />
            Añadir tarea
          </Button>
        )}
      </CardContent>
    </Card>
  );
}
