export interface TodoDTO {
  id: number;
  title: string;
  dueDate: string | null;
  isCompleted: boolean;
}

export interface PagedResult<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}
