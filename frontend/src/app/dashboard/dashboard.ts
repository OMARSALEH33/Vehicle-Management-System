import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { forkJoin } from 'rxjs';
import { MakeService } from '../services/makeservice';
import { ModelService } from '../services/modelservices';
import { VehicleService } from '../services/vehicleservices';
import { Make, VehicleModel } from '../models/model';
import { RouterOutlet } from '@angular/router';

interface Slice {
  id: number;
  name: string;
  models: number;
  color: string;
  path: string;
}

@Component({
  selector: 'app-dashboard',
  imports: [RouterOutlet],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard implements OnInit {
  
  private readonly makeService = inject(MakeService);
  private readonly modelService = inject(ModelService);
  private readonly vehicleService = inject(VehicleService);

  makes = signal<Make[]>([]);
  models = signal<VehicleModel[]>([]);
  vehiclesCount = signal(0);
  hovered = signal<Slice | null>(null);
  makesPage = signal<number>(1);
  modelsPage = signal<number>(1);
  vehiclePage = signal<number>(1);
  makesPageSize = signal<number>(5);
  modelsPageSize = signal<number>(5);
  vehiclePageSize = signal<number>(5);
  totalMakesCount = signal<number>(0);

  totalModels = computed(() => this.models().length);

  slices = computed<Slice[]>(() => {
    const allMakes = this.makes();
    const allModels = this.models();

    
    const counts = allMakes.map((make) => ({
      id: make.id,
      name: make.name,
      models: allModels.filter((model) => model.makeId === make.id).length,
    }));

    const totalModelsCount = counts.reduce((sum, current) => sum + current.models, 0);
    let startAngle = -Math.PI / 2;

    return counts.map((sliceData, index) => {
      const angle = totalModelsCount ? (sliceData.models / totalModelsCount) * 2 * Math.PI : 0;
      const endAngle = startAngle + angle;
      const path = this.calculateArcPath(startAngle, endAngle, angle);
      
      startAngle = endAngle;
      
      const lightness = 35 + index * (45 / Math.max(counts.length - 1, 1));
      const color = `hsl(217, 85%, ${lightness}%)`;

      return { ...sliceData, color, path };
    });
  });

  ngOnInit() {
    this.loadDashboardData();
  }

 
 private loadDashboardData() {
  forkJoin({
    makesPageResult: this.makeService.getMakes(this.makesPage(), this.makesPageSize()),
    modelsPageResult: this.modelService.getModels(undefined, this.modelsPage(), this.modelsPageSize()),
    vehicles: this.vehicleService.getVehicles(),
  }).subscribe({
    next: ({ makesPageResult, modelsPageResult, vehicles }) => {
      this.makes.set(makesPageResult.items);
      this.totalMakesCount.set(makesPageResult.totalCount);
      this.models.set(modelsPageResult.items);
      this.vehiclesCount.set(vehicles.data.totalCount);
    },
  });
}

loadMakesPage(targetPage = this.makesPage()) {
    this.makesPage.set(targetPage);
    this.makeService.getMakes(this.makesPage(), this.makesPageSize()).subscribe({
      next: (res) => {
        this.makes.set(res.items);
        this.totalMakesCount.set(res.totalCount);
      },
    });
  }

  goToMakesPage(targetPage: number) {
    if (targetPage < 1 || targetPage > this.makesTotalPages() || targetPage === this.makesPage()) {
      return;
    }
    this.loadMakesPage(targetPage);
  }

  makesTotalPages = computed(() =>
  Math.max(1, Math.ceil(this.totalMakesCount() / this.makesPageSize()))
  );

  makesStartItem = computed(() =>
  this.totalMakesCount() === 0 ? 0 : (this.makesPage() - 1) * this.makesPageSize() + 1
  );

  makesEndItem = computed(() =>
  Math.min(this.makesPage() * this.makesPageSize(), this.totalMakesCount())
  );

  private calculateArcPath(start: number, end: number, angle: number): string {
    if (angle === 0) return '';
    if (angle >= 2 * Math.PI - 0.0001) {
      return 'M 100 10 A 90 90 0 1 1 100 190 A 90 90 0 1 1 100 10 Z';
    }

    const x1 = 100 + 90 * Math.cos(start);
    const y1 = 100 + 90 * Math.sin(start);
    const x2 = 100 + 90 * Math.cos(end);
    const y2 = 100 + 90 * Math.sin(end);
    const largeArcFlag = angle > Math.PI ? 1 : 0;

    return `M 100 100 L ${x1} ${y1} A 90 90 0 ${largeArcFlag} 1 ${x2} ${y2} Z`;
  }
}