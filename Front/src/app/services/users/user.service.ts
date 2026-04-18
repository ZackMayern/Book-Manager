import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { catchError, Observable, of, tap } from 'rxjs';
import { User } from '../../models/user';
import { createHttpParams } from '../../helpers/http-utils';
import { ArgumentNullException } from '../../helpers/argument-helper';
import { AuthService } from '../auth/auth.service';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly userUrl = '/api/users';
  private readonly authUrl = '/api/auth';
  private readonly httpClient: HttpClient = inject(HttpClient);
  private readonly authService: AuthService = inject(AuthService);
  
  public getAll(): Observable<User[]> {
    return this.httpClient.get<User[]>(`${this.userUrl}/getAll`).pipe(
      catchError(() => {
        return of([] as User[])
      })
    );
  }

  public login(credentials: User): Observable<any> {
    return this.httpClient.post<any>(`${this.authUrl}/login`, credentials).pipe(
      tap(res => {
        this.authService.setAuthData(res.value.accessToken, res.value.refreshToken, res.value.user);
      }),
      catchError((error) => {
        console.error('Login failed:', error);
        throw error;
      })
    );
  }

  public signup(user: User): Observable<unknown> {
    this.validateWhenAddorUpdate(user);
    return this.httpClient.post(`${this.authUrl}/signup`, user);
  }

  public update(user: User): Observable<unknown> {
    return this.httpClient.put(`${this.userUrl}/update`, user);
  }

  public delete(user: User): Observable<unknown> {
    const params: HttpParams = createHttpParams({ id: user.id });
    return this.httpClient.delete(`${this.userUrl}/delete`, { params });
  }

  public refreshToken() {
    const refreshToken = localStorage.getItem('refreshToken');
    return this.httpClient.post<any>(`${this.authUrl}/refresh`, { refreshToken }).pipe(
      tap(res => {
        this.authService.setAccessToken(res.accessToken);
      })
    );
  }

  public getToken() {
    return this.authService.getToken();
  }

  public isLoggedIn(): boolean {
    return this.authService.isLoggedIn();
  }

  public logout() {
    this.authService.logout();
  }

  public validateWhenAddorUpdate(userData: User): void {
    ArgumentNullException.ThrowIfNullOrUndefined(userData.email, 'email is invalid!');
    ArgumentNullException.ThrowIfNullOrUndefined(userData.firstName, 'firstName is invalid!');
    ArgumentNullException.ThrowIfNullOrUndefined(userData.lastName, 'lastName is invalid!');
    ArgumentNullException.ThrowIfNullOrUndefined(userData.password, 'password is invalid!');
    ArgumentNullException.ThrowIfNullOrUndefined(userData.isAdmin, 'isAdmin is invalid!');
  }
  
  public validateWhenUpdateOrDelete(userData: User): void {
    ArgumentNullException.ThrowIfNullOrUndefined(userData.id, 'id is invalid!');
  }
}
