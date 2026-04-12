import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { catchError, Observable, of } from 'rxjs';
import { Book } from '../../models/book';
import { createHttpParams } from '../../helpers/http-utils';
import { ArgumentNullException } from '../../helpers/argument-helper';

@Injectable({
  providedIn: 'root'
})
export class BooksService {
  private readonly url = 'api/books';
  private readonly httpClient: HttpClient = inject(HttpClient);
  
  public getAll(): Observable<Book[]> {
    return this.httpClient.get<Book[]>(`${this.url}/getAll`).pipe(
      catchError(() => {
        return of([] as Book[])
      })
    );
  }

  public get(id: string): Observable<Book> {
    const params: HttpParams = createHttpParams({ id: id });
    return this.httpClient.get<Book>(`${this.url}/get`).pipe(
      catchError(() => {
        return [];
      })
    );
  }

  public add(book: Book): Observable<unknown> {
    return this.httpClient.post(`${this.url}/add`, book);
  }

  public update(book: Book): Observable<unknown> {
    return this.httpClient.put(`${this.url}/update`, book);
  }

  public delete(book: Book): Observable<unknown> {
    const params: HttpParams = createHttpParams({ id: book.id });
    return this.httpClient.delete(`${this.url}/delete`, { params });
  }

  public validateWhenAddorUpdate(bookData: Book): void {
    ArgumentNullException.ThrowIfNullOrUndefined(bookData.publisher, 'publisher');
    ArgumentNullException.ThrowIfNullOrUndefined(bookData.title, 'title');
    ArgumentNullException.ThrowIfNullOrUndefined(bookData.author, 'author');
    ArgumentNullException.ThrowIfNullOrUndefined(bookData.yearOfPublication, 'yop');
    ArgumentNullException.ThrowIfNullOrUndefined(bookData.bookCount, 'bookCount');
    ArgumentNullException.ThrowIfNullOrUndefined(bookData.condition, 'condition');
  }
  public validateWhenUpdateOrDelete(bookData: Book): void {
    ArgumentNullException.ThrowIfNullOrUndefined(bookData.id, 'id');
  }
}
