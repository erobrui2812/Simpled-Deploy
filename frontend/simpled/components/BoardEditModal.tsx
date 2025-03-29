"use client";
import { Board, useBoards } from "@/contexts/BoardsContext";
import { useState } from "react";

export default function BoardEditModal({
  board,
  onClose,
}: {
  board: Board;
  onClose: () => void;
}) {
  const [name, setName] = useState(board.name);
  const [isPublic, setIsPublic] = useState(board.isPublic);
  const { updateBoard } = useBoards();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    await updateBoard(board.id, { name, isPublic });
    onClose();
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white dark:bg-neutral-900 p-6 rounded shadow-md max-w-sm w-full">
        <h2 className="text-lg font-semibold mb-4">Editar tablero</h2>
        <form onSubmit={handleSubmit} className="flex flex-col gap-4">
          <input
            type="text"
            value={name}
            onChange={(e) => setName(e.target.value)}
            className="border rounded px-3 py-2"
            required
          />

          <label className="flex items-center gap-2 text-sm">
            <input
              type="checkbox"
              checked={isPublic}
              onChange={(e) => setIsPublic(e.target.checked)}
            />
            Hacer público
          </label>

          <div className="flex justify-end gap-2 mt-4">
            <button
              type="button"
              onClick={onClose}
              className="text-gray-600 hover:underline"
            >
              Cancelar
            </button>
            <button
              type="submit"
              className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
            >
              Guardar
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
