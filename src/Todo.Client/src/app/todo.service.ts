import { Injectable } from '@angular/core';
import { httpResource } from '@angular/common/http';
import { TodoDTO } from './todo.model';

@Injectable({ providedIn: 'root' })
export class TodoService {
  readonly todos = httpResource<TodoDTO[]>(() => 'http://localhost:5018/todos');
}
