import { NgModule, provideBrowserGlobalErrorListeners } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { provideHttpClient } from '@angular/common/http';
import { JsonPipe, DatePipe } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { TextFieldModule } from '@angular/cdk/text-field';
import { AppRoutingModule } from './app-routing-module';
import { App } from './app.component';
import { CreateTodoDialogComponent } from './create-todo-dialog/create-todo-dialog.component';

@NgModule({
  declarations: [App, CreateTodoDialogComponent],
  imports: [
    BrowserModule,
    AppRoutingModule,
    JsonPipe,
    DatePipe,
    ReactiveFormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatProgressSpinnerModule,
    MatIconModule,
    MatCardModule,
    MatChipsModule,
    MatButtonModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    TextFieldModule,
  ],
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideHttpClient(),
  ],
  bootstrap: [App]
})
export class AppModule { }

