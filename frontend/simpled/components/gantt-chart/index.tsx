'use client';

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { useAuth } from '@/contexts/AuthContext';
import { cn } from '@/lib/utils';
import {
  DndContext,
  type DragEndEvent,
  MouseSensor,
  TouchSensor,
  useSensor,
  useSensors,
} from '@dnd-kit/core';
import { format } from 'date-fns';
import { useRef, useState } from 'react';
import { GanttTaskDialog } from './gantt-task-dialog';
import { GanttToolbar } from './gantt-toolbar';
import { GanttView } from './gantt-view';
import { useGanttData } from './use-gantt-data';
import { useGanttFilters } from './use-gantt-filters';
import { useGanttTimeline } from './use-gantt-timeline';

export interface Task {
  id: string;
  title: string;
  description?: string;
  startDate: string;
  endDate: string;
  progress: number;
  columnId?: string;
  boardId?: string;
  assignedTo?: string;
  status?: 'pending' | 'in-progress' | 'completed' | 'delayed';
  groupId?: string;
  isGroup?: boolean;
  order?: number;
}

interface GanttChartProps {
  readonly boardId: string;
  readonly className?: string;
}

export function GanttChart({ boardId, className }: GanttChartProps) {
  const { auth } = useAuth();
  const containerRef = useRef<HTMLDivElement>(null);
  const [selectedTask, setSelectedTask] = useState<Task | null>(null);
  const [isDialogOpen, setIsDialogOpen] = useState(false);

  const { tasks, loading, error, fetchTasks, updateTask, updateTaskDates } = useGanttData(
    boardId,
    auth,
  );

  const {
    viewMode,
    setViewMode,
    startDate,
    setStartDate,
    daysToShow,
    timelineDates,
    timelineHeaders,
    navigateTimeline,
  } = useGanttTimeline();

  const {
    showCompleted,
    setShowCompleted,
    filterStatus,
    setFilterStatus,
    searchTerm,
    setSearchTerm,
    groupBy,
    setGroupBy,
    zoomLevel,
    setZoomLevel,
    filteredTasks,
    groupedTasks,
    toggleGroupExpansion,
  } = useGanttFilters(tasks);

  // Configure dnd-kit sensors
  const sensors = useSensors(
    useSensor(MouseSensor, {
      // Require the mouse to move by 10 pixels before activating
      activationConstraint: {
        distance: 10,
      },
    }),
    useSensor(TouchSensor, {
      // Press delay of 250ms, with tolerance of 5px of movement
      activationConstraint: {
        delay: 250,
        tolerance: 5,
      },
    }),
  );

  const handleTaskClick = (task: Task) => {
    setSelectedTask(task);
    setIsDialogOpen(true);
  };

  const handleDragEnd = (event: DragEndEvent) => {
    const { active, over } = event;

    if (!over) return;

    const taskId = active.id as string;
    const task = tasks.find((t) => t.id === taskId);

    if (!task) return;

    // Extract the new dates from the data attribute
    const newStartDate = new Date(active.data.current?.startDate);
    const newEndDate = new Date(active.data.current?.endDate);

    // Update the task dates
    updateTaskDates(taskId, newStartDate, newEndDate);
  };

  const exportToCSV = () => {
    const headers = [
      'ID',
      'Título',
      'Descripción',
      'Fecha Inicio',
      'Fecha Fin',
      'Progreso',
      'Estado',
    ];
    const csvData = [
      headers.join(','),
      ...tasks.map((task) =>
        [
          task.id,
          `"${task.title.replace(/"/g, '""')}"`,
          `"${(task.description || '').replace(/"/g, '""')}"`,
          format(new Date(task.startDate), 'yyyy-MM-dd'),
          format(new Date(task.endDate), 'yyyy-MM-dd'),
          task.progress,
          task.status || 'pending',
        ].join(','),
      ),
    ].join('\n');

    const blob = new Blob([csvData], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.setAttribute('href', url);
    link.setAttribute('download', `gantt-export-${format(new Date(), 'yyyy-MM-dd')}.csv`);
    link.style.visibility = 'hidden';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  };

  return (
    <Card className={cn('w-full', className)}>
      <CardHeader className="pb-3">
        <div className="flex flex-col gap-4">
          <div className="flex items-center justify-between">
            <div>
              <CardTitle>Diagrama de Gantt</CardTitle>
              <CardDescription>Visualización de tareas y cronograma del proyecto</CardDescription>
            </div>
          </div>

          <GanttToolbar
            viewMode={viewMode}
            setViewMode={setViewMode}
            showCompleted={showCompleted}
            setShowCompleted={setShowCompleted}
            filterStatus={filterStatus}
            setFilterStatus={setFilterStatus}
            searchTerm={searchTerm}
            setSearchTerm={setSearchTerm}
            groupBy={groupBy}
            setGroupBy={setGroupBy}
            zoomLevel={zoomLevel}
            setZoomLevel={setZoomLevel}
            navigateTimeline={navigateTimeline}
            exportData={exportToCSV}
          />
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
          <div className="gantt-container overflow-x-auto" ref={containerRef}>
            <DndContext sensors={sensors} onDragEnd={handleDragEnd}>
              <GanttView
                tasks={tasks}
                filteredTasks={filteredTasks}
                groupedTasks={groupedTasks}
                timelineDates={timelineDates}
                timelineHeaders={timelineHeaders}
                viewMode={viewMode}
                startDate={startDate}
                daysToShow={daysToShow}
                zoomLevel={zoomLevel}
                onTaskClick={handleTaskClick}
                toggleGroupExpansion={toggleGroupExpansion}
              />
            </DndContext>
          </div>
        )}
      </CardContent>

      {selectedTask && (
        <GanttTaskDialog
          task={selectedTask}
          open={isDialogOpen}
          onOpenChange={setIsDialogOpen}
          onUpdate={updateTask}
          allTasks={tasks}
        />
      )}
    </Card>
  );
}
