import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { AuthResponse } from '../models/model';
import { JsonPipe } from '@angular/common';

@Injectable({
  providedIn: 'root'
})

export class AuthService {
private apiUrl = 'http://localhost:5165/api/Auth';

  constructor(private http: HttpClient) {}

  //  تسجيل الدخول
  login(credentials: { username: string; password: string }): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, credentials).pipe(
      tap(response => {
        
        if (response && response.token) {
          localStorage.setItem('auth_token', response.token);
        }
      })
    );
  }

  forgotPassword(email: string): Observable<any> {
  const clientUri = 'http://localhost:4200/reset-password';
  return this.http.post(`${this.apiUrl}/forgotpassword`, { email, clientUri });
}

resetPassword(model: { email: string; password: string }): Observable<any> {
  return this.http.post(`${this.apiUrl}/resetpassword`, model);
}

isAdmin(): boolean {
  const token = localStorage.getItem('auth_token'); 
  if (!token) return false;

  try {
    const payload = JSON.parse(atob(token.split('.')[1]));    
    return payload.role === 'Admin' || payload.role === 'SuperAdmin';
  } catch {
    return false;
  }
}

  logout() {
    localStorage.removeItem('auth_token');
  }

  

  isLoggedIn(): boolean {
    return !!localStorage.getItem('auth_token');
  }
}