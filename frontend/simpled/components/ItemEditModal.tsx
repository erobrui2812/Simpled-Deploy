'use client';

import { DatePicker } from '@/components/DatePicker';
import { Button } from '@/components/ui/button';
import {
  Dialog,
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Textarea } from '@/components/ui/textarea';
import { useAuth } from '@/contexts/AuthContext';
import type { Item, Subtask, User } from '@/types';
import { Check, Loader2, X } from 'lucide-react';
import type React from 'react';
import { useEffect, useState } from 'react';
import { toast } from 'react-toastify';
import SubtaskList from './SubtaskList';

const API = 'http://localhost:5193';

type Props = Readonly<{
  item: Item;
  onClose: () => void;
  onUpdated: () => void;
  assignees: User[];
  userRole?: string;
  currentUserId: string;
}>;

export default function ItemEditModal({
  item,
  onClose,
  onUpdated,
  assignees,
  userRole,
  currentUserId,
}: Props) {
  const { auth } = useAuth();
  const [title, setTitle] = useState(item.title);
  const [description, setDescription] = useState(item.description ?? '');
  const [startDate, setStartDate] = useState<Date | undefined>(
    item.startDate ? new Date(item.startDate) : undefined,
  );
  const [dueDate, setDueDate] = useState<Date | undefined>(
    item.dueDate ? new Date(item.dueDate) : undefined,
  );
  const [status, setStatus] = useState(item.status ?? 'pending');
  const [assigneeId, setAssigneeId] = useState(item.assigneeId ?? '');
  const [subtasks, setSubtasks] = useState<Subtask[]>(item.subtasks || []);
  const [loading, setLoading] = useState(false);
  const [activeTab, setActiveTab] = useState('details');

  // permisos
  const canChangeAll = userRole === 'admin';
  const canChangeStatus = canChangeAll || item.assigneeId === currentUserId;

  // Fetch subtasks on load
  useEffect(() => {
    const fetchSubtasks = async () => {
      try {
        const res = await fetch(`${API}/api/items/${item.id}/subtasks`, {
          headers: {
            Authorization: `Bearer ${auth.token}`,
          },
        });
        if (res.ok) {
          const data = await res.json();
          setSubtasks(data);
        }
      } catch (error) {
        console.error('Error fetching subtasks:', error);
      }
    };

    fetchSubtasks();
  }, [item.id, auth.token]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!title.trim()) {
      toast.warning('El título es obligatorio');
      return;
    }

    setLoading(true);
    try {
      const payload: any = {
        id: item.id,
        title,
        description,
        startDate: startDate ? startDate.toISOString() : null,
        dueDate: dueDate ? dueDate.toISOString() : null,
        columnId: item.columnId,
        status,
      };
      if (canChangeAll) {
        payload.assigneeId = assigneeId || null;
      }

      const res = await fetch(`${API}/api/Items/${item.id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${auth.token}`,
        },
        body: JSON.stringify(payload),
      });

      if (!res.ok) throw new Error('Error al actualizar');
      toast.success('Tarea actualizada');
      onUpdated();
      onClose();
    } catch {
      toast.error('Error actualizando tarea');
    } finally {
      setLoading(false);
    }
  };

  const handleAddSubtask = async (title: string) => {
    try {
      const res = await fetch(`${API}/api/items/${item.id}/subtasks`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${auth.token}`,
        },
        body: JSON.stringify({
          itemId: item.id,
          title,
        }),
      });

      if (!res.ok) throw new Error('Error al crear subtarea');

      const newSubtask = await res.json();
      setSubtasks((prev) => [...prev, newSubtask]);
      toast.success('Subtarea añadida');
      return true;
    } catch (error) {
      toast.error('Error al crear subtarea');
      console.error(error);
      throw error;
    }
  };

  const handleUpdateSubtask = async (subtask: Subtask) => {
    try {
      const res = await fetch(`${API}/api/items/${item.id}/subtasks/${subtask.id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${auth.token}`,
        },
        body: JSON.stringify({
          id: subtask.id,
          itemId: item.id,
          title: subtask.title,
          isCompleted: subtask.isCompleted,
        }),
      });

      if (!res.ok) throw new Error('Error al actualizar subtarea');

      setSubtasks((prev) => prev.map((st) => (st.id === subtask.id ? subtask : st)));

      // Update task status based on subtasks completion
      const allSubtasks = subtasks.length;
      const completedSubtasks = subtasks.filter((st) =>
        st.id === subtask.id ? subtask.isCompleted : st.isCompleted,
      ).length;

      if (allSubtasks > 0 && completedSubtasks === allSubtasks) {
        setStatus('completed');
      } else if (completedSubtasks > 0) {
        setStatus('in-progress');
      }

      return true;
    } catch (error) {
      toast.error('Error al actualizar subtarea');
      console.error(error);
      throw error;
    }
  };

  const handleDeleteSubtask = async (subtaskId: string) => {
    try {
      const res = await fetch(`${API}/api/items/${item.id}/subtasks/${subtaskId}`, {
        method: 'DELETE',
        headers: {
          Authorization: `Bearer ${auth.token}`,
        },
      });

      if (!res.ok) throw new Error('Error al eliminar subtarea');

      setSubtasks((prev) => prev.filter((st) => st.id !== subtaskId));
      toast.success('Subtarea eliminada');
      return true;
    } catch (error) {
      toast.error('Error al eliminar subtarea');
      console.error(error);
      throw error;
    }
  };

  return (
    <Dialog open={true} onOpenChange={onClose}>
      <DialogContent className="animate-scaleIn sm:max-w-md">
        <DialogHeader>
          <DialogTitle>Editar Tarea</DialogTitle>
        </DialogHeader>

        <Tabs value={activeTab} onValueChange={setActiveTab} className="w-full">
          <TabsList className="grid w-full grid-cols-2">
            <TabsTrigger value="details">Detalles</TabsTrigger>
            <TabsTrigger value="subtasks">Subtareas ({subtasks.length})</TabsTrigger>
          </TabsList>

          <TabsContent value="details" className="space-y-4 py-4">
            <div className="space-y-2">
              <Label htmlFor="item-title">Título</Label>
              <Input
                id="item-title"
                type="text"
                value={title}
                onChange={(e) => setTitle(e.target.value)}
                disabled={!canChangeAll}
                placeholder="Título de la tarea"
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="item-description">Descripción</Label>
              <Textarea
                id="item-description"
                value={description}
                onChange={(e) => setDescription(e.target.value)}
                disabled={!canChangeAll}
                placeholder="Descripción de la tarea"
                rows={3}
              />
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div className="space-y-2">
                <Label htmlFor="item-startDate">Fecha de inicio</Label>
                <DatePicker
                  date={startDate}
                  onDateChange={setStartDate}
                  placeholder="Fecha inicio"
                  disabled={!canChangeAll}
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="item-dueDate">Fecha de vencimiento</Label>
                <DatePicker
                  date={dueDate}
                  onDateChange={setDueDate}
                  placeholder="Fecha fin"
                  disabled={!canChangeAll}
                />
              </div>
            </div>

            <div className="space-y-2">
              <Label htmlFor="item-status">Estado</Label>
              <Select
                value={status}
                onValueChange={(value) => setStatus(value as any)}
                disabled={!canChangeStatus}
              >
                <SelectTrigger id="item-status">
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

            {canChangeAll && (
              <div className="space-y-2">
                <Label htmlFor="item-assignee">Asignar a</Label>
                <Select
                  value={assigneeId || 'not_assigned'}
                  onValueChange={(value) => setAssigneeId(value === 'not_assigned' ? '' : value)}
                >
                  <SelectTrigger id="item-assignee">
                    <SelectValue placeholder="Sin asignar" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="not_assigned">Sin asignar</SelectItem>
                    {assignees.map((u) => (
                      <SelectItem key={u.id} value={u.id}>
                        {u.name}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
            )}
          </TabsContent>

          <TabsContent value="subtasks" className="py-4">
            <SubtaskList
              subtasks={subtasks}
              itemId={item.id}
              onAdd={handleAddSubtask}
              onUpdate={handleUpdateSubtask}
              onDelete={handleDeleteSubtask}
              disabled={!canChangeStatus}
            />
          </TabsContent>
        </Tabs>

        <DialogFooter className="flex pt-4 sm:justify-between">
          <Button type="button" variant="outline" onClick={onClose} disabled={loading}>
            <X className="mr-2 h-4 w-4" />
            Cancelar
          </Button>
          <Button onClick={handleSubmit} disabled={loading || !canChangeStatus}>
            {loading ? (
              <span className="inline-flex items-center">
                <Loader2 className="mr-2 -ml-1 h-4 w-4 animate-spin" />
                Guardando...
              </span>
            ) : (
              <>
                <Check className="mr-2 h-4 w-4" />
                Guardar
              </>
            )}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
