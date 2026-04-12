import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CommonStoreService {
  private booksCount = new BehaviorSubject<number>(0);
  private newBooksCount = new BehaviorSubject<number>(0);
  private usedBooksCount = new BehaviorSubject<number>(0);
  private damagedBooksCount =  new BehaviorSubject<number>(0);
  private missingBooksCount = new BehaviorSubject<number>(0);

  public booksCount$ = this.booksCount.asObservable();
  public newBooksCount$ = this.newBooksCount.asObservable();
  public usedBooksCount$ = this.usedBooksCount.asObservable();
  public damagedBooksCount$ = this.damagedBooksCount.asObservable();
  public missingBooksCount$ = this.missingBooksCount.asObservable();

  public updateBookCount(count: number) {
    this.booksCount.next(count);
  }

  public updateNewBooksCount(count: number) {
    this.newBooksCount.next(count);
  }

  public updateUsedBooksCount(count: number) {
    this.usedBooksCount.next(count);
  }

  public updateDamagedBooksCount(count: number) {
    this.damagedBooksCount.next(count);
  }

  public updateMissingBooksCount(count: number) {
    this.missingBooksCount.next(count);
  }
}
