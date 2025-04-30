'use client';

import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { useTeams } from '@/contexts/TeamsContext';
import { useState } from 'react';
import { toast } from 'react-toastify';

type Props = {
  onClose: () => void;
  onCreated: () => void;
};

export default function TeamCreateModal({ onClose, onCreated }: Props) {
  const { createTeam } = useTeams();
  const [name, setName] = useState('');
  const [loading, setLoading] = useState(false);

  const handleCreate = async () => {
    if (!name.trim()) {
      toast.warning('El nombre no puede estar vacío.');
      return;
    }

    setLoading(true);
    try {
      await createTeam(name.trim());
      onCreated();
    } catch {
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
              disabled={loading}
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
