import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { UserService } from '../../services/users/user.service';
import { Router } from '@angular/router';
import { debounceTime, Observable } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { HttpErrorResponse } from '@angular/common/http';
import { form, FormField } from '@angular/forms/signals';
import { Login } from './login';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormField],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent implements OnInit {
  private readonly userService: UserService = inject(UserService);
  private readonly router: Router = inject(Router);
  private readonly destroyRef: DestroyRef = inject(DestroyRef);
  
  private loginFormModel = signal<Login>({
    email: '',
    password: '',
  });

  loginForm = form(this.loginFormModel);

  public ngOnInit(): void {
  }

  public submit(): void {
    let http$: Observable<unknown> = this.userService.login(this.loginFormModel());

    http$.pipe(takeUntilDestroyed(this.destroyRef), debounceTime(500)).subscribe({
      next: () => {
        console.info(`User login was successful!`);
        this.router.navigate(['/dashboard']);
      },
      error: (error) => {
        if(error instanceof HttpErrorResponse){
          console.error(`${error.status}`, error.message);
        }
        if(error instanceof TypeError){
          console.error(`${error.name}`, error.message);
        }
        else{
          console.error('Error', error.message);
        }
      }
    });
  }
}
