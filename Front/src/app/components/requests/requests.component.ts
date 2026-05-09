import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { ReactiveFormsModule, FormControl, FormGroup, Validators } from '@angular/forms';
import { RequestsService } from '../../services/requests/requests.service';
import { BookRequest } from '../../models/book-request';
import { CommonModule } from '@angular/common';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-requests',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './requests.component.html',
  styleUrl: './requests.component.scss'
})
export class RequestsComponent implements OnInit {
  private readonly requestsService: RequestsService = inject(RequestsService);
  private readonly destroyRef: DestroyRef = inject(DestroyRef);

  public requests: BookRequest[] = [];
  public waitingForResponse: boolean = false;

  public requestForm!: FormGroup<{
    title: FormControl<string>;
    author: FormControl<string>;
    reason: FormControl<string>;
  }>;

  public ngOnInit(): void {
    this.requestForm = new FormGroup({
      title: new FormControl('', { nonNullable: true, validators: Validators.required }),
      author: new FormControl('', { nonNullable: true, validators: Validators.required }),
      reason: new FormControl('', { nonNullable: true, validators: Validators.required })
    });

    this.loadMine();
  }

  public submitRequest(): void {
    if (this.requestForm.invalid) {
      this.requestForm.markAllAsTouched();
      return;
    }

    this.waitingForResponse = true;
    const { title, author, reason } = this.requestForm.getRawValue();

    this.requestsService.create(title, author, reason).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: () => {
        this.waitingForResponse = false;
        this.requestForm.reset({ title: '', author: '', reason: '' });
        this.loadMine();
      },
      error: (error: Error) => {
        this.waitingForResponse = false;
        console.error(error.message);
      }
    });
  }

  private loadMine(): void {
    this.requestsService.getMine().pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (requests: BookRequest[]) => {
        this.requests = requests;
      },
      error: (error: Error) => console.error(error.message)
    });
  }

}
