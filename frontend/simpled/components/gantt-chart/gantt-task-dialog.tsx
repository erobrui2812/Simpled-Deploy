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
import { Link2 } from 'lucide-react';
import type React from 'react';
import { useEffect, useState } from 'react';
import type { Task } from './index';
import { Editor } from '@tinymce/tinymce-react';

interface GanttTaskDialogProps {
  readonly task: Task;
  readonly open: boolean;
  readonly onOpenChange: (open: boolean) => void;
  readonly onUpdate: (task: Task) => void;
  readonly onManageDependencies: () => void;
  readonly allTasks?: readonly Task[];
}

export function GanttTaskDialog({
  task,
  open,
  onOpenChange,
  onUpdate,
  onManageDependencies,
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

  const getStatusColor = (status?: string) => {
    switch (status) {
      case 'completed':
        return 'bg-emerald-500';
      case 'delayed':
        return 'bg-rose-500';
      case 'in-progress':
        return 'bg-blue-500';
      default:
        return 'bg-amber-500';
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
              <Editor
                id="description"
                apiKey="e0ysk0ojdthd8om5r7xxpebz9xunokuc9jsw6bno2ovnz5li"
                onEditorChange={(content) => handleChange('description', content)}
                value={editedTask.description ?? ''}
                init={{
                  height: 300,
                  menubar: false,
                  plugins: [
                    'advlist',
                    'autolink',
                    'lists',
                    'link',
                    'image',
                    'charmap',
                    'preview',
                    'anchor',
                    'searchreplace',
                    'visualblocks',
                    'code',
                    'fullscreen',
                    'insertdatetime',
                    'media',
                    'table',
                    'code',
                    'help',
                    'wordcount',
                  ],
                  toolbar:
                    'undo redo | formatselect | bold italic backcolor | \
                                                alignleft aligncenter alignright alignjustify | \
                                                bullist numlist outdent indent | removeformat | help',
                  content_style: 'body { font-family:Helvetica,Arial,sans-serif; font-size:14px }',
                }}
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

            <Button
              type="button"
              variant="outline"
              className="mt-2 w-full"
              onClick={onManageDependencies}
            >
              <Link2 className="mr-2 h-4 w-4" />
              Gestionar dependencias
            </Button>

            <div className="mt-4 border-t pt-4">
              <h3 className="mb-2 text-sm font-medium">Información de dependencias</h3>
              <div className="flex flex-col gap-2">
                <div className="flex items-center gap-2">
                  <div className="h-1 w-8 bg-blue-500"></div>
                  <span className="text-xs">Fin a Inicio (FS)</span>
                </div>
                <div className="flex items-center gap-2">
                  <div className="h-1 w-8 border-t border-dashed border-green-500 bg-green-500"></div>
                  <span className="text-xs">Inicio a Inicio (SS)</span>
                </div>
                <div className="flex items-center gap-2">
                  <div className="h-1 w-8 border-t-2 border-dashed border-amber-500 bg-amber-500"></div>
                  <span className="text-xs">Fin a Fin (FF)</span>
                </div>
                <div className="flex items-center gap-2">
                  <div className="h-1 w-8 border-t-2 border-dotted border-red-500 bg-red-500"></div>
                  <span className="text-xs">Inicio a Fin (SF)</span>
                </div>
              </div>
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
              <div className="mt-4 w-full rounded-md bg-gray-100 p-3 dark:bg-gray-800">
                <h4 className="mb-2 text-sm font-medium">Visualización de duración</h4>
                <div className="relative h-6 w-full rounded bg-gray-200 dark:bg-gray-700">
                  <div
                    className={`absolute top-0 left-0 h-full rounded ${getStatusColor(editedTask.status)}`}
                    style={{ width: '100%' }}
                  >
                    <span className="absolute inset-0 flex items-center justify-center text-xs font-medium text-white">
                      {differenceInDays(
                        new Date(editedTask.endDate),
                        new Date(editedTask.startDate),
                      ) + 1}{' '}
                      días
                    </span>
                  </div>
                </div>
                <div className="mt-1 flex justify-between text-xs text-gray-500">
                  <span>{format(new Date(editedTask.startDate), 'dd/MM/yyyy')}</span>
                  <span>{format(new Date(editedTask.endDate), 'dd/MM/yyyy')}</span>
                </div>
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
