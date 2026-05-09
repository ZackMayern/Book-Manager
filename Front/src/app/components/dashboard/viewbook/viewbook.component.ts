import { Book } from './../../../models/book';
import { Component, DestroyRef, inject, input, signal } from '@angular/core';
import { combineLatest, Observable, switchMap, tap, timer } from 'rxjs';
import { takeUntilDestroyed, toObservable, toSignal } from '@angular/core/rxjs-interop';
import { BooksService } from '../../../services/books/books.service';
import { AgGridAngular } from 'ag-grid-angular';
import { AllCommunityModule, ClientSideRowModelModule, DateFilterModule, GridOptions, ModuleRegistry, NumberFilterModule, TextFilterModule, themeBalham } from 'ag-grid-community';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ActionsModalComponent } from './actions-modal/actions-modal.component';
import { CommonStoreService } from '../../../store/common-store.service';
import { BorrowService } from '../../../services/borrow/borrow.service';

ModuleRegistry.registerModules([
  ClientSideRowModelModule,
  AllCommunityModule,
  TextFilterModule,
  NumberFilterModule,
  DateFilterModule
]);

@Component({
  selector: 'app-viewbook',
  standalone: true,
  imports: [AgGridAngular],
  templateUrl: './viewbook.component.html',
  styleUrl: './viewbook.component.scss'
})
export class ViewbookComponent {
  private readonly modalService: NgbModal = inject(NgbModal);
  private readonly destroyRef: DestroyRef = inject(DestroyRef);
  private readonly booksService: BooksService = inject(BooksService);
  private readonly commonStore: CommonStoreService = inject(CommonStoreService);
  private readonly borrowService: BorrowService = inject(BorrowService);

  public isAdmin = input<boolean>(false);

  private refreshDataX_ = signal<Date | undefined>(undefined);
  public booksDataX_ = toSignal(this.getBooksData(), { initialValue: undefined });
  public rowHeight: number = 36;
  public theme = themeBalham;

  public getBooksData(): Observable<Book[]> {
    return combineLatest([timer(0, 2 * 60 * 1000), toObservable(this.refreshDataX_)]).pipe(
      takeUntilDestroyed(this.destroyRef),
      switchMap(() => {
        return this.booksService.getAll().pipe(
          tap(books => {
            this.commonStore.updateBookCount(books.length);

            const newBooksCount = books.filter(book => book.condition === 'New').length;
            const usedBooksCount = books.filter(book => book.condition === 'Used').length;
            const damagedBooksCount = books.filter(book => book.condition === 'Damaged').length;

            this.commonStore.updateNewBooksCount(newBooksCount);
            this.commonStore.updateUsedBooksCount(usedBooksCount);
            this.commonStore.updateDamagedBooksCount(damagedBooksCount);
          }),
          takeUntilDestroyed(this.destroyRef)
        );
      })
    );
  }

  public gridOptions: GridOptions<Book> = {
    columnDefs: [
      { headerName: 'Publisher', field: 'publisher', filter: true, floatingFilter: true },
      { headerName: 'Title', field: 'title', filter: true, floatingFilter: true },
      { headerName: 'Author', field: 'author', filter: true, floatingFilter: true },
      { headerName: 'Year Of Publishing', field: 'yearOfPublication', filter: true, floatingFilter: true, sort: 'desc' },
      {
        headerName: 'Availability',
        valueGetter: ({ data }) => (data?.bookCount ?? 0) > 0 ? 'Yes' : 'No',
        filter: true,
        floatingFilter: true
      },
      { headerName: 'Condition', field: 'condition', filter: true, floatingFilter: true },
      { headerName: 'Book Count', field: 'bookCount', filter: true, floatingFilter: true },
      { headerName: 'Actions', cellRenderer: ({ data }: { data: Book }) => this.renderActions(data)}
    ],
    autoSizeStrategy: {
      type: "fitGridWidth",
    },
    pagination: true,
    paginationAutoPageSize: true,
    detailRowAutoHeight: true
  };

  public renderActions(data: Book): HTMLSpanElement {
    const div = document.createElement('div');
    div.className = 'align-center';

    if (this.isAdmin()) {
      const btnUpdate = document.createElement('span');
      btnUpdate.className = 'px-1';
      btnUpdate.innerHTML = '<button class="btn btn-flat-secondary p-0"><i class="material-icons icon-sm">mode</i></button>';
      btnUpdate.addEventListener('click', () => this.showEditModal(data));
      const btnDelete = document.createElement('span');
      btnDelete.className = 'px-1';
      btnDelete.innerHTML = '<button class="btn btn-flat-secondary p-0"><i class="material-icons icon-sm">delete</i></button>';
      btnDelete.addEventListener('click', () => this.showDeleteModal(data));
      div.appendChild(btnUpdate);
      div.appendChild(btnDelete);
      return div;
    }

    const btnBorrow = document.createElement('span');
    btnBorrow.className = 'px-1';
    const isAvailable = (data.bookCount ?? 0) > 0;
    btnBorrow.innerHTML = `<button class="btn btn-sm btn-outline-info" ${isAvailable ? '' : 'disabled'}>Borrow</button>`;
    if (isAvailable && data.id) {
      btnBorrow.addEventListener('click', () => this.borrowBook(data.id!));
    }
    div.appendChild(btnBorrow);

    return div;
  }

  public borrowBook(bookId: string): void {
    this.borrowService.borrow(bookId).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: () => this.refreshDataX_.set(new Date()),
      error: (error: Error) => console.error(error.message)
    });
  }

  public showAddModal(): void {
    const modalRef = this.modalService.open(ActionsModalComponent, { scrollable: true, size: 'md', centered: true });
    const componentInstance = modalRef.componentInstance as ActionsModalComponent;
    componentInstance.actionType = 'Add';
    
    modalRef.result.then(() => {
      this.refreshDataX_.set(new Date());
    })
  }

  public showEditModal(data: Book): void {
    const modalRef = this.modalService.open(ActionsModalComponent, { scrollable: true, size: 'md', centered: true });
    const componentInstance = modalRef.componentInstance as ActionsModalComponent;
    componentInstance.actionType = 'Update';
    componentInstance.selectedData = {...data} as Book;

    modalRef.result.then(() => {
      this.refreshDataX_.set(new Date());
    })
  }

  public showDeleteModal(data: Book): void {
    const modalRef = this.modalService.open(ActionsModalComponent, { scrollable: true, size: 'md', centered: true });
    const componentInstance = modalRef.componentInstance as ActionsModalComponent;
    componentInstance.actionType = 'Delete';
    componentInstance.selectedData = {...data} as Book;

    modalRef.result.then(() => {
      this.refreshDataX_.set(new Date());
    })
  }
}
