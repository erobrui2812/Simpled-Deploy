"use client";
import { createContext, useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { useAuth } from "./AuthContext";

const API_URL = "https://localhost:7177/";

export type Board = {
  id: string;
  name: string;
  isPublic: boolean;
};

type BoardsContextType = {
  boards: Board[];
  loading: boolean;
  fetchBoards: () => Promise<void>;
  createBoard: (name: string, isPublic?: boolean) => Promise<void>;
};

const BoardsContext = createContext<BoardsContextType | undefined>(undefined);

export const BoardsProvider = ({ children }: { children: React.ReactNode }) => {
  const [boards, setBoards] = useState<Board[]>([]);
  const [loading, setLoading] = useState(true);
  const { auth } = useAuth();

  const fetchBoards = async () => {
    setLoading(true);
    try {
      const res = await fetch(`${API_URL}api/Boards`, {
        headers: {
          Authorization: `Bearer ${auth.token}`,
        },
      });

      if (!res.ok) throw new Error("Error al obtener tableros.");

      const data: Board[] = await res.json();
      setBoards(data);
    } catch (err) {
      console.error(err);
      toast.error("No se pudieron cargar los tableros.");
    } finally {
      setLoading(false);
    }
  };

  const createBoard = async (name: string, isPublic = false) => {
    try {
      const res = await fetch(`${API_URL}api/Boards`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${auth.token}`,
        },
        body: JSON.stringify({
          name,
          isPublic,
        }),
      });

      if (!res.ok) throw new Error("No se pudo crear el tablero.");

      toast.success("Tablero creado correctamente.");
      await fetchBoards();
    } catch (err) {
      console.error(err);
      toast.error("Error al crear el tablero.");
    }
  };

  useEffect(() => {
    if (auth.token) {
      fetchBoards();
    }
  }, [auth.token]);

  return (
    <BoardsContext.Provider
      value={{ boards, loading, fetchBoards, createBoard }}
    >
      {children}
    </BoardsContext.Provider>
  );
};

export const useBoards = (): BoardsContextType => {
  const context = useContext(BoardsContext);
  if (!context) {
    throw new Error("useBoards debe usarse dentro de un BoardsProvider");
  }
  return context;
};
