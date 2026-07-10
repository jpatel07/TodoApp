export interface CreateTodoRequest {
  title: string;
  description?: string | null;
  dueDate?: string | null;
}

export interface TodoDTO {
  id: number;
  title: string;
  description: string | null;
  dueDate: string | null;
  isCompleted: boolean;
  createdAt: string;
}

export interface PagedResult<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}
