import { TestBed } from '@angular/core/testing';

import { HttpInternalService } from './http-internal.service';

describe('HttpInternalService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: HttpInternalService = TestBed.get(HttpInternalService);
    expect(service).toBeTruthy();
  });
});
