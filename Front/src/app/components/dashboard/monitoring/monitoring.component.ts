import { Component, DestroyRef, inject, input, OnInit } from '@angular/core';
import { CommonStoreService } from '../../../store/common-store.service';
import { combineLatest, Observable } from 'rxjs';
import { CommonModule } from '@angular/common';
import { BorrowRecord } from '../../../models/borrow-record';
import { BookRequest } from '../../../models/book-request';
import { BorrowService } from '../../../services/borrow/borrow.service';
import { RequestsService } from '../../../services/requests/requests.service';
import { UserService } from '../../../services/users/user.service';
import { User } from '../../../models/user';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-monitoring',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './monitoring.component.html',
  styleUrl: './monitoring.component.scss'
})
export class MonitoringComponent implements OnInit{
  private readonly commonStore: CommonStoreService = inject(CommonStoreService);
  private readonly borrowService: BorrowService = inject(BorrowService);
  private readonly requestsService: RequestsService = inject(RequestsService);
  private readonly userService: UserService = inject(UserService);
  private readonly destroyRef: DestroyRef = inject(DestroyRef);

  public isAdmin = input<boolean>(false);

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

  public myBorrows: BorrowRecord[] = [];
  public adminBorrows: BorrowRecord[] = [];
  public pendingRequests: BookRequest[] = [];

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
        this.myBorrows = records;
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
        this.adminBorrows = borrows;
        this.pendingRequests = requests;
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
