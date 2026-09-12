import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { ReactiveFormsModule, FormControl, FormGroup, Validators } from '@angular/forms';
import { RequestsService } from '../../services/requests/requests.service';
import { BookRequest } from '../../models/book-request';
import { CommonModule } from '@angular/common';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { AgGridAngular } from 'ag-grid-angular';
import { AllCommunityModule, ClientSideRowModelModule, DateFilterModule, GridOptions, ModuleRegistry, NumberFilterModule, TextFilterModule, themeBalham } from 'ag-grid-community';

ModuleRegistry.registerModules([
  ClientSideRowModelModule,
  AllCommunityModule,
  TextFilterModule,
  NumberFilterModule,
  DateFilterModule
]);

@Component({
  selector: 'app-requests',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, AgGridAngular],
  templateUrl: './requests.component.html',
  styleUrl: './requests.component.scss'
})
export class RequestsComponent implements OnInit {
  private readonly requestsService: RequestsService = inject(RequestsService);
  private readonly destroyRef: DestroyRef = inject(DestroyRef);

  public requests: BookRequest[] = [];
  public waitingForResponse: boolean = false;
  public theme = themeBalham;
  public rowHeight: number = 36;

  public gridOptions: GridOptions<BookRequest> = {
    columnDefs: [
      { field: 'userId', headerName: 'User ID', filter: true, floatingFilter: true },
      { field: 'title', headerName: 'Title', filter: true, floatingFilter: true },
      { field: 'author', headerName: 'Author', filter: true, floatingFilter: true },
      { field: 'reason', headerName: 'Reason', filter: true, floatingFilter: true },
      {
        field: 'status',
        headerName: 'Status',
        filter: true,
        floatingFilter: true
      },
      { field: 'adminMessage', headerName: 'Admin Message', filter: true, floatingFilter: true },
      {
        field: 'createdAt',
        headerName: 'Created',
        filter: 'agDateColumnFilter',
        floatingFilter: true,
        valueFormatter: ({ value }) => value ? new Date(value).toLocaleDateString() : ''
      }
    ],
    rowData: [],
    pagination: true,
    paginationPageSize: 10,
    autoSizeStrategy: {
      type: 'fitGridWidth'
    }
  };

  public getCurrentUserId(): string {
    const currentUser = localStorage.getItem('currentUser');
    return currentUser ? JSON.parse(currentUser).id : '';
  }

  public requestForm!: FormGroup<{
    title: FormControl<string>;
    author: FormControl<string>;
    reason: FormControl<string>;
  }>;

  public ngOnInit(): void {
    this.createForm();
    this.loadMine();
  }

  private createForm(): void {
    this.requestForm = new FormGroup({
      title: new FormControl('', { nonNullable: true, validators: Validators.required }),
      author: new FormControl('', { nonNullable: true, validators: Validators.required }),
      reason: new FormControl('', { nonNullable: true, validators: Validators.required })
    });
  }

  public submitRequest(): void {
    if (this.requestForm.invalid) {
      this.requestForm.markAllAsTouched();
      return;
    }

    this.waitingForResponse = true;
    const { title, author, reason } = this.requestForm.getRawValue();

    this.requestsService.create(title, author, reason, this.getCurrentUserId(), new Date()).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: () => {
        this.waitingForResponse = false;
        this.requestForm.reset();
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
