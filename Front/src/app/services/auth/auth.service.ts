import { inject, Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { User } from '../../models/user';
import { AlertHelper } from '../../helpers/alert-helper';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly alertHelper: AlertHelper = inject(AlertHelper);
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  private isAuthenticatedSubject = new BehaviorSubject<boolean>(false);
  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  private accessToken: string | null = null;

  constructor() {
    this.initializeAuth();
  }

  private initializeAuth(): void {
    const storedUser = localStorage.getItem('currentUser');

    if (storedUser) {
      try {
        const user = this.withoutPassword(JSON.parse(storedUser) as User);
        this.currentUserSubject.next(user);
      } catch (error) {
        console.error('Failed to parse stored user:', error);
        localStorage.removeItem('currentUser');
      }
    }
  }

  public setAuthData(accessToken: string, user: User): void {
    const safeUser = this.withoutPassword(user);

    this.accessToken = accessToken;
    localStorage.setItem('currentUser', JSON.stringify(safeUser));

    this.currentUserSubject.next(safeUser);
    this.isAuthenticatedSubject.next(true);
  }

  public getToken(): string | null {
    return this.accessToken;
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
  }

  public setCurrentUser(user: User): void {
    const safeUser = this.withoutPassword(user);
    localStorage.setItem('currentUser', JSON.stringify(safeUser));
    this.currentUserSubject.next(safeUser);
    this.isAuthenticatedSubject.next(true);
  }

  public logout(): void {
    try {
      this.clearAuth();
      this.alertHelper.showSuccess('Logout successful.', 'Success');
    } catch {
      this.alertHelper.showError('Logout failed. Please try again.', 'Error');
    }
  }

  private clearAuth(): void {
    this.accessToken = null;
    localStorage.removeItem('currentUser');

    this.currentUserSubject.next(null);
    this.isAuthenticatedSubject.next(false);
  }

  private withoutPassword(user: User): Omit<User, 'password'> {
    const { password: _password, ...safeUser } = user;
    return safeUser;
  }
}
