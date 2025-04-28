'use client';
import { useState } from 'react';
import { toast } from 'react-toastify';

type User = {
  id: string;
  name: string;
  imageUrl: string;
};

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
  currentUserId?: string | null;
}>;

export default function ItemEditModal({
  item,
  onClose,
  onUpdated,
  assignees,
  userRole,
  currentUserId,
}: Props) {
  const [title, setTitle] = useState(item.title);
  const [description, setDescription] = useState(item.description ?? '');
  const [dueDate, setDueDate] = useState(item.dueDate?.slice(0, 10) ?? '');
  const [status, setStatus] = useState(item.status);
  const [assigneeId, setAssigneeId] = useState<string | null>(item.assigneeId ?? null);
  const [loading, setLoading] = useState(false);

  const isAdmin = userRole === 'admin';
  const isEditorAssigned = userRole === 'editor' && item.assigneeId === currentUserId;

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    try {
      const response = await fetch(`http://localhost:5193/api/Items/${item.id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${
            localStorage.getItem('token') ?? sessionStorage.getItem('token')
          }`,
        },
        body: JSON.stringify({
          id: item.id,
          title,
          description,
          dueDate: dueDate ? new Date(dueDate).toISOString() : null,
          columnId: item.columnId,
          status,
          assigneeId: isAdmin ? assigneeId : item.assigneeId,
        }),
      });
      if (!response.ok) throw new Error();
      toast.success('Tarea actualizada.');
      onUpdated();
      onClose();
    } catch {
      toast.error('No se pudo actualizar la tarea.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="bg-opacity-40 fixed inset-0 z-50 flex items-center justify-center bg-black">
      <div className="w-full max-w-md rounded bg-white p-6 shadow-lg dark:bg-neutral-900">
        <h2 className="mb-4 text-lg font-semibold">Editar tarea</h2>
        <form onSubmit={handleSubmit} className="flex flex-col gap-3">
          <label htmlFor="title" className="block text-sm font-medium">
            Título
          </label>
          <input
            id="title"
            type="text"
            className="rounded border px-3 py-2"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            required
            disabled={isEditorAssigned}
          />

          <label htmlFor="description" className="block text-sm font-medium">
            Descripción
          </label>
          <textarea
            id="description"
            className="rounded border px-3 py-2"
            rows={3}
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            disabled={isEditorAssigned}
          />

          <label htmlFor="dueDate" className="block text-sm font-medium">
            Fecha de vencimiento
          </label>
          <input
            id="dueDate"
            type="date"
            className="rounded border px-3 py-2"
            value={dueDate}
            onChange={(e) => setDueDate(e.target.value)}
            disabled={isEditorAssigned}
          />

          <label htmlFor="status" className="block text-sm font-medium">
            Estado
          </label>
          <select
            id="status"
            className="rounded border px-3 py-2"
            value={status}
            onChange={(e) => setStatus(e.target.value as any)}
          >
            <option value="pending">Pendiente</option>
            <option value="in-progress">En progreso</option>
            <option value="completed">Completada</option>
            <option value="delayed">Retrasada</option>
          </select>

          <label htmlFor="assignee" className="block text-sm font-medium">
            Asignar a
          </label>
          <select
            id="assignee"
            className="rounded border px-3 py-2"
            value={assigneeId ?? ''}
            onChange={(e) => setAssigneeId(e.target.value || null)}
            disabled={!isAdmin}
          >
            <option value="">— Ninguno —</option>
            {assignees.map((u) => (
              <option key={u.id} value={u.id}>
                {u.name}
              </option>
            ))}
          </select>

          <div className="mt-4 flex justify-end gap-2">
            <button
              type="button"
              onClick={onClose}
              className="rounded bg-gray-300 px-4 py-2 dark:bg-gray-700"
            >
              Cancelar
            </button>
            <button
              type="submit"
              disabled={loading}
              className="rounded bg-blue-600 px-4 py-2 text-white hover:bg-blue-700"
            >
              {loading ? 'Guardando...' : 'Guardar cambios'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
