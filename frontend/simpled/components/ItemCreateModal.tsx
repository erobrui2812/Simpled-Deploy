'use client';

import { useAuth } from '@/contexts/AuthContext';
import { useState } from 'react';
import { toast } from 'react-toastify';

type User = {
  id: string;
  name: string;
  imageUrl: string;
};

type Props = {
  readonly columnId: string;
  readonly onCreated: () => void;
  readonly onClose: () => void;
  readonly assignees: User[];
  readonly userRole?: string;
};

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
  const [dueDate, setDueDate] = useState<string>('');
  const [status, setStatus] = useState<'pending' | 'in-progress' | 'completed' | 'delayed'>(
    'pending',
  );
  const [assigneeId, setAssigneeId] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const canAssign = userRole === 'admin';

  const handleCreate = async () => {
    if (!title.trim()) {
      toast.warning('El título es obligatorio.');
      return;
    }
    setLoading(true);
    try {
      const response = await fetch(`http://localhost:5193/api/Items`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${auth.token}`,
        },
        body: JSON.stringify({
          title: title.trim(),
          description: description.trim() || null,
          dueDate: dueDate ? new Date(dueDate).toISOString() : null,
          columnId,
          status,
          assigneeId: canAssign ? assigneeId : null,
        }),
      });
      if (!response.ok) throw new Error();
      toast.success('Tarea creada correctamente.');
      onCreated();
      onClose();
    } catch {
      toast.error('Error al crear tarea.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="bg-opacity-50 fixed inset-0 z-50 flex items-center justify-center bg-black">
      <div className="w-full max-w-sm rounded bg-white p-6 shadow dark:bg-neutral-900">
        <h2 className="mb-4 text-xl font-semibold">Nueva tarea</h2>

        <input
          type="text"
          placeholder="Título"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
          className="mb-3 w-full rounded border px-3 py-2"
        />

        <textarea
          placeholder="Descripción (opcional)"
          value={description}
          onChange={(e) => setDescription(e.target.value)}
          className="mb-3 w-full rounded border px-3 py-2"
          rows={3}
        />

        <label htmlFor="dueDate" className="mb-1 block text-sm font-medium">
          Fecha de vencimiento
        </label>
        <input
          id="dueDate"
          type="date"
          value={dueDate}
          onChange={(e) => setDueDate(e.target.value)}
          className="mb-4 w-full rounded border px-3 py-2"
        />

        <label htmlFor="status" className="mb-1 block text-sm font-medium">
          Estado
        </label>
        <select
          id="status"
          className="mb-4 w-full rounded border px-3 py-2"
          value={status}
          onChange={(e) => setStatus(e.target.value as any)}
        >
          <option value="pending">Pendiente</option>
          <option value="in-progress">En progreso</option>
          <option value="completed">Completada</option>
          <option value="delayed">Retrasada</option>
        </select>

        <label htmlFor="assignee" className="mb-1 block text-sm font-medium">
          Asignar a
        </label>
        <select
          id="assignee"
          className="mb-4 w-full rounded border px-3 py-2"
          value={assigneeId ?? ''}
          onChange={(e) => setAssigneeId(e.target.value || null)}
          disabled={!canAssign}
        >
          <option value="">— Ninguno —</option>
          {assignees.map((u) => (
            <option key={u.id} value={u.id}>
              {u.name}
            </option>
          ))}
        </select>

        <div className="flex justify-end gap-2">
          <button onClick={onClose} className="rounded bg-gray-300 px-4 py-2 dark:bg-gray-700">
            Cancelar
          </button>
          <button
            onClick={handleCreate}
            disabled={loading}
            className="rounded bg-blue-600 px-4 py-2 text-white hover:bg-blue-700"
          >
            {loading ? 'Creando...' : 'Crear'}
          </button>
        </div>
      </div>
    </div>
  );
}
