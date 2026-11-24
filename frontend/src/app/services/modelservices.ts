import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_URL } from '../app.config';
import { Model, PagedResult, ServiceResult, VehicleModel } from '../models/model';



@Injectable({ providedIn: 'root' })
export class ModelService {
 
  private readonly http = inject(HttpClient);

  
  getModels(makeId?: number, page = 1, pageSize = 5): Observable<PagedResult<Model>> {
    let params = new HttpParams()
      .set('page', page)
      .set('pageSize', pageSize);

    if (makeId !== undefined) {
      params = params.set('makeId', makeId);
    }

    return this.http.get<PagedResult<Model>>(`${API_URL}/models`, { params });
  }
  
    delete(id: number): Observable<Model> {
      return this.http.delete<Model>(`${API_URL}/models/${id}`);
    }
     create(model: Omit<Model, 'id'>): Observable<Model> {
      return this.http.post<Model>(`${API_URL}/models`, model);
    }
  
    updateModel(id: number, model: Omit<Model, 'id'>): Observable<Model> {
      return this.http.put<Model>(`${API_URL}/models/${id}`, model);
    }
}