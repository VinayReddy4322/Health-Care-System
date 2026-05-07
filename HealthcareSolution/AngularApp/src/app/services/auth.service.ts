import { HttpClient } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { tap } from 'rxjs';
import { API_BASE_URL } from './api.config';
import { LoginRequest, LoginResponse, RegisterRequest, User } from '../models/user.model';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly tokenKey = 'healthcare_token';
  private readonly userKey = 'healthcare_user';

  readonly currentUser = signal<User | null>(this.readUser());

  constructor(private http: HttpClient) {}

  login(request: LoginRequest) {
    return this.http.post<LoginResponse>(`${API_BASE_URL}/auth/login`, request).pipe(
      tap(response => this.setSession(response))
    );
  }

  register(request: RegisterRequest) {
    return this.http.post<User>(`${API_BASE_URL}/auth/register`, request);
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.userKey);
    this.currentUser.set(null);
  }

  token(): string {
    return localStorage.getItem(this.tokenKey) ?? '';
  }

  isLoggedIn(): boolean {
    return !!this.token() && !!this.currentUser();
  }

  private setSession(response: LoginResponse): void {
    localStorage.setItem(this.tokenKey, response.token);
    localStorage.setItem(this.userKey, JSON.stringify(response.user));
    this.currentUser.set(response.user);
  }

  private readUser(): User | null {
    const raw = localStorage.getItem(this.userKey);
    return raw ? JSON.parse(raw) as User : null;
  }
}
