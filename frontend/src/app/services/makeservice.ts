import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_URL } from '../app.config';
import { Make, PagedResult, ServiceResult } from '../models/model';

@Injectable({ providedIn: 'root' })
export class MakeService {
  private readonly http = inject(HttpClient);

  getMakes(page = 1, pageSize = 5): Observable<PagedResult<Make>> {
  return this.http.get<PagedResult<Make>>(`${API_URL}/makes`, {
    params: { page, pageSize }
  });
}
  getMake(id: number): Observable<Make> {
      return this.http.get<Make>(`${API_URL}/makes/${id}`);
    }
    create(make: Omit<Make, 'id'>): Observable<Make> {
      return this.http.post<Make>(`${API_URL}/makes`, make);
    }
  
    updateMake(id: number, make: Omit<Make, 'id'>): Observable<Make> {
      return this.http.put<Make>(`${API_URL}/makes/${id}`, make);
    }
  
    delete(id: number): Observable<void> {
      return this.http.delete<void>(`${API_URL}/makes/${id}`);
    }
}