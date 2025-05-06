'use client';

import { useAuth } from '@/contexts/AuthContext';
import { type Board, useBoards } from '@/contexts/BoardsContext';
import { cn } from '@/lib/utils';
import { NotebookPen, Star, StarOff, Trash2Icon } from 'lucide-react';
import Link from 'next/link';
import { useState } from 'react';
import { toast } from 'react-toastify';
import BoardEditModal from './BoardEditModal';

export default function BoardCard({ board }: { readonly board: Board }) {
  const { auth, toggleFavoriteBoard } = useAuth();
  const { deleteBoard } = useBoards();
  const [showEdit, setShowEdit] = useState(false);
  const [isFavorite, setIsFavorite] = useState(board.isFavorite);
  const [isDeleting, setIsDeleting] = useState(false);
  const [isFavoriteToggling, setIsFavoriteToggling] = useState(false);

  const userId = getUserIdFromToken(auth.token);
  const isOwner = userId === board.ownerId;

  const handleDelete = async () => {
    if (confirm('¿Seguro que deseas eliminar este tablero?')) {
      setIsDeleting(true);
      try {
        deleteBoard(board.id);
        toast.success('Tablero eliminado con éxito');
      } catch (error) {
        console.error('Error al eliminar el tablero:', error);
        toast.error('Error al eliminar el tablero');
      } finally {
        setIsDeleting(false);
      }
    }
  };

  const toggleFavorite = async () => {
    setIsFavoriteToggling(true);
    try {
      toggleFavoriteBoard(board.id);
      setIsFavorite(!isFavorite);
    } catch (error) {
      console.error('Error al actualizar favorito:', error);
      toast.error('No se pudo actualizar el estado de favorito');
    } finally {
      setIsFavoriteToggling(false);
    }
  };

  return (
    <>
      <div
        className={cn(
          'group bg-card flex rounded border p-4 transition-all duration-300 hover:shadow-md',
          isDeleting && 'opacity-50',
        )}
      >
        <Link href={`/tableros/${board.id}`} className="flex-1">
          <div className="mr-4 flex flex-col justify-center">
            <div>
              <h2 className="group-hover:text-primary mb-1 text-lg font-bold transition-colors">
                {board.name}
              </h2>
            </div>
            <p className="text-muted-foreground text-sm">
              {board.isPublic ? 'Público' : 'Privado'}
            </p>
          </div>
        </Link>

        <div className="ml-auto flex flex-col items-center justify-between gap-2">
          <button
            onClick={toggleFavorite}
            disabled={isFavoriteToggling}
            title={isFavorite ? 'Quitar de favoritos' : 'Marcar como favorito'}
            className="text-yellow-500 transition-all duration-300 hover:scale-110 disabled:opacity-50"
          >
            {isFavorite ? (
              <Star className="size-5 fill-yellow-500" />
            ) : (
              <StarOff className="size-5" />
            )}
          </button>

          {isOwner && (
            <div className="flex flex-col gap-2">
              <button
                onClick={() => setShowEdit(true)}
                title="Editar"
                className="transition-all duration-300"
              >
                <NotebookPen className="size-5 text-blue-500 transition-transform hover:scale-110 hover:text-blue-700" />
              </button>
              <button
                onClick={handleDelete}
                disabled={isDeleting}
                title="Eliminar"
                className="transition-all duration-300"
              >
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

// Decodificación del token para obtener el ID del usuario
function getUserIdFromToken(token: string | null): string | null {
  if (!token) return null;
  try {
    const payload = JSON.parse(atob(token.split('.')[1]));
    return payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];
  } catch (error) {
    console.error('Error al decodificar el token:', error);
    return null;
  }
}
