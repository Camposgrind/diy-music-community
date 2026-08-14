import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
import { provideRouter } from '@angular/router';
import { ReleaseDetailPageComponent } from './release-detail-page.component';
import { ReleaseDetailModel } from '../../../infrastructure/api/models';

const mockRelease: ReleaseDetailModel = {
  id: 'r1',
  title: 'Scum',
  releaseType: 'Album',
  releaseDate: '1987-07-01',
  year: 1987,
  labelText: 'Earache Records',
  coverImageUrl: null,
  band: null,
  formats: ['Vinyl'],
  tracks: [
    { releaseId: 'r1', title: 'You Suffer', trackNumber: 1 },
  ],
};

describe('ReleaseDetailPageComponent', () => {
  let fixture: ComponentFixture<ReleaseDetailPageComponent>;
  let component: ReleaseDetailPageComponent;
  let httpMock: HttpTestingController;

  const setup = async (paramId: string | null) => {
    TestBed.resetTestingModule();
    await TestBed.configureTestingModule({
      imports: [ReleaseDetailPageComponent],
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

    fixture = TestBed.createComponent(ReleaseDetailPageComponent);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);
  };

  afterEach(() => {
    httpMock.verify();
  });

  it('should create', async () => {
    await setup('r1');
    fixture.detectChanges();
    httpMock.expectOne((r) => r.url.includes('/releases/r1')).flush(mockRelease);
    expect(component).toBeTruthy();
  });

  it('sets loading to true before request completes', async () => {
    await setup('r1');
    fixture.detectChanges();
    expect(component.loading()).toBe(true);
    httpMock.expectOne((r) => r.url.includes('/releases/r1')).flush(mockRelease);
  });

  it('sets release and clears loading on success', async () => {
    await setup('r1');
    fixture.detectChanges();
    httpMock.expectOne((r) => r.url.includes('/releases/r1')).flush(mockRelease);
    expect(component.release()).toEqual(mockRelease);
    expect(component.loading()).toBe(false);
  });

  it('hasTracks is true when release has tracks', async () => {
    await setup('r1');
    fixture.detectChanges();
    httpMock.expectOne((r) => r.url.includes('/releases/r1')).flush(mockRelease);
    expect(component.hasTracks()).toBe(true);
  });

  it('hasTracks is false when release has no tracks', async () => {
    await setup('r1');
    fixture.detectChanges();
    httpMock.expectOne((r) => r.url.includes('/releases/r1')).flush({ ...mockRelease, tracks: [] });
    expect(component.hasTracks()).toBe(false);
  });

  it('sets error "Release not found." on 404', async () => {
    await setup('r1');
    fixture.detectChanges();
    httpMock.expectOne((r) => r.url.includes('/releases/r1')).flush(
      {},
      { status: 404, statusText: 'Not Found' },
    );
    expect(component.error()).toBe('Release not found.');
    expect(component.loading()).toBe(false);
  });

  it('sets generic error message on non-404 failure', async () => {
    await setup('r1');
    fixture.detectChanges();
    httpMock.expectOne((r) => r.url.includes('/releases/r1')).flush(
      {},
      { status: 500, statusText: 'Internal Server Error' },
    );
    expect(component.error()).toBe('Something went wrong. Please try again.');
    expect(component.loading()).toBe(false);
  });

  it('sets error when route id is null', async () => {
    await setup(null);
    fixture.detectChanges();
    expect(component.error()).toBe('Invalid release identifier.');
    expect(component.loading()).toBe(false);
    httpMock.expectNone(() => true);
  });

  it('retry() re-fetches the release and clears error', async () => {
    await setup('r1');
    fixture.detectChanges();
    httpMock.expectOne((r) => r.url.includes('/releases/r1')).flush(
      {},
      { status: 500, statusText: 'Internal Server Error' },
    );
    expect(component.error()).toBeTruthy();

    component.retry();
    httpMock.expectOne((r) => r.url.includes('/releases/r1')).flush(mockRelease);
    expect(component.error()).toBeNull();
    expect(component.release()).toEqual(mockRelease);
  });
});
