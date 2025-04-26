'use client';

import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Checkbox } from '@/components/ui/checkbox';
import { Label } from '@/components/ui/label';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { useAuth } from '@/contexts/AuthContext';
import { cn } from '@/lib/utils';
import {
  addDays,
  differenceInDays,
  endOfDay,
  format,
  isWithinInterval,
  startOfDay,
} from 'date-fns';
import { es } from 'date-fns/locale';
import { ChevronLeft, ChevronRight, GripVertical } from 'lucide-react';
import type React from 'react';
import { useEffect, useMemo, useState } from 'react';
import { TaskDialog } from './task-dialog';

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
  status?: 'pending' | 'in-progress' | 'completed' | 'delayed';
}

interface GanttChartProps {
  readonly boardId: string;
  readonly className?: string;
}

const API_URL = 'http://localhost:5193';

export function GanttChart({ boardId, className }: GanttChartProps) {
  const { auth } = useAuth();
  const [tasks, setTasks] = useState<Task[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [viewMode, setViewMode] = useState<'day' | 'week' | 'month'>('week');
  const [startDate, setStartDate] = useState<Date>(() => {
    const today = new Date();
    today.setDate(today.getDate() - 7);
    return today;
  });
  const [selectedTask, setSelectedTask] = useState<Task | null>(null);
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [showCompleted, setShowCompleted] = useState(true);

  // Calcular el número de días
  const daysToShow = useMemo(() => {
    switch (viewMode) {
      case 'day':
        return 14;
      case 'week':
        return 28;
      case 'month':
        return 60;
      default:
        return 28;
    }
  }, [viewMode]);

  // Fechas para la línea
  const timelineDates = useMemo(() => {
    return Array.from({ length: daysToShow }, (_, i) => addDays(startDate, i));
  }, [startDate, daysToShow]);

  // Headers para la línea de tiempo
  const timelineHeaders = useMemo(() => {
    if (viewMode === 'day') {
      return timelineDates.map((date) => format(date, 'EEE d', { locale: es }));
    } else if (viewMode === 'week') {
      const weeks: { [key: string]: Date[] } = {};
      timelineDates.forEach((date) => {
        const weekKey = format(date, 'w-yyyy');
        if (!weeks[weekKey]) {
          weeks[weekKey] = [];
        }
        weeks[weekKey].push(date);
      });
      return Object.entries(weeks).map(([key, dates]) => ({
        label: `Semana ${key.split('-')[0]}`,
        span: dates.length,
        startDate: dates[0],
      }));
    } else {
      const months: { [key: string]: Date[] } = {};
      timelineDates.forEach((date) => {
        const monthKey = format(date, 'MMM-yyyy', { locale: es });
        if (!months[monthKey]) {
          months[monthKey] = [];
        }
        months[monthKey].push(date);
      });
      return Object.entries(months).map(([key, dates]) => ({
        label: key.split('-')[0],
        span: dates.length,
        startDate: dates[0],
      }));
    }
  }, [timelineDates, viewMode]);

  const fetchTasks = async () => {
    setLoading(true);
    setError(null);
    try {
      const headers: HeadersInit = {
        'Content-Type': 'application/json',
      };

      if (auth.token) {
        headers['Authorization'] = `Bearer ${auth.token}`;
      }

      // Tareas de la API
      const itemsResponse = await fetch(`${API_URL}/api/Items`, { headers });
      if (!itemsResponse.ok) throw new Error('Error fetching tasks');

      const items = await itemsResponse.json();

      // Columnas de la API
      const columnsResponse = await fetch(`${API_URL}/api/Columns`, {
        headers,
      });
      if (!columnsResponse.ok) throw new Error('Error fetching columns');

      const columns = await columnsResponse.json();
      const boardColumns = columns.filter((col: any) => col.boardId === boardId);
      const boardColumnIds = boardColumns.map((col: any) => col.id);

      // Filtrado de tareas por columnas del tablero
      const boardTasks = items.filter((item: any) => boardColumnIds.includes(item.columnId));

      // Transformar tareas a formato Gantt
      const ganttTasks = boardTasks.map((item: any) => ({
        id: item.id,
        title: item.title,
        description: item.description ?? '',
        startDate: item.startDate ?? new Date().toISOString(),
        endDate: item.dueDate ?? addDays(new Date(), 3).toISOString(),
        progress: item.status === 'completed' ? 100 : (item.progress ?? 0),
        columnId: item.columnId,
        boardId,
        status: item.status ?? 'pending',
      }));

      setTasks(ganttTasks);
    } catch (err) {
      console.error('Error fetching tasks:', err);
      setError('Error al cargar las tareas. Por favor, inténtalo de nuevo.');
    } finally {
      setLoading(false);
    }
  };

  // Actualizar fechas de tareas
  const updateTaskDates = async (taskId: string, newStartDate: Date, newEndDate: Date) => {
    try {
      const headers: HeadersInit = {
        'Content-Type': 'application/json',
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
        method: 'PUT',
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

      if (!response.ok) throw new Error('Error updating task');

      setTasks(tasks.map((t) => (t.id === taskId ? updatedTask : t)));
    } catch (err) {
      console.error('Error updating task:', err);
      setError('Error al actualizar la tarea. Por favor, inténtalo de nuevo.');
    }
  };

  const handleTaskClick = (task: Task) => {
    setSelectedTask(task);
    setIsDialogOpen(true);
  };

  const handleTaskUpdate = async (updatedTask: Task) => {
    try {
      const headers: HeadersInit = {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${auth.token}`,
      };

      const response = await fetch(`${API_URL}/api/Items/${updatedTask.id}`, {
        method: 'PUT',
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

      if (!response.ok) throw new Error('Error updating task');

      setTasks(tasks.map((t) => (t.id === updatedTask.id ? updatedTask : t)));
      setIsDialogOpen(false);
    } catch (err) {
      console.error('Error updating task:', err);
      setError('Error al actualizar la tarea. Por favor, inténtalo de nuevo.');
    }
  };

  const navigateTimeline = (direction: 'prev' | 'next') => {
    const days = direction === 'prev' ? -daysToShow / 2 : daysToShow / 2;
    setStartDate(addDays(startDate, days));
  };

  // Filtrar por tareas completadas
  const filteredTasks = useMemo(() => {
    return showCompleted
      ? tasks
      : tasks.filter((task) => task.progress < 100 && task.status !== 'completed');
  }, [tasks, showCompleted]);

  // Calcular el estilo de la tarea
  const getTaskStyle = (task: Task) => {
    const taskStart = new Date(task.startDate);
    const taskEnd = new Date(task.endDate);

    const isInView = timelineDates.some((date) =>
      isWithinInterval(date, {
        start: startOfDay(taskStart),
        end: endOfDay(taskEnd),
      }),
    );

    if (!isInView) return { display: 'none' };

    const startDiff = Math.max(0, differenceInDays(taskStart, startDate));
    const duration = Math.max(1, differenceInDays(taskEnd, taskStart) + 1);

    const visibleDuration = Math.min(duration, daysToShow - startDiff);

    return {
      gridColumnStart: startDiff + 1,
      gridColumnEnd: `span ${visibleDuration}`,
    };
  };

  // Colores Status
  const getStatusColor = (status?: string, progress?: number) => {
    if (progress === 100 || status === 'completed') return 'bg-emerald-500 hover:bg-emerald-600';
    if (status === 'delayed') return 'bg-rose-500 hover:bg-rose-600';
    if (status === 'in-progress') return 'bg-blue-500 hover:bg-blue-600';
    return 'bg-amber-500 hover:bg-amber-600'; // pending
  };

  useEffect(() => {
    if (boardId) {
      fetchTasks();
    }
  }, [boardId]);

  const [draggedTask, setDraggedTask] = useState<string | null>(null);
  const [dragStartX, setDragStartX] = useState(0);
  const [dragType, setDragType] = useState<'move' | 'resize-start' | 'resize-end' | null>(null);
  const [originalDates, setOriginalDates] = useState<{
    start: Date;
    end: Date;
  } | null>(null);

  const handleDragStart = (
    e: React.MouseEvent,
    taskId: string,
    type: 'move' | 'resize-start' | 'resize-end',
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

    document.addEventListener('mousemove', handleDragMove);
    document.addEventListener('mouseup', handleDragEnd);
  };

  const handleDragMove = (e: MouseEvent) => {
    if (!draggedTask || !originalDates || !dragType) return;

    const dayWidth = document.querySelector('.gantt-day')?.clientWidth ?? 40;
    const daysDiff = Math.round((e.clientX - dragStartX) / dayWidth);

    if (daysDiff === 0) return;

    const task = tasks.find((t) => t.id === draggedTask);
    if (!task) return;

    const newTasks = [...tasks];
    const taskIndex = newTasks.findIndex((t) => t.id === draggedTask);

    if (dragType === 'move') {
      const newStartDate = addDays(originalDates.start, daysDiff);
      const newEndDate = addDays(originalDates.end, daysDiff);

      newTasks[taskIndex] = {
        ...task,
        startDate: newStartDate.toISOString(),
        endDate: newEndDate.toISOString(),
      };
    } else if (dragType === 'resize-start') {
      const newStartDate = addDays(originalDates.start, daysDiff);
      if (newStartDate < new Date(task.endDate)) {
        newTasks[taskIndex] = {
          ...task,
          startDate: newStartDate.toISOString(),
        };
      }
    } else if (dragType === 'resize-end') {
      const newEndDate = addDays(originalDates.end, daysDiff);
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
        updateTaskDates(draggedTask, new Date(task.startDate), new Date(task.endDate));
      }
    }

    // Limpieza
    setDraggedTask(null);
    setDragStartX(0);
    setDragType(null);
    setOriginalDates(null);

    document.removeEventListener('mousemove', handleDragMove);
    document.removeEventListener('mouseup', handleDragEnd);
  };

  useEffect(() => {
    const updateTodayIndicator = () => {
      const todayIndicator = document.querySelector('.gantt-today-line');
      if (!todayIndicator) return;

      const dayWidth = document.querySelector('.gantt-day')?.clientWidth || 40;
      const daysSinceStart = differenceInDays(new Date(), startDate);

      if (daysSinceStart >= 0 && daysSinceStart < daysToShow) {
        todayIndicator.setAttribute(
          'style',
          `left: calc(200px + ${daysSinceStart * dayWidth}px); display: block;`,
        );
      } else {
        todayIndicator.setAttribute('style', 'display: none;');
      }
    };

    updateTodayIndicator();

    // Update on window resize
    window.addEventListener('resize', updateTodayIndicator);
    return () => window.removeEventListener('resize', updateTodayIndicator);
  }, [startDate, daysToShow]);

  return (
    <Card className={cn('w-full', className)}>
      <CardHeader className="pb-3">
        <div className="flex items-center justify-between">
          <div>
            <CardTitle>Diagrama de Gantt</CardTitle>
            <CardDescription>Visualización de tareas y cronograma del proyecto</CardDescription>
          </div>
          <div className="flex items-center gap-2">
            <div className="mr-4 flex items-center gap-2">
              <Checkbox
                id="show-completed"
                checked={showCompleted}
                onCheckedChange={(checked) => setShowCompleted(!!checked)}
              />
              <Label htmlFor="show-completed">Mostrar completadas</Label>
            </div>
            <Select
              value={viewMode}
              onValueChange={(value: 'day' | 'week' | 'month') => setViewMode(value)}
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
              <Button variant="outline" size="icon" onClick={() => navigateTimeline('prev')}>
                <ChevronLeft className="h-4 w-4" />
              </Button>
              <Button variant="outline" size="icon" onClick={() => navigateTimeline('next')}>
                <ChevronRight className="h-4 w-4" />
              </Button>
            </div>
          </div>
        </div>
      </CardHeader>
      <CardContent>
        {loading ? (
          <div className="flex h-64 items-center justify-center">
            <div className="border-foreground h-12 w-12 animate-spin rounded-full border-b-2" />
          </div>
        ) : error ? (
          <div className="p-4 text-center text-red-500">{error}</div>
        ) : (
          <div className="gantt-container overflow-x-auto">
            <div className="gantt-chart min-w-[800px]">
              <div
                className="gantt-timeline-header grid"
                style={{
                  gridTemplateColumns: `200px repeat(${daysToShow}, minmax(40px, 1fr))`,
                }}
              >
                <div className="gantt-header-cell bg-muted/50 border-r border-b p-2">Tarea</div>
                {viewMode === 'day'
                  ? timelineDates.map((date, i) => (
                      <div
                        key={i}
                        className={cn(
                          'gantt-header-cell gantt-day border-r border-b p-2 text-center text-xs',
                          format(date, 'yyyy-MM-dd') === format(new Date(), 'yyyy-MM-dd')
                            ? 'bg-blue-100 dark:bg-blue-900/20'
                            : 'bg-muted/50',
                        )}
                      >
                        {format(date, 'EEE d', { locale: es })}
                      </div>
                    ))
                  : timelineHeaders.map((header: any, i) => (
                      <div
                        key={i}
                        className="gantt-header-cell bg-muted/50 border-r border-b p-2 text-center text-xs"
                        style={{ gridColumn: `span ${header.span}` }}
                      >
                        {header.label}
                      </div>
                    ))}
              </div>

              {viewMode !== 'day' && (
                <div
                  className="gantt-timeline-subheader grid"
                  style={{
                    gridTemplateColumns: `200px repeat(${daysToShow}, minmax(40px, 1fr))`,
                  }}
                >
                  <div className="gantt-header-cell bg-muted/30 border-r border-b p-2"></div>
                  {timelineDates.map((date, i) => (
                    <div
                      key={i}
                      className={cn(
                        'gantt-day border-r border-b p-1 text-center text-xs',
                        format(date, 'yyyy-MM-dd') === format(new Date(), 'yyyy-MM-dd')
                          ? 'bg-blue-100 dark:bg-blue-900/20'
                          : 'bg-muted/30',
                      )}
                    >
                      {format(date, 'd', { locale: es })}
                    </div>
                  ))}
                </div>
              )}

              <div className="gantt-body">
                {filteredTasks.length === 0 ? (
                  <div className="col-span-full border-b py-8 text-center">
                    No hay tareas para mostrar
                  </div>
                ) : (
                  filteredTasks.map((task) => (
                    <div
                      key={task.id}
                      className="gantt-row relative grid"
                      style={{
                        gridTemplateColumns: `200px repeat(${daysToShow}, minmax(40px, 1fr))`,
                      }}
                    >
                      <div className="gantt-task-info truncate border-r border-b p-2">
                        <div className="truncate font-medium">{task.title}</div>
                      </div>

                      {Array.from({ length: daysToShow }).map((_, i) => (
                        <div
                          key={i}
                          className={cn(
                            'gantt-cell border-r border-b',
                            format(addDays(startDate, i), 'yyyy-MM-dd') ===
                              format(new Date(), 'yyyy-MM-dd')
                              ? 'bg-blue-100 dark:bg-blue-900/20'
                              : '',
                          )}
                        ></div>
                      ))}

                      <div
                        className={cn(
                          'gantt-task-bar absolute flex cursor-pointer items-center rounded-md px-2 text-xs text-white',
                          getStatusColor(task.status, task.progress),
                        )}
                        style={{
                          ...getTaskStyle(task),
                          top: '4px',
                          height: 'calc(100% - 8px)',
                        }}
                        onClick={() => handleTaskClick(task)}
                      >
                        <div
                          className="gantt-task-handle left absolute top-0 bottom-0 left-0 w-2 cursor-ew-resize rounded-l-md"
                          onMouseDown={(e) => handleDragStart(e, task.id, 'resize-start')}
                        ></div>

                        <div
                          className="gantt-task-content flex-1 truncate"
                          onMouseDown={(e) => handleDragStart(e, task.id, 'move')}
                        >
                          <GripVertical className="mr-1 inline h-3 w-3 opacity-70" />
                          {task.title}
                        </div>

                        <div
                          className="gantt-task-handle right absolute top-0 right-0 bottom-0 w-2 cursor-ew-resize rounded-r-md"
                          onMouseDown={(e) => handleDragStart(e, task.id, 'resize-end')}
                        ></div>

                        {task.progress > 0 && (
                          <div
                            className="absolute top-0 bottom-0 left-0 rounded-l-md bg-white/20"
                            style={{ width: `${task.progress}%`, maxWidth: '100%' }}
                          ></div>
                        )}
                      </div>
                    </div>
                  ))
                )}
              </div>
              {/* Today indicator */}
              <div className="gantt-today-line">
                <div className="gantt-today-label">Hoy</div>
              </div>
            </div>
          </div>
        )}
      </CardContent>

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
