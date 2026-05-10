import { Component, DestroyRef, inject, input, OnInit, signal } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { CommonStoreService } from '../../../store/common-store.service';
import { combineLatest, Observable } from 'rxjs';
import { BorrowRecord } from '../../../models/borrow-record';
import { BookRequest } from '../../../models/book-request';
import { BorrowService } from '../../../services/borrow/borrow.service';
import { RequestsService } from '../../../services/requests/requests.service';
import { UserService } from '../../../services/users/user.service';
import { User } from '../../../models/user';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { AgGridAngular } from 'ag-grid-angular';
import { AllCommunityModule, ClientSideRowModelModule, ColDef, DateFilterModule, GridOptions, ModuleRegistry, NumberFilterModule, TextFilterModule, themeBalham } from 'ag-grid-community';

ModuleRegistry.registerModules([
  ClientSideRowModelModule,
  AllCommunityModule,
  TextFilterModule,
  NumberFilterModule,
  DateFilterModule
]);

@Component({
  selector: 'app-monitoring',
  standalone: true,
  imports: [AsyncPipe, AgGridAngular],
  templateUrl: './monitoring.component.html',
  styleUrl: './monitoring.component.scss'
})
export class MonitoringComponent implements OnInit {
  private readonly commonStore: CommonStoreService = inject(CommonStoreService);
  private readonly borrowService: BorrowService = inject(BorrowService);
  private readonly requestsService: RequestsService = inject(RequestsService);
  private readonly userService: UserService = inject(UserService);
  private readonly destroyRef: DestroyRef = inject(DestroyRef);

  public isAdmin = input<boolean>(false);
  public theme = themeBalham;
  public rowHeight: number = 36;

  public booksCount$!: Observable<number>;
  public newBooksCount$!: Observable<number>;
  public usedBooksCount$!: Observable<number>;
  public damagedBooksCount$!: Observable<number>;
  public missingBooksCount$!: Observable<number>;

  public usersCount: number = 0;
  public borrowedUsersCount: number = 0;
  public activeBorrowsCount: number = 0;
  public overdueBorrowsCount: number = 0;
  public pendingRequestsCount: number = 0;

  public adminBorrows_ = signal<BorrowRecord[]>([]);
  public myBorrows_ = signal<BorrowRecord[]>([]);
  public pendingRequests_ = signal<BookRequest[]>([]);

  public adminBorrowColDefs: ColDef<BorrowRecord>[] = [
    { field: 'userFullName', headerName: 'User', filter: true, floatingFilter: true },
    { field: 'bookTitle', headerName: 'Book', filter: true, floatingFilter: true },
    {
      field: 'borrowedAt',
      headerName: 'Borrowed At',
      filter: true,
      floatingFilter: true,
      valueFormatter: (params) => new Date(params.value).toLocaleDateString()
    },
    {
      field: 'dueDate',
      headerName: 'Due Date',
      filter: true,
      floatingFilter: true,
      valueFormatter: (params) => new Date(params.value).toLocaleDateString()
    },
    {
      field: 'status',
      headerName: 'Status',
      filter: true,
      floatingFilter: true,
      cellRenderer: (params: any) => this.renderStatusBadge(params.data)
    }
  ];

  public adminBorrowGridOptions: GridOptions<BorrowRecord> = {
    columnDefs: this.adminBorrowColDefs,
    rowData: [],
    pagination: true,
    paginationPageSize: 10,
    autoSizeStrategy: {
      type: 'fitGridWidth'
    }
  };

  public pendingRequestsColDefs: ColDef<BookRequest>[] = [
    { field: 'userFullName', headerName: 'User', filter: true, floatingFilter: true },
    { field: 'title', headerName: 'Title', filter: true, floatingFilter: true },
    { field: 'author', headerName: 'Author', filter: true, floatingFilter: true },
    { field: 'reason', headerName: 'Reason', filter: true, floatingFilter: true },
    {
      field: 'createdAt',
      headerName: 'Created',
      filter: true,
      floatingFilter: true,
      valueFormatter: (params) => new Date(params.value).toLocaleDateString()
    },
    {
      field: 'id',
      headerName: 'Actions',
      sortable: false,
      filter: false,
      cellRenderer: (params: any) => this.renderRequestActions(params.data)
    }
  ];

  public pendingRequestsGridOptions: GridOptions<BookRequest> = {
    columnDefs: this.pendingRequestsColDefs,
    rowData: [],
    pagination: true,
    paginationPageSize: 10,
    autoSizeStrategy: {
      type: 'fitGridWidth'
    }
  };

  public myBorrowsColDefs: ColDef<BorrowRecord>[] = [
    { field: 'bookTitle', headerName: 'Book', filter: true, floatingFilter: true },
    {
      field: 'borrowedAt',
      headerName: 'Borrowed At',
      filter: true,
      floatingFilter: true,
      valueFormatter: (params) => new Date(params.value).toLocaleDateString()
    },
    {
      field: 'dueDate',
      headerName: 'Due Date',
      filter: true,
      floatingFilter: true,
      valueFormatter: (params) => new Date(params.value).toLocaleDateString()
    },
    {
      field: 'status',
      headerName: 'Status',
      filter: true,
      floatingFilter: true,
      cellRenderer: (params: any) => this.renderStatusBadge(params.data)
    },
    {
      field: 'id',
      headerName: 'Actions',
      sortable: false,
      filter: false,
      cellRenderer: (params: any) => this.renderBorrowActions(params.data)
    }
  ];

