// KanbanBoard.tsx
'use client';

import { Button } from '@/components/ui/button';
import { useAuth } from '@/contexts/AuthContext';
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
import { useEffect, useState } from 'react';
import { toast } from 'react-toastify';
import ColumnCreateModal from './ColumnCreateModal';
import ColumnEditModal from './ColumnEditModal';
import ItemCreateModal from './ItemCreateModal';
import ItemEditModal from './ItemEditModal';
import KanbanColumn from './KanbanColumn';
import KanbanItem from './KanbanItem';

const API = 'http://localhost:5193';

export default function KanbanBoard({ boardId }: { readonly boardId: string }) {
  const { auth } = useAuth();
  const [board, setBoard] = useState<any>(null);
  const [columns, setColumns] = useState<any[]>([]);
  const [items, setItems] = useState<any[]>([]);
  const [members, setMembers] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [activeId, setActiveId] = useState<string | null>(null);
  const [activeItem, setActiveItem] = useState<any>(null);

  const [showCreateColumn, setShowCreateColumn] = useState(false);
  const [createItemColumnId, setCreateItemColumnId] = useState<string | null>(null);
  const [editColumnId, setEditColumnId] = useState<string | null>(null);
  const [editColumnTitle, setEditColumnTitle] = useState<string>('');
  const [editItem, setEditItem] = useState<any>(null);

  const userId = getUserIdFromToken(auth.token);
  const userMember = Array.isArray(members) ? members.find((m) => m.userId === userId) : null;
  const userRole = userMember?.role;
  const canEdit = userRole === 'admin' || userRole === 'editor';

  const headers: HeadersInit = {};
  if (auth.token) headers['Authorization'] = `Bearer ${auth.token}`;

  const sensors = useSensors(
    useSensor(PointerSensor, { activationConstraint: { distance: 0 } }),
    useSensor(KeyboardSensor, { coordinateGetter: sortableKeyboardCoordinates }),
  );

  const fetchData = async () => {
    try {
      setLoading(true);
      const [boardRes, columnRes, itemRes, membersRes] = await Promise.all([
        fetch(`${API}/api/Boards/${boardId}`, { headers }),
        fetch(`${API}/api/Columns`, { headers }),
        fetch(`${API}/api/Items`, { headers }),
        fetch(`${API}/api/BoardMembers/board/${boardId}`, { headers }),
      ]);

      if (!boardRes.ok) throw new Error('Error al cargar el tablero.');

      const boardData = await boardRes.json();
      const allColumns = await columnRes.json();
      const columnData = allColumns.filter((c: any) => c.boardId === boardId);
      const allItems = await itemRes.json();
      const itemData = allItems.filter((i: any) =>
        columnData.some((col: any) => col.id === i.columnId),
      );
      const membersRaw = await membersRes.text();
      const membersData = membersRaw ? JSON.parse(membersRaw) : [];

      setBoard(boardData);
      setColumns(columnData);
      setItems(itemData);
      setMembers(membersData);
    } catch (err) {
      console.error('Error al cargar el tablero:', err);
      toast.error('Error al cargar el tablero');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, [boardId, auth.token]);

  const findActiveItem = (id: string) => items.find((item) => item.id === id);

  const handleDragStart = (event: DragStartEvent) => {
    const { active } = event;
    setActiveId(active.id as string);
    const item = findActiveItem(active.id as string);
    if (item) setActiveItem(item);
  };

  // Ya no mutamos estado aquí
  const handleDragOver = (_event: DragOverEvent) => {};

  const handleDragEnd = async (event: DragEndEvent) => {
    const { active, over } = event;
    if (!over) return resetDrag();

    const activeId = active.id as string;
    const overId = over.id as string;
    if (activeId === overId) return resetDrag();

    const item = findActiveItem(activeId);
    if (!item) return resetDrag();

    const isColumn = columns.some((col) => col.id === overId);
    if (isColumn && item.columnId !== overId) {
      try {
        const res = await fetch(`${API}/api/Items/${activeId}`, {
          method: 'PUT',
          headers: { 'Content-Type': 'application/json', Authorization: `Bearer ${auth.token}` },
          body: JSON.stringify({ ...item, columnId: overId }),
        });
        if (!res.ok) throw new Error();

        setItems((prev) => prev.map((i) => (i.id === activeId ? { ...i, columnId: overId } : i)));
        toast.success('Tarea movida correctamente');
      } catch {
        toast.error('Error al mover la tarea');
        fetchData();
      }
    }

    resetDrag();
  };

  const handleDeleteColumn = async (columnId: string) => {
    const tareas = items.filter((i) => i.columnId === columnId);
    if (tareas.length) {
      if (
        !confirm(`La columna tiene ${tareas.length} tarea(s). ¿Eliminarlas junto con la columna?`)
      ) {
        return;
      }
    } else if (!confirm('¿Eliminar esta columna? Esta acción no se puede deshacer.')) {
      return;
    }

    try {
      const res = await fetch(`${API}/api/Columns/${columnId}`, {
        method: 'DELETE',
        headers: { Authorization: `Bearer ${auth.token}` },
      });
      if (!res.ok) throw new Error();

      setColumns((prev) => prev.filter((c) => c.id !== columnId));
      setItems((prev) => prev.filter((i) => i.columnId !== columnId));
      toast.success('Columna eliminada correctamente');
    } catch {
      toast.error('Error al eliminar la columna');
      fetchData();
    }
  };

  const resetDrag = () => {
    setActiveId(null);
    setActiveItem(null);
  };

  if (loading)
    return (
      <div className="flex items-center justify-center p-8">
        <div className="spinner" />
      </div>
    );

  if (!board) return <div className="p-8 text-red-600">Tablero no encontrado</div>;

  return (
    <div className="mx-auto max-w-full p-4">
      <div className="mb-6 flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold">{board.name}</h1>
          <p className="text-muted-foreground text-sm">
            {board.isPublic ? 'Público' : 'Privado'} • Miembros: {members.length}
          </p>
        </div>
        <div className="flex gap-2">
          {canEdit && <Button onClick={() => setShowCreateColumn(true)}>Añadir columna</Button>}
          <Link href={`/tableros/${boardId}/gantt`}>
            <Button variant="outline">
              <Calendar className="mr-2 h-4 w-4" />
              Vista Gantt
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
        <div className="flex min-h-[calc(100vh-220px)] gap-6 overflow-x-auto pb-4">
          {columns.map((column) => (
            <KanbanColumn
              key={column.id}
              column={column}
              items={items.filter((item) => item.columnId === column.id)}
              canEdit={canEdit}
              onAddItem={() => setCreateItemColumnId(column.id)}
              onEditColumn={() => {
                setEditColumnId(column.id);
                setEditColumnTitle(column.title);
              }}
              onEditItem={(item) => setEditItem(item)}
              onDeleteColumn={() => handleDeleteColumn(column.id)}
            />
          ))}
        </div>
        <DragOverlay>{activeItem && <KanbanItem item={activeItem} isOverlay />}</DragOverlay>
      </DndContext>

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
          token={auth.token!}
          onClose={() => setEditColumnId(null)}
          onUpdated={fetchData}
        />
      )}
      {editItem && (
        <ItemEditModal item={editItem} onClose={() => setEditItem(null)} onUpdated={fetchData} />
      )}
    </div>
  );
}

function getUserIdFromToken(token: string | null): string | null {
  if (!token) return null;
  try {
    const payload = JSON.parse(atob(token.split('.')[1]));
    return payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];
  } catch {
    return null;
  }
}
