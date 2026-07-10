import { Injectable, signal } from '@angular/core';
import { httpResource } from '@angular/common/http';
import { TodoDTO, PagedResult } from './todo.model';

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
}
