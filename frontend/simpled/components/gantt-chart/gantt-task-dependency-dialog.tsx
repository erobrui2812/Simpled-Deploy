'use client';

import { Button } from '@/components/ui/button';
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
import { format } from 'date-fns';
import { Link2, Trash2 } from 'lucide-react';
import { useState } from 'react';
import type { Dependency, Task } from './index';

interface GanttTaskDependencyDialogProps {
  readonly task: Task;
  readonly open: boolean;
  readonly onOpenChange: (open: boolean) => void;
  readonly allTasks: Task[];
  readonly dependencies: Dependency[];
  readonly onAddDependency: (dependency: Omit<Dependency, 'id'>) => Promise<void>;
  readonly onRemoveDependency: (dependencyId: string) => Promise<void>;
}

export function GanttTaskDependencyDialog({
  task,
  open,
  onOpenChange,
  allTasks,
  dependencies,
  onAddDependency,
  onRemoveDependency,
}: GanttTaskDependencyDialogProps) {
  const [newDependency, setNewDependency] = useState<{
    toTaskId: string;
    type: Dependency['type'];
  }>({
    toTaskId: '',
    type: 'finish-to-start',
  });

  const [isAdding, setIsAdding] = useState(false);
  const [isRemoving, setIsRemoving] = useState<string | null>(null);

  const handleAddDependency = async () => {
    if (!newDependency.toTaskId) return;

    setIsAdding(true);
    try {
      await onAddDependency({
        fromTaskId: task.id,
        toTaskId: newDependency.toTaskId,
        type: newDependency.type,
      });
      setNewDependency({
        toTaskId: '',
        type: 'finish-to-start',
      });
    } finally {
      setIsAdding(false);
    }
  };

  const handleRemoveDependency = async (dependencyId: string) => {
    setIsRemoving(dependencyId);
    try {
      await onRemoveDependency(dependencyId);
    } finally {
      setIsRemoving(null);
    }
  };

  const getDependencyTypeLabel = (type: Dependency['type']) => {
    switch (type) {
      case 'finish-to-start':
        return 'Fin a Inicio (FS)';
      case 'start-to-start':
        return 'Inicio a Inicio (SS)';
      case 'finish-to-finish':
        return 'Fin a Fin (FF)';
      case 'start-to-finish':
        return 'Inicio a Fin (SF)';
      default:
        return type;
    }
  };

  return (
    <Sheet open={open} onOpenChange={onOpenChange}>
      <SheetContent className="z-[1100] sm:max-w-md">
        <div className="px-4">
          <SheetHeader>
            <SheetTitle>Gestionar dependencias</SheetTitle>
            <SheetDescription>
              Administrar las dependencias de la tarea "{task.title}"
            </SheetDescription>
          </SheetHeader>

          <div className="grid gap-4 py-4">
            <div className="grid gap-2">
              <Label>Tarea actual</Label>
              <div className="rounded-md border p-2">
                <div className="font-medium">{task.title}</div>
                <div className="text-muted-foreground mt-1 text-xs">
                  {format(new Date(task.startDate), 'dd/MM/yyyy')} -{' '}
                  {format(new Date(task.endDate), 'dd/MM/yyyy')}
                </div>
              </div>
            </div>

            <div className="grid gap-2">
              <Label>Agregar nueva dependencia</Label>
              <div className="flex gap-2">
                <Select
                  value={newDependency.toTaskId}
                  onValueChange={(value) =>
                    setNewDependency((prev) => ({ ...prev, toTaskId: value }))
                  }
                >
                  <SelectTrigger className="flex-1">
                    <SelectValue placeholder="Seleccionar tarea" />
                  </SelectTrigger>
                  <SelectContent className="z-[1200]">
                    {allTasks
                      .filter(
                        (t) =>
                          !dependencies.some(
                            (d) => d.fromTaskId === task.id && d.toTaskId === t.id,
                          ),
                      )
                      .map((t) => (
                        <SelectItem key={t.id} value={t.id}>
                          {t.title}
                        </SelectItem>
                      ))}
                  </SelectContent>
                </Select>

                <Select
                  value={newDependency.type}
                  onValueChange={(value) =>
                    setNewDependency((prev) => ({ ...prev, type: value as Dependency['type'] }))
                  }
                >
                  <SelectTrigger className="w-[180px]">
                    <SelectValue placeholder="Tipo" />
                  </SelectTrigger>
                  <SelectContent className="z-[1200]">
                    <SelectItem value="finish-to-start">Fin a Inicio (FS)</SelectItem>
                    <SelectItem value="start-to-start">Inicio a Inicio (SS)</SelectItem>
                    <SelectItem value="finish-to-finish">Fin a Fin (FF)</SelectItem>
                    <SelectItem value="start-to-finish">Inicio a Fin (SF)</SelectItem>
                  </SelectContent>
                </Select>
              </div>

              <Button
                type="button"
                onClick={handleAddDependency}
                disabled={!newDependency.toTaskId || isAdding}
                className="mt-2"
              >
                <Link2 className="mr-2 h-4 w-4" />
                {isAdding ? 'Agregando...' : 'Agregar dependencia'}
              </Button>
            </div>

            <div className="grid gap-2">
              <Label>Dependencias existentes</Label>
              {dependencies.length === 0 ? (
                <div className="text-muted-foreground text-sm">
                  No hay dependencias para esta tarea
                </div>
              ) : (
                <div className="space-y-2">
                  {dependencies.map((dep) => {
                    const isFromCurrent = dep.fromTaskId === task.id;
                    const otherTaskId = isFromCurrent ? dep.toTaskId : dep.fromTaskId;
                    const otherTask = allTasks.find((t) => t.id === otherTaskId);

                    return (
                      <div
                        key={dep.id}
                        className="flex items-center justify-between rounded-md border p-2"
                      >
                        <div>
                          <div className="flex items-center text-sm">
                            {isFromCurrent ? (
                              <>
                                <span className="font-medium">{task.title}</span>
                                <span className="mx-2">→</span>
                                <span className="font-medium">{otherTask?.title}</span>
                              </>
                            ) : (
                              <>
                                <span className="font-medium">{otherTask?.title}</span>
                                <span className="mx-2">→</span>
                                <span className="font-medium">{task.title}</span>
                              </>
                            )}
                          </div>
                          <div className="text-muted-foreground mt-1 text-xs">
                            {getDependencyTypeLabel(dep.type)}
                          </div>
                        </div>
                        <Button
                          variant="ghost"
                          size="icon"
                          onClick={() => handleRemoveDependency(dep.id)}
                          disabled={isRemoving === dep.id}
                        >
                          <Trash2 className="h-4 w-4" />
                        </Button>
                      </div>
                    );
                  })}
                </div>
              )}
            </div>
          </div>

          <SheetFooter>
            <Button type="button" onClick={() => onOpenChange(false)}>
              Cerrar
            </Button>
          </SheetFooter>
        </div>
      </SheetContent>
    </Sheet>
  );
}
