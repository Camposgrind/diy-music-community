import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ReleasesApiService } from './releases-api.service';
import { environment } from '../../../environments/environment';
import { ReleaseDetailModel } from './models';

describe('ReleasesApiService', () => {
  let service: ReleasesApiService;
  let httpMock: HttpTestingController;
  const base = `${environment.apiBaseUrl}/releases`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(ReleasesApiService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('should GET release detail by id', () => {
    const mockRelease = { id: 'rel-1', title: 'Scream Bloody Gore' } as ReleaseDetailModel;
    service.getReleaseDetail('rel-1').subscribe(res => expect(res).toEqual(mockRelease));
    const req = httpMock.expectOne(`${base}/rel-1`);
    expect(req.request.method).toBe('GET');
    req.flush(mockRelease);
  });

  it('should include the id in the URL', () => {
    service.getReleaseDetail('abc-999').subscribe();
    const req = httpMock.expectOne(`${base}/abc-999`);
    expect(req.request.url).toContain('abc-999');
    req.flush({});
  });

  it('should propagate 404 errors', () => {
    let errorCaught = false;
    service.getReleaseDetail('missing').subscribe({ error: () => (errorCaught = true) });
    httpMock.expectOne(`${base}/missing`).flush('Not found', { status: 404, statusText: 'Not Found' });
    expect(errorCaught).toBe(true);
  });
});
