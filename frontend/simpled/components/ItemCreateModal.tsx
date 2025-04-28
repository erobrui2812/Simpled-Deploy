'use client';

import { useAuth } from '@/contexts/AuthContext';
import { useState } from 'react';
import { toast } from 'react-toastify';

const API = 'http://localhost:5193';

type User = { id: string; name: string };

type Props = Readonly<{
  columnId: string;
  onCreated: () => void;
  onClose: () => void;
  assignees: User[];
  userRole?: string;
}>;

export default function ItemCreateModal({
  columnId,
  onClose,
  onCreated,
  assignees,
  userRole,
}: Props) {
  const { auth } = useAuth();
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [dueDate, setDueDate] = useState('');
  const [status, setStatus] = useState<'pending' | 'in-progress' | 'completed' | 'delayed'>(
    'pending',
  );
  const [assigneeId, setAssigneeId] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const handleCreate = async () => {
    if (!title.trim()) {
      toast.warning('El título es obligatorio');
      return;
    }
    setLoading(true);
    try {
      const payload: any = {
        title: title.trim(),
        description: description.trim() || null,
        dueDate: dueDate ? new Date(dueDate).toISOString() : null,
        columnId,
        status,
      };
      // Solo admin puede asignar al crear
      if (userRole === 'admin' && assigneeId) {
        payload.assigneeId = assigneeId;
      }

      const res = await fetch(`${API}/api/Items`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${auth.token}`,
        },
        body: JSON.stringify(payload),
      });
      if (!res.ok) throw new Error();
      toast.success('Tarea creada');
      onCreated();
      onClose();
    } catch {
      toast.error('Error creando tarea');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="bg-opacity-50 fixed inset-0 z-50 flex items-center justify-center bg-black">
      <div className="w-full max-w-sm rounded bg-white p-6 shadow-lg dark:bg-neutral-900">
        <h2 className="mb-4 text-xl font-semibold">Nueva tarea</h2>
        <form className="space-y-4">
          <div>
            <label htmlFor="item-title" className="mb-1 block text-sm font-medium">
              Título
            </label>
            <input
              id="item-title"
              type="text"
              className="mb-1 w-full rounded border px-3 py-2"
              placeholder="Título de la tarea"
              value={title}
              onChange={(e) => setTitle(e.target.value)}
            />
          </div>

          <div>
            <label htmlFor="item-description" className="mb-1 block text-sm font-medium">
              Descripción
            </label>
            <textarea
              id="item-description"
              className="mb-1 w-full rounded border px-3 py-2"
              rows={3}
              placeholder="Descripción de la tarea"
              value={description}
              onChange={(e) => setDescription(e.target.value)}
            />
          </div>

          <div>
            <label htmlFor="item-dueDate" className="mb-1 block text-sm font-medium">
              Fecha de vencimiento
            </label>
            <input
              id="item-dueDate"
              type="date"
              className="mb-1 w-full rounded border px-3 py-2"
              value={dueDate}
              onChange={(e) => setDueDate(e.target.value)}
            />
          </div>

          <div>
            <label htmlFor="item-status" className="mb-1 block text-sm font-medium">
              Estado
            </label>
            <select
              id="item-status"
              className="mb-1 w-full rounded border px-3 py-2"
              value={status}
              onChange={(e) => setStatus(e.target.value as any)}
            >
              <option value="pending">Pendiente</option>
              <option value="in-progress">En progreso</option>
              <option value="completed">Completada</option>
              <option value="delayed">Retrasada</option>
            </select>
          </div>

          {userRole === 'admin' && (
            <div>
              <label htmlFor="item-assignee" className="mb-1 block text-sm font-medium">
                Asignar a
              </label>
              <select
                id="item-assignee"
                className="mb-1 w-full rounded border px-3 py-2"
                value={assigneeId ?? ''}
                onChange={(e) => setAssigneeId(e.target.value || null)}
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

          {/* Botones */}
          <div className="flex justify-end gap-2">
            <button type="button" onClick={onClose} className="rounded bg-gray-300 px-4 py-2">
              Cancelar
            </button>
            <button
              type="button"
              onClick={handleCreate}
              disabled={loading}
              className="rounded bg-blue-600 px-4 py-2 text-white hover:bg-blue-700 disabled:opacity-50"
            >
              {loading ? 'Creando...' : 'Crear'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
