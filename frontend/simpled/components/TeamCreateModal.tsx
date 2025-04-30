'use client';

import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { useAuth } from '@/contexts/AuthContext';
import { useState } from 'react';
import { toast } from 'react-toastify';

type Props = {
  onClose: () => void;
  onCreated: () => void;
};

export default function TeamCreateModal({ onClose, onCreated }: Props) {
  const { auth } = useAuth();
  const [name, setName] = useState('');
  const [loading, setLoading] = useState(false);
  const API = 'http://localhost:5193';

  const handleCreate = async () => {
    if (!name.trim()) {
      toast.warning('El nombre no puede estar vacío.');
      return;
    }
    setLoading(true);
    try {
      const res = await fetch(`${API}/api/Teams`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${auth.token}`,
        },
        body: JSON.stringify({ name: name.trim() }),
      });
      if (!res.ok) throw new Error();
      toast.success('Equipo creado.');
      onCreated();
    } catch {
      toast.error('Error al crear equipo.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50">
      <div className="w-full max-w-md rounded bg-white p-6 shadow-lg dark:bg-neutral-900">
        <h2 className="mb-4 text-xl font-semibold">Nuevo Equipo</h2>
        <div className="space-y-3">
          <div>
            <Label htmlFor="team-name">Nombre</Label>
            <Input
              id="team-name"
              placeholder="Escribe el nombre del equipo"
              value={name}
              onChange={(e) => setName(e.target.value)}
            />
          </div>
          <div className="flex justify-end gap-2">
            <Button variant="outline" onClick={onClose} disabled={loading}>
              Cancelar
            </Button>
            <Button onClick={handleCreate} disabled={loading}>
              {loading ? 'Creando…' : 'Crear'}
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
}
