import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
import { provideRouter } from '@angular/router';
import { signal } from '@angular/core';
import { BandDetailPageComponent } from './band-detail-page.component';
import { BandDetailModel, BandWriteRequest, MemberWriteRequest, ReleaseWriteRequest } from '../../../infrastructure/api/models';
import { AuthService } from '../../../core/auth/auth.service';

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
    { id: 'm1', name: 'Barney Greenway', instrument: 'Vocals', startYear: 1989, endYear: null, isCurrent: true, isLastKnownLineup: false, otherBands: [] },
    { id: 'm2', name: 'Bill Steer', instrument: 'Guitar', startYear: 1985, endYear: 1989, isCurrent: false, isLastKnownLineup: false, otherBands: [] },
  ],
};

describe('BandDetailPageComponent', () => {
  let fixture: ComponentFixture<BandDetailPageComponent>;
  let component: BandDetailPageComponent;
  let httpMock: HttpTestingController;
  const isAdmin = signal(false);

  const setup = async (paramId: string | null) => {
    TestBed.resetTestingModule();
    await TestBed.configureTestingModule({
      imports: [BandDetailPageComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        provideRouter([]),
        { provide: AuthService, useValue: { isAdmin } },
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
    isAdmin.set(false);
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

  it('should show the edit control only for an Admin', async () => {
    await setup('b1');
    fixture.detectChanges();
    httpMock.expectOne((r) => r.url.includes('/bands/b1')).flush(mockBand);
    fixture.detectChanges();
    expect(fixture.nativeElement.querySelector('[data-testid="edit-band"]')).toBeNull();

    isAdmin.set(true);
    fixture.detectChanges();
    expect(fixture.nativeElement.querySelector('[data-testid="edit-band"]')).toBeTruthy();
  });

  it('should open the preloaded edit modal for an Admin', async () => {
    await setup('b1');
    isAdmin.set(true);
    fixture.detectChanges();
    httpMock.expectOne((r) => r.url.includes('/bands/b1')).flush(mockBand);
    fixture.detectChanges();

    (fixture.nativeElement.querySelector('[data-testid="edit-band"]') as HTMLButtonElement).click();
    const countries = httpMock.expectOne((r) => r.url.endsWith('data/countries.json'));
    const genres = httpMock.expectOne((r) => r.url.endsWith('/api/genres'));
    countries.flush(['United Kingdom']);
    genres.flush([{ id: 'genre-1', name: 'Grindcore' }]);
    fixture.detectChanges();

    expect(component.isEditModalOpen()).toBe(true);
    expect(fixture.nativeElement.querySelector('dmc-band-edit-modal')).toBeTruthy();
  });

  it('should update the band and reload its full detail', async () => {
    await setup('b1');
    fixture.detectChanges();
    httpMock.expectOne((r) => r.url.includes('/bands/b1')).flush(mockBand);
    const update: BandWriteRequest = { name: 'Napalm Death', country: 'United Kingdom', genreId: 'genre-1', status: 'OnHold' };

    component.saveBand(update);
    const updateRequest = httpMock.expectOne((r) => r.url.endsWith('/bands/b1') && r.method === 'PUT');
    updateRequest.flush({ ...mockBand, ...update });
    httpMock.expectOne((r) => r.url.endsWith('/bands/b1') && r.method === 'GET').flush({ ...mockBand, status: 'OnHold' });

    expect(component.isEditModalOpen()).toBe(false);
    expect(component.band()?.status).toBe('OnHold');
  });

  it('should open the release modal in create mode for an Admin', async () => {
    await setup('b1');
    isAdmin.set(true);
    fixture.detectChanges();
    httpMock.expectOne((r) => r.url.includes('/bands/b1')).flush(mockBand);
    fixture.detectChanges();

    (fixture.nativeElement.querySelector('[data-testid="add-release"]') as HTMLButtonElement).click();
    fixture.detectChanges();

    expect(component.releaseModalMode()).toBe('create');
    expect(fixture.nativeElement.querySelector('dmc-release-modal')).toBeTruthy();
  });

  it('should open the release modal preloaded in edit mode for an Admin', async () => {
    await setup('b1');
    isAdmin.set(true);
    fixture.detectChanges();
    httpMock.expectOne((r) => r.url.includes('/bands/b1')).flush(mockBand);
    fixture.detectChanges();

    (fixture.nativeElement.querySelector('[data-testid="edit-release-r1"]') as HTMLButtonElement).click();
    httpMock.expectOne((r) => r.url.endsWith('/releases/r1')).flush({
      id: 'r1', title: 'Scum', releaseType: 'Album', releaseDate: null, year: 1987, labelText: null,
      coverImageUrl: null, band: null, formats: [], tracks: [],
    });
    fixture.detectChanges();

    expect(component.releaseModalMode()).toBe('edit');
    expect(component.releaseModalData()).toEqual({ title: 'Scum', releaseType: 'Album', year: 1987 });
  });

  it('should create a release and reload the full band detail', async () => {
    await setup('b1');
    fixture.detectChanges();
    httpMock.expectOne((r) => r.url.includes('/bands/b1')).flush(mockBand);
    component.openCreateReleaseModal();
    component.saveRelease({ title: 'Harmony Corruption', releaseType: 'Album', year: 1990 });

    const create = httpMock.expectOne((r) => r.url.endsWith('/bands/b1/releases') && r.method === 'POST');
    expect(create.request.body).toMatchObject({ title: 'Harmony Corruption', releaseType: 'Album', year: 1990, tracks: [] } satisfies Partial<ReleaseWriteRequest>);
    create.flush({ id: 'r2' });
    httpMock.expectOne((r) => r.url.endsWith('/bands/b1') && r.method === 'GET').flush({ ...mockBand, releases: [...mockBand.releases, { id: 'r2', title: 'Harmony Corruption', releaseType: 'Album', year: 1990 }] });

    expect(component.releaseModalMode()).toBeNull();
    expect(component.band()?.releases).toHaveLength(2);
  });

  it('should update a release and reload the full band detail', async () => {
    await setup('b1');
    fixture.detectChanges();
    httpMock.expectOne((r) => r.url.includes('/bands/b1')).flush(mockBand);
    component.openEditReleaseModal(mockBand.releases[0]);
    httpMock.expectOne((r) => r.url.endsWith('/releases/r1')).flush({
      id: 'r1', title: 'Scum', releaseType: 'Album', releaseDate: null, year: 1987, labelText: 'Earache',
      coverImageUrl: 'https://example.com/scum.jpg', band: null, formats: [], tracks: [{ id: 't1', title: 'Multinational Corporations', trackNumber: 1 }],
    });
    component.saveRelease({ title: 'Scum', releaseType: 'Album', year: 1988 });

    const update = httpMock.expectOne((r) => r.url.endsWith('/bands/b1/releases/r1') && r.method === 'PUT');
    expect(update.request.body).toMatchObject({ year: 1988, labelText: 'Earache', coverImageUrl: 'https://example.com/scum.jpg', tracks: [{ title: 'Multinational Corporations' }] });
    update.flush({ id: 'r1' });
    httpMock.expectOne((r) => r.url.endsWith('/bands/b1') && r.method === 'GET').flush({ ...mockBand, releases: [{ ...mockBand.releases[0], year: 1988 }] });

    expect(component.band()?.releases[0].year).toBe(1988);
  });

  it('should open a current-member modal for an Admin', async () => {
    await setup('b1');
    isAdmin.set(true);
    fixture.detectChanges();
    httpMock.expectOne((r) => r.url.includes('/bands/b1')).flush(mockBand);
    fixture.detectChanges();

    const buttons = fixture.nativeElement.querySelectorAll('[data-testid="add-member"]') as NodeListOf<HTMLButtonElement>;
    buttons[0].click();
    fixture.detectChanges();
    expect(component.memberModalMode()).toBe('create');
    expect(component.memberModalType()).toBe('current');
  });

  it('should preload and update a past member, then reload the band', async () => {
    await setup('b1');
    fixture.detectChanges();
    httpMock.expectOne((r) => r.url.includes('/bands/b1')).flush(mockBand);
    const pastMember = mockBand.members[1];
    component.openEditMemberModal(pastMember);
    expect(component.memberModalData()).toMatchObject({ name: 'Bill Steer', memberType: 'past', endYear: 1989 });

    component.saveMember({ name: 'Bill Steer', instrument: 'Guitar', startYear: 1985, endYear: 1990, memberType: 'past' });
    const update = httpMock.expectOne((r) => r.url.endsWith('/bands/b1/members/m2') && r.method === 'PUT');
    expect(update.request.body).toEqual({ name: 'Bill Steer', instrument: 'Guitar', startYear: 1985, endYear: 1990, isCurrent: false, isLastKnownLineup: false } satisfies MemberWriteRequest);
    update.flush({ id: 'm2' });
    httpMock.expectOne((r) => r.url.endsWith('/bands/b1') && r.method === 'GET').flush({ ...mockBand, members: [...mockBand.members.slice(0, 1), { ...pastMember, endYear: 1990 }] });
    expect(component.band()?.members[1].endYear).toBe(1990);
  });

  it('should delete a confirmed member and reload the band detail without it', async () => {
    await setup('b1');
    fixture.detectChanges();
    httpMock.expectOne((r) => r.url.includes('/bands/b1')).flush(mockBand);
    component.openDeleteMemberModal(mockBand.members[0]);

    expect(component.deletingMember()).toEqual(mockBand.members[0]);

    component.confirmDeleteMember();
    const deletion = httpMock.expectOne((r) => r.url.endsWith('/bands/b1/members/m1') && r.method === 'DELETE');
    deletion.flush(null);
    httpMock.expectOne((r) => r.url.endsWith('/bands/b1') && r.method === 'GET').flush({ ...mockBand, members: [mockBand.members[1]] });

    expect(component.deletingMember()).toBeNull();
    expect(component.band()?.members).toEqual([mockBand.members[1]]);
  });

  it('should close member deletion confirmation without calling the API', async () => {
    await setup('b1');
    fixture.detectChanges();
    httpMock.expectOne((r) => r.url.includes('/bands/b1')).flush(mockBand);
    component.openDeleteMemberModal(mockBand.members[0]);

    component.closeDeleteMemberModal();

    expect(component.deletingMember()).toBeNull();
    httpMock.expectNone((r) => r.method === 'DELETE');
  });

  it('should show a member edited as past in Past Members after reload', async () => {
    await setup('b1');
    fixture.detectChanges();
    httpMock.expectOne((r) => r.url.includes('/bands/b1')).flush(mockBand);
    component.openEditMemberModal(mockBand.members[0]);

    component.saveMember({ name: 'Barney Greenway', instrument: 'Vocals', startYear: 1989, endYear: 2020, memberType: 'past' });
    httpMock.expectOne((r) => r.url.endsWith('/bands/b1/members/m1') && r.method === 'PUT').flush({ id: 'm1' });
    httpMock.expectOne((r) => r.url.endsWith('/bands/b1') && r.method === 'GET').flush({
      ...mockBand,
      members: [{ ...mockBand.members[0], endYear: 2020, isCurrent: false }, mockBand.members[1]],
    });

    expect(component.currentMembers()).toEqual([]);
    expect(component.pastMembers().map((member) => member.id)).toEqual(['m1', 'm2']);
  });

  it('should replace Current Members with Last Known Lineup after changing the band to SplitUp', async () => {
    await setup('b1');
    fixture.detectChanges();
    httpMock.expectOne((r) => r.url.includes('/bands/b1')).flush(mockBand);
    component.saveBand({ name: mockBand.name, country: mockBand.country, genreId: 'genre-1', status: 'SplitUp' });
    httpMock.expectOne((r) => r.url.endsWith('/bands/b1') && r.method === 'PUT').flush({ id: 'b1' });
    httpMock.expectOne((r) => r.url.endsWith('/bands/b1') && r.method === 'GET').flush({
      ...mockBand,
      status: 'SplitUp',
      members: [{ ...mockBand.members[0], isCurrent: false, isLastKnownLineup: true }],
    });
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('Last Known Lineup');
    expect(fixture.nativeElement.textContent).not.toContain('Current Members');
  });

  it('should show a split-up band\'s last known lineup and use that member type', async () => {
    await setup('b1');
    fixture.detectChanges();
    httpMock.expectOne((r) => r.url.includes('/bands/b1')).flush({
      ...mockBand,
      status: 'SplitUp',
      members: [{ ...mockBand.members[1], isLastKnownLineup: true }],
    });
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('Last Known Lineup');
    component.openCreateMemberModal('lastKnown');
    expect(component.memberModalType()).toBe('lastKnown');
  });
});
