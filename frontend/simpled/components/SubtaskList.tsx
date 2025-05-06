'use client';

import type React from 'react';

import { Button } from '@/components/ui/button';
import { Checkbox } from '@/components/ui/checkbox';
import { Input } from '@/components/ui/input';
import { cn } from '@/lib/utils';
import type { Subtask } from '@/types';
import { Loader2, Trash2 } from 'lucide-react';
import { useState } from 'react';

interface SubtaskListProps {
  readonly subtasks: Subtask[];
  readonly itemId: string;
  readonly onAdd: (title: string) => void;
  readonly onUpdate: (subtask: Subtask) => void;
  readonly onDelete: (subtaskId: string) => void;
  readonly disabled?: boolean;
}

export default function SubtaskList({
  subtasks,
  itemId,
  onAdd,
  onUpdate,
  onDelete,
  disabled = false,
}: SubtaskListProps) {
  const [newSubtaskTitle, setNewSubtaskTitle] = useState('');
  const [isAdding, setIsAdding] = useState(false);
  const [updatingId, setUpdatingId] = useState<string | null>(null);
  const [deletingId, setDeletingId] = useState<string | null>(null);

  const handleAddSubtask = async () => {
    if (newSubtaskTitle.trim()) {
      setIsAdding(true);
      try {
        await onAdd(newSubtaskTitle.trim());
        setNewSubtaskTitle('');
      } finally {
        setIsAdding(false);
      }
    }
  };

  const handleToggleComplete = async (subtask: Subtask) => {
    setUpdatingId(subtask.id);
    try {
      await onUpdate({
        ...subtask,
        isCompleted: !subtask.isCompleted,
      });
    } finally {
      setUpdatingId(null);
    }
  };

  const handleDeleteSubtask = async (subtaskId: string) => {
    setDeletingId(subtaskId);
    try {
      await onDelete(subtaskId);
    } finally {
      setDeletingId(null);
    }
  };

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter') {
      e.preventDefault();
      handleAddSubtask();
    }
  };

  return (
    <div className="space-y-3">
      <h3 className="text-sm font-medium">Subtareas</h3>

      {subtasks.length > 0 ? (
        <ul className="scrollbar-thin max-h-[200px] space-y-2 overflow-y-auto pr-1">
          {subtasks.map((subtask) => (
            <li key={subtask.id} className="group flex items-center gap-2">
              {updatingId === subtask.id ? (
                <div className="flex h-4 w-4 items-center justify-center">
                  <Loader2 className="h-3 w-3 animate-spin" />
                </div>
              ) : (
                <Checkbox
                  id={`subtask-${subtask.id}`}
                  checked={subtask.isCompleted}
                  onCheckedChange={() => handleToggleComplete(subtask)}
                  disabled={disabled}
                />
              )}
              <label
                htmlFor={`subtask-${subtask.id}`}
                className={cn(
                  'flex-1 cursor-pointer text-sm',
                  subtask.isCompleted && 'text-muted-foreground line-through',
                )}
              >
                {subtask.title}
              </label>
              {!disabled && (
                <Button
                  variant="ghost"
                  size="icon"
                  className="h-6 w-6 opacity-0 transition-opacity group-hover:opacity-100"
                  onClick={() => handleDeleteSubtask(subtask.id)}
                  disabled={deletingId === subtask.id}
                >
                  {deletingId === subtask.id ? (
                    <Loader2 className="h-3 w-3 animate-spin" />
                  ) : (
                    <Trash2 className="text-muted-foreground h-4 w-4" />
                  )}
                </Button>
              )}
            </li>
          ))}
        </ul>
      ) : (
        <p className="text-muted-foreground text-sm italic">No hay subtareas</p>
      )}

      {!disabled && (
        <div className="flex gap-2 pt-1">
          <Input
            placeholder="Nueva subtarea"
            value={newSubtaskTitle}
            onChange={(e) => setNewSubtaskTitle(e.target.value)}
            onKeyDown={handleKeyDown}
            className="text-sm"
            disabled={isAdding}
          />
          <Button
            onClick={handleAddSubtask}
            disabled={!newSubtaskTitle.trim() || isAdding}
            size="sm"
          >
            {isAdding ? <Loader2 className="h-4 w-4 animate-spin" /> : 'Añadir'}
          </Button>
        </div>
      )}
    </div>
  );
}
