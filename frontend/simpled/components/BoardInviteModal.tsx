'use client';

import { Button } from '@/components/ui/button';
import { useState } from 'react';
import { toast } from 'react-toastify';

const API = 'http://localhost:5193';

type Props = {
  boardId: string;
  onClose: () => void;
  onInvited: () => void;
};

export default function BoardInviteModal({ boardId, onClose, onInvited }: Props) {
  const [email, setEmail] = useState('');
  const [role, setRole] = useState<'viewer' | 'editor' | 'admin'>('viewer');
  const [loading, setLoading] = useState(false);
  const token = localStorage.getItem('token') ?? sessionStorage.getItem('token');

  const handleInvite = async () => {
    if (!email.trim()) {
      toast.warning('El email es obligatorio.');
      return;
    }
    setLoading(true);
    try {
      const res = await fetch(`${API}/api/BoardInvitations`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({ boardId, email: email.trim(), role }),
      });
      if (!res.ok) throw new Error();
      toast.success('Invitación enviada.');
      onInvited();
    } catch {
      toast.error('Error al enviar invitación.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="bg-opacity-50 fixed inset-0 z-50 flex items-center justify-center bg-black">
      <div className="w-full max-w-sm space-y-4 rounded bg-white p-6 shadow-lg dark:bg-neutral-900">
        <h2 className="text-xl font-semibold">Invitar miembro</h2>

        <label htmlFor="email" className="block text-sm font-medium">
          Email del usuario
        </label>
        <input
          id="email"
          type="email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          className="w-full rounded border px-3 py-2"
          placeholder="usuario@ejemplo.com"
        />

        <label htmlFor="role" className="block text-sm font-medium">
          Rol
        </label>
        <select
          id="role"
          value={role}
          onChange={(e) => setRole(e.target.value as any)}
          className="w-full rounded border px-3 py-2"
        >
          <option value="viewer">Viewer</option>
          <option value="editor">Editor</option>
          <option value="admin">Admin</option>
        </select>

        <div className="flex justify-end gap-2 pt-4">
          <Button variant="ghost" onClick={onClose}>
            Cancelar
          </Button>
          <Button onClick={handleInvite} disabled={loading}>
            {loading ? 'Enviando...' : 'Invitar'}
          </Button>
        </div>
      </div>
    </div>
  );
}
