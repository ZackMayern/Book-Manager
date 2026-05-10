import { inject, Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse, HttpResponse } from '@angular/common/http';
import { catchError, Observable, switchMap, tap, throwError } from 'rxjs';
import { UserService } from '../services/users/user.service';
import { AlertHelper } from '../helpers/alert-helper';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private readonly userService: UserService = inject(UserService);
  private readonly alertHelper: AlertHelper = inject(AlertHelper);

  public intercept(req: HttpRequest<any>, next: HttpHandler):
    Observable<HttpEvent<any>> {

    const token = this.userService.getToken();

    let authReq = req;
    if (token) {
      authReq = req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });
    }

    return next.handle(authReq).pipe(
      catchError((err: HttpErrorResponse) => {
        if (err.status === 401) {
          return this.userService.refreshToken().pipe(
            switchMap(() => {
              const newReq = req.clone({
                setHeaders: {
                  Authorization: `Bearer ${this.userService.getToken()}`
                }
              });
              return next.handle(newReq);
            })
          );
        }
        return throwError(() => err);
      }),
      tap((event: HttpEvent<unknown>) => {
        if (!(event instanceof HttpResponse) || !this.shouldShowSuccess(req)) {
          return;
        }

        this.alertHelper.showRequestSuccess(req.method, req.url);
      }),
      catchError((err: HttpErrorResponse) => {
        if (this.shouldShowFailure(req)) {
          this.alertHelper.showRequestError(err);
        }

        return throwError(() => err);
      })
    );
  }

  private shouldShowSuccess(req: HttpRequest<unknown>): boolean {
    if (!this.isApiRequest(req.url) || this.isRefreshRequest(req.url)) {
      return false;
    }

    const method = req.method.toUpperCase();
    if (method === 'PUT' || method === 'DELETE') {
      return true;
    }

    return this.isLoginRequest(req.url) || this.isSignupRequest(req.url);
  }

  private shouldShowFailure(req: HttpRequest<unknown>): boolean {
    if (!this.isApiRequest(req.url) || this.isRefreshRequest(req.url)) {
      return false;
    }

    const method = req.method.toUpperCase();
    if (method === 'GET' || method === 'PUT' || method === 'DELETE') {
      return true;
    }

    return this.isLoginRequest(req.url) || this.isSignupRequest(req.url);
  }

  private isApiRequest(url: string): boolean {
    return url.includes('/api/');
  }

  private isRefreshRequest(url: string): boolean {
    return url.includes('/api/auth/refresh');
  }

  private isLoginRequest(url: string): boolean {
    return url.includes('/api/auth/login');
  }

  private isSignupRequest(url: string): boolean {
    return url.includes('/api/auth/signup');
  }
}