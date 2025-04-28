'use client';

import { cn } from '@/lib/utils';
import { useDraggable } from '@dnd-kit/core';
import { format } from 'date-fns';
import { es } from 'date-fns/locale';
import { ChevronDown, ChevronRight, Grip } from 'lucide-react';
import { useEffect, useState } from 'react';
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

interface GanttViewProps {
  readonly tasks: Task[];
  readonly filteredTasks: Task[];
  readonly groupedTasks: GroupedTask[];
  readonly timelineDates: Date[];
  readonly timelineHeaders: TimelineHeader[];
  readonly viewMode: 'day' | 'week' | 'month';
  readonly startDate: Date;
  readonly daysToShow: number;
  readonly zoomLevel: number;
  readonly onTaskClick: (task: Task) => void;
  readonly toggleGroupExpansion: (groupId: string) => void;
}

export function GanttView({
  tasks,
  filteredTasks,
  groupedTasks,
  timelineDates,
  timelineHeaders,
  viewMode,
  startDate,
  daysToShow,
  zoomLevel,
  onTaskClick,
  toggleGroupExpansion,
}: GanttViewProps) {
  const [todayPosition, setTodayPosition] = useState<number | null>(null);

  useEffect(() => {
    const today = new Date();
    const daysSinceStart = Math.floor(
      (today.getTime() - startDate.getTime()) / (1000 * 60 * 60 * 24),
    );

    if (daysSinceStart >= 0 && daysSinceStart < daysToShow) {
      setTodayPosition(daysSinceStart);
    } else {
      setTodayPosition(null);
    }
  }, [startDate, daysToShow]);

  return (
    <div className="gantt-chart min-w-[800px]">
      <div
        className="gantt-timeline-header grid"
        style={{
          gridTemplateColumns: `200px repeat(${daysToShow}, minmax(${40 * zoomLevel}px, 1fr))`,
        }}
      >
        <div className="gantt-header-cell bg-muted/50 border-r border-b p-2">Tarea</div>

        {viewMode === 'day'
          ? timelineDates.map((date, index) => (
              <div
                key={index}
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
          : timelineHeaders.map((header, index) => (
              <div
                key={index}
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
            gridTemplateColumns: `200px repeat(${daysToShow}, minmax(${40 * zoomLevel}px, 1fr))`,
          }}
        >
          <div className="gantt-header-cell bg-muted/30 border-r border-b p-2"></div>

          {timelineDates.map((date, index) => (
            <div
              key={index}
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

      <div className="gantt-body relative">
        {groupedTasks.length === 0 ? (
          <div className="col-span-full border-b py-8 text-center">No hay tareas para mostrar</div>
        ) : (
          groupedTasks.map((group) => (
            <div key={group.id}>
              {group.isGroup ? (
                <>
                  <GroupRow
                    group={group}
                    daysToShow={daysToShow}
                    zoomLevel={zoomLevel}
                    toggleGroupExpansion={toggleGroupExpansion}
                    timelineDates={timelineDates}
                    startDate={startDate}
                  />

                  {group.expanded &&
                    group.tasks.map((task) => (
                      <TaskRow
                        key={task.id}
                        task={task}
                        daysToShow={daysToShow}
                        zoomLevel={zoomLevel}
                        startDate={startDate}
                        timelineDates={timelineDates}
                        onTaskClick={onTaskClick}
                        isInGroup={true}
                      />
                    ))}
                </>
              ) : (
                <TaskRow
                  key={group.tasks[0].id}
                  task={group.tasks[0]}
                  daysToShow={daysToShow}
                  zoomLevel={zoomLevel}
                  startDate={startDate}
                  timelineDates={timelineDates}
                  onTaskClick={onTaskClick}
                  isInGroup={false}
                />
              )}
            </div>
          ))
        )}

        {todayPosition !== null && (
          <div
            className="gantt-today-line pointer-events-none absolute top-0 bottom-0 z-10 w-[2px] bg-red-500/80"
            style={{
              left: `calc(200px + ${todayPosition * 40 * zoomLevel}px)`,
              height: '100%',
            }}
          >
            <div className="gantt-today-label absolute -top-6 left-1/2 -translate-x-1/2 transform rounded bg-red-500 px-2 py-1 text-xs whitespace-nowrap text-white">
              Hoy
            </div>
          </div>
        )}
      </div>
    </div>
  );
}

interface GroupRowProps {
  group: GroupedTask;
  daysToShow: number;
  zoomLevel: number;
  toggleGroupExpansion: (groupId: string) => void;
  timelineDates: Date[];
  startDate: Date;
}

function GroupRow({
  group,
  daysToShow,
  zoomLevel,
  toggleGroupExpansion,
  timelineDates,
  startDate,
}: GroupRowProps) {
  return (
    <div
      className="gantt-row bg-muted/30 relative grid"
      style={{
        gridTemplateColumns: `200px repeat(${daysToShow}, minmax(${40 * zoomLevel}px, 1fr))`,
      }}
    >
      <div
        className="gantt-task-info cursor-pointer truncate border-r border-b p-2 font-medium"
        onClick={() => toggleGroupExpansion(group.id)}
      >
        <div className="flex items-center">
          {group.expanded ? (
            <ChevronDown className="mr-1 h-4 w-4" />
          ) : (
            <ChevronRight className="mr-1 h-4 w-4" />
          )}
          <span>{`${group.title} (${group.tasks.length})`}</span>
        </div>
      </div>

      {timelineDates.map((date, index) => (
        <div
          key={index}
          className={cn(
            'gantt-cell border-r border-b',
            format(date, 'yyyy-MM-dd') === format(new Date(), 'yyyy-MM-dd')
              ? 'bg-blue-100 dark:bg-blue-900/20'
              : '',
          )}
        />
      ))}
    </div>
  );
}

interface TaskRowProps {
  task: Task;
  daysToShow: number;
  zoomLevel: number;
  startDate: Date;
  timelineDates: Date[];
  onTaskClick: (task: Task) => void;
  isInGroup: boolean;
}

function TaskRow({
  task,
  daysToShow,
  zoomLevel,
  startDate,
  timelineDates,
  onTaskClick,
  isInGroup,
}: TaskRowProps) {
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

  const isVisible = startDiff < daysToShow && startDiff + duration > 0;

  const { attributes, listeners, setNodeRef, transform, isDragging } = useDraggable({
    id: task.id,
    data: {
      type: 'task',
      startDate: taskStart,
      endDate: taskEnd,
      task,
    },
  });

  const style = transform
    ? {
        transform: `translate3d(${transform.x}px, ${transform.y}px, 0)`,
      }
    : undefined;

  const getStatusColor = (status?: string, progress?: number) => {
    if (progress === 100 || status === 'completed') return 'bg-emerald-500 hover:bg-emerald-600';
    if (status === 'delayed') return 'bg-rose-500 hover:bg-rose-600';
    if (status === 'in-progress') return 'bg-blue-500 hover:bg-blue-600';
    return 'bg-amber-500 hover:bg-amber-600';
  };

  return (
    <div
      className="gantt-row relative grid"
      style={{
        gridTemplateColumns: `200px repeat(${daysToShow}, minmax(${40 * zoomLevel}px, 1fr))`,
      }}
    >
      <div className={cn('gantt-task-info truncate border-r border-b p-2', isInGroup && 'pl-6')}>
        <div className="truncate font-medium">{task.title}</div>
      </div>

      {timelineDates.map((date, index) => (
        <div
          key={index}
          className={cn(
            'gantt-cell border-r border-b',
            format(date, 'yyyy-MM-dd') === format(new Date(), 'yyyy-MM-dd')
              ? 'bg-blue-100 dark:bg-blue-900/20'
              : '',
          )}
        />
      ))}

      {isVisible && (
        <div
          ref={setNodeRef}
          {...attributes}
          {...listeners}
          className={cn(
            'gantt-task-bar absolute flex cursor-pointer items-center rounded-md px-2 text-xs text-white',
            getStatusColor(task.status, task.progress),
            isDragging && 'z-10 opacity-70',
          )}
          style={{
            gridColumnStart: startDiff + 2,
            gridColumnEnd: `span ${visibleDuration}`,
            top: '4px',
            height: 'calc(100% - 8px)',
            ...style,
          }}
          onClick={() => onTaskClick(task)}
        >
          <div className="gantt-task-content flex-1 truncate">
            <Grip className="mr-1 inline h-3 w-3 opacity-70" />
            {task.title}
          </div>

          {task.progress > 0 && (
            <div
              className="absolute top-0 bottom-0 left-0 rounded-l-md bg-white/20"
              style={{ width: `${task.progress}%`, maxWidth: '100%' }}
            />
          )}
        </div>
      )}
    </div>
  );
}
