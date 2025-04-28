'use client';
import { useAuth } from '@/contexts/AuthContext';
import { Board, useBoards } from '@/contexts/BoardsContext';
import { NotebookPen, Trash2Icon } from 'lucide-react';
import Link from 'next/link';
import { useState } from 'react';
import BoardEditModal from './BoardEditModal';

export default function BoardCard({ board }: { readonly board: Board }) {
  const { auth } = useAuth();
  const { deleteBoard } = useBoards();
  const [showEdit, setShowEdit] = useState(false);

  const userId = getUserIdFromToken(auth.token);
  const isOwner = userId === board.ownerId;

  const handleDelete = async () => {
    if (confirm('¿Seguro que deseas eliminar este tablero?')) {
      await deleteBoard(board.id);
    }
  };

  return (
    <>
      <div className="relative rounded border bg-white p-4 hover:shadow-md dark:bg-neutral-800">
        <Link href={`/tableros/${board.id}`}>
          <h2 className="mb-1 text-lg font-bold">{board.name}</h2>
        </Link>
        <p className="text-sm text-gray-500 dark:text-gray-400">
          {board.isPublic ? 'Público' : 'Privado'}
        </p>

        {isOwner && (
          <div className="absolute top-2 right-2 flex gap-2">
            <button
              onClick={() => setShowEdit(true)}
              title="Editar"
              className="text-sm text-blue-500 hover:cursor-pointer hover:text-blue-700 hover:underline"
            >
              <NotebookPen className="size-6" />
            </button>
            <div className="h-7 w-px bg-gray-300" />
            <button
              onClick={handleDelete}
              title="Eliminar"
              className="text-sm text-red-500 hover:cursor-pointer hover:text-red-700 hover:underline"
            >
              <Trash2Icon className="size-6" />
            </button>
          </div>
        )}
      </div>

      {showEdit && <BoardEditModal board={board} onClose={() => setShowEdit(false)} />}
    </>
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
