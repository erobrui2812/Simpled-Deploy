"use client";
import { useState } from "react";

export default function ColumnCreateModal({
  boardId,
  onClose,
  onCreated,
}: {
  boardId: string;
  onClose: () => void;
  onCreated: () => void;
}) {
  const [title, setTitle] = useState("");
  const [order, setOrder] = useState(0);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const res = await fetch("https://localhost:7177/api/Columns", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ boardId, title, order }),
    });

    if (res.ok) {
      onCreated();
      onClose();
    } else {
      alert("Error al crear la columna");
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-40 flex items-center justify-center z-50">
      <div className="bg-white dark:bg-neutral-900 p-6 rounded max-w-sm w-full shadow">
        <h2 className="text-lg font-semibold mb-4">Crear nueva columna</h2>
        <form onSubmit={handleSubmit} className="flex flex-col gap-4">
          <input
            type="text"
            placeholder="Título"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            required
            className="border rounded px-3 py-2"
          />
          <input
            type="number"
            value={order}
            onChange={(e) => setOrder(parseInt(e.target.value))}
            className="border rounded px-3 py-2"
            placeholder="Orden (opcional)"
          />
          <div className="flex justify-end gap-2">
            <button
              onClick={onClose}
              type="button"
              className="text-sm text-gray-600"
            >
              Cancelar
            </button>
            <button
              type="submit"
              className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
            >
              Crear
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
