import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { catchError, Observable, of } from 'rxjs';
import { BorrowRecord } from '../../models/borrow-record';

@Injectable({
  providedIn: 'root'
})
export class BorrowService {
  private readonly url = '/api/borrow';
  private readonly httpClient: HttpClient = inject(HttpClient);

  public getMine(): Observable<BorrowRecord[]> {
    return this.httpClient.get<BorrowRecord[]>(`${this.url}/mine`).pipe(
      catchError(() => of([] as BorrowRecord[]))
    );
  }

  public getAll(): Observable<BorrowRecord[]> {
    return this.httpClient.get<BorrowRecord[]>(`${this.url}/all`).pipe(
      catchError(() => of([] as BorrowRecord[]))
    );
  }

  public borrow(bookId: string, borrowDays: number = 14): Observable<BorrowRecord> {
    return this.httpClient.post<BorrowRecord>(this.url, { bookId, borrowDays });
  }

  public renew(id: string, extendDays: number = 7): Observable<BorrowRecord> {
    return this.httpClient.put<BorrowRecord>(`${this.url}/renew/${id}`, { extendDays });
  }

  public return(id: string): Observable<BorrowRecord> {
    return this.httpClient.put<BorrowRecord>(`${this.url}/return/${id}`, {});
  }
}