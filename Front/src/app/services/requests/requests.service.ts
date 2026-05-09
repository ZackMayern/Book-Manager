import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { catchError, Observable, of } from 'rxjs';
import { BookRequest } from '../../models/book-request';

@Injectable({
  providedIn: 'root'
})
export class RequestsService {
  private readonly url = '/api/requests';
  private readonly httpClient: HttpClient = inject(HttpClient);

  public getMine(): Observable<BookRequest[]> {
    return this.httpClient.get<BookRequest[]>(`${this.url}/mine`).pipe(
      catchError(() => of([] as BookRequest[]))
    );
  }

  public getPending(): Observable<BookRequest[]> {
    return this.httpClient.get<BookRequest[]>(`${this.url}/pending`).pipe(
      catchError(() => of([] as BookRequest[]))
    );
  }

  public create(title: string, author: string, reason: string): Observable<BookRequest> {
    return this.httpClient.post<BookRequest>(this.url, { title, author, reason });
  }

  public approve(id: string, message: string): Observable<BookRequest> {
    return this.httpClient.put<BookRequest>(`${this.url}/${id}/approve`, { message });
  }

  public reject(id: string, message: string): Observable<BookRequest> {
    return this.httpClient.put<BookRequest>(`${this.url}/${id}/reject`, { message });
  }
}