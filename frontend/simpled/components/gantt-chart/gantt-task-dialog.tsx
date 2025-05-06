'use client';

import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import {
  Sheet,
  SheetContent,
  SheetDescription,
  SheetFooter,
  SheetHeader,
  SheetTitle,
} from '@/components/ui/sheet';
import { differenceInDays, format } from 'date-fns';
import type React from 'react';
import { useEffect, useState } from 'react';
import type { Task } from './index';

interface GanttTaskDialogProps {
  readonly task: Task;
  readonly open: boolean;
  readonly onOpenChange: (open: boolean) => void;
  readonly onUpdate: (task: Task) => void;
  readonly allTasks?: readonly Task[];
}

export function GanttTaskDialog({
  task,
  open,
  onOpenChange,
  onUpdate,
  allTasks = [],
}: GanttTaskDialogProps) {
  const [editedTask, setEditedTask] = useState<Task>(task);

  useEffect(() => {
    setEditedTask(task);
  }, [task]);

  const handleChange = (field: keyof Task, value: any) => {
    setEditedTask((prev) => ({ ...prev, [field]: value }));
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onUpdate(editedTask);
  };

  const getStatusBadgeColor = (status?: string) => {
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
  };

  const getStatusLabel = (status?: string) => {
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
  };

  return (
    <Sheet open={open} onOpenChange={onOpenChange}>
      <SheetContent className="z-[1100] sm:max-w-md">
        <form onSubmit={handleSubmit} className="px-4">
          <SheetHeader>
            <SheetTitle>Detalles de la tarea</SheetTitle>
            <SheetDescription>Ver y editar los detalles de la tarea</SheetDescription>
          </SheetHeader>

          <div className="grid gap-4 py-4">
            <div className="grid gap-2">
              <Label htmlFor="title">Título</Label>
              <Input
                id="title"
                value={editedTask.title}
                onChange={(e) => handleChange('title', e.target.value)}
                required
              />
            </div>

            <div className="grid gap-2">
              <Label htmlFor="description">Descripción</Label>
              <Input
                id="description"
                value={editedTask.description ?? ''}
                onChange={(e) => handleChange('description', e.target.value)}
              />
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div className="grid gap-2">
                <Label htmlFor="startDate">Fecha de inicio</Label>
                <Input
                  id="startDate"
                  type="date"
                  value={format(new Date(editedTask.startDate), 'yyyy-MM-dd')}
                  onChange={(e) =>
                    handleChange('startDate', new Date(e.target.value).toISOString())
                  }
                  required
                />
              </div>

              <div className="grid gap-2">
                <Label htmlFor="endDate">Fecha de fin</Label>
                <Input
                  id="endDate"
                  type="date"
                  value={format(new Date(editedTask.endDate), 'yyyy-MM-dd')}
                  onChange={(e) => handleChange('endDate', new Date(e.target.value).toISOString())}
                  required
                />
              </div>
            </div>

            <div className="grid gap-2">
              <Label htmlFor="progress">Progreso (%)</Label>
              <div className="flex items-center gap-4">
                <Input
                  id="progress"
                  type="range"
                  min="0"
                  max="100"
                  value={editedTask.progress}
                  onChange={(e) => handleChange('progress', Number.parseInt(e.target.value))}
                  className="flex-1"
                />
                <span className="w-12 text-center">{editedTask.progress}%</span>
              </div>
            </div>

            <div className="grid gap-2">
              <Label htmlFor="status">Estado</Label>
              <Select
                value={editedTask.status}
                onValueChange={(value) => handleChange('status', value)}
              >
                <SelectTrigger id="status">
                  <SelectValue placeholder="Seleccionar estado" />
                </SelectTrigger>
                <SelectContent className="z-[1200]">
                  <SelectItem value="pending">
                    <div className="flex items-center">
                      <div className="mr-2 h-2 w-2 rounded-full bg-amber-500"></div>
                      Pendiente
                    </div>
                  </SelectItem>
                  <SelectItem value="in-progress">
                    <div className="flex items-center">
                      <div className="mr-2 h-2 w-2 rounded-full bg-blue-500"></div>
                      En progreso
                    </div>
                  </SelectItem>
                  <SelectItem value="completed">
                    <div className="flex items-center">
                      <div className="mr-2 h-2 w-2 rounded-full bg-emerald-500"></div>
                      Completada
                    </div>
                  </SelectItem>
                  <SelectItem value="delayed">
                    <div className="flex items-center">
                      <div className="mr-2 h-2 w-2 rounded-full bg-rose-500"></div>
                      Retrasada
                    </div>
                  </SelectItem>
                </SelectContent>
              </Select>
            </div>

            <div className="mt-2">
              <div className="flex items-center justify-between">
                <span className="text-sm font-medium">Estado actual:</span>
                <span
                  className={`rounded-full px-2 py-1 text-xs ${getStatusBadgeColor(editedTask.status)}`}
                >
                  {getStatusLabel(editedTask.status)}
                </span>
              </div>
              <div className="mt-2 flex items-center justify-between">
                <span className="text-sm font-medium">Duración:</span>
                <span className="text-sm">
                  {differenceInDays(new Date(editedTask.endDate), new Date(editedTask.startDate)) +
                    1}{' '}
                  días
                </span>
              </div>
            </div>
          </div>

          <SheetFooter>
            <Button type="submit">Guardar cambios</Button>
          </SheetFooter>
        </form>
      </SheetContent>
    </Sheet>
  );
}
