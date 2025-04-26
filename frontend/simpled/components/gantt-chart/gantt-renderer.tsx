'use client';

import type React from 'react';

import { cn } from '@/lib/utils';
import { format } from 'date-fns';
import { es } from 'date-fns/locale';
import { useEffect, useRef } from 'react';
import type { Task } from './index';

interface TimelineHeader {
  label: string;
  span: number;
  startDate: Date;
}

interface GroupedTask {
  id: string;
  title: string;
  isGroup: boolean;
  tasks: Task[];
  expanded: boolean;
}

interface GanttRendererProps {
  tasks: Task[];
  filteredTasks: Task[];
  groupedTasks: GroupedTask[];
  timelineDates: Date[];
  timelineHeaders: TimelineHeader[];
  viewMode: 'day' | 'week' | 'month';
  startDate: Date;
  daysToShow: number;
  zoomLevel: number;
  showDependencies: boolean;
  draggedTask: string | null;
  handleTaskClick: (task: Task) => void;
  handleDragStart: (
    e: React.MouseEvent,
    taskId: string,
    type: 'move' | 'resize-start' | 'resize-end',
  ) => void;
  toggleGroupExpansion: (groupId: string) => void;
}

export function GanttRenderer({
  tasks,
  filteredTasks,
  groupedTasks,
  timelineDates,
  timelineHeaders,
  viewMode,
  startDate,
  daysToShow,
  zoomLevel,
  showDependencies,
  draggedTask,
  handleTaskClick,
  handleDragStart,
  toggleGroupExpansion,
}: GanttRendererProps) {
  const svgRef = useRef<SVGSVGElement>(null);
  const containerRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (!showDependencies || !svgRef.current || !containerRef.current) return;

    while (svgRef.current.firstChild) {
      if (svgRef.current.firstChild.nodeName === 'defs') {
        break;
      }
      svgRef.current.removeChild(svgRef.current.firstChild);
    }

    const containerRect = containerRef.current.getBoundingClientRect();

    tasks.forEach((task) => {
      if (!task.dependencies || task.dependencies.length === 0) return;

      task.dependencies.forEach((depId) => {
        const sourceElement = document.getElementById(`task-${depId}`);
        const targetElement = document.getElementById(`task-${task.id}`);

        if (!sourceElement || !targetElement) return;

        const sourceRect = sourceElement.getBoundingClientRect();
        const targetRect = targetElement.getBoundingClientRect();

        const startX = sourceRect.right - containerRect.left;
        const startY = sourceRect.top + sourceRect.height / 2 - containerRect.top;
        const endX = targetRect.left - containerRect.left;
        const endY = targetRect.top + targetRect.height / 2 - containerRect.top;

        const controlPointX = (startX + endX) / 2;
        const path = document.createElementNS('http://www.w3.org/2000/svg', 'path');
        path.setAttribute(
          'd',
          `M ${startX} ${startY} C ${controlPointX} ${startY}, ${controlPointX} ${endY}, ${endX} ${endY}`,
        );
        path.setAttribute('stroke', '#888');
        path.setAttribute('stroke-width', '1.5');
        path.setAttribute('stroke-dasharray', '4');
        path.setAttribute('fill', 'none');
        path.setAttribute('marker-end', 'url(#arrowhead)');

        svgRef.current?.appendChild(path);
      });
    });
  }, [tasks, showDependencies, filteredTasks, groupedTasks, zoomLevel, startDate]);

  const renderTimelineHeader = () => {
    const headerElement = document.createElement('div');
    headerElement.className = 'gantt-timeline-header grid';
    headerElement.style.gridTemplateColumns = `200px repeat(${daysToShow}, minmax(${40 * zoomLevel}px, 1fr))`;

    const taskHeaderCell = document.createElement('div');
    taskHeaderCell.className = 'gantt-header-cell bg-muted/50 border-r border-b p-2';
    taskHeaderCell.textContent = 'Tarea';
    headerElement.appendChild(taskHeaderCell);

    if (viewMode === 'day') {
      timelineDates.forEach((date) => {
        const cell = document.createElement('div');
        cell.className = cn(
          'gantt-header-cell gantt-day border-r border-b p-2 text-center text-xs',
          format(date, 'yyyy-MM-dd') === format(new Date(), 'yyyy-MM-dd')
            ? 'bg-blue-100 dark:bg-blue-900/20'
            : 'bg-muted/50',
        );
        cell.textContent = format(date, 'EEE d', { locale: es });
        headerElement.appendChild(cell);
      });
    } else {
      timelineHeaders.forEach((header) => {
        const cell = document.createElement('div');
        cell.className = 'gantt-header-cell bg-muted/50 border-r border-b p-2 text-center text-xs';
        cell.style.gridColumn = `span ${header.span}`;
        cell.textContent = header.label;
        headerElement.appendChild(cell);
      });
    }

    return headerElement;
  };

  const renderTimelineSubheader = () => {
    if (viewMode === 'day') return null;

    const subheaderElement = document.createElement('div');
    subheaderElement.className = 'gantt-timeline-subheader grid';
    subheaderElement.style.gridTemplateColumns = `200px repeat(${daysToShow}, minmax(${40 * zoomLevel}px, 1fr))`;

    const emptyCell = document.createElement('div');
    emptyCell.className = 'gantt-header-cell bg-muted/30 border-r border-b p-2';
    subheaderElement.appendChild(emptyCell);

    timelineDates.forEach((date) => {
      const cell = document.createElement('div');
      cell.className = cn(
        'gantt-day border-r border-b p-1 text-center text-xs',
        format(date, 'yyyy-MM-dd') === format(new Date(), 'yyyy-MM-dd')
          ? 'bg-blue-100 dark:bg-blue-900/20'
          : 'bg-muted/30',
      );
      cell.textContent = format(date, 'd', { locale: es });
      subheaderElement.appendChild(cell);
    });

    return subheaderElement;
  };

  const getStatusColor = (status?: string, progress?: number) => {
    if (progress === 100 || status === 'completed') return 'bg-emerald-500 hover:bg-emerald-600';
    if (status === 'delayed') return 'bg-rose-500 hover:bg-rose-600';
    if (status === 'in-progress') return 'bg-blue-500 hover:bg-blue-600';
    return 'bg-amber-500 hover:bg-amber-600';
  };

  const getTaskStyle = (task: Task) => {
    const taskStart = new Date(task.startDate);
    const taskEnd = new Date(task.endDate);

    const startDiff = Math.max(
      0,
      Math.floor((taskStart.getTime() - startDate.getTime()) / (1000 * 60 * 60 * 24)),
    );
    const duration = Math.max(
      1,
      Math.floor((taskEnd.getTime() - taskStart.getTime()) / (1000 * 60 * 60 * 24)) + 1,
    );

    const visibleDuration = Math.min(duration, daysToShow - startDiff);

    return {
      gridColumnStart: startDiff + 1,
      gridColumnEnd: `span ${visibleDuration}`,
      display: startDiff < daysToShow && startDiff + duration > 0 ? 'flex' : 'none',
    };
  };

  const renderTaskRow = (task: Task, isInGroup = false) => {
    const rowElement = document.createElement('div');
    rowElement.className = 'gantt-row relative grid';
    rowElement.style.gridTemplateColumns = `200px repeat(${daysToShow}, minmax(${40 * zoomLevel}px, 1fr))`;

    const taskInfoCell = document.createElement('div');
    taskInfoCell.className = cn(
      'gantt-task-info truncate border-r border-b p-2',
      isInGroup && 'pl-6',
    );

    const taskTitle = document.createElement('div');
    taskTitle.className = 'truncate font-medium';
    taskTitle.textContent = task.title;
    taskInfoCell.appendChild(taskTitle);

    rowElement.appendChild(taskInfoCell);

    for (let i = 0; i < daysToShow; i++) {
      const cell = document.createElement('div');
      cell.className = cn(
        'gantt-cell border-r border-b',
        format(new Date(startDate.getTime() + i * 24 * 60 * 60 * 1000), 'yyyy-MM-dd') ===
          format(new Date(), 'yyyy-MM-dd')
          ? 'bg-blue-100 dark:bg-blue-900/20'
          : '',
      );
      rowElement.appendChild(cell);
    }

    const taskStyle = getTaskStyle(task);
    if (taskStyle.display !== 'none') {
      const taskBar = document.createElement('div');
      taskBar.id = `task-${task.id}`;
      taskBar.className = cn(
        'gantt-task-bar absolute flex cursor-pointer items-center rounded-md px-2 text-xs text-white',
        getStatusColor(task.status, task.progress),
      );
      taskBar.style.gridColumnStart = String(taskStyle.gridColumnStart);
      taskBar.style.gridColumnEnd = String(taskStyle.gridColumnEnd);
      taskBar.style.top = '4px';
      taskBar.style.height = 'calc(100% - 8px)';

      const leftHandle = document.createElement('div');
      leftHandle.className =
        'gantt-task-handle left absolute top-0 bottom-0 left-0 w-2 cursor-ew-resize rounded-l-md';
      leftHandle.addEventListener('mousedown', (e) =>
        handleDragStart(e as any, task.id, 'resize-start'),
      );
      taskBar.appendChild(leftHandle);

      const taskContent = document.createElement('div');
      taskContent.className = 'gantt-task-content flex-1 truncate';
      taskContent.addEventListener('mousedown', (e) => handleDragStart(e as any, task.id, 'move'));

      const gripIcon = document.createElementNS('http://www.w3.org/2000/svg', 'svg');
      gripIcon.setAttribute('class', 'mr-1 inline h-3 w-3 opacity-70');
      gripIcon.setAttribute('viewBox', '0 0 24 24');
      gripIcon.setAttribute('fill', 'none');
      gripIcon.setAttribute('stroke', 'currentColor');
      gripIcon.setAttribute('stroke-width', '2');
      gripIcon.setAttribute('stroke-linecap', 'round');
      gripIcon.setAttribute('stroke-linejoin', 'round');

      const path1 = document.createElementNS('http://www.w3.org/2000/svg', 'path');
      path1.setAttribute('d', 'M9 5v14');
      gripIcon.appendChild(path1);

      const path2 = document.createElementNS('http://www.w3.org/2000/svg', 'path');
      path2.setAttribute('d', 'M15 5v14');
      gripIcon.appendChild(path2);

      taskContent.appendChild(gripIcon);
      taskContent.appendChild(document.createTextNode(task.title));
      taskBar.appendChild(taskContent);

      const rightHandle = document.createElement('div');
      rightHandle.className =
        'gantt-task-handle right absolute top-0 right-0 bottom-0 w-2 cursor-ew-resize rounded-r-md';
      rightHandle.addEventListener('mousedown', (e) =>
        handleDragStart(e as any, task.id, 'resize-end'),
      );
      taskBar.appendChild(rightHandle);

      if (task.progress > 0) {
        const progressBar = document.createElement('div');
        progressBar.className = 'absolute top-0 bottom-0 left-0 rounded-l-md bg-white/20';
        progressBar.style.width = `${task.progress}%`;
        progressBar.style.maxWidth = '100%';
        taskBar.appendChild(progressBar);
      }

      taskBar.addEventListener('click', () => handleTaskClick(task));

      rowElement.appendChild(taskBar);
    }

    return rowElement;
  };

  const renderGroupRow = (group: GroupedTask) => {
    const rowElement = document.createElement('div');
    rowElement.className = 'gantt-row relative grid bg-muted/30';
    rowElement.style.gridTemplateColumns = `200px repeat(${daysToShow}, minmax(${40 * zoomLevel}px, 1fr))`;

    const groupInfoCell = document.createElement('div');
    groupInfoCell.className =
      'gantt-task-info truncate border-r border-b p-2 font-medium cursor-pointer';
    groupInfoCell.addEventListener('click', () => toggleGroupExpansion(group.id));

    const groupTitle = document.createElement('div');
    groupTitle.className = 'flex items-center';

    const chevronIcon = document.createElementNS('http://www.w3.org/2000/svg', 'svg');
    chevronIcon.setAttribute('class', 'h-4 w-4 mr-1');
    chevronIcon.setAttribute('viewBox', '0 0 24 24');
    chevronIcon.setAttribute('fill', 'none');
    chevronIcon.setAttribute('stroke', 'currentColor');
    chevronIcon.setAttribute('stroke-width', '2');
    chevronIcon.setAttribute('stroke-linecap', 'round');
    chevronIcon.setAttribute('stroke-linejoin', 'round');

    if (group.expanded) {
      const path = document.createElementNS('http://www.w3.org/2000/svg', 'path');
      path.setAttribute('d', 'M6 9l6 6 6-6');
      chevronIcon.appendChild(path);
    } else {
      const path = document.createElementNS('http://www.w3.org/2000/svg', 'path');
      path.setAttribute('d', 'M9 18l6-6-6-6');
      chevronIcon.appendChild(path);
    }

    groupTitle.appendChild(chevronIcon);
    groupTitle.appendChild(document.createTextNode(`${group.title} (${group.tasks.length})`));

    groupInfoCell.appendChild(groupTitle);
    rowElement.appendChild(groupInfoCell);

    for (let i = 0; i < daysToShow; i++) {
      const cell = document.createElement('div');
      cell.className = cn(
        'gantt-cell border-r border-b',
        format(new Date(startDate.getTime() + i * 24 * 60 * 60 * 1000), 'yyyy-MM-dd') ===
          format(new Date(), 'yyyy-MM-dd')
          ? 'bg-blue-100 dark:bg-blue-900/20'
          : '',
      );
      rowElement.appendChild(cell);
    }

    return rowElement;
  };

  const renderGanttBody = () => {
    const bodyElement = document.createElement('div');
    bodyElement.className = 'gantt-body';

    if (groupedTasks.length === 0) {
      const emptyMessage = document.createElement('div');
      emptyMessage.className = 'col-span-full border-b py-8 text-center';
      emptyMessage.textContent = 'No hay tareas para mostrar';
      bodyElement.appendChild(emptyMessage);
    } else {
      groupedTasks.forEach((group) => {
        if (group.isGroup) {
          bodyElement.appendChild(renderGroupRow(group));

          if (group.expanded) {
            group.tasks.forEach((task) => {
              bodyElement.appendChild(renderTaskRow(task, true));
            });
          }
        } else {
          bodyElement.appendChild(renderTaskRow(group.tasks[0]));
        }
      });
    }

    const todayIndicator = document.createElement('div');
    todayIndicator.className = 'gantt-today-line';

    const todayLabel = document.createElement('div');
    todayLabel.className = 'gantt-today-label';
    todayLabel.textContent = 'Hoy';

    todayIndicator.appendChild(todayLabel);
    bodyElement.appendChild(todayIndicator);

    return bodyElement;
  };

  const renderGanttChart = () => {
    const chartElement = document.createElement('div');
    chartElement.className = 'gantt-chart min-w-[800px]';

    chartElement.appendChild(renderTimelineHeader());

    if (viewMode !== 'day') {
      const subheader = renderTimelineSubheader();
      if (subheader) {
        chartElement.appendChild(subheader);
      }
    }

    chartElement.appendChild(renderGanttBody());

    return chartElement;
  };

  useEffect(() => {
    if (!containerRef.current) return;

    while (containerRef.current.firstChild) {
      containerRef.current.removeChild(containerRef.current.firstChild);
    }

    const ganttChart = renderGanttChart();
    containerRef.current.appendChild(ganttChart);
  }, [
    tasks,
    filteredTasks,
    groupedTasks,
    timelineDates,
    timelineHeaders,
    viewMode,
    startDate,
    daysToShow,
    zoomLevel,
  ]);

  return (
    <div className="gantt-chart-container" ref={containerRef}>
      <svg
        ref={svgRef}
        className="gantt-dependencies"
        style={{
          position: 'absolute',
          top: 0,
          left: 0,
          width: '100%',
          height: '100%',
          pointerEvents: 'none',
          zIndex: 5,
        }}
      >
        <defs>
          <marker id="arrowhead" markerWidth="6" markerHeight="6" refX="5" refY="3" orient="auto">
            <path d="M 0 0 L 6 3 L 0 6 Z" fill="#888" />
          </marker>
        </defs>
      </svg>
    </div>
  );
}
