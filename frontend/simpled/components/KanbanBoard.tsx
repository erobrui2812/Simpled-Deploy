'use client';

import { Button } from '@/components/ui/button';
import { useAuth } from '@/contexts/AuthContext';
import { useSignalR } from '@/contexts/SignalRContext';
import {
  DndContext,
  DragOverlay,
  KeyboardSensor,
  PointerSensor,
  pointerWithin,
  useSensor,
  useSensors,
  type DragEndEvent,
  type DragOverEvent,
  type DragStartEvent,
} from '@dnd-kit/core';
import { sortableKeyboardCoordinates } from '@dnd-kit/sortable';
import { Calendar } from 'lucide-react';
import Link from 'next/link';
import { useCallback, useEffect, useRef, useState } from 'react';
import { toast } from 'react-toastify';
import BoardInviteModal from './BoardInviteModal';
import ColumnCreateModal from './ColumnCreateModal';
import ColumnEditModal from './ColumnEditModal';
import ItemCreateModal from './ItemCreateModal';
import ItemEditModal from './ItemEditModal';
import KanbanColumn from './KanbanColumn';
import KanbanItem from './KanbanItem';

const API = 'http://localhost:5193';

type User = {
  id: string;
  name: string;
  imageUrl: string;
};

export default function KanbanBoard({ boardId }: { readonly boardId: string }) {
  const { auth } = useAuth();
  const { connection } = useSignalR();
  const lastRef = useRef<string | null>(null);

  const [board, setBoard] = useState<any>(null);
  const [columns, setColumns] = useState<any[]>([]);
  const [items, setItems] = useState<any[]>([]);
  const [members, setMembers] = useState<any[]>([]);
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(true);

  const [activeId, setActiveId] = useState<string | null>(null);
  const [activeItem, setActiveItem] = useState<any>(null);

  const [showCreateColumn, setShowCreateColumn] = useState(false);
  const [createItemColumnId, setCreateItemColumnId] = useState<string | null>(null);
  const [editColumnId, setEditColumnId] = useState<string | null>(null);
  const [editColumnTitle, setEditColumnTitle] = useState<string>('');
  const [editItem, setEditItem] = useState<any>(null);

  const [showInvite, setShowInvite] = useState(false);

  const getUserIdFromToken = (token: string | null): string | null => {
    if (!token) return null;
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];
    } catch {
      return null;
    }
  };

  const userId = getUserIdFromToken(auth.token);
  const userMember = members.find((m) => m.userId === userId);
  const userRole = userMember?.role;
  const canEdit = userRole === 'admin' || userRole === 'editor';

  const headers: HeadersInit = {};
  if (auth.token) headers['Authorization'] = `Bearer ${auth.token}`;

  const sensors = useSensors(
    useSensor(PointerSensor, { activationConstraint: { distance: 0 } }),
    useSensor(KeyboardSensor, { coordinateGetter: sortableKeyboardCoordinates }),
  );

  const fetchData = useCallback(async () => {
    setLoading(true);
    try {
      const [bR, cR, iR, mR, uR] = await Promise.all([
        fetch(`${API}/api/Boards/${boardId}`, { headers }),
        fetch(`${API}/api/Columns`, { headers }),
        fetch(`${API}/api/Items`, { headers }),
        fetch(`${API}/api/BoardMembers/board/${boardId}`, { headers }),
        fetch(`${API}/api/Users`, { headers }),
      ]);
      if (!bR.ok) throw new Error('Error al cargar el tablero');
      const [bd, allCols, allItems, mem, us] = await Promise.all([
        bR.json(),
        cR.json(),
        iR.json(),
        mR.json(),
        uR.json(),
      ]);
      const cols = allCols.filter((c: any) => c.boardId === boardId);
      const its = allItems.filter((i: any) => cols.some((c: any) => c.id === i.columnId));
      setBoard(bd);
      setColumns(cols);
      setItems(its);
      setMembers(mem);
      setUsers(us);
    } catch (err) {
      console.error('[fetchData] Error:', err);
      toast.error('Error cargando datos');
    } finally {
      setLoading(false);
    }
  }, [boardId, auth.token]);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  useEffect(() => {
    if (!connection) return;
    const handler = (id: string, action: string, payload: any) => {
      if (id !== boardId) return;
      const key = `${action}:${payload.id ?? payload.columnId}`;
      if (lastRef.current === key) return;
      lastRef.current = key;
      const currentUserId = userId;

      switch (action) {
        case 'ColumnCreated':
          toast.info('📌 Nueva columna añadida');
          setColumns((c) => [...c, payload]);
          break;

        case 'ColumnUpdated':
          toast.info('✏️ Columna actualizada');
          setColumns((c) =>
            c.map((col) =>
              col.id === payload.id ? { ...col, title: payload.title, order: payload.order } : col,
            ),
          );
          break;

        case 'ColumnDeleted':
          toast.info('🗑️ Columna eliminada');
          setColumns((c) => c.filter((col) => col.id !== payload.columnId));
          setItems((i) => i.filter((it) => it.columnId !== payload.columnId));
          break;

        case 'ItemCreated':
          setItems((i) => [...i, payload]);
          if (payload.assigneeId === currentUserId) {
            toast.info(`✅ Te han asignado la tarea "${payload.title}"`, {
              toastId: `item-assigned-${payload.id}`,
            });
          }
          break;

        case 'ItemUpdated':
          setItems((i) => i.map((it) => (it.id === payload.id ? { ...it, ...payload } : it)));
          if (payload.assigneeId === currentUserId) {
            toast.info(`🔁 La tarea "${payload.title}" ha sido modificada`, {
              toastId: `item-updated-${payload.id}`,
            });
          }
          break;

        case 'ItemDeleted':
          setItems((i) => i.filter((it) => it.id !== payload.id));
          if (payload.assigneeId === currentUserId) {
            toast.info(`🗑️ Tu tarea "${payload.title}" ha sido eliminada`, {
              toastId: `item-deleted-${payload.id}`,
            });
          }
          break;

        case 'ItemStatusChanged':
          setItems((i) =>
            i.map((it) => (it.id === payload.id ? { ...it, status: payload.status } : it)),
          );
          if (payload.assigneeId === currentUserId) {
            toast.info(`📍 Estado actualizado de "${payload.title}"`, {
              toastId: `item-status-${payload.id}`,
            });
          }
          break;

        case 'ItemFileUploaded':
          if (payload.assigneeId === currentUserId) {
            toast.info(`📎 Se ha subido un archivo a "${payload.title}"`, {
              toastId: `item-file-${payload.id}`,
            });
          }
          break;

        default:
          fetchData();
      }
    };

    connection.on('BoardUpdated', handler);
    return () => void connection.off('BoardUpdated', handler);
  }, [connection, boardId, fetchData, userId]);

  useEffect(() => {
    if (!connection) return;
    connection.invoke('JoinBoardGroup', boardId).catch(console.error);
    return () => connection.invoke('LeaveBoardGroup', boardId).catch(console.error);
  }, [connection, boardId]);

  const findActiveItem = (id: string) => items.find((it) => it.id === id);

  const handleDragStart = ({ active }: DragStartEvent) => {
    setActiveId(active.id as string);
    const it = findActiveItem(active.id as string);
    if (it) setActiveItem(it);
  };

  const handleDragOver = (_: DragOverEvent) => {};

  const handleDragEnd = async ({ active, over }: DragEndEvent) => {
    if (!over) return resetDrag();
    const aId = active.id as string;
    const oId = over.id as string;
    if (aId === oId) return resetDrag();
    const it = findActiveItem(aId);
    if (!it) return resetDrag();
    const isCol = columns.some((c) => c.id === oId);
    if (isCol && it.columnId !== oId) {
      try {
        const res = await fetch(`${API}/api/Items/${aId}`, {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${auth.token}`,
          },
          body: JSON.stringify({ ...it, columnId: oId }),
        });
        if (!res.ok) throw new Error();
        setItems((prev) => prev.map((i) => (i.id === aId ? { ...i, columnId: oId } : i)));
        toast.success('Tarea movida');
      } catch {
        toast.error('Error moviendo');
        fetchData();
      }
    }
    resetDrag();
  };

  const handleDeleteColumn = async (colId: string) => {
    const tareas = items.filter((i) => i.columnId === colId);
    const confirmDelete = tareas.length
      ? confirm(`La columna tiene ${tareas.length} tareas. ¿Eliminar también?`)
      : confirm('¿Eliminar columna?');
    if (!confirmDelete) return;
    try {
      const url = new URL(`${API}/api/Columns/${colId}`);
      if (tareas.length) url.searchParams.set('cascadeItems', 'true');
      const res = await fetch(url.toString(), {
        method: 'DELETE',
        headers: { Authorization: `Bearer ${auth.token}` },
      });
      if (!res.ok) throw new Error(await res.text());
      setColumns((p) => p.filter((c) => c.id !== colId));
      setItems((p) => p.filter((i) => i.columnId !== colId));
      toast.success('Columna eliminada');
    } catch (e: any) {
      toast.error(e.message);
      fetchData();
    }
  };

  const resetDrag = () => {
    setActiveId(null);
    setActiveItem(null);
  };

  if (loading)
    return (
      <div className="flex justify-center p-8">
        <div className="spinner" />
      </div>
    );

  if (!board) return <div className="p-8 text-red-600">Tablero no encontrado</div>;

  return (
    <div className="mx-auto p-4">
      <div className="mb-6 flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold">{board.name}</h1>
          <p className="text-muted-foreground text-sm">
            {board.isPublic ? 'Público' : 'Privado'} • Miembros: {members.length}
          </p>
        </div>
        <div className="flex gap-2">
          {userRole === 'admin' && <Button onClick={() => setShowInvite(true)}>Invitar</Button>}
          {canEdit && <Button onClick={() => setShowCreateColumn(true)}>Añadir columna</Button>}
          <Link href={`/tableros/${boardId}/gantt`}>
            <Button variant="outline">
              <Calendar className="mr-2 h-4 w-4" /> Vista Gantt
            </Button>
          </Link>
        </div>
      </div>

      <DndContext
        sensors={sensors}
        collisionDetection={pointerWithin}
        onDragStart={handleDragStart}
        onDragOver={handleDragOver}
        onDragEnd={handleDragEnd}
      >
        <div className="flex gap-6 overflow-x-auto pb-4">
          {columns.map((col) => (
            <KanbanColumn
              key={col.id}
              column={col}
              items={items.filter((i) => i.columnId === col.id)}
              users={users}
              canEdit={canEdit}
              onAddItem={() => setCreateItemColumnId(col.id)}
              onEditColumn={() => {
                setEditColumnId(col.id);
                setEditColumnTitle(col.title);
              }}
              onEditItem={(it) => {
                if (userRole === 'admin' || it.assigneeId === userId) {
                  setEditItem(it);
                }
              }}
              onDeleteColumn={() => handleDeleteColumn(col.id)}
            />
          ))}
        </div>
        <DragOverlay>
          {activeItem && <KanbanItem item={activeItem} users={users} isOverlay />}
        </DragOverlay>
      </DndContext>

      {showInvite && (
        <BoardInviteModal
          boardId={boardId}
          onClose={() => setShowInvite(false)}
          onInvited={() => {
            setShowInvite(false);
            fetchData();
          }}
        />
      )}
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
          assignees={
            members
              .filter((m) => m.role === 'editor')
              .map((m) => users.find((u) => u.id === m.userId))
              .filter(Boolean) as User[]
          }
          userRole={userRole}
        />
      )}
      {editColumnId && (
        <ColumnEditModal
          columnId={editColumnId}
          currentTitle={editColumnTitle}
          boardId={boardId}
          token={auth.token!}
          onClose={() => setEditColumnId(null)}
          onUpdated={fetchData}
        />
      )}
      {editItem && (
        <ItemEditModal
          item={editItem}
          onClose={() => setEditItem(null)}
          onUpdated={fetchData}
          assignees={
            members
              .filter((m) => m.role === 'editor')
              .map((m) => users.find((u) => u.id === m.userId))
              .filter(Boolean) as User[]
          }
          userRole={userRole}
          currentUserId={userId!}
        />
      )}
    </div>
  );
}
