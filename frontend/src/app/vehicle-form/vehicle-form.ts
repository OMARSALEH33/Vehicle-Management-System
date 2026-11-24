import { Component, OnInit, computed, inject, input, output, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { catchError, finalize, forkJoin, map, of, switchMap, tap } from 'rxjs';

import { MakeService } from '../services/makeservice';
import { ModelService } from '../services/modelservices';
import { FeatureService } from '../services/featureservices';
import { VehicleService } from '../services/vehicleservices';
import { Feature, Make, Vehicle, VehicleModel } from '../models/model';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-vehicle-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  styleUrl: './vehicle-form.css',
  templateUrl: './vehicle-form.html',
})
export class VehicleForm implements OnInit {

  private readonly fb = inject(NonNullableFormBuilder);
  private readonly makeService = inject(MakeService);
  private readonly modelService = inject(ModelService);
  private readonly featureService = inject(FeatureService);
  private readonly vehicleService = inject(VehicleService);
  private readonly toastr = inject(ToastrService);

  vehicle = input<Vehicle | null>(null);
  saved = output<void>();
  closed = output<void>();

  isEdit = computed(() => this.vehicle() !== null);
  makes = signal<Make[]>([]);
  models = signal<VehicleModel[]>([]);
  features = signal<Feature[]>([]);
  selectedFeatures = signal<number[]>([]);

  submitted = signal(false);
  saving = signal(false);
  serverErrors = signal<Record<string, string>>({});

  form = this.fb.group({
    code: ['', [Validators.required, Validators.maxLength(4)]],
    makeName: ['', [Validators.required, Validators.maxLength(255)]],
    modelName: ['', [Validators.required, Validators.maxLength(255)]],
    contact: this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(255)]],
      email: ['', [Validators.email, Validators.maxLength(255)]],
      phone: ['', [Validators.required, Validators.maxLength(255)]],
    }),
  });

  constructor() {
    this.setupMakeNameListener();
  }

  ngOnInit() {
    this.loadInitialData();
  }

  private setupMakeNameListener() {
    this.form.controls.makeName.valueChanges
      .pipe(
        tap((makeName) => {
          this.resetModels();
          this.autoFillCode(makeName);
        }),
        switchMap((makeName) => this.fetchModelsForMake(makeName)),
        takeUntilDestroyed()
      )
      .subscribe((models) => this.models.set(models));
  }

  private autoFillCode(makeName: string) {
    const matchedMake = this.makes().find(
      (m) => m.name.toLowerCase() === makeName.trim().toLowerCase()
    );
    if (matchedMake && matchedMake.code) {
      this.form.patchValue({ code: matchedMake.code }, { emitEvent: false });
    } else {
      this.form.patchValue({ code: '' }, { emitEvent: false });
    }
  }

  private resetModels() {
    this.models.set([]);
    this.form.controls.modelName.reset('');
  }

  private fetchModelsForMake(makeName: string) {
    const matchedMake = this.makes().find(
      (m) => m.name.toLowerCase() === makeName.trim().toLowerCase()
    );
    if (!matchedMake) return of<VehicleModel[]>([]);

    return this.modelService.getModels(matchedMake.id, 1, 1000).pipe(
      map((res) => res.items),
      catchError(() => of<VehicleModel[]>([]))
    );
  }

  private loadInitialData() {
    const currentVehicle = this.vehicle();

    forkJoin({
      makes: this.makeService.getMakes(1, 1000),
      features: this.featureService.getFeatures(),
      models: currentVehicle
        ? this.modelService.getModels(currentVehicle.make.id, 1, 1000).pipe(
            map((res) => res.items),
            catchError(() => of<VehicleModel[]>([]))
          )
        : of<VehicleModel[]>([]),
    }).subscribe({
      next: ({ makes, features, models }) => {
        this.makes.set(makes.items);
        this.features.set(features);

        if (currentVehicle) {
          this.populateForm(currentVehicle, models);
        }
      },
    });
  }

  private populateForm(v: Vehicle, models: VehicleModel[]) {
    this.models.set(models);
    this.selectedFeatures.set(v.features.map((f) => f.id));

    this.form.patchValue({
      makeName: v.make.name,
      modelName: v.model.name,
      contact: {
        name: v.contact.name,
        email: v.contact.email ?? '',
        phone: v.contact.phone,
      },
    }, { emitEvent: false });
  }

  toggleFeature(id: number, checked: boolean) {
    this.selectedFeatures.update((ids) =>
      checked ? (ids.includes(id) ? ids : [...ids, id]) : ids.filter((x) => x !== id)
    );
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
    const currentVehicle = this.vehicle();

    const requestBody = {
      code: formValue.code.trim(),
      makeName: formValue.makeName.trim(),
      modelName: formValue.modelName.trim(),
      isRegistered: currentVehicle?.isRegistered ?? false,
      contact: {
        name: formValue.contact.name.trim(),
        email: formValue.contact.email.trim() || null,
        phone: formValue.contact.phone.trim(),
      },
      features: this.selectedFeatures(),
    };

    const apiCall = currentVehicle
      ? this.vehicleService.update(currentVehicle.id, requestBody)
      : this.vehicleService.create(requestBody);

    this.saving.set(true);

    apiCall.pipe(finalize(() => this.saving.set(false))).subscribe({
      next: () => {
        const action = currentVehicle ? 'updated' : 'created';
        this.toastr.success(`Vehicle ${action} successfully!`, 'Success', {
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
      let codeErrorMessage = '';

      for (const [key, messages] of Object.entries(err.error.errors as Record<string, string[]>)) {
        errorMap[key] = messages[0];

        if (
          (key.toLowerCase() === 'code' ||
            key.toLowerCase() === 'modelname' ||
            key.toLowerCase() === 'modelid') &&
          messages.length > 0
        ) {
          codeErrorMessage = messages[0];
        }
      }

      this.serverErrors.set(errorMap);

      const toastMessage = codeErrorMessage || 'Please fix the validation errors in the form.';
      this.toastr.error(toastMessage, 'Validation Error', {
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