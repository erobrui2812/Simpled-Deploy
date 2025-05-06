'use client';

import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from '@/components/ui/tooltip';
import { cn } from '@/lib/utils';
import { useDraggable } from '@dnd-kit/core';
import { format } from 'date-fns';
import { es } from 'date-fns/locale';
import { ChevronDown, ChevronRight, Grip, Link2 } from 'lucide-react';
import type React from 'react';
import { useEffect, useMemo, useState } from 'react';
import type { Dependency, Task } from './index';

interface TimelineHeader {
  readonly label: string;
  readonly span: number;
  readonly startDate: Date;
}

interface GroupedTask {
  readonly id: string;
  readonly title: string;
  readonly isGroup: boolean;
  readonly tasks: Task[];
  readonly expanded: boolean;
}

interface GanttViewProps {
  readonly tasks: Task[];
  readonly dependencies: Dependency[];
  readonly filteredTasks: Task[];
  readonly groupedTasks: GroupedTask[];
  readonly timelineDates: Date[];
  readonly timelineHeaders: TimelineHeader[];
  readonly viewMode: 'day' | 'week' | 'month';
  readonly startDate: Date;
  readonly daysToShow: number;
  readonly zoomLevel: number;
  readonly onTaskClick: (task: Task) => void;
  readonly onTimelineClick: (e: React.MouseEvent, date: Date) => void;
  readonly onManageDependencies: (task: Task) => void;
  readonly toggleGroupExpansion: (groupId: string) => void;
}

