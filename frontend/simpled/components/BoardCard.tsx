import { Board } from "@/contexts/BoardsContext";
import Link from "next/link";

export default function BoardCard({ board }: { board: Board }) {
  return (
    <Link href={`/boards/${board.id}`}>
      <div className="p-4 border rounded hover:shadow-md transition bg-white dark:bg-neutral-800">
        <h2 className="text-lg font-semibold">{board.name}</h2>
        <p className="text-sm text-gray-500 dark:text-gray-400">
          {board.isPublic ? "Público" : "Privado"}
        </p>
      </div>
    </Link>
  );
}
