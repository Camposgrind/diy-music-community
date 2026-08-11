import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { CountriesService } from './countries.service';

describe('CountriesService', () => {
  let service: CountriesService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(CountriesService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should call GET data/countries.json', () => {
    service.getCountries().subscribe();

    const req = httpMock.expectOne('data/countries.json');
    expect(req.request.method).toBe('GET');
    req.flush(['Spain', 'Japan']);
  });

  it('should return string array of countries', () => {
    let result: string[] | undefined;
    service.getCountries().subscribe((r) => (result = r));

    const req = httpMock.expectOne('data/countries.json');
    req.flush(['Spain', 'Japan', 'Brazil']);

    expect(result).toEqual(['Spain', 'Japan', 'Brazil']);
  });

  it('should cache the result with shareReplay', () => {
    service.getCountries().subscribe();
    service.getCountries().subscribe();

    const requests = httpMock.match('data/countries.json');
    expect(requests.length).toBe(1);
    requests[0].flush(['Spain']);
  });
});
