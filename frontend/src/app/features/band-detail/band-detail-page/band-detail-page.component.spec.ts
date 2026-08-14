import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
import { provideRouter } from '@angular/router';
import { BandDetailPageComponent } from './band-detail-page.component';
import { BandDetailModel } from '../../../infrastructure/api/models';

const mockBand: BandDetailModel = {
  id: 'b1',
  name: 'Napalm Death',
  country: 'United Kingdom',
  location: 'Birmingham',
  status: 'Active',
  genre: 'Grindcore',
  formationYear: 1981,
  description: 'Legendary grindcore band.',
  logoImageUrl: null,
  bandImageUrl: null,
  musicUrlPortal: null,
  bandContact: null,
  releases: [
    { id: 'r1', title: 'Scum', releaseType: 'Album', year: 1987 },
  ],
  members: [
    { id: 'm1', name: 'Barney Greenway', instrument: 'Vocals', startYear: 1989, endYear: null, isCurrent: true, otherBands: [] },
    { id: 'm2', name: 'Bill Steer', instrument: 'Guitar', startYear: 1985, endYear: 1989, isCurrent: false, otherBands: [] },
  ],
};

describe('BandDetailPageComponent', () => {
  let fixture: ComponentFixture<BandDetailPageComponent>;
  let component: BandDetailPageComponent;
  let httpMock: HttpTestingController;

  const setup = async (paramId: string | null) => {
    TestBed.resetTestingModule();
    await TestBed.configureTestingModule({
      imports: [BandDetailPageComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        provideRouter([]),
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: { paramMap: { get: (_: string) => paramId } },
          },
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(BandDetailPageComponent);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);
  };

  afterEach(() => {
    httpMock.verify();
  });

  it('should create', async () => {
    await setup('b1');
    fixture.detectChanges();
    httpMock.expectOne((r) => r.url.includes('/bands/b1')).flush(mockBand);
    expect(component).toBeTruthy();
  });

  it('sets loading to true before request completes', async () => {
    await setup('b1');
    fixture.detectChanges();
    expect(component.loading()).toBe(true);
    httpMock.expectOne((r) => r.url.includes('/bands/b1')).flush(mockBand);
  });

  it('sets band and clears loading on success', async () => {
    await setup('b1');
    fixture.detectChanges();
    httpMock.expectOne((r) => r.url.includes('/bands/b1')).flush(mockBand);
    expect(component.band()).toEqual(mockBand);
    expect(component.loading()).toBe(false);
  });

  it('currentMembers() returns only current members', async () => {
    await setup('b1');
    fixture.detectChanges();
    httpMock.expectOne((r) => r.url.includes('/bands/b1')).flush(mockBand);
    expect(component.currentMembers().every((m) => m.isCurrent)).toBe(true);
    expect(component.currentMembers().length).toBe(1);
  });

  it('pastMembers() returns only non-current members', async () => {
    await setup('b1');
    fixture.detectChanges();
    httpMock.expectOne((r) => r.url.includes('/bands/b1')).flush(mockBand);
    expect(component.pastMembers().every((m) => !m.isCurrent)).toBe(true);
    expect(component.pastMembers().length).toBe(1);
  });

  it('hasReleases is true when band has releases', async () => {
    await setup('b1');
    fixture.detectChanges();
    httpMock.expectOne((r) => r.url.includes('/bands/b1')).flush(mockBand);
    expect(component.hasReleases()).toBe(true);
  });

  it('hasReleases is false when band has no releases', async () => {
    await setup('b1');
    fixture.detectChanges();
    httpMock.expectOne((r) => r.url.includes('/bands/b1')).flush({ ...mockBand, releases: [] });
    expect(component.hasReleases()).toBe(false);
  });

  it('hasCurrentMembers is true when there are current members', async () => {
    await setup('b1');
    fixture.detectChanges();
    httpMock.expectOne((r) => r.url.includes('/bands/b1')).flush(mockBand);
    expect(component.hasCurrentMembers()).toBe(true);
  });

  it('hasPastMembers is true when there are past members', async () => {
    await setup('b1');
    fixture.detectChanges();
    httpMock.expectOne((r) => r.url.includes('/bands/b1')).flush(mockBand);
    expect(component.hasPastMembers()).toBe(true);
  });

  it('sets error "Band not found." on 404', async () => {
    await setup('b1');
    fixture.detectChanges();
    httpMock.expectOne((r) => r.url.includes('/bands/b1')).flush(
      {},
      { status: 404, statusText: 'Not Found' },
    );
    expect(component.error()).toBe('Band not found.');
    expect(component.loading()).toBe(false);
  });

  it('sets generic error message on non-404 failure', async () => {
    await setup('b1');
    fixture.detectChanges();
    httpMock.expectOne((r) => r.url.includes('/bands/b1')).flush(
      {},
      { status: 500, statusText: 'Internal Server Error' },
    );
    expect(component.error()).toBe('Failed to load band details. Please try again.');
    expect(component.loading()).toBe(false);
  });

  it('sets error when route id is null', async () => {
    await setup(null);
    fixture.detectChanges();
    expect(component.error()).toBe('Invalid band identifier.');
    expect(component.loading()).toBe(false);
    httpMock.expectNone(() => true);
  });

  it('retry() re-fetches the band and clears error', async () => {
    await setup('b1');
    fixture.detectChanges();
    httpMock.expectOne((r) => r.url.includes('/bands/b1')).flush(
      {},
      { status: 500, statusText: 'Internal Server Error' },
    );
    expect(component.error()).toBeTruthy();

    component.retry();
    httpMock.expectOne((r) => r.url.includes('/bands/b1')).flush(mockBand);
    expect(component.error()).toBeNull();
    expect(component.band()).toEqual(mockBand);
  });
});
