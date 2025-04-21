"use client";
import { useState } from "react";
import { toast } from "react-toastify";

const API = "http://localhost:5193";

export default function ColumnEditModal({
  columnId,
  currentTitle,
  boardId,
  token,
  onClose,
  onUpdated,
}: {
  columnId: string;
  currentTitle: string;
  boardId: string;
  token: string;
  onClose: () => void;
  onUpdated: () => void;
}) {
  const [title, setTitle] = useState(currentTitle);
  const [loading, setLoading] = useState(false);

  const handleUpdate = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);

    try {
      const res = await fetch(`${API}/api/Columns/${columnId}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({
          id: columnId,
          title,
          boardId, // 👈 requerido por el backend
        }),
      });

      if (!res.ok) throw new Error("Error al actualizar la columna.");

      toast.success("Columna actualizada.");
      onUpdated();
      onClose();
    } catch (err) {
      console.error(err);
      toast.error("Error al actualizar columna.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-40 flex items-center justify-center z-50">
      <div className="bg-white dark:bg-neutral-900 p-6 rounded shadow-lg w-full max-w-md">
        <h2 className="text-lg font-semibold mb-4">Editar columna</h2>
        <form onSubmit={handleUpdate} className="flex flex-col gap-4">
          <input
            type="text"
            placeholder="Nuevo título"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            className="border rounded px-3 py-2"
          />
          <div className="flex justify-end gap-2">
            <button
              type="button"
              onClick={onClose}
              className="text-sm px-4 py-2 border rounded hover:bg-gray-100 dark:hover:bg-neutral-800"
            >
              Cancelar
            </button>
            <button
              type="submit"
              disabled={loading}
              className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 text-sm"
            >
              {loading ? "Guardando..." : "Guardar cambios"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