  public myBorrowsGridOptions: GridOptions<BorrowRecord> = {
    columnDefs: this.myBorrowsColDefs,
    rowData: [],
    pagination: true,
    paginationPageSize: 10,
    autoSizeStrategy: {
      type: 'fitGridWidth'
    }
  };

  public ngOnInit(): void {
    this.booksCount$ = this.commonStore.booksCount$;
    this.newBooksCount$ = this.commonStore.newBooksCount$;
    this.usedBooksCount$ = this.commonStore.usedBooksCount$;
    this.damagedBooksCount$ = this.commonStore.damagedBooksCount$;
    this.missingBooksCount$ = this.commonStore.missingBooksCount$;

    if (this.isAdmin()) {
      this.loadAdminData();
      return;
    }

    this.loadMyBorrows();
  }

  public isOverdue(dueDate: string): boolean {
    return new Date(dueDate) < new Date();
  }

  public renderStatusBadge(data: BorrowRecord): HTMLSpanElement {
    const span = document.createElement('span');
    const isOverdue = this.isOverdue(data.dueDate) && data.status !== 'Returned';
    const badgeClass = isOverdue ? 'badge text-bg-danger' : 'badge text-bg-secondary';
    const badgeText = isOverdue ? 'Overdue' : data.status;
    span.className = badgeClass;
    span.textContent = badgeText;
    return span;
  }

  public renderRequestActions(data: BookRequest): HTMLDivElement {
    const div = document.createElement('div');
    div.className = 'd-flex gap-2';

    const approveBtn = document.createElement('button');
    approveBtn.className = 'btn btn-success btn-sm';
    approveBtn.textContent = 'Approve';
    approveBtn.addEventListener('click', () => this.approveRequest(data.id));

    const rejectBtn = document.createElement('button');
    rejectBtn.className = 'btn btn-danger btn-sm';
    rejectBtn.textContent = 'Reject';
    rejectBtn.addEventListener('click', () => this.rejectRequest(data.id));

    div.appendChild(approveBtn);
    div.appendChild(rejectBtn);
    return div;
  }

  public renderBorrowActions(data: BorrowRecord): HTMLDivElement {
    const div = document.createElement('div');
    div.className = 'd-flex gap-2';
    const isReturned = data.status === 'Returned';

    const renewBtn = document.createElement('button');
    renewBtn.className = 'btn btn-outline-info btn-sm';
    renewBtn.textContent = 'Renew';
    renewBtn.disabled = isReturned;
    renewBtn.addEventListener('click', () => this.renewBorrow(data.id));

    const returnBtn = document.createElement('button');
    returnBtn.className = 'btn btn-outline-warning btn-sm';
    returnBtn.textContent = 'Return';
    returnBtn.disabled = isReturned;
    returnBtn.addEventListener('click', () => this.returnBorrow(data.id));

    div.appendChild(renewBtn);
    div.appendChild(returnBtn);
    return div;
  }

  public renewBorrow(id: string): void {
    this.borrowService.renew(id).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: () => this.loadMyBorrows(),
      error: (error: Error) => console.error(error.message)
    });
  }

  public returnBorrow(id: string): void {
    this.borrowService.return(id).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: () => this.loadMyBorrows(),
      error: (error: Error) => console.error(error.message)
    });
  }

  public approveRequest(id: string): void {
    const message = window.prompt('Approval message for the requester:', 'Your request has been approved.');
    if (message === null) {
      return;
    }

    this.requestsService.approve(id, message).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: () => this.loadAdminData(),
      error: (error: Error) => console.error(error.message)
    });
  }

  public rejectRequest(id: string): void {
    const message = window.prompt('Rejection message for the requester:', 'Your request has been rejected.');
    if (message === null) {
      return;
    }

    this.requestsService.reject(id, message).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: () => this.loadAdminData(),
      error: (error: Error) => console.error(error.message)
    });
  }

  private loadMyBorrows(): void {
    this.borrowService.getMine().pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (records: BorrowRecord[]) => {
        this.myBorrows_.set(records);
        this.activeBorrowsCount = records.filter(r => r.status !== 'Returned').length;
        this.overdueBorrowsCount = records.filter(r => r.status !== 'Returned' && this.isOverdue(r.dueDate)).length;
      },
      error: (error: Error) => console.error(error.message)
    });
  }

  private loadAdminData(): void {
    combineLatest([
      this.userService.getAll(),
      this.borrowService.getAll(),
      this.requestsService.getPending()
    ]).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: ([users, borrows, requests]: [User[], BorrowRecord[], BookRequest[]]) => {
        this.usersCount = users.length;
        this.adminBorrows_.set(borrows);
        this.pendingRequests_.set(requests);
        this.pendingRequestsCount = requests.length;

        const activeBorrows = borrows.filter(r => r.status !== 'Returned');
        this.activeBorrowsCount = activeBorrows.length;
        this.overdueBorrowsCount = activeBorrows.filter(r => this.isOverdue(r.dueDate)).length;
        this.borrowedUsersCount = new Set(activeBorrows.map(r => r.userId)).size;
      },
      error: (error: Error) => console.error(error.message)
    });
  }
}
