'use client';

import { useAuth } from '@/contexts/AuthContext';
import { useState } from 'react';
import { toast } from 'react-toastify';

const API = 'http://localhost:5193';

type Props = {
  readonly boardId: string;
  readonly onClose: () => void;
  readonly onCreated: () => void;
};

export default function ColumnCreateModal({ boardId, onClose, onCreated }: Props) {
  const { auth } = useAuth();
  const [title, setTitle] = useState('');
  const [loading, setLoading] = useState(false);

  const handleCreate = async () => {
    if (!title.trim()) {
      toast.warning('El título es obligatorio.');
      return;
    }

    setLoading(true);
    try {
      const res = await fetch(`${API}/api/Columns`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${auth.token}`,
        },
        body: JSON.stringify({
          boardId,
          title,
          order: 0,
        }),
      });

      if (!res.ok) throw new Error('Error al crear la columna.');

      toast.success('Columna creada correctamente.');
      onCreated();
      onClose();
    } catch (err) {
      console.error(err);
      toast.error('No se pudo crear la columna.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="bg-opacity-50 fixed inset-0 z-50 flex items-center justify-center bg-black">
      <div className="w-full max-w-sm rounded bg-white p-6 shadow dark:bg-neutral-900">
        <h2 className="mb-4 text-xl font-semibold">Nueva columna</h2>

        <input
          type="text"
          placeholder="Título"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
          className="mb-4 w-full rounded border px-3 py-2"
        />

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
