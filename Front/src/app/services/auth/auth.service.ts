import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { User } from '../../models/user';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  private isAuthenticatedSubject = new BehaviorSubject<boolean>(false);
  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  private accessToken: string | null = null;

  constructor() {
    this.initializeAuth();
  }

  private initializeAuth(): void {
    const storedToken = localStorage.getItem('accessToken');
    const storedUser = localStorage.getItem('currentUser');
    
    if (storedToken && storedUser) {
      this.accessToken = storedToken;
      try {
        const user = JSON.parse(storedUser);
        this.currentUserSubject.next(user);
        this.isAuthenticatedSubject.next(true);
      } catch (error) {
        console.error('Failed to parse stored user:', error);
        this.clearAuth();
      }
    }
  }

  public setAuthData(accessToken: string, refreshToken: string, user: User): void {
    this.accessToken = accessToken;
    localStorage.setItem('accessToken', accessToken);
    localStorage.setItem('refreshToken', refreshToken);
    localStorage.setItem('currentUser', JSON.stringify(user));
    
    this.currentUserSubject.next(user);
    this.isAuthenticatedSubject.next(true);
  }

  public getToken(): string | null {
    return this.accessToken || localStorage.getItem('accessToken');
  }

  public isLoggedIn(): boolean {
    return this.getToken() !== null;
  }

  public getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }

  public getRoles(): string[] {
    const token = this.getToken();
    if (!token) {
      return [];
    }

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const roleClaim = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

      if (Array.isArray(roleClaim)) {
        return roleClaim;
      }

      if (typeof roleClaim === 'string') {
        return [roleClaim];
      }

      return [];
    } catch {
      return [];
    }
  }

  public hasRole(role: string): boolean {
    return this.getRoles().includes(role);
  }

  public setAccessToken(token: string): void {
    this.accessToken = token;
    localStorage.setItem('accessToken', token);
  }

  public logout(): void {
    this.clearAuth();
  }

  private clearAuth(): void {
    this.accessToken = null;
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('currentUser');
    
    this.currentUserSubject.next(null);
    this.isAuthenticatedSubject.next(false);
  }
}
