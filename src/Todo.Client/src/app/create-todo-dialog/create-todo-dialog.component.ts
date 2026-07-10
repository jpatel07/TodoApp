import { Component, inject } from '@angular/core';
import { AbstractControl, FormBuilder, ValidationErrors, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
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

  submitting = false;

  submit(): void {
    if (this.form.invalid) return;
    this.submitting = true;
    const { title, description, dueDate } = this.form.getRawValue();
    this.todoService.createTodo({
      title: title!,
      description: description || null,
      dueDate: dueDate || null,
    }).subscribe({
      next: () => {
        this.submitting = false;
        this.dialogRef.close(true);
      },
      error: () => {
        this.submitting = false;
      },
    });
  }

  cancel(): void {
    this.dialogRef.close(false);
  }
}
