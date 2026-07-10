import { Component, inject } from '@angular/core';
import { TodoService } from './todo.service';
import { PageEvent } from '@angular/material/paginator';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.css'
})
export class App {
  protected readonly todoService = inject(TodoService);

  readonly displayedColumns = ['id', 'title', 'dueDate', 'isCompleted'];

  onPageChange(event: PageEvent): void {
    this.todoService.pageNumber.set(event.pageIndex + 1);
    this.todoService.pageSize.set(event.pageSize);
  }
}
