import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_URL } from '../app.config';
import { PagedResult, SaveVehicle, Vehicle, ServiceResult } from '../models/model';


@Injectable({ providedIn: 'root' })
export class VehicleService {
  private readonly http = inject(HttpClient);
  private readonly url = `${API_URL}/vehicles`;

 getVehicles(page = 1, pageSize = 5): Observable<ServiceResult<PagedResult<Vehicle>>> {
    const params = new HttpParams()
      .set('page', page)
      .set('pageSize', pageSize);

    return this.http.get<ServiceResult<PagedResult<Vehicle>>>(this.url, { params });
  }

  getVehicle(id: number): Observable<Vehicle> {
    return this.http.get<Vehicle>(`${this.url}/${id}`);
  }

  create(vehicle: SaveVehicle): Observable<Vehicle> {
    return this.http.post<Vehicle>(this.url, vehicle);
  }

  update(id: number, vehicle: SaveVehicle): Observable<Vehicle> {
    return this.http.put<Vehicle>(`${this.url}/${id}`, vehicle);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}