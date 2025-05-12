export type User = {
  id: string;
  name: string;
  imageUrl: string;
};

export type Subtask = {
  id: string;
  itemId: string;
  title: string;
  isCompleted: boolean;
};

export type Comment = {
  id: string;
  itemId: string;
  userId: string;
  userName: string;
  userImageUrl: string;
  text: string;
  createdAt: string;
  updatedAt?: string;
  isResolved?: boolean;
};

export type ActivityType =
  | 'Created'
  | 'Updated'
  | 'StatusChanged'
  | 'Assigned'
  | 'DateChanged'
  | 'Deleted'
  | 'FileUploaded'
  | 'SubtaskCreated'
  | 'SubtaskUpdated'
  | 'SubtaskDeleted'
  | 'CommentAdded'
  | 'CommentEdited'
  | 'CommentDeleted'
  | 'CommentResolved';

export type ActivityLog = {
  id: string;
  itemId: string;
  userId: string;
  userName: string;
  userAvatarUrl?: string | null;
  type: ActivityType;
  field?: string | null;
  oldValue?: string | null;
  newValue?: string | null;
  details: string;
  timestamp: string;
};

export type Item = {
  id: string;
  title: string;
  description?: string;
  dueDate?: string;
  startDate?: string;
  columnId: string;
  status?: 'pending' | 'in-progress' | 'completed' | 'delayed';
  assigneeId?: string | null;
  subtasks?: Subtask[];
  progress?: number;
  comments?: Comment[];
  activityLogs?: ActivityLog[];
};

export type Column = {
  id: string;
  title: string;
  boardId: string;
  order: number;
};

export type Board = {
  id: string;
  name: string;
  isPublic: boolean;
  ownerId?: string;
  userRole?: 'admin' | 'editor' | 'viewer';
  isFavorite: boolean;
};

export type Member = {
  userId: string;
  userName: string;
  role: string;
};
