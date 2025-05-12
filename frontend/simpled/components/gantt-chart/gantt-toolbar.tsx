'use client';

import { Button } from '@/components/ui/button';
import { Checkbox } from '@/components/ui/checkbox';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import {
  Calendar,
  ChevronLeft,
  ChevronRight,
  Download,
  Search,
  ZoomIn,
  ZoomOut,
} from 'lucide-react';

type ViewMode = 'day' | 'week' | 'month';
type GroupBy = 'none' | 'status' | 'assignee';
type TimelineDirection = 'prev' | 'next';

interface GanttToolbarProps {
  readonly viewMode: ViewMode;
  readonly setViewMode: (mode: ViewMode) => void;
  readonly showCompleted: boolean;
  readonly setShowCompleted: (show: boolean) => void;
  readonly filterStatus: string | null;
  readonly setFilterStatus: (status: string | null) => void;
  readonly searchTerm: string;
  readonly setSearchTerm: (term: string) => void;
  readonly groupBy: GroupBy;
  readonly setGroupBy: (groupBy: GroupBy) => void;
  readonly zoomLevel: number;
  readonly setZoomLevel: (level: number) => void;
  readonly navigateTimeline: (direction: TimelineDirection) => void;
  readonly goToToday: () => void;
  readonly setDateRange: (startDate: Date, days: number) => void;
  readonly exportData: () => void;
}

export function GanttToolbar({
  viewMode,
  setViewMode,
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
  navigateTimeline,
  goToToday,
  setDateRange,
  exportData,
}: GanttToolbarProps) {
  const dateRangePresets = [
    {
      label: 'Esta semana',
      fn: () => {
        const today = new Date();
        const startOfWeek = new Date(today);
        startOfWeek.setDate(today.getDate() - today.getDay());
        setDateRange(startOfWeek, 7);
      },
    },
    {
      label: 'Próximos 14 días',
      fn: () => {
        const today = new Date();
        setDateRange(today, 14);
      },
    },
    {
      label: 'Este mes',
      fn: () => {
        const today = new Date();
        const startOfMonth = new Date(today.getFullYear(), today.getMonth(), 1);
        const daysInMonth = new Date(today.getFullYear(), today.getMonth() + 1, 0).getDate();
        setDateRange(startOfMonth, daysInMonth);
      },
    },
    {
      label: 'Próximos 30 días',
      fn: () => {
        const today = new Date();
        setDateRange(today, 30);
      },
    },
    {
      label: 'Próximos 90 días',
      fn: () => {
        const today = new Date();
        setDateRange(today, 90);
      },
    },
  ];

  return (
    <div className="flex flex-col gap-4">
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-2">
          <Button
            variant="outline"
            size="sm"
            onClick={() => setZoomLevel(Math.max(0.5, zoomLevel - 0.25))}
          >
            <ZoomOut className="h-4 w-4" />
          </Button>
          <span className="text-sm">{Math.round(zoomLevel * 100)}%</span>
          <Button
            variant="outline"
            size="sm"
            onClick={() => setZoomLevel(Math.min(2, zoomLevel + 0.25))}
          >
            <ZoomIn className="h-4 w-4" />
          </Button>
        </div>

        <div className="flex items-center gap-2">
          <Popover>
            <PopoverTrigger asChild>
              <Button variant="outline" size="sm">
                <Calendar className="mr-2 h-4 w-4" />
                Rango de fechas
              </Button>
            </PopoverTrigger>
            <PopoverContent className="w-56 p-2">
              <div className="grid gap-2">
                {dateRangePresets.map((preset) => (
                  <Button
                    key={preset.label}
                    variant="ghost"
                    className="justify-start"
                    onClick={preset.fn}
                  >
                    {preset.label}
                  </Button>
                ))}
              </div>
            </PopoverContent>
          </Popover>

          <Button variant="outline" size="sm" onClick={goToToday}>
            Hoy
          </Button>

          <Button variant="outline" size="sm" onClick={exportData}>
            <Download className="mr-2 h-4 w-4" />
            Exportar
          </Button>
        </div>
      </div>

      <div className="flex flex-wrap items-center gap-4">
        <div className="flex items-center gap-2">
          <Checkbox
            id="show-completed"
            checked={showCompleted}
            onCheckedChange={(checked) => setShowCompleted(!!checked)}
          />
          <Label htmlFor="show-completed">Mostrar completadas</Label>
        </div>

        <Select
          value={filterStatus ?? 'all'}
          onValueChange={(value) => setFilterStatus(value === 'all' ? null : value)}
        >
          <SelectTrigger className="w-[150px]">
            <SelectValue placeholder="Estado" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="all">Todos</SelectItem>
            <SelectItem value="pending">Pendiente</SelectItem>
            <SelectItem value="in-progress">En progreso</SelectItem>
            <SelectItem value="completed">Completada</SelectItem>
            <SelectItem value="delayed">Retrasada</SelectItem>
          </SelectContent>
        </Select>

        <Select
          value={groupBy}
          onValueChange={(value: 'none' | 'status' | 'assignee') => setGroupBy(value)}
        >
          <SelectTrigger className="w-[150px]">
            <SelectValue placeholder="Agrupar por" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="none">Sin agrupar</SelectItem>
            <SelectItem value="status">Estado</SelectItem>
            <SelectItem value="assignee">Asignado a</SelectItem>
          </SelectContent>
        </Select>

        <div className="relative min-w-[200px] flex-1">
          <Search className="text-muted-foreground absolute top-2.5 left-2 h-4 w-4" />
          <Input
            placeholder="Buscar tareas..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="pl-8"
          />
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
  );
}
