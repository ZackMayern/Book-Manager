//Create toaster alert helper
import { Injectable } from '@angular/core';
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
}