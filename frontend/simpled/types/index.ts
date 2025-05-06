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
