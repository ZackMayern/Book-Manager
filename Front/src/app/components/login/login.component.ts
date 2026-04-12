import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { UserService } from '../../services/users/user.service';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { HttpErrorResponse } from '@angular/common/http';
import { User } from '../../models/user';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent implements OnInit {
  private readonly userService: UserService = inject(UserService);
  private readonly router: Router = inject(Router);
  private readonly destroyRef: DestroyRef = inject(DestroyRef);
  
  public loginForm!: FormGroup<{
    email: FormControl<string>;
    password: FormControl<string>;
  }>;

  public createForm(): void {
    this.loginForm = new FormGroup({
      email: new FormControl({ value: '', disabled: false }, { nonNullable: true }),
      password: new FormControl({ value: '', disabled: false }, { nonNullable: true }),
    });
  }

  public ngOnInit(): void {
    this.createForm();
  }

  private convertFormToModel(): User {
    return {
      email: this.loginForm.controls.email.value,
      password: this.loginForm.controls.password.value,
    }
  }

  public submit(): void {
    const model = this.convertFormToModel();
    let http$: Observable<unknown> = this.userService.login(model);

    http$.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
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
