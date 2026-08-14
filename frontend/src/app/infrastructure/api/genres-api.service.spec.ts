import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { GenresApiService } from './genres-api.service';
import { GenreModel } from './models';

describe('GenresApiService', () => {
  let service: GenresApiService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.resetTestingModule();
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(GenresApiService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should call GET /api/genres', () => {
    service.getGenres().subscribe();

    const req = httpMock.expectOne((r) => r.url.endsWith('/api/genres'));
    expect(req.request.method).toBe('GET');
    req.flush([]);
  });

  it('should return typed GenreModel array', () => {
    const mockGenres: GenreModel[] = [
      { id: '1', name: 'Grindcore' },
      { id: '2', name: 'Crust' },
    ];

    let result: GenreModel[] | undefined;
    service.getGenres().subscribe((r) => (result = r));

    const req = httpMock.expectOne((r) => r.url.endsWith('/api/genres'));
    req.flush(mockGenres);

    expect(result).toEqual(mockGenres);
    expect(result!.length).toBe(2);
  });
});
