// components/ItemEditModal.tsx
'use client';

import { useAuth } from '@/contexts/AuthContext';
import { useState } from 'react';
import { toast } from 'react-toastify';

type User = { id: string; name: string };
type Props = Readonly<{
  item: {
    id: string;
    title: string;
    description?: string;
    dueDate?: string;
    columnId: string;
    status: 'pending' | 'in-progress' | 'completed' | 'delayed';
    assigneeId?: string | null;
  };
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
  const [dueDate, setDueDate] = useState(item.dueDate?.slice(0, 10) ?? '');
  const [status, setStatus] = useState(item.status);
  const [assigneeId, setAssigneeId] = useState(item.assigneeId ?? '');
  const [loading, setLoading] = useState(false);

  // permisos
  const canChangeAll = userRole === 'admin';
  const canChangeStatus = canChangeAll || item.assigneeId === currentUserId;

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    try {
      const payload: any = {
        id: item.id,
        title,
        description,
        dueDate: dueDate ? new Date(dueDate).toISOString() : null,
        columnId: item.columnId,
        status,
      };
      if (canChangeAll) {
        payload.assigneeId = assigneeId || null;
      }

      const res = await fetch(`http://localhost:5193/api/Items/${item.id}`, {
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

  return (
    <div className="bg-opacity-50 fixed inset-0 z-50 flex items-center justify-center bg-black">
      <div className="w-full max-w-md rounded bg-white p-6 shadow-lg dark:bg-neutral-900">
        <h2 className="mb-4 text-lg font-semibold">Editar Tarea</h2>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label htmlFor="item-title" className="mb-1 block text-sm font-medium">
              Título
            </label>
            <input
              id="item-title"
              type="text"
              className="w-full rounded border px-3 py-2"
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              disabled={!canChangeAll}
              placeholder="Título de la tarea"
            />
          </div>

          <div>
            <label htmlFor="item-description" className="mb-1 block text-sm font-medium">
              Descripción
            </label>
            <textarea
              id="item-description"
              className="w-full rounded border px-3 py-2"
              rows={3}
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              disabled={!canChangeAll}
              placeholder="Descripción de la tarea"
            />
          </div>

          <div>
            <label htmlFor="item-dueDate" className="mb-1 block text-sm font-medium">
              Fecha de vencimiento
            </label>
            <input
              id="item-dueDate"
              type="date"
              className="w-full rounded border px-3 py-2"
              value={dueDate}
              onChange={(e) => setDueDate(e.target.value)}
              disabled={!canChangeAll}
            />
          </div>

          <div>
            <label htmlFor="item-status" className="mb-1 block text-sm font-medium">
              Estado
            </label>
            <select
              id="item-status"
              className="w-full rounded border px-3 py-2"
              value={status}
              onChange={(e) => setStatus(e.target.value as any)}
              disabled={!canChangeStatus}
            >
              <option value="pending">Pendiente</option>
              <option value="in-progress">En progreso</option>
              <option value="completed">Completada</option>
              <option value="delayed">Retrasada</option>
            </select>
          </div>

          {canChangeAll && (
            <div>
              <label htmlFor="item-assignee" className="mb-1 block text-sm font-medium">
                Asignar a
              </label>
              <select
                id="item-assignee"
                className="w-full rounded border px-3 py-2"
                value={assigneeId}
                onChange={(e) => setAssigneeId(e.target.value)}
              >
                <option value="">Sin asignar</option>
                {assignees.map((u) => (
                  <option key={u.id} value={u.id}>
                    {u.name}
                  </option>
                ))}
              </select>
            </div>
          )}

          <div className="flex justify-end gap-2">
            <button type="button" onClick={onClose} className="rounded bg-gray-300 px-4 py-2">
              Cancelar
            </button>
            <button
              type="submit"
              disabled={loading || !canChangeStatus}
              className="rounded bg-blue-600 px-4 py-2 text-white hover:bg-blue-700 disabled:opacity-50"
            >
              {loading ? 'Guardando...' : 'Guardar'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
