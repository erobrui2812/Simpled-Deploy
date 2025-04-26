'use client';

import { addDays } from 'date-fns';
import { useEffect, useState } from 'react';
import type { Task } from './index';

const API_URL = 'http://localhost:5193';

export function useGanttData(boardId: string, auth: { token: string | null; id: string | null }) {
  const [tasks, setTasks] = useState<Task[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

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

      const itemsResponse = await fetch(`${API_URL}/api/Items`, { headers });
      if (!itemsResponse.ok) throw new Error('Error fetching tasks');

      const items = await itemsResponse.json();

      const columnsResponse = await fetch(`${API_URL}/api/Columns`, {
        headers,
      });
      if (!columnsResponse.ok) throw new Error('Error fetching columns');

      const columns = await columnsResponse.json();
      const boardColumns = columns.filter((col: any) => col.boardId === boardId);
      const boardColumnIds = boardColumns.map((col: any) => col.id);

      const boardTasks = items.filter((item: any) => boardColumnIds.includes(item.columnId));

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
        dependencies: item.dependencies ?? [],
        assignedTo: item.assignedTo,
      }));

      setTasks(ganttTasks);
    } catch (err) {
      console.error('Error fetching tasks:', err);
      setError('Error al cargar las tareas. Por favor, inténtalo de nuevo.');
    } finally {
      setLoading(false);
    }
  };

  const updateTask = async (updatedTask: Task) => {
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
          dependencies: updatedTask.dependencies,
          progress: updatedTask.progress,
          status: updatedTask.status,
          assignedTo: updatedTask.assignedTo,
        }),
      });

      if (!response.ok) throw new Error('Error updating task');

      setTasks(tasks.map((t) => (t.id === updatedTask.id ? updatedTask : t)));
      return true;
    } catch (err) {
      console.error('Error updating task:', err);
      setError('Error al actualizar la tarea. Por favor, inténtalo de nuevo.');
      return false;
    }
  };

  const updateTaskDates = async (taskId: string, newStartDate: Date, newEndDate: Date) => {
    try {
      const headers: HeadersInit = {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${auth.token}`,
      };

      const task = tasks.find((t) => t.id === taskId);
      if (!task) return false;

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
          dependencies: task.dependencies,
        }),
      });

      if (!response.ok) throw new Error('Error updating task');

      setTasks(tasks.map((t) => (t.id === taskId ? updatedTask : t)));
      return true;
    } catch (err) {
      console.error('Error updating task dates:', err);
      setError('Error al actualizar las fechas de la tarea. Por favor, inténtalo de nuevo.');
      return false;
    }
  };

  useEffect(() => {
    if (boardId) {
      fetchTasks();
    }
  }, [boardId]);

  return {
    tasks,
    loading,
    error,
    fetchTasks,
    updateTask,
    updateTaskDates,
  };
}
