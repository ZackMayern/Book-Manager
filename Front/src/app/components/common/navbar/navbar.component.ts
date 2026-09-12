import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../services/auth/auth.service';
import { UserService } from '../../../services/users/user.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent implements OnInit {
  private readonly router: Router = inject(Router);
  private readonly authService: AuthService = inject(AuthService);
  private readonly userService: UserService = inject(UserService);

  public collapsed: boolean = true;
  public isLoggedIn$ = this.authService.isAuthenticated$;
  public currentUser$ = this.authService.currentUser$;

  public ngOnInit(): void {
    // Subscribe to authentication state if needed for other logic
  }

  public navigateToLogin(){
    this.router.navigateByUrl('/login');
  }

  public navigateToSignUp(){
    this.router.navigateByUrl('/signup');
  }

  public logout(): void {
    this.userService.logout().subscribe({
      next: () => this.router.navigateByUrl('/login'),
      error: () => this.router.navigateByUrl('/login')
    });
  }
}
