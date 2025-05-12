'use client';

import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { Card, CardContent } from '@/components/ui/card';
import type { ActivityLog } from '@/types';
import { formatDistanceToNow } from 'date-fns';
import { es } from 'date-fns/locale';
import { CheckCircle2, Clock, Edit, MessageSquare, Tag, Trash2, UserPlus } from 'lucide-react';
import React from 'react';

interface ActivityLogProps {
  logs: ActivityLog[];
}

function getActivityMessage(log: ActivityLog) {
  console.log('log.type:', log.type, log);
  switch (log.type) {
    case 'Created':
      return `Tarea creada por ${log.userName}`;
    case 'Updated':
      if (['StartDate', 'DueDate'].includes(log.field || '')) {
        return `${log.userName} cambió la fecha: '${log.oldValue}' → '${log.newValue}'`;
      }
      return log.field
        ? `${log.userName} actualizó ${log.field}: '${log.oldValue}' → '${log.newValue}'`
        : `${log.userName} actualizó la tarea`;
    case 'StatusChanged':
      return `${log.userName} cambió el estado: '${log.oldValue}' → '${log.newValue}'`;
    case 'Assigned':
      return `${log.userName} cambió el responsable: '${log.oldValue}' → '${log.newValue}'`;
    case 'Deleted':
      return `${log.userName} eliminó la tarea`;
    case 'FileUploaded':
      return `${log.userName} subió un archivo: ${log.details}`;
    case 'SubtaskCreated':
      return `${log.userName} creó una subtarea: ${log.details}`;
    case 'SubtaskUpdated':
      return `${log.userName} actualizó una subtarea: ${log.details}`;
    case 'SubtaskDeleted':
      return `${log.userName} eliminó una subtarea`;
    default:
      return `${log.userName} realizó una acción`;
  }
}

const getActivityIcon = (type: string, field?: string) => {
  switch (type) {
    case 'Created':
      return <Edit className="h-4 w-4 text-blue-500" />;
    case 'Updated':
      if (['StartDate', 'DueDate'].includes(field || '')) {
        return <Clock className="h-4 w-4 text-amber-500" />;
      }
      return <Edit className="h-4 w-4 text-amber-500" />;
    case 'StatusChanged':
      return <Tag className="h-4 w-4 text-blue-500" />;
    case 'Assigned':
      return <UserPlus className="h-4 w-4 text-rose-500" />;
    case 'Deleted':
      return <Trash2 className="h-4 w-4 text-red-500" />;
    case 'FileUploaded':
      return <MessageSquare className="h-4 w-4 text-purple-500" />;
    case 'SubtaskCreated':
      return <CheckCircle2 className="h-4 w-4 text-emerald-500" />;
    case 'SubtaskUpdated':
      return <CheckCircle2 className="h-4 w-4 text-amber-500" />;
    case 'SubtaskDeleted':
      return <Trash2 className="h-4 w-4 text-red-500" />;
    default:
      return <Edit className="h-4 w-4 text-gray-500" />;
  }
};

export const ActivityLogComponent: React.FC<ActivityLogProps> = ({ logs }) => (
  <div className="space-y-4">
    <h3 className="text-lg font-medium">Historial de Actividad</h3>
    {logs.length > 0 ? (
      <div className="space-y-3">
        {logs.map((log) => (
          <Card key={log.id}>
            <CardContent className="p-3">
              <div className="flex gap-3">
                <Avatar className="h-8 w-8">
                  <AvatarImage src={log.userAvatarUrl ?? '/placeholder.svg'} alt={log.userName} />
                  <AvatarFallback>{log.userName.charAt(0)}</AvatarFallback>
                </Avatar>
                <div className="space-y-1">
                  <div className="flex items-center gap-2">
                    {getActivityIcon(log.type, log.field)}
                    <span className="font-medium">{getActivityMessage(log)}</span>
                    <span className="text-muted-foreground text-xs">
                      {formatDistanceToNow(new Date(log.timestamp), {
                        addSuffix: true,
                        locale: es,
                      })}
                    </span>
                  </div>
                </div>
              </div>
            </CardContent>
          </Card>
        ))}
      </div>
    ) : (
      <p className="text-muted-foreground py-4 text-center">
        No hay actividad registrada para esta tarea.
      </p>
    )}
  </div>
);
