import { HttpClient, provideHttpClient, withInterceptors } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { LoadingService } from '../loading/loading.service';
import { loadingInterceptor } from './loading.interceptor';

describe('loadingInterceptor', () => {
  let http: HttpClient;
  let httpMock: HttpTestingController;
  let loading: LoadingService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptors([loadingInterceptor])),
        provideHttpClientTesting(),
      ],
    });
    http = TestBed.inject(HttpClient);
    httpMock = TestBed.inject(HttpTestingController);
    loading = TestBed.inject(LoadingService);
  });

  afterEach(() => httpMock.verify());

  it('should show the loader while an HTTP request is pending and hide it after completion', () => {
    http.get('/api/bands').subscribe();

    expect(loading.isLoading()).toBe(1);

    httpMock.expectOne('/api/bands').flush([]);

    expect(loading.isLoading()).toBe(0);
  });

  it('should hide the loader after an HTTP request fails', () => {
    http.get('/api/bands').subscribe({ error: () => undefined });

    httpMock.expectOne('/api/bands').flush('Request failed', {
      status: 500,
      statusText: 'Server Error',
    });

    expect(loading.isLoading()).toBe(0);
  });
});
