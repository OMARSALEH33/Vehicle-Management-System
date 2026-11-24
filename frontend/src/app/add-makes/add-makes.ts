import { Component, computed, inject, input, OnInit, output, signal } from '@angular/core';
import { FormsModule, NonNullableFormBuilder, Validators } from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';

import { MakeService } from '../services/makeservice';
import { Make } from '../models/model';
import { ToastrService } from 'ngx-toastr';
import { finalize } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';


@Component({
  imports: [FormsModule, ReactiveFormsModule],
  standalone: true,
  selector: 'app-add-makes',
  styleUrl: './add-makes.css',
  templateUrl: './add-makes.html',
})
export class AddMakes implements OnInit {

  private readonly fb = inject(NonNullableFormBuilder);
  private readonly makeService = inject(MakeService);
  private readonly toastr = inject(ToastrService);
  
  make = input<Make | null>(null);
  saved = output<void>();
  closed = output<void>();

  isEdit = computed(() => this.make() !== null);
  makes = signal<Make[]>([]);
  submitted = signal(false);
  saving = signal(false);
  serverErrors = signal<Record<string, string>>({});


  form = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(255)]],
    code: ['', [Validators.required, Validators.maxLength(4)]],
  });

   ngOnInit() {
    if (this.isEdit()) {
      this.form.patchValue({
        name: this.make()!.name,
        code: this.make()!.code
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
    const currentMake = this.make();

    const requestBody = {
      name: formValue.name.trim(),
      code: formValue.code.trim(),
    };

   
    const apiCall = currentMake
      ? this.makeService.updateMake(currentMake.id, requestBody) 
      : this.makeService.create(requestBody);      

    this.saving.set(true);

    apiCall.pipe(finalize(() => this.saving.set(false))).subscribe({
      next: () => {
        const action = currentMake ? 'updated' : 'created';
        this.toastr.success(`Make ${action} successfully!`, 'Success', {
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

  
