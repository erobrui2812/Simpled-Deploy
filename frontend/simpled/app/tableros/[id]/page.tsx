"use client";

import dynamic from "next/dynamic";
import { useParams } from "next/navigation";

const BoardDetails = dynamic(() => import("./BoardDetails"), { ssr: false });

export default function BoardPage() {
  const params = useParams();
  const boardId = Array.isArray(params?.id) ? params.id[0] : params?.id;

  if (!boardId) return <p>Error: ID no válido</p>;

  return <BoardDetails boardId={boardId} />;
}
