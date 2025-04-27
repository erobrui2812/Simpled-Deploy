'use client';

import { useCallback, useMemo, useState } from 'react';
import type { Task } from './index';

interface GroupedTask {
  id: string;
  title: string;
  isGroup: boolean;
  tasks: Task[];
  expanded: boolean;
}

export function useGanttFilters(tasks: Task[]) {
  const [showCompleted, setShowCompleted] = useState(true);
  const [filterStatus, setFilterStatus] = useState<string | null>(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [groupBy, setGroupBy] = useState<'none' | 'status' | 'assignee'>('none');
  const [zoomLevel, setZoomLevel] = useState(1);
  const [groupExpansionState, setGroupExpansionState] = useState<Record<string, boolean>>({});

  const filteredTasks = useMemo(() => {
    let filtered = tasks;

    if (!showCompleted) {
      filtered = filtered.filter((task) => task.progress < 100 && task.status !== 'completed');
    }

    if (filterStatus) {
      filtered = filtered.filter((task) => task.status === filterStatus);
    }

    if (searchTerm) {
      const term = searchTerm.toLowerCase();
      filtered = filtered.filter(
        (task) =>
          task.title.toLowerCase().includes(term) ||
          (task.description && task.description.toLowerCase().includes(term)),
      );
    }

    return filtered;
  }, [tasks, showCompleted, filterStatus, searchTerm]);

  const getStatusLabel = useCallback((status: string) => {
    switch (status) {
      case 'completed':
        return 'Completada';
      case 'delayed':
        return 'Retrasada';
      case 'in-progress':
        return 'En progreso';
      default:
        return 'Pendiente';
    }
  }, []);

  const groupedTasks = useMemo(() => {
    if (groupBy === 'none') {
      return filteredTasks.map((task) => ({
        id: task.id,
        title: task.title,
        isGroup: false,
        tasks: [task],
        expanded: true,
      }));
    }

    const groups: Record<string, Task[]> = {};

    filteredTasks.forEach((task) => {
      let groupKey = 'Unknown';

      if (groupBy === 'status') {
        groupKey = task.status || 'Unknown';
      } else if (groupBy === 'assignee') {
        groupKey = task.assignedTo || 'Unassigned';
      }

      if (!groups[groupKey]) {
        groups[groupKey] = [];
      }

      groups[groupKey].push(task);
    });

    const result: GroupedTask[] = Object.entries(groups).map(([key, tasks]) => ({
      id: `group-${key}`,
      title: key === 'Unknown' ? 'Unknown' : groupBy === 'status' ? getStatusLabel(key) : key,
      isGroup: true,
      tasks,
      expanded: groupExpansionState[`group-${key}`] !== false,
    }));

    return result;
  }, [filteredTasks, groupBy, getStatusLabel, groupExpansionState]);

  const toggleGroupExpansion = useCallback((groupId: string) => {
    setGroupExpansionState((prev) => ({
      ...prev,
      [groupId]: !prev[groupId],
    }));
  }, []);

  return {
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
  };
}
