import { Component, computed, inject, input, OnInit, output, signal } from '@angular/core';
import { FormsModule, NonNullableFormBuilder, Validators } from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

import { ModelService } from '../services/modelservices';
import { Model , Make} from '../models/model';
import { ToastrService } from 'ngx-toastr';
import { finalize } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-add-model',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './add-model.html',
  styleUrls: ['./add-model.css']
})
export class AddModel implements OnInit {

  private readonly fb = inject(NonNullableFormBuilder);
  private readonly modelService = inject(ModelService);
  private readonly toastr = inject(ToastrService);
  
  model = input<Model | null>(null);
  makes = input<Make[]>([]);

  saved = output<void>();
  closed = output<void>();

  isEdit = computed(() => this.model() !== null);
  submitted = signal(false);
  saving = signal(false);
  serverErrors = signal<Record<string, string>>({});

  form = this.fb.group({
    makeId: [0, [Validators.required, Validators.min(1)]],
    name: ['', [Validators.required, Validators.maxLength(255)]],
  });

  ngOnInit() {
    const currentModel = this.model();
    if (this.isEdit() && currentModel) {
      this.form.patchValue({
        makeId: currentModel.makeId,
        name: currentModel.name,
      });
    }
  }

  showError(control: { invalid: boolean; touched: boolean }): boolean {
    return control.invalid && (control.touched || this.submitted());
  }

  submit() {
    this.submitted.set(true);
    this.serverErrors.set({});
    this.form.markAllAsTouched();

    if (this.form.invalid) return;

    const formValue = this.form.getRawValue();
    const currentModel = this.model();

    const requestBody = {
      makeId: Number(formValue.makeId),
      name: formValue.name.trim(),
    };

    const apiCall = currentModel
      ? this.modelService.updateModel(currentModel.id, requestBody) 
      : this.modelService.create(requestBody);      

    this.saving.set(true);

    apiCall.pipe(finalize(() => this.saving.set(false))).subscribe({
      next: () => {
        const action = currentModel ? 'updated' : 'created';
        this.toastr.success(`Model ${action} successfully!`, 'Success', {
          timeOut: 5000,
          closeButton: true,
          progressBar: true,
          positionClass: 'toast-top-right',
        });
        this.saved.emit();
      },
      error: (err: HttpErrorResponse) => this.handleError(err),
    });
  }

  private handleError(err: HttpErrorResponse) {
    if (err.status === 0) {
      this.toastr.error('Unable to connect to the server. Please check your network.', 'Connection Error', {
        timeOut: 5000,
        closeButton: true,
        progressBar: true,
        positionClass: 'toast-top-right',
      });
      return;
    }

    if (err.status === 400 && err.error?.errors) {
      const errorMap: Record<string, string> = {};
      for (const [key, messages] of Object.entries(err.error.errors as Record<string, string[]>)) {
        errorMap[key] = messages[0];
      }
      this.serverErrors.set(errorMap);
      this.toastr.error('Please fix the validation errors in the form.', 'Validation Error', {
        timeOut: 5000,
        closeButton: true,
        progressBar: true,
        positionClass: 'toast-top-right',
      });
      return;
    }

    const generalMessage = err.error?.message || 'An unexpected error occurred!';
    this.toastr.error(generalMessage, `Error (${err.status})`, {
      timeOut: 5000,
      closeButton: true,
      progressBar: true,
      positionClass: 'toast-top-right',
    });
  }
}