import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_URL } from '../app.config';
import { Feature } from '../models/model';

@Injectable({ providedIn: 'root' })
export class FeatureService {
  private readonly http = inject(HttpClient);

  getFeatures(): Observable<Feature[]> {
    return this.http.get<Feature[]>(`${API_URL}/features`);
  }
}