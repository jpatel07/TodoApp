import { Component, inject } from '@angular/core';
import { TodoService } from './todo.service';
import { PageEvent } from '@angular/material/paginator';
import { MatDialog } from '@angular/material/dialog';
import { CreateTodoDialogComponent } from './create-todo-dialog/create-todo-dialog.component';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.css'
})
export class App {
  protected readonly todoService = inject(TodoService);
  private readonly dialog = inject(MatDialog);

  readonly displayedColumns = ['id', 'title', 'dueDate', 'isCompleted'];

  onPageChange(event: PageEvent): void {
    this.todoService.pageNumber.set(event.pageIndex + 1);
    this.todoService.pageSize.set(event.pageSize);
  }

  openCreateDialog(): void {
    const ref = this.dialog.open(CreateTodoDialogComponent, { width: '480px' });
    ref.afterClosed().subscribe((created: boolean) => {
      if (created) {
        this.todoService.todosPage.reload();
      }
    });
  }
}