export function GanttView({
  tasks,
  dependencies,
  groupedTasks,
  timelineDates,
  timelineHeaders,
  viewMode,
  startDate,
  daysToShow,
  zoomLevel,
  onTaskClick,
  onTimelineClick,
  onManageDependencies,
  toggleGroupExpansion,
}: GanttViewProps) {
  const [todayPosition, setTodayPosition] = useState<number | null>(null);
  const [dependencyLines, setDependencyLines] = useState<
    Array<{
      id: string;
      fromX: number;
      fromY: number;
      toX: number;
      toY: number;
      type: Dependency['type'];
    }>
  >([]);

  // Formateo de la fecha de hoy para resaltar la columna correspondiente
  const todayFormatted = useMemo(() => format(new Date(), 'yyyy-MM-dd'), []);

  // Cálculo de la posición de la línea de "Hoy" en el timeline
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

  // Efecto para calcular las líneas de dependencia entre tareas
  // Nota: este cálculo es simplificado y asume posiciones fijas para los elementos
  useEffect(() => {
    const lines: typeof dependencyLines = [];
    const dayWidth = 40 * zoomLevel;

    dependencies.forEach((dep) => {
      const fromTask = tasks.find((t) => t.id === dep.fromTaskId);
      const toTask = tasks.find((t) => t.id === dep.toTaskId);
      if (!fromTask || !toTask) return;

      // Determinar índices de fila según grupos y expansión
      let fromIndex = -1;
      let toIndex = -1;
      let fromVisible = true;
      let toVisible = true;

      groupedTasks.forEach((group, groupIdx) => {
        if (group.isGroup) {
          const inFromGroup = group.tasks.some((t) => t.id === fromTask.id);
          const inToGroup = group.tasks.some((t) => t.id === toTask.id);
          if (!group.expanded) {
            if (inFromGroup) fromVisible = false;
            if (inToGroup) toVisible = false;
          }
          group.tasks.forEach((task, taskIdx) => {
            if (task.id === fromTask.id) fromIndex = groupIdx + (group.expanded ? taskIdx + 1 : 0);
            if (task.id === toTask.id) toIndex = groupIdx + (group.expanded ? taskIdx + 1 : 0);
          });
        } else {
          if (group.tasks[0].id === fromTask.id) fromIndex = groupIdx;
          if (group.tasks[0].id === toTask.id) toIndex = groupIdx;
        }
      });
      if (fromIndex === -1 || toIndex === -1 || !fromVisible || !toVisible) return;

      // Calcular días relativos al inicio visible
      const toDays = (dateStr: string) =>
        Math.max(
          0,
          Math.floor((new Date(dateStr).getTime() - startDate.getTime()) / (1000 * 60 * 60 * 24)),
        );

      const fromStart = toDays(fromTask.startDate);
      const fromEnd = toDays(fromTask.endDate);
      const toStart = toDays(toTask.startDate);

      // Coordenadas X (en píxeles) para inicio y fin según tipo de dependencia
      const fromX =
        200 +
        (dep.type === 'finish-to-start' || dep.type === 'finish-to-finish'
          ? fromEnd * dayWidth
          : fromStart * dayWidth);
      // Evitamos condicional redundante: las otras opciones también usan start
      const toX = 200 + toStart * dayWidth;

      // Coordenadas Y basadas en el índice de fila
      const rowHeight = 40;
      const fromY = (fromIndex + 1) * rowHeight + rowHeight / 2;
      const toY = (toIndex + 1) * rowHeight + rowHeight / 2;

      lines.push({ id: dep.id, fromX, fromY, toX, toY, type: dep.type });
    });

    setDependencyLines(lines);
  }, [dependencies, tasks, groupedTasks, startDate, zoomLevel, daysToShow]);

  // Manejo de clic en el timeline para detectar la fecha seleccionada
  const handleTimelineClick = (e: React.MouseEvent) => {
    const rect = e.currentTarget.getBoundingClientRect();
    const x = e.clientX - rect.left;
    if (x <= 200) return; // columna fija de nombres

    const dayWidth = 40 * zoomLevel;
    const dayIndex = Math.floor((x - 200) / dayWidth);
    if (dayIndex >= 0 && dayIndex < timelineDates.length) {
      onTimelineClick(e, timelineDates[dayIndex]);
    }
  };

  return (
    <TooltipProvider>
      <div className="gantt-chart min-w-[800px]">
        {/* Cabecera principal con días o periodos */}
        <div
          className="gantt-timeline-header grid"
          style={{
            gridTemplateColumns: `200px repeat(${daysToShow}, minmax(${40 * zoomLevel}px, 1fr))`,
          }}
        >
          <div className="gantt-header-cell bg-muted/50 border-r border-b p-2">Tarea</div>
          {viewMode === 'day'
            ? timelineDates.map((date) => {
                const formatted = format(date, 'yyyy-MM-dd');
                return (
                  <div
                    key={formatted}
                    className={cn(
                      'gantt-header-cell gantt-day border-r border-b p-2 text-center text-xs',
                      formatted === todayFormatted
                        ? 'bg-blue-100 dark:bg-blue-900/20'
                        : 'bg-muted/50',
                    )}
                  >
                    {format(date, 'EEE d', { locale: es })}
                  </div>
                );
              })
            : timelineHeaders.map((header) => (
                <div
                  key={header.label}
                  className="gantt-header-cell bg-muted/50 border-r border-b p-2 text-center text-xs"
                  style={{ gridColumn: `span ${header.span}` }}
                >
                  {header.label}
                </div>
              ))}
        </div>

        {/* Subcabecera para mes o semana cuando no es vista "day" */}
        {viewMode !== 'day' && (
          <div
            className="gantt-timeline-subheader grid"
            style={{
              gridTemplateColumns: `200px repeat(${daysToShow}, minmax(${40 * zoomLevel}px, 1fr))`,
            }}
          >
            <div className="gantt-header-cell bg-muted/30 border-r border-b p-2"></div>
            {timelineDates.map((date) => {
              const formatted = format(date, 'yyyy-MM-dd');
              return (
                <div
                  key={formatted}
                  className={cn(
                    'gantt-day border-r border-b p-1 text-center text-xs',
                    formatted === todayFormatted
                      ? 'bg-blue-100 dark:bg-blue-900/20'
                      : 'bg-muted/30',
                  )}
                >
                  {format(date, 'd', { locale: es })}
                </div>
              );
            })}
          </div>
        )}

        {/* Cuerpo del Gantt con tareas */}
        <div
          className="gantt-body relative"
          role="button"
          tabIndex={0}
          onClick={handleTimelineClick}
          onKeyDown={(e) => {
            if (e.key === 'Enter' || e.key === ' ') {
              e.preventDefault();
              // Se incluye para accesibilidad, pero no dispara onTimelineClick por falta de coordenadas de puntero
            }
          }}
        >
          {groupedTasks.length === 0 ? (
            <div className="col-span-full border-b py-8 text-center">
              No hay tareas para mostrar
            </div>
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
                          onManageDependencies={onManageDependencies}
                          isInGroup={true}
                          hasDependencies={dependencies.some(
                            (d) => d.fromTaskId === task.id || d.toTaskId === task.id,
                          )}
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
                    onManageDependencies={onManageDependencies}
                    isInGroup={false}
                    hasDependencies={dependencies.some(
                      (d) => d.fromTaskId === group.tasks[0].id || d.toTaskId === group.tasks[0].id,
                    )}
                  />
                )}
              </div>
            ))
          )}

          {/* Renderizado de líneas de dependencia */}
          <svg
            className="pointer-events-none absolute top-0 left-0 h-full w-full"
            style={{ zIndex: 5 }}
          >
            {dependencyLines.map((line) => (
              <g key={line.id}>
                <path
                  d={`M ${line.fromX},${line.fromY} C ${line.fromX + 20},${line.fromY} ${line.toX - 20},${line.toY} ${line.toX},${line.toY}`}
                  stroke="rgba(100, 100, 100, 0.5)"
                  strokeWidth="1.5"
                  fill="none"
                  strokeDasharray={
                    line.type === 'start-to-start' || line.type === 'finish-to-finish' ? '5,5' : ''
                  }
                  markerEnd="url(#arrowhead)"
                />
              </g>
            ))}
            <defs>
              <marker
                id="arrowhead"
                markerWidth="6"
                markerHeight="6"
                refX="5"
                refY="3"
                orient="auto"
              >
                <path d="M 0 0 L 6 3 L 0 6 z" fill="rgba(100, 100, 100, 0.5)" />
              </marker>
            </defs>
          </svg>

          {/* Línea vertical de "Hoy" */}
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
    </TooltipProvider>
  );
}

