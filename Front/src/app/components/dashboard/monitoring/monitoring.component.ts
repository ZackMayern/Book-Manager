import { Component, inject, input, OnInit } from '@angular/core';
import { CommonStoreService } from '../../../store/common-store.service';
import { Observable } from 'rxjs';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-monitoring',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './monitoring.component.html',
  styleUrl: './monitoring.component.scss'
})
export class MonitoringComponent implements OnInit{
  private readonly commonStore: CommonStoreService = inject(CommonStoreService)

  public booksCount$!: Observable<number>;
  public newBooksCount$!: Observable<number>;
  public usedBooksCount$!: Observable<number>;
  public damagedBooksCount$!: Observable<number>;
  public missingBooksCount$!: Observable<number>;

  public ngOnInit(): void {
    this.booksCount$ = this.commonStore.booksCount$;
    this.newBooksCount$ = this.commonStore.newBooksCount$;
    this.usedBooksCount$ = this.commonStore.usedBooksCount$;
    this.damagedBooksCount$ = this.commonStore.damagedBooksCount$;
    this.missingBooksCount$ = this.commonStore.missingBooksCount$;
  }
}
