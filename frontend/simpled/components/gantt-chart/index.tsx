'use client';

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { useAuth } from '@/contexts/AuthContext';
import { cn } from '@/lib/utils';
import { format } from 'date-fns';
import { useEffect, useRef } from 'react';
import { GanttRenderer } from './gantt-renderer';
import { GanttTaskDialog } from './gantt-task-dialog';
import { GanttToolbar } from './gantt-toolbar';
import { useGanttData } from './use-gantt-data';
import { useGanttFilters } from './use-gantt-filters';
import { useGanttInteractions } from './use-gantt-interactions';
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
  dependencies?: string[];
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
    showDependencies,
    setShowDependencies,
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

  const {
    selectedTask,
    setSelectedTask,
    isDialogOpen,
    setIsDialogOpen,
    draggedTask,
    setDraggedTask,
    handleTaskClick,
    handleDragStart,
    handleDragMove,
    handleDragEnd,
  } = useGanttInteractions(tasks, updateTaskDates);

  const exportToCSV = () => {
    const headers = [
      'ID',
      'Título',
      'Descripción',
      'Fecha Inicio',
      'Fecha Fin',
      'Progreso',
      'Estado',
      'Dependencias',
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
          (task.dependencies || []).join(';'),
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

  useEffect(() => {
    const updateTodayIndicator = () => {
      if (!containerRef.current) return;

      const todayIndicator = containerRef.current.querySelector('.gantt-today-line');
      if (!todayIndicator) return;

      const dayWidth = containerRef.current.querySelector('.gantt-day')?.clientWidth || 40;
      const daysSinceStart = Math.floor(
        (new Date().getTime() - startDate.getTime()) / (1000 * 60 * 60 * 24),
      );

      if (daysSinceStart >= 0 && daysSinceStart < daysToShow) {
        todayIndicator.setAttribute(
          'style',
          `left: calc(200px + ${daysSinceStart * dayWidth * zoomLevel}px); display: block;`,
        );
      } else {
        todayIndicator.setAttribute('style', 'display: none;');
      }
    };

    updateTodayIndicator();
    window.addEventListener('resize', updateTodayIndicator);
    return () => window.removeEventListener('resize', updateTodayIndicator);
  }, [startDate, daysToShow, zoomLevel]);

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
            showDependencies={showDependencies}
            setShowDependencies={setShowDependencies}
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
            <GanttRenderer
              tasks={tasks}
              filteredTasks={filteredTasks}
              groupedTasks={groupedTasks}
              timelineDates={timelineDates}
              timelineHeaders={timelineHeaders}
              viewMode={viewMode}
              startDate={startDate}
              daysToShow={daysToShow}
              zoomLevel={zoomLevel}
              showDependencies={showDependencies}
              draggedTask={draggedTask}
              handleTaskClick={handleTaskClick}
              handleDragStart={handleDragStart}
              toggleGroupExpansion={toggleGroupExpansion}
            />
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
