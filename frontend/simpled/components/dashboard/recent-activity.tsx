import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { formatDistanceToNow, parseISO } from 'date-fns';
import { es } from 'date-fns/locale';
import { CheckCircle2, MessageSquare, PenSquare, Tag, UserPlus } from 'lucide-react';

export interface Activity {
  id: string;
  type: string;
  user: {
    name: string;
    imageUrl: string;
  };
  task?: {
    title: string;
  };
  board?: {
    name: string;
  };
  comment?: {
    text: string;
  };
  commentText?: string;
  oldStatus?: string;
  newStatus?: string;
  assignee?: {
    name: string;
  };
  timestamp: string;
}

interface RecentActivityProps {
  readonly activities: Activity[];
}

const mapActionType = (action: string) => {
  switch (action) {
    case 'Comentario añadido':
    case 'CommentAdded':
      return 'comment_added';
    case 'Comentario editado':
    case 'CommentEdited':
      return 'comment_edited';
    case 'Comentario eliminado':
    case 'CommentDeleted':
      return 'comment_deleted';
    case 'Comentario resuelto':
    case 'CommentResolved':
      return 'comment_resolved';
    case 'Comentario reabierto':
    case 'CommentReopened':
      return 'comment_reopened';
    case 'Subtarea creada':
    case 'SubtaskCreated':
      return 'task_created';
    case 'StatusChanged':
      return 'task_status_changed';
    case 'Asignado':
    case 'Assigned':
      return 'task_assigned';
    case 'Tarea completada':
    case 'Completed':
      return 'task_completed';
    default:
      return action?.toLowerCase().replace(/ /g, '_') || 'unknown';
  }
};

export function RecentActivity({ activities }: RecentActivityProps) {
  const getActivityIcon = (type: string) => {
    switch (type) {
      case 'task_created':
        return <PenSquare className="h-4 w-4 text-blue-500" />;
      case 'task_completed':
        return <CheckCircle2 className="h-4 w-4 text-emerald-500" />;
      case 'comment_added':
        return <MessageSquare className="h-4 w-4 text-amber-500" />;
      case 'task_status_changed':
        return <Tag className="h-4 w-4 text-purple-500" />;
      case 'task_assigned':
        return <UserPlus className="h-4 w-4 text-rose-500" />;
      default:
        return <PenSquare className="h-4 w-4" />;
    }
  };

  const getActivityText = (activity: Activity) => {
    switch (activity.type) {
      case 'task_created':
        return (
          <>
            <span className="font-medium">{activity.user.name}</span> creó la tarea{' '}
            <span className="font-medium">{activity.task?.title}</span> en{' '}
            <span className="font-medium">{activity.board?.name}</span>
          </>
        );
      case 'task_completed':
        return (
          <>
            <span className="font-medium">{activity.user.name}</span> completó la tarea{' '}
            <span className="font-medium">{activity.task?.title}</span> en{' '}
            <span className="font-medium">{activity.board?.name}</span>
          </>
        );
      case 'comment_added':
        return (
          <>
            <span className="font-medium">{activity.user.name}</span> comentó en{' '}
            <span className="font-medium">{activity.task?.title}</span>:{' '}
            <span className="italic">"{activity.comment?.text || activity.commentText || ''}"</span>
          </>
        );
      case 'comment_edited':
        return (
          <>
            <span className="font-medium">{activity.user.name}</span> editó un comentario en{' '}
            <span className="font-medium">{activity.task?.title}</span>:{' '}
            <span className="italic">"{activity.comment?.text || activity.commentText || ''}"</span>
          </>
        );
      case 'comment_deleted':
        return (
          <>
            <span className="font-medium">{activity.user.name}</span> eliminó un comentario en{' '}
            <span className="font-medium">{activity.task?.title}</span>
          </>
        );
      case 'task_status_changed':
        return (
          <>
            <span className="font-medium">{activity.user.name}</span> cambió el estado de{' '}
            <span className="font-medium">{activity.task?.title}</span> de{' '}
            <span className="font-medium">{activity.oldStatus}</span> a{' '}
            <span className="font-medium">{activity.newStatus}</span>
          </>
        );
      case 'task_assigned':
        return (
          <>
            <span className="font-medium">{activity.user.name}</span> asignó{' '}
            <span className="font-medium">{activity.task?.title}</span> a{' '}
            <span className="font-medium">{activity.assignee?.name}</span>
          </>
        );
      case 'subtask_created':
        return (
          <>
            <span className="font-medium">{activity.user.name}</span> creó una subtarea en{' '}
            <span className="font-medium">{activity.task?.title}</span>
          </>
        );
      case 'subtask_updated':
        return (
          <>
            <span className="font-medium">{activity.user.name}</span> actualizó una subtarea en{' '}
            <span className="font-medium">{activity.task?.title}</span>
          </>
        );
      case 'subtask_deleted':
        return (
          <>
            <span className="font-medium">{activity.user.name}</span> eliminó una subtarea en{' '}
            <span className="font-medium">{activity.task?.title}</span>
          </>
        );
      case 'comment_resolved':
        return (
          <>
            <span className="font-medium">{activity.user.name}</span> marcó un comentario como
            resuelto en <span className="font-medium">{activity.task?.title}</span>
          </>
        );
      case 'comment_reopened':
        return (
          <>
            <span className="font-medium">{activity.user.name}</span> reabrió un comentario en{' '}
            <span className="font-medium">{activity.task?.title}</span>
          </>
        );
      default:
        return <span>Actividad desconocida</span>;
    }
  };

  return (
    <Card>
      <CardHeader>
        <CardTitle>Actividad Reciente</CardTitle>
        <CardDescription>Últimas actualizaciones en tus proyectos</CardDescription>
      </CardHeader>
      <CardContent>
        <div className="space-y-8">
          {activities.length > 0 ? (
            activities.map((activity) => (
              <div key={activity.id} className="flex gap-4">
                <Avatar className="h-9 w-9">
                  <AvatarImage
                    src={activity.user.imageUrl || '/placeholder.svg'}
                    alt={activity.user.name}
                  />
                  <AvatarFallback>{activity.user.name.charAt(0)}</AvatarFallback>
                </Avatar>
                <div className="space-y-1">
                  <div className="flex items-center gap-2">
                    {getActivityIcon(mapActionType(activity.type))}
                    <p className="text-sm">
                      {getActivityText({ ...activity, type: mapActionType(activity.type) })}
                    </p>
                  </div>
                  <p className="text-muted-foreground text-xs">
                    {formatDistanceToNow(parseISO(activity.timestamp), {
                      addSuffix: true,
                      locale: es,
                    })}
                  </p>
                </div>
              </div>
            ))
          ) : (
            <div className="text-muted-foreground py-4 text-center">No hay actividad reciente</div>
          )}
        </div>
      </CardContent>
    </Card>
  );
}
