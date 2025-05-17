'use client';
import { useBoards } from '@/contexts/BoardsContext';
import { Layers } from 'lucide-react';
import { useEffect, useState } from 'react';
import BoardCard from './BoardCard';

type SortDirection = 'asc' | 'desc';
type SortField = 'name' | 'createdAt';

export default function BoardList() {
  const { boards, loading } = useBoards();
  const [filteredBoards, setFilteredBoards] = useState(boards);
  const [searchTerm, setSearchTerm] = useState('');
  const [sortDirection, setSortDirection] = useState<SortDirection>('asc');
  const [sortField, setSortField] = useState<SortField>('name');

  useEffect(() => {
    let result = [...boards];

    if (searchTerm) {
      result = result.filter((board) =>
        board.name.toLowerCase().includes(searchTerm.toLowerCase()),
      );
    }

    result.sort((a, b) => {
      if (sortField === 'name') {
        return sortDirection === 'asc'
          ? a.name.localeCompare(b.name)
          : b.name.localeCompare(a.name);
      }
      return 0;
    });

    setFilteredBoards(result);
  }, [boards, searchTerm, sortDirection, sortField]);

  const toggleSort = () => {
    setSortDirection((prev) => (prev === 'asc' ? 'desc' : 'asc'));
  };

  return (
    <div className="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
      {loading ? (
        <div className="col-span-full flex justify-center py-8">
          <div className="h-12 w-12 animate-spin rounded-full border-b-2 border-blue-600"></div>
        </div>
      ) : boards.length === 0 ? (
        <div className="col-span-full flex flex-col items-center justify-center py-12 text-center">
          <Layers className="mb-4 h-16 w-16 text-gray-400" />
          <h2 className="mb-2 text-xl font-semibold">No tienes tableros</h2>
          <p className="text-muted-foreground mb-6 max-w-md">
            Crea tu primer tablero para comenzar a organizar tus proyectos y tareas.
          </p>
        </div>
      ) : (
        boards.map((board) => <BoardCard key={board.id} board={board} />)
      )}
    </div>
  );
}
