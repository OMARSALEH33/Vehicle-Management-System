import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { forkJoin } from 'rxjs';

import { MakeService } from '../services/makeservice';
import { Make } from '../models/model';
import { RouterLink } from '@angular/router';
import { AddMakes } from '../add-makes/add-makes';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

interface Slice {
  id: number;
  name: string;
}

@Component({
  selector: 'app-makes',
  standalone: true,
  imports: [RouterLink, AddMakes, CommonModule, FormsModule],
  templateUrl: './makes.html',
  styleUrl: './makes.css',
})
export class Makes implements OnInit {
  private readonly makeService = inject(MakeService);

  makes = signal<Make[]>([]);
  keyword = signal<string>('');
  loading = signal(true);

  editing = signal<Make | null>(null);
  drawerOpen = signal(false);

  page = signal<number>(1);
  pageSize = signal<number>(5);
  totalCount = signal<number>(0);

  totalPages = computed(() =>
    Math.max(1, Math.ceil(this.totalCount() / this.pageSize()))
  );

  startItem = computed(() =>
    this.totalCount() === 0 ? 0 : (this.page() - 1) * this.pageSize() + 1
  );

  endItem = computed(() =>
    Math.min(this.page() * this.pageSize(), this.totalCount())
  );

  slices = computed<Slice[]>(() => {
    const allMakes = this.makes();
    return allMakes.map((make) => ({
      id: make.id,
      name: make.name,
    }));
  });

  filtered = computed(() => {
    const query = this.keyword().trim().toLowerCase();
    if (!query) return this.makes();

    return this.makes().filter(
      (make) =>
        make.name.toLowerCase().includes(query) ||
        make.code.toLowerCase().includes(query)
    );
  });

  ngOnInit() {
    this.loadMakesData();
  }

  loadMakesData() {
    this.loading.set(true);
    forkJoin({
      makes: this.makeService.getMakes(this.page(), this.pageSize()),
    }).subscribe({
      next: (response) => {
        this.makes.set(response.makes.items);
        this.totalCount.set(response.makes.totalCount);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
      },
    });
  }

  loadMakes(targetPage = this.page()) {
    this.page.set(targetPage);
    this.loading.set(true);
    this.makeService.getMakes(this.page(), this.pageSize()).subscribe({
      next: (data) => {
        this.makes.set(data.items);
        this.totalCount.set(data.totalCount);
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
    this.loadMakes(targetPage);
  }

  search(value: string) {
    this.keyword.set(value);
  }

  openDrawer(make: Make | null) {
    this.editing.set(make);
    this.drawerOpen.set(true);
  }

  closeDrawer() {
    this.drawerOpen.set(false);
  }

  openCreate() {
    this.editing.set(null);
    this.drawerOpen.set(true);
  }

  openEdit(make: Make) {
    this.editing.set(make);
    this.drawerOpen.set(true);
  }

  onSaved() {
    this.drawerOpen.set(false);
    this.loadMakes(this.page());
  }

  remove(make: Make) {
    const isConfirmed = confirm(`Are you sure you want to delete ${make.name}?`);
    if (!isConfirmed) return;

    this.makeService.delete(make.id).subscribe({
      next: () => {
        const nextPage =
          this.makes().length === 1 && this.page() > 1
            ? this.page() - 1
            : this.page();
        this.loadMakes(nextPage);
      },
    });
  }
}