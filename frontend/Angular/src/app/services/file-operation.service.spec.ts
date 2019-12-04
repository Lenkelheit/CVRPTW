import { TestBed } from '@angular/core/testing';

import { FileOperationService } from './file-operation.service';

describe('FileOperationService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: FileOperationService = TestBed.get(FileOperationService);
    expect(service).toBeTruthy();
  });
});
