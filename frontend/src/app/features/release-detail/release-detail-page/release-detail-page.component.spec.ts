import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
import { provideRouter } from '@angular/router';
import { Router } from '@angular/router';
import { signal } from '@angular/core';
import { ReleaseDetailPageComponent } from './release-detail-page.component';
import { ReleaseDetailModel } from '../../../infrastructure/api/models';
import { AuthService } from '../../../core/auth/auth.service';
import { ToastService } from '../../../core/toast/toast.service';
import { vi } from 'vitest';

const mockRelease: ReleaseDetailModel = {
  id: 'r1',
  title: 'Scum',
  releaseType: 'Album',
  releaseDate: '1987-07-01',
  year: 1987,
  labelText: 'Earache Records',
  coverImageUrl: null,
  band: { bandId: 'b1', name: 'Napalm Death' },
  formats: ['Vinyl'],
  tracks: [
    { releaseId: 'r1', title: 'You Suffer', trackNumber: 1 },
  ],
};

describe('ReleaseDetailPageComponent', () => {
  let fixture: ComponentFixture<ReleaseDetailPageComponent>;
  let component: ReleaseDetailPageComponent;
  let httpMock: HttpTestingController;
  const isAdmin = signal(false);
  let toastSuccess: ReturnType<typeof vi.fn>;
  let toastError: ReturnType<typeof vi.fn>;

  const setup = async (paramId: string | null) => {
    TestBed.resetTestingModule();
    toastSuccess = vi.fn();
    toastError = vi.fn();
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
        { provide: AuthService, useValue: { isAdmin } },
        { provide: ToastService, useValue: { success: toastSuccess, error: toastError } },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(ReleaseDetailPageComponent);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);
  };

  afterEach(() => {
    httpMock.verify();
    isAdmin.set(false);
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

  it('updates ordered tracks through the dedicated tracks endpoint', async () => {
    await setup('r1');
    isAdmin.set(true);
    fixture.detectChanges();
    httpMock.expectOne((r) => r.url.includes('/releases/r1')).flush(mockRelease);
    fixture.detectChanges();

    component.openEditReleaseModal();
    fixture.detectChanges();
    expect(fixture.nativeElement.querySelector('[data-testid="edit-release-modal"]')).toBeTruthy();

    component.saveReleaseEdit({
      title: 'From Enslavement to Obliteration',
      releaseType: 'Album',
      year: 1988,
      tracks: [{ title: 'Evolved As One' }, { title: 'Life?' }],
    });

    const request = httpMock.expectOne((r) => r.method === 'PUT' && r.url.includes('/bands/b1/releases/r1'));
    expect(request.request.url).toContain('/tracks');
    expect(request.request.body).toEqual({ tracks: [{ title: 'Evolved As One' }, { title: 'Life?' }] });
    request.flush(mockRelease);
    httpMock.expectOne((r) => r.url.includes('/releases/r1')).flush({
      ...mockRelease,
      tracks: [
        { releaseId: 'r1', title: 'Evolved As One', trackNumber: 1 },
        { releaseId: 'r1', title: 'Life?', trackNumber: 2 },
      ],
    });
    expect(component.isEditModalOpen()).toBe(false);
    expect(component.release()?.tracks[0].title).toBe('Evolved As One');
    expect(toastSuccess).toHaveBeenCalledWith('Tracks updated successfully.');
  });

  it('updates release details while preserving tracks and the cover', async () => {
    await setup('r1');
    fixture.detectChanges();
    httpMock.expectOne((r) => r.url.includes('/releases/r1')).flush(mockRelease);

    component.openEditDetailsModal();
    component.saveReleaseDetails({ title: 'Scum', releaseType: 'Album', releaseDate: '1987-07-01', year: 1987, labelText: 'Earache', formats: ['CD'] });

    const request = httpMock.expectOne((r) => r.method === 'PUT' && r.url.includes('/bands/b1/releases/r1') && !r.url.endsWith('/tracks'));
    expect(request.request.body).toEqual({ title: 'Scum', releaseType: 'Album', releaseDate: '1987-07-01', year: 1987, labelText: 'Earache', formats: ['CD'], coverImageUrl: null, tracks: [{ title: 'You Suffer' }] });
    request.flush(mockRelease);
    httpMock.expectOne((r) => r.url.includes('/releases/r1')).flush(mockRelease);
    expect(component.isDetailsModalOpen()).toBe(false);
    expect(toastSuccess).toHaveBeenCalledWith('Release updated successfully.');
  });

  it('deletes a release after confirmation and returns to its band', async () => {
    await setup('r1');
    const router = TestBed.inject(Router);
    const navigate = vi.spyOn(router, 'navigate');
    fixture.detectChanges();
    httpMock.expectOne((r) => r.url.includes('/releases/r1')).flush(mockRelease);

    component.openReleaseDeleteConfirmation();
    component.confirmReleaseDeletion();

    const request = httpMock.expectOne((r) => r.method === 'DELETE' && r.url.endsWith('/releases/r1'));
    request.flush(null);
    expect(navigate).toHaveBeenCalledWith(['/bands', 'b1']);
    expect(toastSuccess).toHaveBeenCalledWith('Release deleted successfully.');
  });

  it('deletes all tracks after confirmation and reloads the release', async () => {
    await setup('r1');
    fixture.detectChanges();
    httpMock.expectOne((r) => r.url.includes('/releases/r1')).flush(mockRelease);

    component.openDeleteAllTracksConfirmation();
    component.confirmDeleteAllTracks();

    const request = httpMock.expectOne((r) => r.method === 'DELETE' && r.url.endsWith('/releases/r1/tracks'));
    request.flush(null);
    httpMock.expectOne((r) => r.method === 'GET' && r.url.endsWith('/releases/r1')).flush({ ...mockRelease, tracks: [] });
    expect(component.release()?.tracks).toEqual([]);
    expect(toastSuccess).toHaveBeenCalledWith('All tracks deleted successfully.');
  });

  it('shows the API error in a toast when deleting a release fails', async () => {
    await setup('r1');
    fixture.detectChanges();
    httpMock.expectOne((r) => r.url.includes('/releases/r1')).flush(mockRelease);

    component.openReleaseDeleteConfirmation();
    component.confirmReleaseDeletion();
    httpMock.expectOne((r) => r.method === 'DELETE' && r.url.endsWith('/releases/r1')).flush(
      { message: 'The release is locked.' },
      { status: 409, statusText: 'Conflict' },
    );

    expect(component.releaseDeleteError()).toBe('The release is locked.');
    expect(toastError).toHaveBeenCalledWith('The release is locked.');
  });
});
