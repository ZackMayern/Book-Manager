import { TestBed } from '@angular/core/testing';

import { CommonStoreService } from './common-store.service';

describe('CommonStoreService', () => {
  let service: CommonStoreService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CommonStoreService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
