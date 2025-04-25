'use client';

import ColumnCreateModal from '@/components/ColumnCreateModal';
import ColumnEditModal from '@/components/ColumnEditModal';
import ItemCreateModal from '@/components/ItemCreateModal';
import ItemEditModal from '@/components/ItemEditModal';
import { useAuth } from '@/contexts/AuthContext';
import { BookUser, Lock, LockOpen, PenLine, Plus, UserPlus } from 'lucide-react';
import { useEffect, useState } from 'react';
import { toast } from 'react-toastify';

const API = 'http://localhost:5193';

export default function BoardDetails({ boardId }: Readonly<{ boardId: string }>) {
  const { auth } = useAuth();

  const [board, setBoard] = useState<any>(null);
  const [columns, setColumns] = useState<any[]>([]);
  const [items, setItems] = useState<any[]>([]);
  const [members, setMembers] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);

  const [showCreateColumn, setShowCreateColumn] = useState(false);
  const [showInviteModal, setShowInviteModal] = useState(false);

  const [createItemColumnId, setCreateItemColumnId] = useState<string | null>(null);
  const [editColumnId, setEditColumnId] = useState<string | null>(null);
  const [editColumnTitle, setEditColumnTitle] = useState<string>('');
  const [editItem, setEditItem] = useState<any>(null);

  const [inviteEmail, setInviteEmail] = useState('');
  const [inviteRole, setInviteRole] = useState('editor');

  const userId = getUserIdFromToken(auth.token);
  const userMember = Array.isArray(members) ? members.find((m) => m.userId === userId) : null;
  const userRole = userMember?.role;
  const canEdit = userRole === 'admin' || userRole === 'editor';

  const headers: HeadersInit = {};
  if (auth.token) headers['Authorization'] = `Bearer ${auth.token}`;

  const fetchData = async () => {
    try {
      const [boardRes, columnRes, itemRes, membersRes] = await Promise.all([
        fetch(`${API}/api/Boards/${boardId}`, { headers }),
        fetch(`${API}/api/Columns`, { headers }),
        fetch(`${API}/api/Items`, { headers }),
        fetch(`${API}/api/BoardMembers/board/${boardId}`, { headers }),
      ]);

      if (!boardRes.ok) throw new Error('Error al cargar el tablero.');

      const boardData = await boardRes.json();
      const columnData = (await columnRes.json()).filter((c: any) => c.boardId === boardId);
      const itemData = await itemRes.json();

      let membersData: any[] = [];
      try {
        const raw = await membersRes.text();
        membersData = raw ? JSON.parse(raw) : [];
      } catch {
        membersData = [];
      }

      setBoard(boardData);
      setColumns(columnData);
      setItems(itemData);
      setMembers(membersData);
    } catch (err) {
      console.error('Error al cargar el tablero:', err);
    } finally {
      setLoading(false);
    }
  };

  const sendInvitation = async () => {
    try {
      const res = await fetch(`${API}/api/BoardInvitations`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${auth.token}`,
        },
        body: JSON.stringify({
          boardId,
          email: inviteEmail,
          role: inviteRole,
        }),
      });

      if (!res.ok) throw new Error('No se pudo enviar la invitación.');

      toast.success('Invitación enviada correctamente.');
      setShowInviteModal(false);
      setInviteEmail('');
      setInviteRole('editor');
    } catch (err) {
      console.error(err);
      toast.error('Error al enviar la invitación.');
    }
  };

  useEffect(() => {
    fetchData();
  }, [boardId, auth.token]);

  if (loading) return <div className="p-8">Cargando tablero...</div>;
  if (!board) return <div className="p-8 text-red-600">Tablero no encontrado</div>;

  return (
    <div className="mx-auto max-w-6xl p-8">
      <div className="mb-4 flex items-center justify-between">
        <h1 className="text-2xl font-bold">{board.name}</h1>
        <span className="rounded bg-gray-200 p-2 text-sm dark:bg-gray-700">
          {board.isPublic ? (
            <div className="inline-flex items-center gap-1 align-middle">
              <LockOpen className="h-5 w-5" /> Público
            </div>
          ) : (
            <div className="inline-flex items-center gap-1 align-middle">
              <Lock className="h-5 w-5" /> Privado
            </div>
          )}
        </span>
      </div>

      {userRole === 'admin' && (
        <div className="mb-3 flex flex-wrap gap-2">
          <button
            onClick={() => setShowCreateColumn(true)}
            className="rounded bg-blue-600 p-2 px-3 text-sm text-white hover:bg-blue-700"
          >
            <div className="inline-flex items-center gap-1 align-middle">
              <Plus className="h-5 w-5" /> Añadir columna
            </div>
          </button>

          <button
            onClick={() => setShowInviteModal(true)}
            className="rounded bg-green-600 p-2 px-3 text-sm text-white hover:bg-green-700"
          >
            <div className="inline-flex items-center gap-1 align-middle">
              <UserPlus className="h-5 w-5" /> Invitar usuarios
            </div>
          </button>
        </div>
      )}

      <p className="mb-6 inline-flex items-center gap-1 align-middle text-sm text-gray-500 dark:text-gray-400">
        <BookUser className="h-5 w-5" /> Miembros: {members.length}
      </p>

      <div className="grid grid-cols-1 gap-6 md:grid-cols-2">
        {columns.map((col) => (
          <div key={col.id} className="rounded bg-white p-4 shadow dark:bg-neutral-800">
            <div className="mb-2 flex items-center justify-between">
              <h2 className="text-lg font-semibold">{col.title}</h2>
              {canEdit && (
                <button
                  onClick={() => {
                    setEditColumnId(col.id);
                    setEditColumnTitle(col.title);
                  }}
                  className="rounded bg-blue-600 p-1.5 text-sm text-white hover:bg-blue-700"
                >
                  <div className="inline-flex items-center gap-1 align-middle">
                    <PenLine className="h-4 w-4" /> Editar
                  </div>
                </button>
              )}
            </div>
            <div className="mb-2.5 flex flex-col gap-2">
              {items
                .filter((item) => item.columnId === col.id)
                .map((item) => (
                  <button
                    key={item.id}
                    className="cursor-pointer rounded border bg-neutral-50 p-2 text-left dark:bg-neutral-900"
                    onClick={() => canEdit && setEditItem(item)}
                  >
                    <strong>{item.title}</strong>
                    {item.description && (
                      <p className="text-sm text-gray-500">{item.description}</p>
                    )}
                  </button>
                ))}
            </div>
            {canEdit && (
              <button
                onClick={() => setCreateItemColumnId(col.id)}
                className="rounded bg-blue-600 p-1.5 text-sm text-white hover:bg-blue-700"
              >
                <div className="inline-flex items-center gap-1 align-middle">
                  <Plus className="h-5 w-5" /> Añadir tarea
                </div>
              </button>
            )}
          </div>
        ))}
      </div>

      {showCreateColumn && (
        <ColumnCreateModal
          boardId={boardId}
          onClose={() => setShowCreateColumn(false)}
          onCreated={fetchData}
        />
      )}

      {createItemColumnId && (
        <ItemCreateModal
          columnId={createItemColumnId}
          onClose={() => setCreateItemColumnId(null)}
          onCreated={fetchData}
        />
      )}

      {editColumnId && (
        <ColumnEditModal
          columnId={editColumnId}
          currentTitle={editColumnTitle}
          boardId={boardId}
          onClose={() => setEditColumnId(null)}
          onUpdated={fetchData}
          token={auth.token!}
        />
      )}

      {editItem && (
        <ItemEditModal item={editItem} onClose={() => setEditItem(null)} onUpdated={fetchData} />
      )}

      {showInviteModal && (
        <div className="bg-opacity-40 fixed inset-0 z-50 flex items-center justify-center bg-black backdrop-blur-sm">
          <div className="w-full max-w-md rounded bg-white p-6 shadow-lg dark:bg-neutral-800">
            <h2 className="mb-4 text-lg font-semibold">Invitar a un usuario</h2>

            <label htmlFor="inviteEmail" className="mb-1 block text-sm">
              Correo electrónico
            </label>
            <input
              id="inviteEmail"
              type="email"
              className="mb-4 w-full rounded border p-2"
              value={inviteEmail}
              onChange={(e) => setInviteEmail(e.target.value)}
              placeholder="Introduce el correo electrónico"
            />

            <label htmlFor="inviteRole" className="mb-1 block text-sm">
              Rol asignado
            </label>
            <select
              id="inviteRole"
              className="mb-4 w-full rounded border p-2"
              value={inviteRole}
              onChange={(e) => setInviteRole(e.target.value)}
            >
              <option value="editor">Editor</option>
              <option value="viewer">Visualizador</option>
            </select>

            <div className="flex justify-end gap-2">
              <button
                className="rounded bg-gray-400 px-4 py-2 text-white hover:bg-gray-500"
                onClick={() => setShowInviteModal(false)}
              >
                Cancelar
              </button>
              <button
                className="rounded bg-green-600 px-4 py-2 text-white hover:bg-green-700"
                onClick={sendInvitation}
              >
                Enviar invitación
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );

  function getUserIdFromToken(token: string | null): string | null {
    if (!token) return null;
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];
    } catch {
      return null;
    }
  }
}
