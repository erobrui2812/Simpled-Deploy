"use client";

import { useAuth } from "@/contexts/AuthContext";
import { useState } from "react";
import { toast } from "react-toastify";

const API = "http://localhost:5193";

type Props = {
  readonly columnId: string;
  readonly onCreated: () => void;
  readonly onClose: () => void;
};

export default function ItemCreateModal({
  columnId,
  onClose,
  onCreated,
}: Props) {
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [dueDate, setDueDate] = useState<string>("");
  const [loading, setLoading] = useState(false);
  const { auth } = useAuth();

  const handleCreate = async () => {
    if (!title.trim()) {
      toast.warning("El título es obligatorio.");
      return;
    }

    setLoading(true);

    try {
      const response = await fetch(`${API}/api/Items`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${auth.token}`,
        },
        body: JSON.stringify({
          title: title.trim(),
          description: description?.trim() || null,
          dueDate: dueDate ? new Date(dueDate).toISOString() : null,
          columnId: columnId,
        }),
      });

      if (!response.ok) throw new Error("No se pudo crear la tarea.");

      toast.success("Tarea creada correctamente.");
      onCreated();
      onClose();
    } catch (err) {
      console.error(err);
      toast.error("Error al crear tarea.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 z-50 flex items-center justify-center">
      <div className="bg-white dark:bg-neutral-900 p-6 rounded shadow max-w-sm w-full">
        <h2 className="text-xl font-semibold mb-4">Nueva tarea</h2>

        <input
          type="text"
          placeholder="Título"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
          className="w-full border rounded px-3 py-2 mb-3"
        />

        <textarea
          placeholder="Descripción (opcional)"
          value={description}
          onChange={(e) => setDescription(e.target.value)}
          className="w-full border rounded px-3 py-2 mb-3"
          rows={3}
        />

        <label htmlFor="dueDate" className="block text-sm font-medium mb-1">
          Fecha de vencimiento
        </label>
        <input
          id="dueDate"
          type="date"
          value={dueDate}
          onChange={(e) => setDueDate(e.target.value)}
          className="w-full border rounded px-3 py-2 mb-4"
          title="Selecciona una fecha de vencimiento"
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
