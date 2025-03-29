"use client";
import BoardList from "@/components/BoardList";
import { useBoards } from "@/contexts/BoardsContext";
import { useState } from "react";

export default function BoardsPage() {
  const { createBoard } = useBoards();
  const [name, setName] = useState("");

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!name.trim()) return;
    await createBoard(name);
    setName("");
  };

  return (
    <main className="max-w-5xl mx-auto py-8 px-4">
      <h1 className="text-3xl font-bold mb-6">Tus Tableros</h1>

      <form onSubmit={handleCreate} className="mb-8 flex gap-4 flex-wrap">
        <input
          type="text"
          placeholder="Nombre del nuevo tablero"
          value={name}
          onChange={(e) => setName(e.target.value)}
          className="flex-1 border rounded px-4 py-2"
        />
        <button
          type="submit"
          className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
        >
          Crear
        </button>
      </form>

      <BoardList />
    </main>
  );
}
