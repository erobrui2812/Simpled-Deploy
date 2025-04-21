"use client";

import { useAuth } from "@/contexts/AuthContext";
import { useState } from "react";
import { toast } from "react-toastify";

const API = "http://localhost:5193";

type Props = {
  readonly boardId: string;
  readonly onClose: () => void;
  readonly onCreated: () => void;
};

export default function ColumnCreateModal({
  boardId,
  onClose,
  onCreated,
}: Props) {
  const { auth } = useAuth();
  const [title, setTitle] = useState("");
  const [loading, setLoading] = useState(false);

  const handleCreate = async () => {
    if (!title.trim()) {
      toast.warning("El título es obligatorio.");
      return;
    }

    setLoading(true);
    try {
      const res = await fetch(`${API}/api/Columns`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${auth.token}`,
        },
        body: JSON.stringify({
          boardId,
          title,
          order: 0,
        }),
      });

      if (!res.ok) throw new Error("Error al crear la columna.");

      toast.success("Columna creada correctamente.");
      onCreated();
      onClose();
    } catch (err) {
      console.error(err);
      toast.error("No se pudo crear la columna.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 z-50 flex items-center justify-center">
      <div className="bg-white dark:bg-neutral-900 p-6 rounded shadow max-w-sm w-full">
        <h2 className="text-xl font-semibold mb-4">Nueva columna</h2>

        <input
          type="text"
          placeholder="Título"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
          className="w-full border rounded px-3 py-2 mb-4"
        />

        <div className="flex justify-end gap-2">
          <button
            onClick={onClose}
            className="px-4 py-2 bg-gray-300 dark:bg-gray-700 rounded"
          >
            Cancelar
          </button>
          <button
            onClick={handleCreate}
            disabled={loading}
            className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
          >
            {loading ? "Creando..." : "Crear"}
          </button>
        </div>
      </div>
    </div>
  );
}
