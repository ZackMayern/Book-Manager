import { actionType } from '../../../../constants/action-type';
import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Book } from '../../../../models/book';
import { Observable } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { BooksService } from '../../../../services/books/books.service';
import { ConditionType } from '../../../../constants/condition-type';

@Component({
  selector: 'app-actions-modal',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './actions-modal.component.html',
  styleUrl: './actions-modal.component.scss'
})
export class ActionsModalComponent implements OnInit {
  private readonly destroyRef: DestroyRef = inject(DestroyRef);
  private readonly booksService: BooksService = inject(BooksService);
  private readonly ngbActiveModal: NgbActiveModal = inject(NgbActiveModal);

  public waitingForResponse: boolean = false;
  public actionType!: actionType;
  public selectedData?: Book;
  public modalTitle?: string;
  public conditions: string[] = ["New", "Used", "Damaged"];

  public actionsForm!: FormGroup<{
    publisher: FormControl<string>;
    title: FormControl<string>;
    author: FormControl<string>;
    yearOfPublication: FormControl<string>;
    bookCount: FormControl<number>;
    condition: FormControl<ConditionType>;
  }>;

  public ngOnInit(): void {
    this.modalTitle = this.selectedData?.title;
    this.createForm(this.selectedData);
  }

  private createForm(bookData?: Book): void {
    this.actionsForm = new FormGroup({
      publisher: new FormControl({ value: bookData?.publisher ?? '', disabled: this.disableWhenDelete() }, { nonNullable: true, validators: Validators.required }),
      title: new FormControl({ value: bookData?.title ?? '', disabled: this.disableWhenDelete() }, { nonNullable: true, validators: Validators.required }),
      author: new FormControl({ value: bookData?.author ?? '', disabled: this.disableWhenDelete() }, { nonNullable: true, validators: Validators.required }),
      yearOfPublication: new FormControl({ value: bookData?.yearOfPublication ?? '', disabled: this.disableWhenDelete() }, { nonNullable: true, validators: Validators.required }),
      bookCount: new FormControl({ value: bookData?.bookCount ?? 0, disabled: this.disableWhenDelete() }, { nonNullable: true, validators: Validators.required }),
      condition: new FormControl({ value: bookData?.condition ?? 'New', disabled: this.disableWhenDelete() }, { nonNullable: true })
    });
  }

  private convertFormToModel(): Book {
    return {
      id: this.actionType === 'Add' ? '' : this.selectedData!.id,
      publisher: this.actionsForm.controls.publisher.value,
      title: this.actionsForm.controls.title.value,
      author: this.actionsForm.controls.author.value,
      yearOfPublication: this.actionsForm.controls.yearOfPublication.value,
      bookCount: this.actionsForm.controls.bookCount.value,
      condition: this.actionsForm.controls.condition.value
    }
  }

  private disableWhenDelete(): boolean {
    return this.actionType.includes('Delete') ? true : false;
  }

  public close(): void {
    this.ngbActiveModal.close(true);
  }

  public submit(): void {
    this.waitingForResponse = true;
    const model = this.convertFormToModel();
    let http$!: Observable<unknown>;

    if (this.actionType === "Add") {
      this.booksService.validateWhenAddorUpdate(model);
      http$ = this.booksService.add(model);
    }
    if (this.actionType === "Update") {
      this.booksService.validateWhenUpdateOrDelete(model);
      this.booksService.validateWhenAddorUpdate(model);
      http$ = this.booksService.update(model);
    }
    if (this.actionType === "Delete") {
      this.booksService.validateWhenUpdateOrDelete(model);
      http$ = this.booksService.delete(model);
    }
    else {
      console.info("Invalid action type");
    }

    http$.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: () => {
        console.info(`${this.actionType} was successful!`);
        this.close();
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
