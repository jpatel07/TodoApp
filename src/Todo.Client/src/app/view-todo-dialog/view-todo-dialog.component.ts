import { Component, inject, signal } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { TodoService } from '../todo.service';
import { TodoDTO } from '../todo.model';

@Component({
  selector: 'app-view-todo-dialog',
  templateUrl: './view-todo-dialog.component.html',
  styleUrl: './view-todo-dialog.component.css',
  standalone: false,
})
export class ViewTodoDialogComponent {
  private dialogRef = inject(MatDialogRef<ViewTodoDialogComponent>);
  private todoService = inject(TodoService);
  private id = inject<number>(MAT_DIALOG_DATA);

  readonly todo = signal<TodoDTO | null>(null);
  readonly loading = signal(true);
  readonly error = signal(false);

  constructor() {
    this.todoService.getTodoById(this.id).subscribe({
      next: (data) => {
        this.todo.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.error.set(true);
        this.loading.set(false);
      },
    });
  }

  close(): void {
    this.dialogRef.close();
  }
}
