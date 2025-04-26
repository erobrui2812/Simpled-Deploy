'use client';
import type React from 'react';
import { useState } from 'react';

import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { toast } from 'react-toastify';

const API = 'https://localhost:7177';

export default function ColumnEditModal({
  columnId,
  currentTitle,
  onClose,
  onUpdated,
  token,
}: Readonly<{
  columnId: string;
  currentTitle: string;
  token: string;
  onClose: () => void;
  onUpdated: () => void;
}>) {
  const [title, setTitle] = useState(currentTitle);
  const [loading, setLoading] = useState(false);

  const handleUpdate = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);

    try {
      const res = await fetch(`${API}/api/Columns/${columnId}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({ id: columnId, title }),
      });

      if (!res.ok) throw new Error('Error al actualizar la columna.');

      toast.success('Columna actualizada.');
      onUpdated();
      onClose();
    } catch (err) {
      console.error(err);
      toast.error('Error al actualizar columna.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="bg-opacity-40 fixed inset-0 z-50 flex items-center justify-center bg-black p-4">
      <div className="w-full max-w-md rounded bg-white p-6 shadow-lg dark:bg-neutral-900">
        <h2 className="mb-4 text-lg font-semibold">Editar columna</h2>
        <form onSubmit={handleUpdate} className="flex flex-col gap-4">
          <Input
            type="text"
            placeholder="Nuevo título"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            className="mb-4"
          />
          <div className="flex justify-end gap-2">
            <Button type="button" onClick={onClose} variant="outline">
              Cancelar
            </Button>
            <Button type="submit" disabled={loading}>
              {loading ? 'Guardando...' : 'Guardar cambios'}
            </Button>
          </div>
        </form>
      </div>
    </div>
  );
}
