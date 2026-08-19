import { Component, inject, OnInit, signal, computed, ChangeDetectionStrategy } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { BandsApiService } from '../../../infrastructure/api/bands-api.service';
import { BandDetailModel, BandWriteRequest, GenreModel } from '../../../infrastructure/api/models';
import { CountriesService, GenresApiService } from '../../../infrastructure/api';
import { ReleasesApiService } from '../../../infrastructure/api';
import { MembersApiService } from '../../../infrastructure/api';
import { forkJoin } from 'rxjs';
import { AuthService } from '../../../core/auth/auth.service';
import { BackToResultsComponent } from '../back-to-results/back-to-results.component';
import { BandHeroComponent } from '../band-hero/band-hero.component';
import { BandDiscographyComponent } from '../band-discography/band-discography.component';
import { BandMembersComponent } from '../band-members/band-members.component';
import { BandEditModalComponent, BandGeneralEditForm } from '../band-edit-modal/band-edit-modal.component';
import { ReleaseModalComponent, ReleaseModalForm } from '../release-modal/release-modal.component';
import { BandReleaseModel, ReleaseDetailModel, ReleaseWriteRequest } from '../../../infrastructure/api/models';
import { BandMemberModel, MemberWriteRequest } from '../../../infrastructure/api/models';
import { MemberModalComponent, MemberModalForm, MemberType } from '../member-modal/member-modal.component';
import { MemberDeleteConfirmationComponent } from '../member-delete-confirmation/member-delete-confirmation.component';

