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

  // Filter tasks based on criteria
  const filteredTasks = useMemo(() => {
    let filtered = tasks;

    // Filter by completion status
    if (!showCompleted) {
      filtered = filtered.filter((task) => task.progress < 100 && task.status !== 'completed');
    }

    // Filter by status
    if (filterStatus) {
      filtered = filtered.filter((task) => task.status === filterStatus);
    }

    // Filter by search term
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

  // Get status label for display
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

  // Group tasks based on selected grouping
  const groupedTasks = useMemo(() => {
    if (groupBy === 'none') {
      // No grouping, just set tasks directly
      return filteredTasks.map((task) => ({
        id: task.id,
        title: task.title,
        isGroup: false,
        tasks: [task],
        expanded: true,
      }));
    }

    const groups: Record<string, Task[]> = {};

    // Group tasks
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

    // Convert groups to GroupedTask array
    const result: GroupedTask[] = Object.entries(groups).map(([key, tasks]) => ({
      id: `group-${key}`,
      title: key === 'Unknown' ? 'Unknown' : groupBy === 'status' ? getStatusLabel(key) : key,
      isGroup: true,
      tasks,
      expanded: groupExpansionState[`group-${key}`] !== false, // Default to expanded
    }));

    return result;
  }, [filteredTasks, groupBy, getStatusLabel, groupExpansionState]);

  // Toggle group expansion
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
