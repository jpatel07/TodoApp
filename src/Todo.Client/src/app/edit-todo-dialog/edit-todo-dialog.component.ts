import { Component, inject, signal } from '@angular/core';
import { AbstractControl, FormBuilder, ValidationErrors, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { HttpErrorResponse } from '@angular/common/http';
import { TodoService } from '../todo.service';

function futureDateValidator(control: AbstractControl): ValidationErrors | null {
  if (!control.value) return null;
  const today = new Date();
  today.setHours(0, 0, 0, 0);
  const selected = new Date(control.value + 'T00:00:00');
  return selected < today ? { pastDate: true } : null;
}

@Component({
  selector: 'app-edit-todo-dialog',
  templateUrl: './edit-todo-dialog.component.html',
  styleUrl: './edit-todo-dialog.component.css',
  standalone: false,
})
export class EditTodoDialogComponent {
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<EditTodoDialogComponent>);
  private todoService = inject(TodoService);
  private id = inject<number>(MAT_DIALOG_DATA);

  readonly today = new Date().toISOString().split('T')[0];

  readonly loading = signal(true);
  readonly loadError = signal(false);
  readonly submitting = signal(false);
  readonly apiError = signal<string | null>(null);

  form = this.fb.group({
    title: ['', [Validators.required, Validators.maxLength(200)]],
    description: [''],
    dueDate: ['', [futureDateValidator]],
  });

  constructor() {
    this.todoService.getTodoById(this.id).subscribe({
      next: (todo) => {
        this.form.patchValue({
          title: todo.title,
          description: todo.description ?? '',
          dueDate: todo.dueDate ?? '',
        });
        this.loading.set(false);
      },
      error: () => {
        this.loadError.set(true);
        this.loading.set(false);
      },
    });
  }

  submit(): void {
    if (this.form.invalid) return;
    this.submitting.set(true);
    this.apiError.set(null);
    const { title, description, dueDate } = this.form.getRawValue();
    this.todoService.updateTodo(this.id, {
      title: title!,
      description: description || null,
      dueDate: dueDate || null,
    }).subscribe({
      next: () => {
        this.submitting.set(false);
        this.dialogRef.close(true);
      },
      error: (err: HttpErrorResponse) => {
        this.submitting.set(false);
        if (err.status >= 400 && err.status < 500) {
          this.apiError.set(this.extractMessage(err));
        } else {
          this.apiError.set('An unexpected error occurred. Please try again.');
        }
      },
    });
  }

  private extractMessage(err: HttpErrorResponse): string {
    const body = err.error;
    if (typeof body === 'string' && body.trim()) return body.trim();
    if (body && typeof body === 'object') {
      if (typeof body.detail === 'string' && body.detail.trim()) return body.detail.trim();
      if (typeof body.title === 'string' && body.title.trim()) return body.title.trim();
      if (typeof body.message === 'string' && body.message.trim()) return body.message.trim();
      const errors = body.errors;
      if (errors && typeof errors === 'object') {
        const messages = Object.values(errors).flat() as string[];
        if (messages.length) return messages.join(' ');
      }
    }
    return `Request failed with status ${err.status}.`;
  }

  cancel(): void {
    this.dialogRef.close(false);
  }
}