@Component({
  selector: 'dmc-band-detail-page',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    RouterLink,
    BackToResultsComponent,
    BandHeroComponent,
    BandDiscographyComponent,
    BandMembersComponent,
    BandEditModalComponent,
    ReleaseModalComponent,
    MemberModalComponent,
    MemberDeleteConfirmationComponent,
  ],
  templateUrl: './band-detail-page.component.html',
  styleUrl: './band-detail-page.component.scss',
})
export class BandDetailPageComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly bandsApi = inject(BandsApiService);
  private readonly countriesService = inject(CountriesService);
  private readonly genresApi = inject(GenresApiService);
  private readonly auth = inject(AuthService);
  private readonly releasesApi = inject(ReleasesApiService);
  private readonly membersApi = inject(MembersApiService);

  readonly band = signal<BandDetailModel | null>(null);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly countries = signal<string[]>([]);
  readonly genres = signal<GenreModel[]>([]);
  readonly isEditModalOpen = signal(false);
  readonly isUpdatingBand = signal(false);
  readonly editError = signal<string | null>(null);
  readonly editData = signal<BandGeneralEditForm | null>(null);
  readonly releaseModalMode = signal<'create' | 'edit' | null>(null);
  readonly releaseModalData = signal<ReleaseModalForm | null>(null);
  readonly releaseRequestContext = signal<Omit<ReleaseWriteRequest, 'title' | 'releaseType' | 'year'> | null>(null);
  readonly isSavingRelease = signal(false);
  readonly releaseError = signal<string | null>(null);
  readonly editingReleaseId = signal<string | null>(null);
  readonly memberModalMode = signal<'create' | 'edit' | null>(null);
  readonly memberModalType = signal<MemberType>('current');
  readonly memberModalData = signal<MemberModalForm | null>(null);
  readonly editingMemberId = signal<string | null>(null);
  readonly isSavingMember = signal(false);
  readonly memberError = signal<string | null>(null);
  readonly deletingMember = signal<BandMemberModel | null>(null);
  readonly isDeletingMember = signal(false);
  readonly deleteMemberError = signal<string | null>(null);
  readonly isAdmin = this.auth.isAdmin;
  private readonly bandId = signal<string | null>(null);

  readonly currentMembers = computed(() =>
    (this.band()?.members ?? []).filter((m) => m.isCurrent)
  );

  readonly isSplitUp = computed(() => this.band()?.status === 'SplitUp');

  readonly lastKnownMembers = computed(() =>
    (this.band()?.members ?? []).filter((m) => m.isLastKnownLineup)
  );

  readonly pastMembers = computed(() =>
    (this.band()?.members ?? []).filter((m) => !m.isCurrent && !m.isLastKnownLineup)
  );

  readonly hasReleases = computed(() => (this.band()?.releases.length ?? 0) > 0);
  readonly hasCurrentMembers = computed(() => this.currentMembers().length > 0);
  readonly hasPastMembers = computed(() => this.pastMembers().length > 0);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.error.set('Invalid band identifier.');
      this.loading.set(false);
      return;
    }
    this.bandId.set(id);
    this.loadBandDetail();
  }

  retry(): void {
    this.error.set(null);
    this.loading.set(true);
    this.loadBandDetail();
  }

  openEditModal(): void {
    const band = this.band();
    if (!band || this.isUpdatingBand()) return;

    this.editError.set(null);
    forkJoin({ countries: this.countriesService.getCountries(), genres: this.genresApi.getGenres() }).subscribe({
      next: ({ countries, genres }) => {
        this.countries.set(countries);
        this.genres.set(genres);
        this.editData.set({
          name: band.name,
          country: band.country,
          location: band.location,
          formationYear: band.formationYear,
          genreId: genres.find((genre) => genre.name === band.genre)?.id ?? '',
          status: band.status as BandGeneralEditForm['status'],
          musicUrlPortal: band.musicUrlPortal,
          bandContact: band.bandContact,
        });
        this.isEditModalOpen.set(true);
      },
      error: () => this.editError.set('Could not load edit options. Please try again.'),
    });
  }

  closeEditModal(): void {
    if (!this.isUpdatingBand()) {
      this.isEditModalOpen.set(false);
      this.editError.set(null);
    }
  }

  saveBand(data: BandGeneralEditForm): void {
    const band = this.band();
    const id = this.bandId();
    if (!band || !id || this.isUpdatingBand()) return;

    const request: BandWriteRequest = {
      ...data,
      description: band.description,
      logoImageUrl: band.logoImageUrl,
      bandImageUrl: band.bandImageUrl,
    };
    this.isUpdatingBand.set(true);
    this.editError.set(null);
    this.bandsApi.updateBand(id, request).subscribe({
      next: () => {
        this.isUpdatingBand.set(false);
        this.isEditModalOpen.set(false);
        this.loadBandDetail();
      },
      error: (err: { error?: { message?: string } }) => {
        this.isUpdatingBand.set(false);
        this.editError.set(err.error?.message ?? 'Could not update band. Please try again.');
      },
    });
  }

  openCreateReleaseModal(): void {
    if (this.isSavingRelease()) return;
    this.releaseError.set(null);
    this.editingReleaseId.set(null);
    this.releaseRequestContext.set({ releaseDate: null, labelText: null, coverImageUrl: null, tracks: [] });
    this.releaseModalData.set({ title: '', releaseType: 'Album', year: null });
    this.releaseModalMode.set('create');
  }

  openEditReleaseModal(release: BandReleaseModel): void {
    if (this.isSavingRelease()) return;
    this.releaseError.set(null);
    this.releasesApi.getReleaseDetail(release.id).subscribe({
      next: (detail) => {
        this.editingReleaseId.set(release.id);
        this.releaseRequestContext.set(this.releaseContextFromDetail(detail));
        this.releaseModalData.set({ title: detail.title, releaseType: detail.releaseType as ReleaseModalForm['releaseType'], year: detail.year });
        this.releaseModalMode.set('edit');
      },
      error: () => this.releaseError.set('Could not load release details. Please try again.'),
    });
  }

  closeReleaseModal(): void {
    if (!this.isSavingRelease()) {
      this.releaseModalMode.set(null);
      this.releaseError.set(null);
    }
  }

  saveRelease(data: ReleaseModalForm): void {
    const bandId = this.bandId();
    const context = this.releaseRequestContext();
    const mode = this.releaseModalMode();
    if (!bandId || !context || !mode || this.isSavingRelease()) return;

    const request: ReleaseWriteRequest = { ...context, ...data };
    this.isSavingRelease.set(true);
    this.releaseError.set(null);
    const request$ = mode === 'create'
      ? this.releasesApi.createRelease(bandId, request)
      : this.releasesApi.updateRelease(bandId, this.editingReleaseId()!, request);

    request$.subscribe({
      next: () => {
        this.isSavingRelease.set(false);
        this.releaseModalMode.set(null);
        this.loadBandDetail();
      },
      error: (err: { error?: { message?: string } }) => {
        this.isSavingRelease.set(false);
        this.releaseError.set(err.error?.message ?? 'Could not save release. Please try again.');
      },
    });
  }

  openCreateMemberModal(type: MemberType): void {
    if (this.isSavingMember()) return;
    this.memberError.set(null);
    this.editingMemberId.set(null);
    this.memberModalType.set(type);
    this.memberModalData.set({ name: '', instrument: null, startYear: null, endYear: null, memberType: type });
    this.memberModalMode.set('create');
  }

  openEditMemberModal(member: BandMemberModel): void {
    if (this.isSavingMember()) return;
    const type: MemberType = member.isLastKnownLineup ? 'lastKnown' : member.isCurrent ? 'current' : 'past';
    this.memberError.set(null);
    this.editingMemberId.set(member.id);
    this.memberModalType.set(type);
    this.memberModalData.set({ name: member.name, instrument: member.instrument, startYear: member.startYear, endYear: member.endYear, memberType: type });
    this.memberModalMode.set('edit');
  }

  closeMemberModal(): void {
    if (!this.isSavingMember()) {
      this.memberModalMode.set(null);
      this.memberError.set(null);
    }
  }

  openDeleteMemberModal(member: BandMemberModel): void {
    if (this.isSavingMember() || this.isDeletingMember()) return;
    this.deleteMemberError.set(null);
    this.deletingMember.set(member);
  }

  closeDeleteMemberModal(): void {
    if (!this.isDeletingMember()) {
      this.deletingMember.set(null);
      this.deleteMemberError.set(null);
    }
  }

  confirmDeleteMember(): void {
    const bandId = this.bandId();
    const member = this.deletingMember();
    if (!bandId || !member || this.isDeletingMember()) return;

    this.isDeletingMember.set(true);
    this.deleteMemberError.set(null);
    this.membersApi.deleteMember(bandId, member.id).subscribe({
      next: () => {
        this.isDeletingMember.set(false);
        this.deletingMember.set(null);
        this.loadBandDetail();
      },
      error: (err: { error?: { message?: string } }) => {
        this.isDeletingMember.set(false);
        this.deleteMemberError.set(err.error?.message ?? 'Could not delete member. Please try again.');
      },
    });
  }

  saveMember(data: MemberModalForm): void {
    const bandId = this.bandId();
    const mode = this.memberModalMode();
    if (!bandId || !mode || this.isSavingMember()) return;

    const request: MemberWriteRequest = {
      name: data.name, instrument: data.instrument, startYear: data.startYear,
      endYear: data.memberType === 'past' || data.memberType === 'lastKnown' ? data.endYear : null,
      isCurrent: data.memberType === 'current',
      isLastKnownLineup: data.memberType === 'lastKnown',
    };
    this.isSavingMember.set(true);
    this.memberError.set(null);
    const request$ = mode === 'create'
      ? this.membersApi.createMember(bandId, request)
      : this.membersApi.updateMember(bandId, this.editingMemberId()!, request);
    request$.subscribe({
      next: () => {
        this.isSavingMember.set(false);
        this.memberModalMode.set(null);
        this.loadBandDetail();
      },
      error: (err: { error?: { message?: string } }) => {
        this.isSavingMember.set(false);
        this.memberError.set(err.error?.message ?? 'Could not save member. Please try again.');
      },
    });
  }

  private loadBandDetail(): void {
    const id = this.bandId();
    if (!id) return;

    this.loading.set(true);
    this.bandsApi.getBandDetail(id).subscribe({
      next: (data) => {
        this.band.set(data);
        this.loading.set(false);
      },
      error: (err: { status?: number }) => {
        this.loading.set(false);
        if (err.status === 404) {
          this.error.set('Band not found.');
        } else {
          this.error.set('Failed to load band details. Please try again.');
        }
      },
    });
  }

  private releaseContextFromDetail(detail: ReleaseDetailModel): Omit<ReleaseWriteRequest, 'title' | 'releaseType' | 'year'> {
    return {
      releaseDate: detail.releaseDate,
      labelText: detail.labelText,
      coverImageUrl: detail.coverImageUrl,
      tracks: detail.tracks.map((track) => ({ title: track.title })),
    };
  }
}
