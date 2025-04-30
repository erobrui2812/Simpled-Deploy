'use client';

import TeamCreateModal from '@/components/TeamCreateModal';
import TeamDetailModal from '@/components/TeamDetailModal';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { useAuth } from '@/contexts/AuthContext';
import { Users } from 'lucide-react';
import { notFound } from 'next/navigation';
import { useEffect, useState } from 'react';
import { toast } from 'react-toastify';

interface Team {
  id: string;
  name: string;
  ownerId: string;
  ownerName: string;
  members: { userId: string; userName: string; role: string }[];
}

export default function TeamsPage() {
  const { auth } = useAuth();
  const [teams, setTeams] = useState<Team[]>([]);
  const [loading, setLoading] = useState(true);
  const [showCreate, setShowCreate] = useState(false);
  const [activeTeam, setActiveTeam] = useState<Team | null>(null);

  const API = 'http://localhost:5193';

  const fetchTeams = async () => {
    setLoading(true);
    try {
      const res = await fetch(`${API}/api/Teams`, {
        headers: { Authorization: `Bearer ${auth.token}` },
      });
      if (!res.ok) throw new Error();
      setTeams(await res.json());
    } catch {
      toast.error('No se pudieron cargar los equipos.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchTeams();
  }, [auth.token]);

  if (!auth.id) notFound();

  return (
    <div className="container mx-auto p-4">
      <div className="mb-6 flex items-center justify-between">
        <h1 className="flex items-center gap-2 text-2xl font-bold">
          <Users className="h-6 w-6 text-blue-500" /> Mis Equipos
        </h1>
        <Button onClick={() => setShowCreate(true)}>Nuevo Equipo</Button>
      </div>

      {loading ? (
        <p>Cargando equipos…</p>
      ) : teams.length === 0 ? (
        <div className="flex h-64 items-center justify-center">
          <Card className="max-w-md text-center">
            <CardHeader>
              <Users className="mx-auto mb-4 h-8 w-8 text-gray-400" />
              <CardTitle>No perteneces a ningún equipo</CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              <p className="text-muted-foreground">
                Aún no formas parte de ningún equipo. Crea uno para invitar a tus compañeros o
                espera una invitación.
              </p>
              <div className="flex justify-center">
                <Button onClick={() => setShowCreate(true)}>Crear mi primer equipo</Button>
              </div>
            </CardContent>
          </Card>
        </div>
      ) : (
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {teams.map((t) => (
            <Card key={t.id} className="cursor-pointer" onClick={() => setActiveTeam(t)}>
              <CardHeader>
                <CardTitle>{t.name}</CardTitle>
              </CardHeader>
              <CardContent>
                <p className="text-muted-foreground text-sm">Owner: {t.ownerName}</p>
                <p className="text-muted-foreground text-sm">Miembros: {t.members.length}</p>
              </CardContent>
            </Card>
          ))}
        </div>
      )}

      {showCreate && (
        <TeamCreateModal
          onClose={() => setShowCreate(false)}
          onCreated={() => {
            setShowCreate(false);
            fetchTeams();
          }}
        />
      )}

      {activeTeam && (
        <TeamDetailModal
          team={activeTeam}
          isOwner={activeTeam.ownerId === auth.id}
          onClose={() => setActiveTeam(null)}
          onUpdated={() => {
            setActiveTeam(null);
            fetchTeams();
          }}
        />
      )}
    </div>
  );
}
