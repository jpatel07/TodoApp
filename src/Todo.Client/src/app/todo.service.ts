import { Injectable, signal } from '@angular/core';
import { httpResource, HttpClient } from '@angular/common/http';
import { TodoDTO, PagedResult, CreateTodoRequest, UpdateTodoRequest } from './todo.model';

const API_BASE = 'http://localhost:5018/todos';

@Injectable({ providedIn: 'root' })
export class TodoService {
  readonly pageNumber = signal(1);
  readonly pageSize = signal(10);

  readonly todosPage = httpResource<PagedResult<TodoDTO>>(() => ({
    url: API_BASE,
    params: {
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
    },
  }));

  constructor(private http: HttpClient) {}

  createTodo(request: CreateTodoRequest) {
    return this.http.post(API_BASE, request);
  }

  getTodoById(id: number) {
    return this.http.get<TodoDTO>(`${API_BASE}/${id}`);
  }

  updateTodo(id: number, request: UpdateTodoRequest) {
    return this.http.put(`${API_BASE}/${id}`, request);
  }

  setCompleted(id: number, isCompleted: boolean) {
    const action = isCompleted ? 'complete' : 'incomplete';
    return this.http.patch(`${API_BASE}/${id}/${action}`, {});
  }

  deleteTodo(id: number) {
    return this.http.delete(`${API_BASE}/${id}`);
  }
}
