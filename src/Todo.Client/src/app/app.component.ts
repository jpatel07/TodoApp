import { Component, inject, signal } from '@angular/core';
import { TodoService } from './todo.service';
import { PageEvent } from '@angular/material/paginator';
import { MatDialog } from '@angular/material/dialog';
import { CreateTodoDialogComponent } from './create-todo-dialog/create-todo-dialog.component';
import { ViewTodoDialogComponent } from './view-todo-dialog/view-todo-dialog.component';
import { EditTodoDialogComponent } from './edit-todo-dialog/edit-todo-dialog.component';
import { ConfirmDialogComponent } from './confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.css'
})
export class App {
  protected readonly todoService = inject(TodoService);
  private readonly dialog = inject(MatDialog);

  readonly displayedColumns = ['id', 'title', 'dueDate', 'isCompleted', 'actions'];

  /** Tracks which row IDs are currently being toggled */
  readonly togglingIds = signal<Set<number>>(new Set());

  /** Tracks which row IDs are currently being deleted */
  readonly deletingIds = signal<Set<number>>(new Set());

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

  openViewDialog(id: number): void {
    this.dialog.open(ViewTodoDialogComponent, { width: '440px', data: id });
  }

  openEditDialog(id: number): void {
    const ref = this.dialog.open(EditTodoDialogComponent, { width: '480px', data: id });
    ref.afterClosed().subscribe((updated: boolean) => {
      if (updated) {
        this.todoService.todosPage.reload();
      }
    });
  }

  toggleCompleted(id: number, currentValue: boolean): void {
    this.togglingIds.update(s => new Set(s).add(id));
    this.todoService.setCompleted(id, !currentValue).subscribe({
      next: () => {
        this.togglingIds.update(s => { const n = new Set(s); n.delete(id); return n; });
        this.todoService.todosPage.reload();
      },
      error: () => {
        this.togglingIds.update(s => { const n = new Set(s); n.delete(id); return n; });
      },
    });
  }

  confirmDelete(id: number): void {
    const ref = this.dialog.open(ConfirmDialogComponent, { width: '380px' });
    ref.afterClosed().subscribe((confirmed: boolean) => {
      if (!confirmed) return;
      this.deletingIds.update(s => new Set(s).add(id));
      this.todoService.deleteTodo(id).subscribe({
        next: () => {
          this.deletingIds.update(s => { const n = new Set(s); n.delete(id); return n; });
          this.todoService.todosPage.reload();
        },
        error: () => {
          this.deletingIds.update(s => { const n = new Set(s); n.delete(id); return n; });
        },
      });
    });
  }
}
