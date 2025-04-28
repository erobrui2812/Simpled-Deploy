'use client';

import type React from 'react';

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
import { format } from 'date-fns';
import { X } from 'lucide-react';
import { useState } from 'react';
import type { Task } from './index';

interface GanttTaskCreatorProps {
  readonly position: { x: number; y: number; date: Date };
  readonly onClose: () => void;
  readonly onSave: (task: Partial<Task>) => void;
  readonly columns: { id: string; title: string }[];
}

export function GanttTaskCreator({ position, onClose, onSave, columns }: GanttTaskCreatorProps) {
  const [taskData, setTaskData] = useState<Partial<Task>>({
    title: '',
    description: '',
    startDate: position.date.toISOString(),
    endDate: new Date(position.date.getTime() + 3 * 24 * 60 * 60 * 1000).toISOString(),
    progress: 0,
    status: 'pending',
    columnId: columns[0]?.id,
  });

  const handleChange = (field: keyof Task, value: any) => {
    setTaskData((prev) => ({ ...prev, [field]: value }));
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSave(taskData);
  };

  const style = {
    position: 'absolute' as const,
    left: `${position.x}px`,
    top: `${position.y}px`,
    zIndex: 100,
    transform: 'translateY(-50%)',
  };

  return (
    <div style={style} className="bg-card w-80 rounded-md border p-4 shadow-lg">
      <div className="mb-4 flex items-center justify-between">
        <h3 className="font-medium">Nueva Tarea</h3>
        <Button variant="ghost" size="sm" onClick={onClose} className="h-8 w-8 p-0">
          <X className="h-4 w-4" />
        </Button>
      </div>

      <form onSubmit={handleSubmit} className="space-y-4">
        <div className="space-y-2">
          <Label htmlFor="title">Título</Label>
          <Input
            id="title"
            value={taskData.title ?? ''}
            onChange={(e) => handleChange('title', e.target.value)}
            placeholder="Título de la tarea"
            required
          />
        </div>

        <div className="space-y-2">
          <Label htmlFor="description">Descripción</Label>
          <Input
            id="description"
            value={taskData.description ?? ''}
            onChange={(e) => handleChange('description', e.target.value)}
            placeholder="Descripción (opcional)"
          />
        </div>

        <div className="grid grid-cols-2 gap-4">
          <div className="space-y-2">
            <Label htmlFor="startDate">Fecha inicio</Label>
            <Input
              id="startDate"
              type="date"
              value={format(new Date(taskData.startDate ?? position.date), 'yyyy-MM-dd')}
              onChange={(e) => handleChange('startDate', new Date(e.target.value).toISOString())}
              required
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="endDate">Fecha fin</Label>
            <Input
              id="endDate"
              type="date"
              value={format(
                new Date(
                  taskData.endDate ?? new Date(position.date.getTime() + 3 * 24 * 60 * 60 * 1000),
                ),
                'yyyy-MM-dd',
              )}
              onChange={(e) => handleChange('endDate', new Date(e.target.value).toISOString())}
              required
            />
          </div>
        </div>

        <div className="space-y-2">
          <Label htmlFor="status">Estado</Label>
          <Select
            value={taskData.status ?? 'pending'}
            onValueChange={(value) => handleChange('status', value)}
          >
            <SelectTrigger id="status">
              <SelectValue placeholder="Seleccionar estado" />
            </SelectTrigger>
            <SelectContent>
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

        {columns.length > 0 && (
          <div className="space-y-2">
            <Label htmlFor="column">Columna</Label>
            <Select
              value={taskData.columnId ?? columns[0]?.id}
              onValueChange={(value) => handleChange('columnId', value)}
            >
              <SelectTrigger id="column">
                <SelectValue placeholder="Seleccionar columna" />
              </SelectTrigger>
              <SelectContent>
                {columns.map((column) => (
                  <SelectItem key={column.id} value={column.id}>
                    {column.title}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>
        )}

        <div className="flex justify-end gap-2 pt-2">
          <Button type="button" variant="outline" onClick={onClose}>
            Cancelar
          </Button>
          <Button type="submit">Crear Tarea</Button>
        </div>
      </form>
    </div>
  );
}
