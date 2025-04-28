'use client';

import { Button } from '@/components/ui/button';
import { useAuth } from '@/contexts/AuthContext';
import { useEffect, useState } from 'react';
import { toast } from 'react-toastify';

type Invite = {
  id: string;
  boardId: string;
  boardName: string;
  role: string;
  token: string;
  createdAt: string;
};

type Props = {
  onClose: () => void;
};

export default function InvitationModal({ onClose }: Props) {
  const { auth } = useAuth();
  const [invites, setInvites] = useState<Invite[]>([]);
  const [loading, setLoading] = useState(true);

  const API = 'http://localhost:5193';

  const fetchInvites = async () => {
    setLoading(true);
    try {
      const res = await fetch(`${API}/api/BoardInvitations/user`, {
        headers: { Authorization: `Bearer ${auth.token}` },
      });
      if (!res.ok) throw new Error();
      setInvites(await res.json());
    } catch {
      toast.error('Error al cargar invitaciones.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchInvites();
  }, []);

  const handleAction = async (token: string, action: 'accept' | 'reject') => {
    try {
      const res = await fetch(`${API}/api/BoardInvitations/${action}`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${auth.token}`,
        },
        body: JSON.stringify({ token }),
      });
      if (!res.ok) throw new Error();
      toast.success(action === 'accept' ? 'Invitación aceptada' : 'Invitación rechazada');
      setInvites((prev) => prev.filter((i) => i.token !== token));
    } catch {
      toast.error('Error al procesar invitación.');
    }
  };

  return (
    <div className="bg-opacity-50 fixed inset-0 z-50 flex items-center justify-center bg-black">
      <div className="w-full max-w-lg space-y-4 rounded bg-white p-6 shadow-lg dark:bg-neutral-900">
        <h2 className="text-xl font-semibold">Invitaciones pendientes</h2>

        {loading ? (
          <p>Cargando...</p>
        ) : invites.length === 0 ? (
          <p>No tienes invitaciones pendientes.</p>
        ) : (
          <ul className="space-y-4">
            {invites.map((inv) => (
              <li key={inv.token} className="flex items-center justify-between rounded border p-4">
                <div>
                  <p className="font-medium">{inv.boardName}</p>
                  <p className="text-sm text-gray-500">
                    Rol invitado: {inv.role} • Recibido:{' '}
                    {new Date(inv.createdAt).toLocaleDateString()}
                  </p>
                </div>
                <div className="flex gap-2">
                  <Button
                    size="sm"
                    variant="ghost"
                    onClick={() => handleAction(inv.token, 'reject')}
                  >
                    Rechazar
                  </Button>
                  <Button size="sm" onClick={() => handleAction(inv.token, 'accept')}>
                    Aceptar
                  </Button>
                </div>
              </li>
            ))}
          </ul>
        )}

        <div className="flex justify-end pt-4">
          <Button variant="ghost" onClick={onClose}>
            Cerrar
          </Button>
        </div>
      </div>
    </div>
  );
}
