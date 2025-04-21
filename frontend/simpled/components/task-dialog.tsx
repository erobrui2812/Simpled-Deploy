"use client";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import {
  Sheet,
  SheetContent,
  SheetDescription,
  SheetFooter,
  SheetHeader,
  SheetTitle,
} from "@/components/ui/sheet";
import { format } from "date-fns";
import React from "react";
import type { Task } from "./gantt-chart";

interface TaskDialogProps {
  task: Task;
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onUpdate: (task: Task) => void;
}

export function TaskDialog({
  task,
  open,
  onOpenChange,
  onUpdate,
}: TaskDialogProps) {
  const [editedTask, setEditedTask] = React.useState<Task>(task);

  React.useEffect(() => {
    setEditedTask(task);
  }, [task]);

  const handleChange = (field: keyof Task, value: any) => {
    setEditedTask((prev) => ({ ...prev, [field]: value }));
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onUpdate(editedTask);
  };

  return (
    <Sheet open={open} onOpenChange={onOpenChange}>
      <SheetContent className="sm:max-w-md">
        <form onSubmit={handleSubmit}>
          <SheetHeader>
            <SheetTitle>Detalles de la tarea</SheetTitle>
            <SheetDescription>
              Ver y editar los detalles de la tarea
            </SheetDescription>
          </SheetHeader>

          <div className="grid gap-4 py-4">
            <div className="grid gap-2">
              <Label htmlFor="title">Título</Label>
              <Input
                id="title"
                value={editedTask.title}
                onChange={(e) => handleChange("title", e.target.value)}
                required
              />
            </div>

            <div className="grid gap-2">
              <Label htmlFor="description">Descripción</Label>
              <Input
                id="description"
                value={editedTask.description || ""}
                onChange={(e) => handleChange("description", e.target.value)}
              />
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div className="grid gap-2">
                <Label htmlFor="startDate">Fecha de inicio</Label>
                <Input
                  id="startDate"
                  type="date"
                  value={format(new Date(editedTask.startDate), "yyyy-MM-dd")}
                  onChange={(e) =>
                    handleChange(
                      "startDate",
                      new Date(e.target.value).toISOString()
                    )
                  }
                  required
                />
              </div>

              <div className="grid gap-2">
                <Label htmlFor="endDate">Fecha de fin</Label>
                <Input
                  id="endDate"
                  type="date"
                  value={format(new Date(editedTask.endDate), "yyyy-MM-dd")}
                  onChange={(e) =>
                    handleChange(
                      "endDate",
                      new Date(e.target.value).toISOString()
                    )
                  }
                  required
                />
              </div>
            </div>

            <div className="grid gap-2">
              <Label htmlFor="progress">Progreso (%)</Label>
              <Input
                id="progress"
                type="number"
                min="0"
                max="100"
                value={editedTask.progress}
                onChange={(e) =>
                  handleChange("progress", Number.parseInt(e.target.value))
                }
              />
            </div>

            <div className="grid gap-2">
              <Label htmlFor="status">Estado</Label>
              <Select
                value={editedTask.status}
                onValueChange={(value) => handleChange("status", value)}
              >
                <SelectTrigger id="status">
                  <SelectValue placeholder="Seleccionar estado" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="pending">Pendiente</SelectItem>
                  <SelectItem value="in-progress">En progreso</SelectItem>
                  <SelectItem value="completed">Completada</SelectItem>
                  <SelectItem value="delayed">Retrasada</SelectItem>
                </SelectContent>
              </Select>
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
