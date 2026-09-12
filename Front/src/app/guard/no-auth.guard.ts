import { inject, Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { UserService } from '../services/users/user.service';

@Injectable({
  providedIn: 'root'
})
export class NoAuthGuard implements CanActivate {
  private readonly router: Router = inject(Router);
  private readonly userService: UserService = inject(UserService);

  public canActivate(_route: ActivatedRouteSnapshot, _state: RouterStateSnapshot): Observable<boolean> {
    return this.userService.ensureSession().pipe(
      map(isAuthenticated => {
        if (isAuthenticated) {
          this.router.navigate(['/dashboard']);
        }

        return !isAuthenticated;
      }),
      catchError(() => of(true))
    );
  }
}
