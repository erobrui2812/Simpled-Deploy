"use client";

import {
  Card,
  CardContent,
  CardDescription,
  CardTitle,
} from "@/components/ui/card";
import { cn } from "@/lib/utils";
import { useSortable } from "@dnd-kit/sortable";
import { CSS } from "@dnd-kit/utilities";
import { format } from "date-fns";
import { es } from "date-fns/locale";
import { CalendarIcon } from "lucide-react";

interface KanbanItemProps {
  item: {
    id: string;
    title: string;
    description?: string;
    dueDate?: string;
  };
  onClick?: () => void;
  isOverlay?: boolean;
}

export default function KanbanItem({
  item,
  onClick,
  isOverlay = false,
}: KanbanItemProps) {
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
    isDragging,
  } = useSortable({
    id: item.id,
    data: {
      type: "item",
      item,
    },
  });

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
  };

  const formattedDate = item.dueDate
    ? format(new Date(item.dueDate), "d MMM yyyy", { locale: es })
    : null;

  return (
    <Card
      ref={setNodeRef}
      style={style}
      {...attributes}
      {...listeners}
      onClick={onClick}
      className={cn(
        "cursor-pointer border shadow-sm hover:shadow-md transition-shadow",
        isDragging && "opacity-50",
        isOverlay && "shadow-lg rotate-3"
      )}
    >
      <CardContent className="p-3">
        <CardTitle className="text-base font-medium mb-1">
          {item.title}
        </CardTitle>

        {item.description && (
          <CardDescription className="text-sm line-clamp-2 mb-2">
            {item.description}
          </CardDescription>
        )}

        {formattedDate && (
          <div className="flex items-center text-xs text-muted-foreground">
            <CalendarIcon className="h-3 w-3 mr-1" />
            {formattedDate}
          </div>
        )}
      </CardContent>
    </Card>
  );
}
