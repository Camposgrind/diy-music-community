import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { HomeComponent } from './home.component';
import { BandListItemModel, PagedResult } from '../../../infrastructure/api/models';

describe('HomeComponent', () => {
  let fixture: ComponentFixture<HomeComponent>;
  let component: HomeComponent;
  let httpMock: HttpTestingController;

  const mockBand: BandListItemModel = {
    id: '1',
    name: 'Discharge',
    country: 'UK',
    genre: 'D-Beat',
    status: 'Active',
    formationYear: 1977,
  };

  beforeEach(async () => {
    TestBed.resetTestingModule();
    await TestBed.configureTestingModule({
      imports: [HomeComponent],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();

    fixture = TestBed.createComponent(HomeComponent);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  function flushInitialRequests(): void {
    const countriesReq = httpMock.expectOne((r) => r.url.endsWith('data/countries.json'));
    countriesReq.flush(['Spain', 'Japan']);
    const genresReq = httpMock.expectOne((r) => r.url.endsWith('/api/genres'));
    genresReq.flush([{ id: 'g1', name: 'Grindcore' }]);
  }

  it('should create', () => {
    fixture.detectChanges();
    flushInitialRequests();
    expect(component).toBeTruthy();
  });

  it('should load countries on init', () => {
    fixture.detectChanges();
    flushInitialRequests();
    expect(component.countries()).toEqual(['Spain', 'Japan']);
  });

  it('should load genres on init', () => {
    fixture.detectChanges();
    flushInitialRequests();
    expect(component.genres()).toEqual([{ id: 'g1', name: 'Grindcore' }]);
  });

  it('should set loading to true when search is triggered', () => {
    fixture.detectChanges();
    flushInitialRequests();

    component.onSearch({ name: 'Discharge', country: '', genreId: '' });
    expect(component.loading()).toBe(true);

    const req = httpMock.expectOne((r) => r.url.endsWith('/api/bands'));
    req.flush({ items: [mockBand], page: 1, pageSize: 20, totalCount: 1 });
  });

  it('should set results on successful search', () => {
    fixture.detectChanges();
    flushInitialRequests();

    component.onSearch({ name: '', country: '', genreId: '' });

    const req = httpMock.expectOne((r) => r.url.endsWith('/api/bands'));
    const mockResult: PagedResult<BandListItemModel> = {
      items: [mockBand],
      page: 1,
      pageSize: 20,
      totalCount: 1,
    };
    req.flush(mockResult);

    expect(component.results()).toEqual(mockResult);
    expect(component.loading()).toBe(false);
  });

  it('should set error on non-422 failure', () => {
    fixture.detectChanges();
    flushInitialRequests();

    component.onSearch({ name: '', country: '', genreId: '' });

    const req = httpMock.expectOne((r) => r.url.endsWith('/api/bands'));
    req.flush(
      { code: 'Band.InvalidFilter', message: 'Invalid filter' },
      { status: 400, statusText: 'Bad Request' },
    );

    expect(component.error()).toBe('Invalid filter');
    expect(component.loading()).toBe(false);
  });

  it('should set toast message on 422 TooManyResults', () => {
    fixture.detectChanges();
    flushInitialRequests();

    component.onSearch({ name: '', country: '', genreId: '' });

    const req = httpMock.expectOne((r) => r.url.endsWith('/api/bands'));
    req.flush(
      { code: 'Band.TooManyResults', message: 'Too many results, refine filters' },
      { status: 422, statusText: 'Unprocessable Entity' },
    );

    expect(component.toastMessage()).toBe('Too many results, refine filters');
    expect(component.results()).toBeNull();
    expect(component.loading()).toBe(false);
  });

  it('should clear state on reset', () => {
    fixture.detectChanges();
    flushInitialRequests();

    component.onSearch({ name: 'test', country: '', genreId: '' });
    const req = httpMock.expectOne((r) => r.url.endsWith('/api/bands'));
    req.flush({ items: [mockBand], page: 1, pageSize: 20, totalCount: 1 });

    component.onReset();

    expect(component.results()).toBeNull();
    expect(component.error()).toBeNull();
    expect(component.toastMessage()).toBeNull();
  });

  it('should handle pagination', () => {
    fixture.detectChanges();
    flushInitialRequests();

    component.onSearch({ name: '', country: '', genreId: '' });
    const req1 = httpMock.expectOne((r) => r.url.endsWith('/api/bands'));
    req1.flush({ items: [mockBand], page: 1, pageSize: 20, totalCount: 40 });

    component.onPageChange(2);
    const req2 = httpMock.expectOne((r) => r.url.endsWith('/api/bands'));
    expect(req2.request.params.get('page')).toBe('2');
    req2.flush({ items: [mockBand], page: 2, pageSize: 20, totalCount: 40 });

    expect(component.results()?.page).toBe(2);
  });

  it('should set genresError when genres API fails', () => {
    fixture.detectChanges();

    const countriesReq = httpMock.expectOne((r) => r.url.endsWith('data/countries.json'));
    countriesReq.flush(['Spain']);
    const genresReq = httpMock.expectOne((r) => r.url.endsWith('/api/genres'));
    genresReq.flush('error', { status: 500, statusText: 'Server Error' });

    expect(component.genresError()).toContain('Could not load genres');
  });

  it('should keep genres empty array when API fails', () => {
    fixture.detectChanges();

    const countriesReq = httpMock.expectOne((r) => r.url.endsWith('data/countries.json'));
    countriesReq.flush([]);
    const genresReq = httpMock.expectOne((r) => r.url.endsWith('/api/genres'));
    genresReq.flush('error', { status: 500, statusText: 'Server Error' });

    expect(component.genres()).toEqual([]);
  });

  it('should dismiss toast', () => {
    fixture.detectChanges();
    flushInitialRequests();

    component.onSearch({ name: '', country: '', genreId: '' });
    const req = httpMock.expectOne((r) => r.url.endsWith('/api/bands'));
    req.flush(
      { code: 'Band.TooManyResults', message: 'Too many' },
      { status: 422, statusText: 'Unprocessable Entity' },
    );

    component.dismissToast();
    expect(component.toastMessage()).toBeNull();
  });

  it('should include name filter in API call', () => {
    fixture.detectChanges();
    flushInitialRequests();

    component.onSearch({ name: 'Napalm', country: '', genreId: '' });
    const req = httpMock.expectOne((r) => r.url.endsWith('/api/bands'));
    expect(req.request.params.get('name')).toBe('Napalm');
    expect(req.request.params.has('country')).toBe(false);
    expect(req.request.params.has('genreId')).toBe(false);
    req.flush({ items: [], page: 1, pageSize: 20, totalCount: 0 });
  });

  it('should include country and genreId in API call when provided', () => {
    fixture.detectChanges();
    flushInitialRequests();

    component.onSearch({ name: '', country: 'Japan', genreId: 'g1' });
    const req = httpMock.expectOne((r) => r.url.endsWith('/api/bands'));
    expect(req.request.params.get('country')).toBe('Japan');
    expect(req.request.params.get('genreId')).toBe('g1');
    req.flush({ items: [], page: 1, pageSize: 20, totalCount: 0 });
  });
});
