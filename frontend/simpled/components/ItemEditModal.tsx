'use client';
import { useBoards } from '@/contexts/BoardsContext';
import { useState } from 'react';
import { toast } from 'react-toastify';

type Props = {
  item: {
    id: string;
    title: string;
    description?: string;
    dueDate?: string;
    columnId: string;
  };
  onClose: () => void;
  onUpdated: () => void;
};

export default function ItemEditModal({ item, onClose, onUpdated }: Props) {
  const [title, setTitle] = useState(item.title);
  const [description, setDescription] = useState(item.description || '');
  const [dueDate, setDueDate] = useState(item.dueDate?.slice(0, 10) || '');
  const { updateBoard } = useBoards(); // reutilizamos contexto por si se refresca al cerrar

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const response = await fetch(`http://localhost:5193/api/Items/${item.id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${
            localStorage.getItem('token') || sessionStorage.getItem('token')
          }`,
        },
        body: JSON.stringify({
          id: item.id,
          title,
          description,
          dueDate: dueDate ? new Date(dueDate).toISOString() : null,
          columnId: item.columnId,
        }),
      });

      if (!response.ok) throw new Error('Error al actualizar la tarea.');
      toast.success('Tarea actualizada.');
      onUpdated();
      onClose();
    } catch (err) {
      console.error(err);
      toast.error('No se pudo actualizar la tarea.');
    }
  };

  return (
    <div className="bg-opacity-40 fixed inset-0 z-50 flex items-center justify-center bg-black">
      <div className="w-full max-w-md rounded bg-white p-6 shadow-md dark:bg-neutral-900">
        <h2 className="mb-4 text-lg font-semibold">Editar Tarea</h2>
        <form onSubmit={handleSubmit} className="flex flex-col gap-3">
          <label
            htmlFor="title"
            className="block text-sm font-medium text-gray-700 dark:text-gray-300"
          >
            Título
          </label>
          <input
            id="title"
            type="text"
            className="rounded border px-3 py-2"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            placeholder="Ingrese el título"
            required
          />
          <textarea
            className="rounded border px-3 py-2"
            rows={3}
            placeholder="Descripción (opcional)"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
          />
          <input
            type="date"
            className="rounded border px-3 py-2"
            value={dueDate}
            onChange={(e) => setDueDate(e.target.value)}
            title="Fecha de vencimiento"
            placeholder="Seleccione una fecha"
          />
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
              className="rounded bg-blue-600 px-4 py-2 text-white hover:bg-blue-700"
            >
              Guardar
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
