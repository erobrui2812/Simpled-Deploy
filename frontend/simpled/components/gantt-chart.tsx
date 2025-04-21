"use client";

import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Checkbox } from "@/components/ui/checkbox";
import { Label } from "@/components/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { useAuth } from "@/contexts/AuthContext";
import { cn } from "@/lib/utils";
import {
  addDays,
  differenceInDays,
  endOfDay,
  format,
  isWithinInterval,
  startOfDay,
} from "date-fns";
import { es } from "date-fns/locale";
import { ChevronLeft, ChevronRight, GripVertical } from "lucide-react";
import type React from "react";
import { useEffect, useMemo, useState } from "react";
import { TaskDialog } from "./task-dialog";

// Types
export interface Task {
  id: string;
  title: string;
  description?: string;
  startDate: string;
  endDate: string;
  progress: number;
  columnId?: string;
  boardId?: string;
  dependencies?: string[];
  assignedTo?: string;
  status?: "pending" | "in-progress" | "completed" | "delayed";
}

interface GanttChartProps {
  boardId: string;
  className?: string;
}

const API_URL = "http://localhost:5193";

export function GanttChart({ boardId, className }: GanttChartProps) {
  const { auth } = useAuth();
  const [tasks, setTasks] = useState<Task[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [viewMode, setViewMode] = useState<"day" | "week" | "month">("week");
  const [startDate, setStartDate] = useState<Date>(() => {
    const today = new Date();
    today.setDate(today.getDate() - 7);
    return today;
  });
  const [selectedTask, setSelectedTask] = useState<Task | null>(null);
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [showCompleted, setShowCompleted] = useState(true);

  // Calculate the number of days to display based on view mode
  const daysToShow = useMemo(() => {
    switch (viewMode) {
      case "day":
        return 14;
      case "week":
        return 28;
      case "month":
        return 60;
      default:
        return 28;
    }
  }, [viewMode]);

  // Generate dates for the timeline
  const timelineDates = useMemo(() => {
    return Array.from({ length: daysToShow }, (_, i) => addDays(startDate, i));
  }, [startDate, daysToShow]);

  // Headers for the timeline based on view mode
  const timelineHeaders = useMemo(() => {
    if (viewMode === "day") {
      return timelineDates.map((date) => format(date, "EEE d", { locale: es }));
    } else if (viewMode === "week") {
      const weeks: { [key: string]: Date[] } = {};
      timelineDates.forEach((date) => {
        const weekKey = format(date, "w-yyyy");
        if (!weeks[weekKey]) {
          weeks[weekKey] = [];
        }
        weeks[weekKey].push(date);
      });
      return Object.entries(weeks).map(([key, dates]) => ({
        label: `Semana ${key.split("-")[0]}`,
        span: dates.length,
        startDate: dates[0],
      }));
    } else {
      const months: { [key: string]: Date[] } = {};
      timelineDates.forEach((date) => {
        const monthKey = format(date, "MMM-yyyy", { locale: es });
        if (!months[monthKey]) {
          months[monthKey] = [];
        }
        months[monthKey].push(date);
      });
      return Object.entries(months).map(([key, dates]) => ({
        label: key.split("-")[0],
        span: dates.length,
        startDate: dates[0],
      }));
    }
  }, [timelineDates, viewMode]);

  // Fetch tasks from API
  const fetchTasks = async () => {
    setLoading(true);
    setError(null);
    try {
      const headers: HeadersInit = {
        "Content-Type": "application/json",
      };

      if (auth.token) {
        headers["Authorization"] = `Bearer ${auth.token}`;
      }

      // First, get all items
      const itemsResponse = await fetch(`${API_URL}/api/Items`, { headers });
      if (!itemsResponse.ok) throw new Error("Error fetching tasks");

      const items = await itemsResponse.json();

      // Get columns to filter by boardId
      const columnsResponse = await fetch(`${API_URL}/api/Columns`, {
        headers,
      });
      if (!columnsResponse.ok) throw new Error("Error fetching columns");

      const columns = await columnsResponse.json();
      const boardColumns = columns.filter(
        (col: any) => col.boardId === boardId
      );
      const boardColumnIds = boardColumns.map((col: any) => col.id);

      // Filter items by column IDs
      const boardTasks = items.filter((item: any) =>
        boardColumnIds.includes(item.columnId)
      );

      // Transform to Gantt tasks
      const ganttTasks = boardTasks.map((item: any) => ({
        id: item.id,
        title: item.title,
        description: item.description || "",
        startDate: item.startDate || new Date().toISOString(),
        endDate: item.dueDate || addDays(new Date(), 3).toISOString(),
        progress: item.status === "completed" ? 100 : item.progress || 0,
        columnId: item.columnId,
        boardId,
        status: item.status || "pending",
      }));

      setTasks(ganttTasks);
    } catch (err) {
      console.error("Error fetching tasks:", err);
      setError("Error al cargar las tareas. Por favor, inténtalo de nuevo.");
    } finally {
      setLoading(false);
    }
  };

  // Update task dates when dragged
  const updateTaskDates = async (
    taskId: string,
    newStartDate: Date,
    newEndDate: Date
  ) => {
    try {
      const headers: HeadersInit = {
        "Content-Type": "application/json",
        Authorization: `Bearer ${auth.token}`,
      };

      const task = tasks.find((t) => t.id === taskId);
      if (!task) return;

      const updatedTask = {
        ...task,
        startDate: newStartDate.toISOString(),
        endDate: newEndDate.toISOString(),
      };

      const response = await fetch(`${API_URL}/api/Items/${taskId}`, {
        method: "PUT",
        headers,
        body: JSON.stringify({
          id: taskId,
          title: task.title,
          description: task.description,
          startDate: newStartDate.toISOString(),
          dueDate: newEndDate.toISOString(),
          columnId: task.columnId,
        }),
      });

      if (!response.ok) throw new Error("Error updating task");

      // Update local state
      setTasks(tasks.map((t) => (t.id === taskId ? updatedTask : t)));
    } catch (err) {
      console.error("Error updating task:", err);
      setError("Error al actualizar la tarea. Por favor, inténtalo de nuevo.");
    }
  };

  // Handle task selection
  const handleTaskClick = (task: Task) => {
    setSelectedTask(task);
    setIsDialogOpen(true);
  };

  // Handle task update from dialog
  const handleTaskUpdate = async (updatedTask: Task) => {
    try {
      const headers: HeadersInit = {
        "Content-Type": "application/json",
        Authorization: `Bearer ${auth.token}`,
      };

      const response = await fetch(`${API_URL}/api/Items/${updatedTask.id}`, {
        method: "PUT",
        headers,
        body: JSON.stringify({
          id: updatedTask.id,
          title: updatedTask.title,
          description: updatedTask.description,
          startDate: updatedTask.startDate,
          dueDate: updatedTask.endDate,
          columnId: updatedTask.columnId,
        }),
      });

      if (!response.ok) throw new Error("Error updating task");

      // Update local state
      setTasks(tasks.map((t) => (t.id === updatedTask.id ? updatedTask : t)));
      setIsDialogOpen(false);
    } catch (err) {
      console.error("Error updating task:", err);
      setError("Error al actualizar la tarea. Por favor, inténtalo de nuevo.");
    }
  };

  // Navigate timeline
  const navigateTimeline = (direction: "prev" | "next") => {
    const days = direction === "prev" ? -daysToShow / 2 : daysToShow / 2;
    setStartDate(addDays(startDate, days));
  };

  // Filter tasks based on showCompleted
  const filteredTasks = useMemo(() => {
    return showCompleted
      ? tasks
      : tasks.filter(
          (task) => task.progress < 100 && task.status !== "completed"
        );
  }, [tasks, showCompleted]);

  // Calculate task position and width
  const getTaskStyle = (task: Task) => {
    const taskStart = new Date(task.startDate);
    const taskEnd = new Date(task.endDate);

    // Check if task is in view
    const isInView = timelineDates.some((date) =>
      isWithinInterval(date, {
        start: startOfDay(taskStart),
        end: endOfDay(taskEnd),
      })
    );

    if (!isInView) return { display: "none" };

    // Calculate position
    const startDiff = Math.max(0, differenceInDays(taskStart, startDate));
    const duration = Math.max(1, differenceInDays(taskEnd, taskStart) + 1);

    // Limit width to visible area
    const visibleDuration = Math.min(duration, daysToShow - startDiff);

    return {
      gridColumnStart: startDiff + 1,
      gridColumnEnd: `span ${visibleDuration}`,
    };
  };

  // Status color mapping
  const getStatusColor = (status?: string, progress?: number) => {
    if (progress === 100 || status === "completed") return "bg-green-500";
    if (status === "delayed") return "bg-red-500";
    if (status === "in-progress") return "bg-blue-500";
    return "bg-yellow-500"; // pending
  };

  // Load tasks on mount and when boardId changes
  useEffect(() => {
    if (boardId) {
      fetchTasks();
    }
  }, [boardId]);

  // Drag and drop functionality
  const [draggedTask, setDraggedTask] = useState<string | null>(null);
  const [dragStartX, setDragStartX] = useState(0);
  const [dragType, setDragType] = useState<
    "move" | "resize-start" | "resize-end" | null
  >(null);
  const [originalDates, setOriginalDates] = useState<{
    start: Date;
    end: Date;
  } | null>(null);

  const handleDragStart = (
    e: React.MouseEvent,
    taskId: string,
    type: "move" | "resize-start" | "resize-end"
  ) => {
    e.preventDefault();
    const task = tasks.find((t) => t.id === taskId);
    if (!task) return;

    setDraggedTask(taskId);
    setDragStartX(e.clientX);
    setDragType(type);
    setOriginalDates({
      start: new Date(task.startDate),
      end: new Date(task.endDate),
    });

    // Add event listeners for drag
    document.addEventListener("mousemove", handleDragMove);
    document.addEventListener("mouseup", handleDragEnd);
  };

  const handleDragMove = (e: MouseEvent) => {
    if (!draggedTask || !originalDates || !dragType) return;

    const dayWidth = document.querySelector(".gantt-day")?.clientWidth || 40;
    const daysDiff = Math.round((e.clientX - dragStartX) / dayWidth);

    if (daysDiff === 0) return;

    const task = tasks.find((t) => t.id === draggedTask);
    if (!task) return;

    const newTasks = [...tasks];
    const taskIndex = newTasks.findIndex((t) => t.id === draggedTask);

    if (dragType === "move") {
      // Move entire task
      const newStartDate = addDays(originalDates.start, daysDiff);
      const newEndDate = addDays(originalDates.end, daysDiff);

      newTasks[taskIndex] = {
        ...task,
        startDate: newStartDate.toISOString(),
        endDate: newEndDate.toISOString(),
      };
    } else if (dragType === "resize-start") {
      // Resize from start
      const newStartDate = addDays(originalDates.start, daysDiff);
      // Ensure start date is not after end date
      if (newStartDate < new Date(task.endDate)) {
        newTasks[taskIndex] = {
          ...task,
          startDate: newStartDate.toISOString(),
        };
      }
    } else if (dragType === "resize-end") {
      // Resize from end
      const newEndDate = addDays(originalDates.end, daysDiff);
      // Ensure end date is not before start date
      if (newEndDate > new Date(task.startDate)) {
        newTasks[taskIndex] = {
          ...task,
          endDate: newEndDate.toISOString(),
        };
      }
    }

    setTasks(newTasks);
  };

  const handleDragEnd = () => {
    if (draggedTask && originalDates) {
      const task = tasks.find((t) => t.id === draggedTask);
      if (task) {
        // Save changes to server
        updateTaskDates(
          draggedTask,
          new Date(task.startDate),
          new Date(task.endDate)
        );
      }
    }

    // Clean up
    setDraggedTask(null);
    setDragStartX(0);
    setDragType(null);
    setOriginalDates(null);

    // Remove event listeners
    document.removeEventListener("mousemove", handleDragMove);
    document.removeEventListener("mouseup", handleDragEnd);
  };

  return (
    <Card className={cn("w-full", className)}>
      <CardHeader className="pb-3">
        <div className="flex items-center justify-between">
          <div>
            <CardTitle>Diagrama de Gantt</CardTitle>
            <CardDescription>
              Visualización de tareas y cronograma del proyecto
            </CardDescription>
          </div>
          <div className="flex items-center gap-2">
            <div className="flex items-center gap-2 mr-4">
              <Checkbox
                id="show-completed"
                checked={showCompleted}
                onCheckedChange={(checked) => setShowCompleted(!!checked)}
              />
              <Label htmlFor="show-completed">Mostrar completadas</Label>
            </div>
            <Select
              value={viewMode}
              onValueChange={(value: "day" | "week" | "month") =>
                setViewMode(value)
              }
            >
              <SelectTrigger className="w-[120px]">
                <SelectValue placeholder="Vista" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="day">Diaria</SelectItem>
                <SelectItem value="week">Semanal</SelectItem>
                <SelectItem value="month">Mensual</SelectItem>
              </SelectContent>
            </Select>
            <div className="flex">
              <Button
                variant="outline"
                size="icon"
                onClick={() => navigateTimeline("prev")}
              >
                <ChevronLeft className="h-4 w-4" />
              </Button>
              <Button
                variant="outline"
                size="icon"
                onClick={() => navigateTimeline("next")}
              >
                <ChevronRight className="h-4 w-4" />
              </Button>
            </div>
          </div>
        </div>
      </CardHeader>
      <CardContent>
        {loading ? (
          <div className="flex justify-center items-center h-64">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-foreground" />
          </div>
        ) : error ? (
          <div className="text-center text-red-500 p-4">{error}</div>
        ) : (
          <div className="gantt-container overflow-x-auto">
            <div className="gantt-chart min-w-[800px]">
              {/* Timeline headers */}
              <div
                className="gantt-timeline-header grid"
                style={{
                  gridTemplateColumns: `200px repeat(${daysToShow}, minmax(40px, 1fr))`,
                }}
              >
                <div className="gantt-header-cell border-r border-b p-2 bg-muted/50">
                  Tarea
                </div>
                {viewMode === "day"
                  ? // Daily view
                    timelineDates.map((date, i) => (
                      <div
                        key={i}
                        className={cn(
                          "gantt-header-cell gantt-day border-r border-b p-2 text-center text-xs",
                          format(date, "yyyy-MM-dd") ===
                            format(new Date(), "yyyy-MM-dd")
                            ? "bg-blue-100 dark:bg-blue-900/20"
                            : "bg-muted/50"
                        )}
                      >
                        {format(date, "EEE d", { locale: es })}
                      </div>
                    ))
                  : // Week or Month view
                    timelineHeaders.map((header: any, i) => (
                      <div
                        key={i}
                        className="gantt-header-cell border-r border-b p-2 text-center text-xs bg-muted/50"
                        style={{ gridColumn: `span ${header.span}` }}
                      >
                        {header.label}
                      </div>
                    ))}
              </div>

              {/* Days subheader for week/month views */}
              {viewMode !== "day" && (
                <div
                  className="gantt-timeline-subheader grid"
                  style={{
                    gridTemplateColumns: `200px repeat(${daysToShow}, minmax(40px, 1fr))`,
                  }}
                >
                  <div className="gantt-header-cell border-r border-b p-2 bg-muted/30"></div>
                  {timelineDates.map((date, i) => (
                    <div
                      key={i}
                      className={cn(
                        "gantt-day border-r border-b p-1 text-center text-xs",
                        format(date, "yyyy-MM-dd") ===
                          format(new Date(), "yyyy-MM-dd")
                          ? "bg-blue-100 dark:bg-blue-900/20"
                          : "bg-muted/30"
                      )}
                    >
                      {format(date, "d", { locale: es })}
                    </div>
                  ))}
                </div>
              )}

              {/* Tasks */}
              <div className="gantt-body">
                {filteredTasks.length === 0 ? (
                  <div className="col-span-full text-center py-8 border-b">
                    No hay tareas para mostrar
                  </div>
                ) : (
                  filteredTasks.map((task) => (
                    <div
                      key={task.id}
                      className="gantt-row grid relative"
                      style={{
                        gridTemplateColumns: `200px repeat(${daysToShow}, minmax(40px, 1fr))`,
                      }}
                    >
                      <div className="gantt-task-info border-r border-b p-2 truncate">
                        <div className="font-medium truncate">{task.title}</div>
                      </div>

                      {/* Timeline cells */}
                      {Array.from({ length: daysToShow }).map((_, i) => (
                        <div
                          key={i}
                          className={cn(
                            "gantt-cell border-r border-b",
                            format(addDays(startDate, i), "yyyy-MM-dd") ===
                              format(new Date(), "yyyy-MM-dd")
                              ? "bg-blue-100 dark:bg-blue-900/20"
                              : ""
                          )}
                        ></div>
                      ))}

                      {/* Task bar */}
                      <div
                        className={cn(
                          "gantt-task-bar absolute rounded-md cursor-pointer flex items-center px-2 text-white text-xs",
                          getStatusColor(task.status, task.progress)
                        )}
                        style={{
                          ...getTaskStyle(task),
                          top: "4px",
                          height: "calc(100% - 8px)",
                        }}
                        onClick={() => handleTaskClick(task)}
                      >
                        <div
                          className="gantt-task-handle left absolute left-0 top-0 bottom-0 w-2 cursor-ew-resize"
                          onMouseDown={(e) =>
                            handleDragStart(e, task.id, "resize-start")
                          }
                        ></div>

                        <div
                          className="gantt-task-content flex-1 truncate"
                          onMouseDown={(e) =>
                            handleDragStart(e, task.id, "move")
                          }
                        >
                          <GripVertical className="h-3 w-3 inline mr-1 opacity-70" />
                          {task.title}
                        </div>

                        <div
                          className="gantt-task-handle right absolute right-0 top-0 bottom-0 w-2 cursor-ew-resize"
                          onMouseDown={(e) =>
                            handleDragStart(e, task.id, "resize-end")
                          }
                        ></div>

                        {/* Progress bar */}
                        {task.progress > 0 && (
                          <div
                            className="absolute left-0 top-0 bottom-0 bg-white/20"
                            style={{ width: `${task.progress}%` }}
                          ></div>
                        )}
                      </div>
                    </div>
                  ))
                )}
              </div>
            </div>
          </div>
        )}
      </CardContent>

      {/* Task dialog */}
      {selectedTask && (
        <TaskDialog
          task={selectedTask}
          open={isDialogOpen}
          onOpenChange={setIsDialogOpen}
          onUpdate={handleTaskUpdate}
        />
      )}
    </Card>
  );
}
