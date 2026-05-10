import { inject, Injectable } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root'
})
export class AlertHelper {
  private readonly toastr: ToastrService = inject(ToastrService);

  public showSuccess(message: string, title?: string) {
    this.toastr.success(message, title);
  }

  public showError(message: string, title?: string) {
    this.toastr.error(message, title);
  }

  public showInfo(message: string, title?: string) {
    this.toastr.info(message, title);
  }

  public showWarning(message: string, title?: string) {
    this.toastr.warning(message, title);
  }

  public showRequestSuccess(method: string, url: string): void {
    this.showSuccess(this.getSuccessMessage(method, url), 'Success');
  }

  public showRequestError(error: HttpErrorResponse): void {
    this.showError(this.getErrorMessage(error), 'Error');
  }

  private isLoginRequest(url: string): boolean {
    return url.includes('/api/auth/login');
  }

  private isSignupRequest(url: string): boolean {
    return url.includes('/api/auth/signup');
  }

  private getSuccessMessage(method: string, url: string): string {
    const methodName = method.toUpperCase();

    if (this.isLoginRequest(url)) {
      return 'Login successful.';
    }

    if (this.isSignupRequest(url)) {
      return 'Registration successful.';
    }

    if (methodName === 'PUT') {
      return 'Updated successfully.';
    }

    if (methodName === 'DELETE') {
      return 'Deleted successfully.';
    }

    return 'Request completed successfully.';
  }

  private getErrorMessage(err: HttpErrorResponse): string {
    if (typeof err.error === 'string' && err.error.trim()) {
      return err.error;
    }

    if (err.error?.message) {
      return err.error.message;
    }

    if (err.message) {
      return err.message;
    }

    return 'Request failed. Please try again.';
  }
}