import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { VehicleService } from '../services/vehicleservices';
import { Vehicle } from '../models/model';
import { VehicleForm } from '../vehicle-form/vehicle-form';
import { AuthService } from '../services/authservice';

@Component({
  selector: 'app-vehicles',
  standalone: true,
  imports: [CommonModule, FormsModule, VehicleForm],
  templateUrl: './vehicles.html',
  styleUrl: './vehicles.css',
})
export class Vehicles implements OnInit {
  private readonly vehicleService = inject(VehicleService);
  private readonly authService = inject(AuthService);

  vehicles = signal<Vehicle[]>([]);
  keyword = signal('');
  loading = signal(true);

  editing = signal<Vehicle | null>(null);
  drawerOpen = signal(false);

  page = signal(1);
  pageSize = signal(5);
  totalCount = signal(0);

  totalPages = computed(() => Math.max(1, Math.ceil(this.totalCount() / this.pageSize())));

  startItem = computed(() => {
    if (this.totalCount() === 0) return 0;
    return (this.page() - 1) * this.pageSize() + 1;
  });

  endItem = computed(() => {
    return Math.min(this.page() * this.pageSize(), this.totalCount());
  });

  filtered = computed(() => {
    const query = this.keyword().trim().toLowerCase();
    if (!query) return this.vehicles();

    return this.vehicles().filter((vehicle) => {
      const searchFields = [
        vehicle.make.name,
        vehicle.model.name,
        vehicle.contact.phone,
        vehicle.contact.email ?? '',
        ...vehicle.features.map((f) => f.name),
      ];
      return searchFields.some((text) => text.toLowerCase().includes(query));
    });
  });

  ngOnInit() {
    this.loadVehicles();
  }

  loadVehicles(targetPage = this.page()) {
    this.page.set(targetPage);
    this.loading.set(true);

    this.vehicleService.getVehicles(this.page(), this.pageSize()).subscribe({
      next: (response) => {
        this.vehicles.set(response.data.items);
        this.totalCount.set(response.data.totalCount);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
      },
    });
  }

  goToPage(targetPage: number) {
    if (targetPage < 1 || targetPage > this.totalPages() || targetPage === this.page()) {
      return;
    }
    this.loadVehicles(targetPage);
  }

  search(value: string) {
    this.keyword.set(value);
  }

  isRegistered(name: string): boolean {
    return name.toLowerCase().startsWith('regist');
  }

  openCreate() {
    this.editing.set(null);
    this.drawerOpen.set(true);
  }

  openEdit(vehicle: Vehicle) {
    this.editing.set(vehicle);
    this.drawerOpen.set(true);
  }

  closeDrawer() {
    this.drawerOpen.set(false);
  }

  onSaved() {
    this.drawerOpen.set(false);
    this.loadVehicles();
  }

  remove(vehicle: Vehicle) {
    const isConfirmed = confirm(`Are you sure you want to delete ${vehicle.make.name} ${vehicle.model.name}?`);
    if (!isConfirmed) return;

    this.vehicleService.delete(vehicle.id).subscribe({
      next: () => {
        this.loadVehicles();
      },
    });
  }
}