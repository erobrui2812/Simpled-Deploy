'use client';

import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { Card, CardContent } from '@/components/ui/card';
import type { ActivityLog } from '@/types';
import { formatDistanceToNow } from 'date-fns';
import { es } from 'date-fns/locale';
import { CheckCircle2, Clock, Edit, MessageSquare, Tag, UserPlus } from 'lucide-react';

interface ActivityLogProps {
  logs: ActivityLog[];
}

export function ActivityLogComponent({ logs }: ActivityLogProps) {
  const getActivityIcon = (action: string) => {
    switch (action) {
      case 'created':
        return <Edit className="h-4 w-4 text-blue-500" />;
      case 'updated':
        return <Edit className="h-4 w-4 text-amber-500" />;
      case 'completed':
        return <CheckCircle2 className="h-4 w-4 text-emerald-500" />;
      case 'commented':
        return <MessageSquare className="h-4 w-4 text-purple-500" />;
      case 'status_changed':
        return <Tag className="h-4 w-4 text-blue-500" />;
      case 'assigned':
        return <UserPlus className="h-4 w-4 text-rose-500" />;
      case 'due_date_changed':
        return <Clock className="h-4 w-4 text-amber-500" />;
      default:
        return <Edit className="h-4 w-4 text-gray-500" />;
    }
  };

  return (
    <div className="space-y-4">
      <h3 className="text-lg font-medium">Historial de Actividad</h3>

      {logs.length > 0 ? (
        <div className="space-y-3">
          {logs.map((log) => (
            <Card key={log.id}>
              <CardContent className="p-3">
                <div className="flex gap-3">
                  <Avatar className="h-8 w-8">
                    <AvatarImage src={log.userImageUrl || '/placeholder.svg'} alt={log.userName} />
                    <AvatarFallback>{log.userName.charAt(0)}</AvatarFallback>
                  </Avatar>
                  <div className="space-y-1">
                    <div className="flex items-center gap-2">
                      {getActivityIcon(log.action)}
                      <span className="font-medium">{log.userName}</span>
                      <span className="text-muted-foreground text-xs">
                        {formatDistanceToNow(new Date(log.timestamp), {
                          addSuffix: true,
                          locale: es,
                        })}
                      </span>
                    </div>
                    <p className="text-sm">{log.details}</p>
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
}
