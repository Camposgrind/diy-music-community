import { TestBed } from '@angular/core/testing';
import { LoadingService } from './loading.service';

describe('LoadingService', () => {
  let service: LoadingService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(LoadingService);
  });

  it('should keep loading visible until every pending request has completed', () => {
    service.begin();
    service.begin();

    expect(service.isLoading()).toBe(2);

    service.end();
    expect(service.isLoading()).toBe(1);

    service.end();
    expect(service.isLoading()).toBe(0);
  });

  it('should not allow the pending request count to become negative', () => {
    service.end();

    expect(service.isLoading()).toBe(0);
  });
});
