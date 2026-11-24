import { Component, OnInit, computed, inject, model, signal } from '@angular/core';
import { RouterLink, } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';

import { MakeService } from '../services/makeservice';
import { ModelService } from '../services/modelservices';
import { Model } from '../models/model';
import { Make } from '../models/model';
import { AddModel } from '../add-model/add-model';


interface Slice{
  id: number;
  name: string;
}

@Component({
  standalone: true, 
  imports: [CommonModule, FormsModule,AddModel,RouterLink], 
  selector: 'app-models',
  styleUrl: './models.css',
  templateUrl: './models.html',
})
export class Models implements OnInit{
 

  private readonly modelService = inject(ModelService);
  private readonly makeService = inject(MakeService);

  models = signal<Model[]>([]);
  makes = signal<Make[]>([]);
  keyword = signal<string>('');
  loading = signal(true);
  editing = signal<Model | null>(null);
  drawerOpen = signal(false);

  page = signal<number>(1);
  pageSize = signal<number>(5);
  totalCount = signal<number>(0); 

  getMakeName(makeId: number): string{
    const make = this.makes().find((m) => m.id === makeId);
    return make ? make.name : 'unknown'
  }
  slices = computed<Slice[]>(() => {
    const allModels = this.models();
    return allModels.map((model) => ({
      id: model.id,
      name: model.name,
    }));
  });

  filtered = computed(() => {
    
    const query = this.keyword().trim().toLowerCase();
    if (!query) return this.models();

    return this.models().filter((model) => model.name.toLowerCase().includes(query));
  });

   ngOnInit() {
    this.loadModelsData();
  }
  
  loadModelsData() {
  this.loading.set(true);
  forkJoin({
    models: this.modelService.getModels(undefined, this.page(), this.pageSize()),
    makes: this.makeService.getMakes(1, 1000),
  }).subscribe({
    next: (response) => {
      this.models.set(response.models.items);
      this.totalCount.set(response.models.totalCount);
      this.makes.set(response.makes.items);
      this.loading.set(false);
    },
    error: (err) => {
      console.error('Failed to load models or makes:', err);
      this.loading.set(false);
    },
  });
}

loadModels(targetPage = this.page()) {
  this.page.set(targetPage);
  this.loading.set(true);
  this.modelService.getModels(undefined, this.page(), this.pageSize()).subscribe({
    next: (response) => {
      this.models.set(response.items);
      this.totalCount.set(response.totalCount);
      this.loading.set(false);
    },
    error: (err) => {
      console.error('Failed to load models:', err);
      this.loading.set(false);
    },
  });
}

  goToPage(targetPage: number){
    if (targetPage < 1 || targetPage > this.totalPages() || targetPage === this.page()){
      return;

    }
    this.loadModels(targetPage);
  }

  totalPages = computed(() =>
    Math.max(1, Math.ceil(this.totalCount() / this.pageSize()))
  );

  startItem = computed(() =>
    this.totalCount() === 0 ? 0 : (this.page() - 1) * this.pageSize() + 1
  );

  endItem = computed(() =>
    Math.min(this.page() * this.pageSize(), this.totalCount())
  );

  search(value: string){
    this.keyword.set(value);
  }
  openDrawer(model: Model | null){
    this.editing.set(model);
    this.drawerOpen.set(true);
  }
  closeDrawer(){
    this.drawerOpen.set(false);
  }
  openCreate(){
    this.editing.set(null);
    this.drawerOpen.set(true);
  }
  openEdit(model: Model){
    this.editing.set(model);
    this.drawerOpen.set(true);
  }
  onSaved(){
    this.drawerOpen.set(false);
    this.loadModels();
  }
  remove(model: Model){
    const isCocnfirmed = confirm(`Are you sure you want to delete ${model.name}?`);  
    if(!isCocnfirmed) return;

    this.modelService.delete(model.id).subscribe({
      next: () => {
        const nextPage =
          this.models().length === 1 && this.page() > 1
           ? this.page() - 1
           : this.page();
          this.loadModels();
      },
    });
  }

}
