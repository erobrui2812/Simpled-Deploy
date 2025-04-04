"use client";
import { useBoards } from "@/contexts/BoardsContext";
import { useState } from "react";
import { toast } from "react-toastify";

type Props = {
  item: {
    id: string;
    title: string;
    description?: string;
    dueDate?: string;
    columnId: string;
  };
  onClose: () => void;
  onUpdated: () => void;
};

export default function ItemEditModal({ item, onClose, onUpdated }: Props) {
  const [title, setTitle] = useState(item.title);
  const [description, setDescription] = useState(item.description || "");
  const [dueDate, setDueDate] = useState(item.dueDate?.slice(0, 10) || "");
  const { updateBoard } = useBoards(); // reutilizamos contexto por si se refresca al cerrar

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const response = await fetch(
        `https://localhost:7177/api/Items/${item.id}`,
        {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${
              localStorage.getItem("token") || sessionStorage.getItem("token")
            }`,
          },
          body: JSON.stringify({
            id: item.id,
            title,
            description,
            dueDate: dueDate ? new Date(dueDate).toISOString() : null,
            columnId: item.columnId,
          }),
        }
      );

      if (!response.ok) throw new Error("Error al actualizar la tarea.");
      toast.success("Tarea actualizada.");
      onUpdated();
      onClose();
    } catch (err) {
      console.error(err);
      toast.error("No se pudo actualizar la tarea.");
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-40 flex items-center justify-center z-50">
      <div className="bg-white dark:bg-neutral-900 p-6 rounded w-full max-w-md shadow-md">
        <h2 className="text-lg font-semibold mb-4">Editar Tarea</h2>
        <form onSubmit={handleSubmit} className="flex flex-col gap-3">
          <label
            htmlFor="title"
            className="block text-sm font-medium text-gray-700 dark:text-gray-300"
          >
            Título
          </label>
          <input
            id="title"
            type="text"
            className="border rounded px-3 py-2"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            placeholder="Ingrese el título"
            required
          />
          <textarea
            className="border rounded px-3 py-2"
            rows={3}
            placeholder="Descripción (opcional)"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
          />
          <input
            type="date"
            className="border rounded px-3 py-2"
            value={dueDate}
            onChange={(e) => setDueDate(e.target.value)}
            title="Fecha de vencimiento"
            placeholder="Seleccione una fecha"
          />
          <div className="flex justify-end gap-2 mt-4">
            <button
              type="button"
              onClick={onClose}
              className="px-4 py-2 rounded bg-gray-300 dark:bg-gray-700"
            >
              Cancelar
            </button>
            <button
              type="submit"
              className="px-4 py-2 rounded bg-blue-600 text-white hover:bg-blue-700"
            >
              Guardar
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
