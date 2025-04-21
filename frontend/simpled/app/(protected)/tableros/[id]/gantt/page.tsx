"use client";

import { GanttChart } from "@/components/gantt-chart";
import { Button } from "@/components/ui/button";
import { LayoutGrid } from "lucide-react";
import Link from "next/link";
import { useParams } from "next/navigation";

export default function GanttPage() {
  const params = useParams();
  const boardId = Array.isArray(params?.id) ? params.id[0] : params?.id;

  if (!boardId) return <p>Error: ID no válido</p>;

  return (
    <div className="space-y-4">
      <div className="flex justify-between items-center">
        <h1 className="text-2xl font-bold">Vista de Gantt</h1>
        <Link href={`/tableros/${boardId}`}>
          <Button variant="outline" size="sm">
            <LayoutGrid className="mr-2 h-4 w-4" />
            Vista Kanban
          </Button>
        </Link>
      </div>

      <GanttChart boardId={boardId} />
    </div>
  );
}
