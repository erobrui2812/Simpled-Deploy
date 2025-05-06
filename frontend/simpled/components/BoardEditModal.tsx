'use client';
import { Board, useBoards } from '@/contexts/BoardsContext';
import { useState } from 'react';

export default function BoardEditModal({
  board,
  onClose,
}: Readonly<{ board: Board; onClose: () => void }>) {
  const [name, setName] = useState(board.name);
  const [isPublic, setIsPublic] = useState(board.isPublic);
  const { updateBoard } = useBoards();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    await updateBoard(board.id, { name, isPublic });
    onClose();
  };

  return (
    <div className="bg-opacity-50 fixed inset-0 z-50 flex items-center justify-center bg-black">
      <div className="w-full max-w-sm rounded bg-white p-6 shadow-md dark:bg-neutral-900">
        <h2 className="mb-4 text-lg font-semibold">Editar tablero</h2>
        <form onSubmit={handleSubmit} className="flex flex-col gap-4">
          <input
            type="text"
            value={name}
            onChange={(e) => setName(e.target.value)}
            className="rounded border px-3 py-2"
            placeholder="Nombre del tablero"
            required
          />
          <label className="flex items-center gap-2 text-sm">
            <input
              type="checkbox"
              checked={isPublic}
              onChange={(e) => setIsPublic(e.target.checked)}
            />
            <span>Hacer público</span>
          </label>

          <div className="mt-4 flex justify-end gap-2">
            <button type="button" onClick={onClose} className="text-gray-600 hover:underline">
              Cancelar
            </button>
            <button
              type="submit"
              className="rounded bg-blue-600 px-4 py-2 text-white hover:bg-blue-700"
            >
              Guardar
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
