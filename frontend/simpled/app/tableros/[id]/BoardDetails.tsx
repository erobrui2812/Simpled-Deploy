"use client";

import ColumnCreateModal from "@/components/ColumnCreateModal";
import ColumnEditModal from "@/components/ColumnEditModal";
import ItemCreateModal from "@/components/ItemCreateModal";
import ItemEditModal from "@/components/ItemEditModal";
import { useAuth } from "@/contexts/AuthContext";
import { useEffect, useState } from "react";

const API = "https://localhost:7177";

export default function BoardDetails({ boardId }: { boardId: string }) {
  const { auth } = useAuth();

  const [board, setBoard] = useState<any>(null);
  const [columns, setColumns] = useState<any[]>([]);
  const [items, setItems] = useState<any[]>([]);
  const [members, setMembers] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [showCreateColumn, setShowCreateColumn] = useState(false);
  const [createItemColumnId, setCreateItemColumnId] = useState<string | null>(
    null
  );
  const [editColumnId, setEditColumnId] = useState<string | null>(null);
  const [editColumnTitle, setEditColumnTitle] = useState<string>("");
  const [editItem, setEditItem] = useState<any>(null);

  const userId = getUserIdFromToken(auth.token);
  const userMember = Array.isArray(members)
    ? members.find((m) => m.userId === userId)
    : null;
  const userRole = userMember?.role;
  const canEdit = userRole === "admin" || userRole === "editor";

  const headers: HeadersInit = {};
  if (auth.token) headers["Authorization"] = `Bearer ${auth.token}`;

  const fetchData = async () => {
    try {
      const [boardRes, columnRes, itemRes, membersRes] = await Promise.all([
        fetch(`${API}/api/Boards/${boardId}`, { headers }),
        fetch(`${API}/api/Columns`, { headers }),
        fetch(`${API}/api/Items`, { headers }),
        fetch(`${API}/api/BoardMembers/board/${boardId}`, { headers }),
      ]);

      if (!boardRes.ok) throw new Error("Error al cargar el tablero.");

      const boardData = await boardRes.json();
      const columnData = (await columnRes.json()).filter(
        (c: any) => c.boardId === boardId
      );
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
      console.error("Error al cargar el tablero:", err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, [boardId, auth.token]);

  if (loading) return <div className="p-8">Cargando tablero...</div>;
  if (!board)
    return <div className="p-8 text-red-600">Tablero no encontrado</div>;

  return (
    <div className="p-8 max-w-6xl mx-auto">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-2xl font-bold">{board.name}</h1>
        <span className="text-sm px-2 py-1 rounded bg-gray-200 dark:bg-gray-700">
          {board.isPublic ? "Público" : "Privado"}
        </span>
      </div>

      {canEdit && (
        <button
          onClick={() => setShowCreateColumn(true)}
          className="bg-blue-600 text-white px-3 py-1 rounded text-sm hover:bg-blue-700"
        >
          ➕ Añadir columna
        </button>
      )}

      <p className="text-sm text-gray-500 dark:text-gray-400 mb-6">
        Miembros: {members.length}
      </p>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        {columns.map((col) => (
          <div
            key={col.id}
            className="bg-white dark:bg-neutral-800 p-4 rounded shadow"
          >
            <div className="flex justify-between items-center mb-2">
              <h2 className="font-semibold text-lg">{col.title}</h2>
              {canEdit && (
                <button
                  onClick={() => {
                    setEditColumnId(col.id);
                    setEditColumnTitle(col.title);
                  }}
                  className="text-sm text-blue-500 hover:underline"
                >
                  ✏️ Editar
                </button>
              )}
            </div>
            <div className="flex flex-col gap-2">
              {items
                .filter((item) => item.columnId === col.id)
                .map((item) => (
                  <button
                    key={item.id}
                    className="border rounded p-2 bg-neutral-50 dark:bg-neutral-900 cursor-pointer text-left"
                    onClick={() => canEdit && setEditItem(item)}
                  >
                    <strong>{item.title}</strong>
                    {item.description && (
                      <p className="text-sm text-gray-500">
                        {item.description}
                      </p>
                    )}
                  </button>
                ))}
            </div>
            {canEdit && (
              <button
                onClick={() => setCreateItemColumnId(col.id)}
                className="text-blue-500 text-sm mt-2 hover:underline"
              >
                ➕ Añadir tarea
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
          onClose={() => setEditColumnId(null)}
          onUpdated={fetchData}
          token={auth.token!}
        />
      )}

      {editItem && (
        <ItemEditModal
          item={editItem}
          onClose={() => setEditItem(null)}
          onUpdated={fetchData}
        />
      )}
    </div>
  );

  function getUserIdFromToken(token: string | null): string | null {
    if (!token) return null;
    try {
      const payload = JSON.parse(atob(token.split(".")[1]));
      return payload[
        "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
      ];
    } catch {
      return null;
    }
  }
}
