import { Component, inject, signal } from '@angular/core';
import { AbstractControl, FormBuilder, ValidationErrors, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
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
  selector: 'app-create-todo-dialog',
  templateUrl: './create-todo-dialog.component.html',
  styleUrl: './create-todo-dialog.component.css',
  standalone: false,
})
export class CreateTodoDialogComponent {
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<CreateTodoDialogComponent>);
  private todoService = inject(TodoService);

  readonly today = new Date().toISOString().split('T')[0];

  form = this.fb.group({
    title: ['', [Validators.required, Validators.maxLength(200)]],
    description: [''],
    dueDate: ['', [futureDateValidator]],
  });

  readonly submitting = signal(false);
  readonly apiError = signal<string | null>(null);

  submit(): void {
    if (this.form.invalid) return;
    this.submitting.set(true);
    this.apiError.set(null);
    const { title, description, dueDate } = this.form.getRawValue();
    this.todoService.createTodo({
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
