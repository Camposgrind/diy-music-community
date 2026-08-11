import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { BandsApiService } from './bands-api.service';
import { GetBandsQuery, PagedResult, BandListItemModel } from './models';

describe('BandsApiService', () => {
  let service: BandsApiService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(BandsApiService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should send page and pageSize as query params', () => {
    const query: GetBandsQuery = { page: 2, pageSize: 10 };
    service.getBands(query).subscribe();

    const req = httpMock.expectOne((r) => r.url === '/api/bands');
    expect(req.request.params.get('page')).toBe('2');
    expect(req.request.params.get('pageSize')).toBe('10');
    expect(req.request.params.has('name')).toBe(false);
    expect(req.request.params.has('country')).toBe(false);
    expect(req.request.params.has('genreId')).toBe(false);
    req.flush({ items: [], page: 2, pageSize: 10, totalCount: 0 });
  });

  it('should include name filter when provided', () => {
    const query: GetBandsQuery = { page: 1, pageSize: 20, name: 'Napalm' };
    service.getBands(query).subscribe();

    const req = httpMock.expectOne((r) => r.url === '/api/bands');
    expect(req.request.params.get('name')).toBe('Napalm');
    req.flush({ items: [], page: 1, pageSize: 20, totalCount: 0 });
  });

  it('should include country filter when provided', () => {
    const query: GetBandsQuery = { page: 1, pageSize: 20, country: 'United Kingdom' };
    service.getBands(query).subscribe();

    const req = httpMock.expectOne((r) => r.url === '/api/bands');
    expect(req.request.params.get('country')).toBe('United Kingdom');
    req.flush({ items: [], page: 1, pageSize: 20, totalCount: 0 });
  });

  it('should include genreId filter when provided', () => {
    const query: GetBandsQuery = { page: 1, pageSize: 20, genreId: 'abc-123' };
    service.getBands(query).subscribe();

    const req = httpMock.expectOne((r) => r.url === '/api/bands');
    expect(req.request.params.get('genreId')).toBe('abc-123');
    req.flush({ items: [], page: 1, pageSize: 20, totalCount: 0 });
  });

  it('should include status filter when provided', () => {
    const query: GetBandsQuery = { page: 1, pageSize: 20, status: 'Active' };
    service.getBands(query).subscribe();

    const req = httpMock.expectOne((r) => r.url === '/api/bands');
    expect(req.request.params.get('status')).toBe('Active');
    req.flush({ items: [], page: 1, pageSize: 20, totalCount: 0 });
  });

  it('should return typed PagedResult', () => {
    const query: GetBandsQuery = { page: 1, pageSize: 20 };
    const mockResult: PagedResult<BandListItemModel> = {
      items: [{ id: '1', name: 'Discharge', country: 'UK', genre: 'D-Beat', status: 'Active', formationYear: 1977 }],
      page: 1,
      pageSize: 20,
      totalCount: 1,
    };

    let result: PagedResult<BandListItemModel> | undefined;
    service.getBands(query).subscribe((r) => (result = r));

    const req = httpMock.expectOne((r) => r.url === '/api/bands');
    req.flush(mockResult);

    expect(result).toEqual(mockResult);
  });
});