interface GroupRowProps {
  readonly group: GroupedTask;
  readonly daysToShow: number;
  readonly zoomLevel: number;
  readonly toggleGroupExpansion: (groupId: string) => void;
  readonly timelineDates: Date[];
  readonly startDate: Date;
}

function GroupRow({
  group,
  daysToShow,
  zoomLevel,
  toggleGroupExpansion,
  timelineDates,
  startDate,
}: GroupRowProps) {
  const todayFormatted = useMemo(() => format(new Date(), 'yyyy-MM-dd'), []);

  return (
    <div
      className="gantt-row bg-muted/30 relative grid"
      style={{
        gridTemplateColumns: `200px repeat(${daysToShow}, minmax(${40 * zoomLevel}px, 1fr))`,
      }}
    >
      <button
        type="button"
        className="gantt-task-info w-full truncate border-r border-b p-2 text-left font-medium"
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
      </button>

      {timelineDates.map((date) => {
        const formatted = format(date, 'yyyy-MM-dd');
        return (
          <div
            key={formatted}
            className={cn(
              'gantt-cell border-r border-b',
              formatted === todayFormatted ? 'bg-blue-100 dark:bg-blue-900/20' : '',
            )}
          />
        );
      })}
    </div>
  );
}

interface TaskRowProps {
  readonly task: Task;
  readonly daysToShow: number;
  readonly zoomLevel: number;
  readonly startDate: Date;
  readonly timelineDates: Date[];
  readonly onTaskClick: (task: Task) => void;
  readonly onManageDependencies: (task: Task) => void;
  readonly isInGroup: boolean;
  readonly hasDependencies: boolean;
}

