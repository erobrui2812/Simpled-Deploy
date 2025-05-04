'use client';
import { useAuth } from '@/contexts/AuthContext';
import { Board, useBoards } from '@/contexts/BoardsContext';
import { NotebookPen, Star, StarOff, Trash2Icon } from 'lucide-react';
import Link from 'next/link';
import { useState } from 'react';
import BoardEditModal from './BoardEditModal';

export default function BoardCard({ board }: { readonly board: Board }) {
  const { auth, toggleFavoriteBoard } = useAuth();
  const { deleteBoard } = useBoards();
  const [showEdit, setShowEdit] = useState(false);
  const [isFavorite, setIsFavorite] = useState(board.isFavorite);

  const userId = getUserIdFromToken(auth.token);
  const isOwner = userId === board.ownerId;

  const handleDelete = async () => {
    if (confirm('¿Seguro que deseas eliminar este tablero?')) {
      await deleteBoard(board.id);
    }
  };

  const toggleFavorite = async () => {
    if (confirm('¿Seguro que deseas añadir este tablero a favorito?')) {
      await toggleFavoriteBoard(board.id);
    }
  };

  return (
    <>
      <div className="flex rounded border bg-white p-4 transition-shadow hover:shadow-md dark:bg-neutral-800">
        <Link href={`/tableros/${board.id}`}>
          <div className="mr-4 flex flex-col justify-center">
            <div>
              <h2 className="mb-1 text-lg font-bold">{board.name}</h2>
            </div>
            <p className="text-sm text-gray-500 dark:text-gray-400">
              {board.isPublic ? 'Público' : 'Privado'}
            </p>
          </div>
        </Link>

        <div className="ml-auto flex flex-col items-center justify-between gap-2">
          <button
            onClick={toggleFavorite}
            title={isFavorite ? 'Quitar de favoritos' : 'Marcar como favorito'}
            className="text-yellow-500 transition-transform hover:scale-110"
          >
            {isFavorite ? (
              <Star className="size-5 fill-yellow-500" />
            ) : (
              <StarOff className="size-5" />
            )}
          </button>

          {isOwner && (
            <div className="flex flex-col gap-2">
              <button onClick={() => setShowEdit(true)} title="Editar">
                <NotebookPen className="size-5 text-blue-500 transition-transform hover:scale-110 hover:text-blue-700" />
              </button>
              <button onClick={handleDelete} title="Eliminar">
                <Trash2Icon className="size-5 text-red-500 transition-transform hover:scale-110 hover:text-red-700" />
              </button>
            </div>
          )}
        </div>
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
