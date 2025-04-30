'use client';

import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { useTeams } from '@/contexts/TeamsContext';
import { XIcon } from 'lucide-react';
import { useState } from 'react';
import { toast } from 'react-toastify';

type Member = { userId: string; userName: string; role: string };

interface Props {
  team: {
    id: string;
    name: string;
    ownerId: string;
    members: Member[];
  };
  isOwner: boolean;
  onClose: () => void;
  onUpdated: () => void;
}

export default function TeamDetailModal({ team, isOwner, onClose, onUpdated }: Props) {
  const { inviteToTeam, removeMember } = useTeams();
  const [email, setEmail] = useState('');
  const [processing, setProcessing] = useState(false);

  const handleInvite = async () => {
    const trimmed = email.trim().toLowerCase();
    if (!trimmed) {
      toast.warning('Escribe un correo válido.');
      return;
    }

    setProcessing(true);
    try {
      await inviteToTeam(team.id, trimmed);
      setEmail('');
      onUpdated();
    } catch {
    } finally {
      setProcessing(false);
    }
  };

  const handleRemove = async (userId: string) => {
    setProcessing(true);
    try {
      await removeMember(team.id, userId);
      onUpdated();
    } catch {
    } finally {
      setProcessing(false);
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4">
      <div className="relative w-full max-w-lg rounded bg-white p-6 shadow-lg dark:bg-neutral-900">
        <button
          onClick={onClose}
          className="text-muted-foreground hover:text-foreground absolute top-4 right-4"
        >
          <XIcon className="size-5" />
        </button>
        <h2 className="mb-4 text-xl font-semibold">{team.name}</h2>

        <div className="mb-4 space-y-2">
          <h3 className="font-medium">Miembros actuales</h3>
          <ul className="space-y-1">
            {team.members.map((m) => (
              <li key={m.userId} className="flex items-center justify-between">
                <span>
                  {m.userName} ({m.role})
                </span>
                {isOwner && m.userId !== team.ownerId && (
                  <Button
                    size="sm"
                    variant="destructive"
                    onClick={() => handleRemove(m.userId)}
                    disabled={processing}
                  >
                    Eliminar
                  </Button>
                )}
              </li>
            ))}
          </ul>
        </div>

        {isOwner && (
          <div className="space-y-3">
            <h3 className="font-medium">Invitar por correo</h3>
            <div>
              <Label htmlFor="invite-email">Correo</Label>
              <Input
                id="invite-email"
                placeholder="usuario@dominio.com"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                disabled={processing}
              />
            </div>
            <div className="flex justify-end">
              <Button onClick={handleInvite} disabled={processing}>
                {processing ? 'Enviando…' : 'Invitar'}
              </Button>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
