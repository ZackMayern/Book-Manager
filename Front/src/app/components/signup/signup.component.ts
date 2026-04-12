import { UserService } from './../../services/users/user.service';
import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { User } from '../../models/user';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-signup',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './signup.component.html',
  styleUrl: './signup.component.scss'
})
export class SignupComponent implements OnInit {
  private readonly userService: UserService = inject(UserService);
  private readonly router: Router = inject(Router);
  private readonly destroyRef: DestroyRef = inject(DestroyRef);

  public isAdmin: boolean = false;

  public signupForm!: FormGroup<{
    email: FormControl<string>;
    firstName: FormControl<string>;
    lastName: FormControl<string>;
    password: FormControl<string>;
  }>;

  public ngOnInit(): void {
    this.createForm();
  }

  public createForm(): void {
    this.signupForm = new FormGroup({
      email: new FormControl({ value: '', disabled: false }, { nonNullable: true }),
      firstName: new FormControl({ value: '', disabled: false }, { nonNullable: true }),
      lastName: new FormControl({ value: '', disabled: false }, { nonNullable: true }),
      password: new FormControl({ value: '', disabled: false }, { nonNullable: true }),
    });
  }

  private convertFormToModel(): User {
    return {
      id: '',
      email: this.signupForm.controls.email.value,
      firstName: this.signupForm.controls.firstName.value,
      lastName: this.signupForm.controls.lastName.value,
      password: this.signupForm.controls.password.value,
      isAdmin: false
    }
  }

  public submit(): void {
    const model = this.convertFormToModel();
    let http$: Observable<unknown> = this.userService.signup(model);

    http$.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: () => {
        console.info(`User registration was successful!`);
        this.router.navigate(['/login']);
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