function TaskRow({
  task,
  daysToShow,
  zoomLevel,
  startDate,
  timelineDates,
  onTaskClick,
  onManageDependencies,
  isInGroup,
  hasDependencies,
}: TaskRowProps) {
  const todayFormatted = useMemo(() => format(new Date(), 'yyyy-MM-dd'), []);
  const taskStart = new Date(task.startDate);
  const taskEnd = new Date(task.endDate);

  // Días desde el inicio del timeline hasta el inicio de la tarea
  const startDiff = Math.max(
    0,
    Math.floor((taskStart.getTime() - startDate.getTime()) / (1000 * 60 * 60 * 24)),
  );
  // Duración de la tarea en días (al menos 1)
  const duration = Math.max(
    1,
    Math.floor((taskEnd.getTime() - taskStart.getTime()) / (1000 * 60 * 60 * 24)) + 1,
  );
  // Duración visible teniendo en cuenta los días restantes del timeline
  const visibleDuration = Math.min(duration, daysToShow - startDiff);
  // Determina si la tarea se ve dentro del periodo actual
  const isVisible = startDiff < daysToShow && startDiff + duration > 0;

  // Hook de dnd-kit para arrastrar tareas
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
    ? { transform: `translate3d(${transform.x}px, ${transform.y}px, 0)` }
    : undefined;

  // Función para determinar el color de la barra según estado/progreso
  const getStatusColor = (status?: string, progress?: number) => {
    if (progress === 100 || status === 'completed') return 'bg-emerald-500 hover:bg-emerald-600';
    if (status === 'delayed') return 'bg-rose-500 hover:bg-rose-600';
    if (status === 'in-progress') return 'bg-blue-500 hover:bg-blue-600';
    return 'bg-amber-500 hover:bg-amber-600';
  };

  // Etiqueta legible del estado de la tarea
  const statusLabel = useMemo(() => {
    switch (task.status) {
      case 'completed':
        return 'Completada';
      case 'delayed':
        return 'Retrasada';
      case 'in-progress':
        return 'En progreso';
      default:
        return 'Pendiente';
    }
  }, [task.status]);

  return (
    <div
      className="gantt-row relative grid"
      style={{
        gridTemplateColumns: `200px repeat(${daysToShow}, minmax(${40 * zoomLevel}px, 1fr))`,
      }}
    >
      {/* Nombre de la tarea y anidamiento si está en un grupo */}
      <div className={cn('gantt-task-info truncate border-r border-b p-2', isInGroup && 'pl-6')}>
        <div className="flex items-center truncate font-medium">
          {task.title}
          {hasDependencies && <Link2 className="text-muted-foreground ml-1 h-3 w-3" />}
        </div>
      </div>

      {/* Celdas vacías para el fondo del timeline */}
      {timelineDates.map((date) => {
        const formatted = format(date, 'yyyy-MM-dd');
        return (
          <div
            key={formatted}
            className={cn(
              'gantt-cell border-r border-b',
              formatted === todayFormatted ? 'bg-blue-100 dark:bg-blue-900/20' : '',
            )}
          />
        );
      })}

      {/* Barra de tarea (si es visible) con tooltip y arrastrable */}
      {isVisible && (
        <Tooltip>
          <TooltipTrigger asChild>
            {/* Contenedor arrastrable y cliquable para seleccionar la tarea */}
            <div
              ref={setNodeRef}
              {...attributes}
              {...listeners}
              role="button"
              tabIndex={0}
              onKeyDown={(e) => {
                if (e.key === 'Enter' || e.key === ' ') {
                  e.preventDefault();
                  onTaskClick(task);
                }
              }}
              onClick={(e) => {
                e.stopPropagation();
                onTaskClick(task);
              }}
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
            >
              <div className="gantt-task-content flex-1 truncate">
                <Grip className="mr-1 inline h-3 w-3 opacity-70" />
                {task.title}
              </div>

              {/* Indicador de progreso */}
              {task.progress > 0 && (
                <div
                  className="absolute top-0 bottom-0 left-0 rounded-l-md bg-white/20"
                  style={{ width: `${task.progress}%`, maxWidth: '100%' }}
                />
              )}

              {/* Botón para gestionar dependencias */}
              <button
                type="button"
                aria-label="Gestionar dependencias"
                title="Gestionar dependencias"
                className="gantt-task-handle absolute top-0 right-0 bottom-0 flex w-6 items-center justify-center opacity-0"
                onClick={(e) => {
                  e.stopPropagation();
                  onManageDependencies(task);
                }}
              >
                <Link2 className="h-3 w-3" />
              </button>
            </div>
          </TooltipTrigger>
          <TooltipContent>
            {/* Contenido del tooltip con detalles de la tarea */}
            <div className="text-xs">
              <div className="font-bold">{task.title}</div>
              {task.description && <div className="mt-1">{task.description}</div>}
              <div className="mt-1">
                <span className="font-medium">Inicio:</span> {format(taskStart, 'dd/MM/yyyy')}
              </div>
              <div>
                <span className="font-medium">Fin:</span> {format(taskEnd, 'dd/MM/yyyy')}
              </div>
              <div>
                <span className="font-medium">Progreso:</span> {task.progress}%
              </div>
              <div>
                <span className="font-medium">Estado:</span> {statusLabel}
              </div>
            </div>
          </TooltipContent>
        </Tooltip>
      )}
    </div>
  );
}
