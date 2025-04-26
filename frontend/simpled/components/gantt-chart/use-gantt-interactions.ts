'use client';

import type React from 'react';

import { useCallback, useState } from 'react';
import type { Task } from './index';

export function useGanttInteractions(
  tasks: Task[],
  updateTaskDates: (taskId: string, newStartDate: Date, newEndDate: Date) => Promise<boolean>,
) {
  const [selectedTask, setSelectedTask] = useState<Task | null>(null);
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [draggedTask, setDraggedTask] = useState<string | null>(null);
  const [dragStartX, setDragStartX] = useState(0);
  const [dragType, setDragType] = useState<'move' | 'resize-start' | 'resize-end' | null>(null);
  const [originalDates, setOriginalDates] = useState<{
    start: Date;
    end: Date;
  } | null>(null);

  const handleTaskClick = useCallback((task: Task) => {
    setSelectedTask(task);
    setIsDialogOpen(true);
  }, []);

  const handleDragStart = useCallback(
    (e: React.MouseEvent, taskId: string, type: 'move' | 'resize-start' | 'resize-end') => {
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
    },
    [tasks],
  );

  const handleDragMove = useCallback(
    (e: MouseEvent) => {
      if (!draggedTask || !originalDates || !dragType) return;

      const dayWidth = document.querySelector('.gantt-day')?.clientWidth ?? 40;
      const daysDiff = Math.round((e.clientX - dragStartX) / dayWidth);

      if (daysDiff === 0) return;

      const task = tasks.find((t) => t.id === draggedTask);
      if (!task) return;

      const taskElement = document.getElementById(`task-${draggedTask}`);
      if (!taskElement) return;

      if (dragType === 'move') {
        const newStartDate = new Date(
          originalDates.start.getTime() + daysDiff * 24 * 60 * 60 * 1000,
        );
        const newEndDate = new Date(originalDates.end.getTime() + daysDiff * 24 * 60 * 60 * 1000);

        const currentGridColumn = taskElement.style.gridColumnStart;
        const currentSpan = taskElement.style.gridColumnEnd.replace('span ', '');

        taskElement.style.gridColumnStart = String(Number.parseInt(currentGridColumn) + daysDiff);
        taskElement.style.gridColumnEnd = `span ${currentSpan}`;
      } else if (dragType === 'resize-start') {
        const newStartDate = new Date(
          originalDates.start.getTime() + daysDiff * 24 * 60 * 60 * 1000,
        );
        if (newStartDate < new Date(task.endDate)) {
          const currentGridColumn = Number.parseInt(taskElement.style.gridColumnStart);
          const currentSpan = Number.parseInt(taskElement.style.gridColumnEnd.replace('span ', ''));

          taskElement.style.gridColumnStart = String(currentGridColumn + daysDiff);
          taskElement.style.gridColumnEnd = `span ${currentSpan - daysDiff}`;
        }
      } else if (dragType === 'resize-end') {
        const newEndDate = new Date(originalDates.end.getTime() + daysDiff * 24 * 60 * 60 * 1000);
        if (newEndDate > new Date(task.startDate)) {
          const currentSpan = Number.parseInt(taskElement.style.gridColumnEnd.replace('span ', ''));

          taskElement.style.gridColumnEnd = `span ${currentSpan + daysDiff}`;
        }
      }
    },
    [draggedTask, dragStartX, dragType, originalDates, tasks],
  );

  const handleDragEnd = useCallback(() => {
    if (draggedTask && originalDates && dragType) {
      const task = tasks.find((t) => t.id === draggedTask);
      if (task) {
        let newStartDate = new Date(task.startDate);
        let newEndDate = new Date(task.endDate);

        const taskElement = document.getElementById(`task-${draggedTask}`);
        if (taskElement) {
          const dayWidth = document.querySelector('.gantt-day')?.clientWidth ?? 40;
          const daysDiff = Math.round(
            (Number.parseInt(taskElement.style.gridColumnStart) - Number.parseInt(task.startDate)) /
              dayWidth,
          );

          if (dragType === 'move') {
            newStartDate = new Date(originalDates.start.getTime() + daysDiff * 24 * 60 * 60 * 1000);
            newEndDate = new Date(originalDates.end.getTime() + daysDiff * 24 * 60 * 60 * 1000);
          } else if (dragType === 'resize-start') {
            newStartDate = new Date(originalDates.start.getTime() + daysDiff * 24 * 60 * 60 * 1000);
          } else if (dragType === 'resize-end') {
            const spanDiff =
              Number.parseInt(taskElement.style.gridColumnEnd.replace('span ', '')) -
              (new Date(task.endDate).getTime() - new Date(task.startDate).getTime()) /
                (24 * 60 * 60 * 1000);
            newEndDate = new Date(originalDates.end.getTime() + spanDiff * 24 * 60 * 60 * 1000);
          }

          updateTaskDates(draggedTask, newStartDate, newEndDate);
        }
      }
    }

    setDraggedTask(null);
    setDragStartX(0);
    setDragType(null);
    setOriginalDates(null);

    document.removeEventListener('mousemove', handleDragMove);
    document.removeEventListener('mouseup', handleDragEnd);
  }, [draggedTask, dragType, originalDates, tasks, updateTaskDates]);

  return {
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
  };
}
