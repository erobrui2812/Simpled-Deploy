'use client';

import { Button } from '@/components/ui/button';
import { Checkbox } from '@/components/ui/checkbox';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { ChevronLeft, ChevronRight, Download, Search, ZoomIn, ZoomOut } from 'lucide-react';

interface GanttToolbarProps {
  viewMode: 'day' | 'week' | 'month';
  setViewMode: (mode: 'day' | 'week' | 'month') => void;
  showCompleted: boolean;
  setShowCompleted: (show: boolean) => void;
  showDependencies: boolean;
  setShowDependencies: (show: boolean) => void;
  filterStatus: string | null;
  setFilterStatus: (status: string | null) => void;
  searchTerm: string;
  setSearchTerm: (term: string) => void;
  groupBy: 'none' | 'status' | 'assignee';
  setGroupBy: (groupBy: 'none' | 'status' | 'assignee') => void;
  zoomLevel: number;
  setZoomLevel: (level: number) => void;
  navigateTimeline: (direction: 'prev' | 'next') => void;
  exportData: () => void;
}

export function GanttToolbar({
  viewMode,
  setViewMode,
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
  navigateTimeline,
  exportData,
}: GanttToolbarProps) {
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

        <Button variant="outline" size="sm" onClick={exportData}>
          <Download className="mr-2 h-4 w-4" />
          Exportar
        </Button>
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

        <div className="flex items-center gap-2">
          <Checkbox
            id="show-dependencies"
            checked={showDependencies}
            onCheckedChange={(checked) => setShowDependencies(!!checked)}
          />
          <Label htmlFor="show-dependencies">Mostrar dependencias</Label>
        </div>

        <Select
          value={filterStatus || 'all'}
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
