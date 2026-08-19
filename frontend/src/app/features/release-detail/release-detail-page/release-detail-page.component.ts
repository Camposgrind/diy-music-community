import {
  Component,
  inject,
  OnInit,
  signal,
  computed,
  ChangeDetectionStrategy,
} from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ReleasesApiService } from '../../../infrastructure/api/releases-api.service';
import { ReleaseDetailModel, ReleaseWriteRequest } from '../../../infrastructure/api/models';
import { AuthService } from '../../../core/auth/auth.service';
import { BackToBandComponent } from '../back-to-band/back-to-band.component';
import { ReleaseHeroComponent } from '../release-hero/release-hero.component';
import { ReleaseTracksComponent } from '../release-tracks/release-tracks.component';
import { ReleaseDetailEditForm, ReleaseDetailEditModalComponent } from '../release-detail-edit-modal/release-detail-edit-modal.component';
import { ReleaseModalComponent, ReleaseModalForm } from '../../band-detail/release-modal/release-modal.component';
import { ReleaseDeleteConfirmationComponent } from '../../band-detail/release-delete-confirmation/release-delete-confirmation.component';
import { TrackListDeleteConfirmationComponent } from '../track-list-delete-confirmation/track-list-delete-confirmation.component';
import { ToastService } from '../../../core/toast/toast.service';

@Component({
  selector: 'dmc-release-detail-page',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterLink, BackToBandComponent, ReleaseHeroComponent, ReleaseTracksComponent, ReleaseDetailEditModalComponent, ReleaseModalComponent, ReleaseDeleteConfirmationComponent, TrackListDeleteConfirmationComponent],
  templateUrl: './release-detail-page.component.html',
  styleUrl: './release-detail-page.component.scss',
})
export class ReleaseDetailPageComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly releasesApi = inject(ReleasesApiService);
  private readonly auth = inject(AuthService);
  private readonly toast = inject(ToastService);

  readonly release = signal<ReleaseDetailModel | null>(null);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly isEditModalOpen = signal(false);
  readonly isUpdatingRelease = signal(false);
  readonly editError = signal<string | null>(null);
  readonly editData = signal<ReleaseDetailEditForm | null>(null);
  readonly isDetailsModalOpen = signal(false);
  readonly detailsData = signal<ReleaseModalForm | null>(null);
  readonly isReleaseDeleteOpen = signal(false);
  readonly isDeletingRelease = signal(false);
  readonly releaseDeleteError = signal<string | null>(null);
  readonly isDeleteAllTracksOpen = signal(false);
  readonly isDeletingAllTracks = signal(false);
  readonly deleteAllTracksError = signal<string | null>(null);
  readonly isAdmin = this.auth.isAdmin;
  private readonly releaseId = signal<string | null>(null);

  readonly hasTracks = computed(() => (this.release()?.tracks.length ?? 0) > 0);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.error.set('Invalid release identifier.');
      this.loading.set(false);
      return;
    }
    this.releaseId.set(id);
    this.loadReleaseDetail();
  }

  retry(): void {
    if (!this.releaseId()) return;
    this.loading.set(true);
    this.error.set(null);
    this.loadReleaseDetail();
  }

  openEditReleaseModal(): void {
    const release = this.release();
    if (!release || this.isUpdatingRelease()) return;

    this.editError.set(null);
    this.editData.set({
      title: release.title,
      releaseType: release.releaseType as ReleaseDetailEditForm['releaseType'],
      year: release.year,
      tracks: release.tracks.map((track) => ({ title: track.title })),
    });
    this.isEditModalOpen.set(true);
  }

  openEditDetailsModal(): void {
    const release = this.release();
    if (!release || this.isUpdatingRelease()) return;
    this.editError.set(null);
    this.detailsData.set({ title: release.title, releaseType: release.releaseType as ReleaseModalForm['releaseType'], releaseDate: release.releaseDate, year: release.year, labelText: release.labelText, formats: release.formats as ReleaseModalForm['formats'] });
    this.isDetailsModalOpen.set(true);
  }

  closeEditDetailsModal(): void {
    if (!this.isUpdatingRelease()) {
      this.isDetailsModalOpen.set(false);
      this.editError.set(null);
    }
  }

  saveReleaseDetails(data: ReleaseModalForm): void {
    const release = this.release();
    const bandId = release?.band?.bandId;
    if (!release || !bandId || this.isUpdatingRelease()) return;
    const request: ReleaseWriteRequest = { ...data, coverImageUrl: release.coverImageUrl, tracks: release.tracks.map(track => ({ title: track.title })) };
    this.isUpdatingRelease.set(true);
    this.editError.set(null);
    this.releasesApi.updateRelease(bandId, release.id, request).subscribe({
      next: () => { this.isUpdatingRelease.set(false); this.isDetailsModalOpen.set(false); this.toast.success('Release updated successfully.'); this.loadReleaseDetail(); },
      error: (err: { error?: { message?: string } }) => { this.isUpdatingRelease.set(false); const message = err.error?.message ?? 'Could not update release. Please try again.'; this.editError.set(message); this.toast.error(message); },
    });
  }

  closeEditReleaseModal(): void {
    if (!this.isUpdatingRelease()) {
      this.isEditModalOpen.set(false);
      this.editError.set(null);
    }
  }

  openReleaseDeleteConfirmation(): void {
    if (!this.isDeletingRelease()) {
      this.releaseDeleteError.set(null);
      this.isReleaseDeleteOpen.set(true);
    }
  }

  closeReleaseDeleteConfirmation(): void {
    if (!this.isDeletingRelease()) {
      this.isReleaseDeleteOpen.set(false);
      this.releaseDeleteError.set(null);
    }
  }

  confirmReleaseDeletion(): void {
    const release = this.release();
    if (!release || this.isDeletingRelease()) {
      return;
    }

    this.isDeletingRelease.set(true);
    this.releaseDeleteError.set(null);
    this.releasesApi.deleteRelease(release.id).subscribe({
      next: () => {
        this.isDeletingRelease.set(false);
        this.isReleaseDeleteOpen.set(false);
        this.toast.success('Release deleted successfully.');
        this.router.navigate(['/bands', release.band?.bandId]);
      },
      error: (err: { error?: { message?: string } }) => {
        this.isDeletingRelease.set(false);
        const message = err.error?.message ?? 'Could not delete release. Please try again.';
        this.releaseDeleteError.set(message);
        this.toast.error(message);
      },
    });
  }

  openDeleteAllTracksConfirmation(): void {
    if (!this.isDeletingAllTracks() && this.hasTracks()) {
      this.deleteAllTracksError.set(null);
      this.isDeleteAllTracksOpen.set(true);
    }
  }

  closeDeleteAllTracksConfirmation(): void {
    if (!this.isDeletingAllTracks()) {
      this.isDeleteAllTracksOpen.set(false);
      this.deleteAllTracksError.set(null);
    }
  }

  confirmDeleteAllTracks(): void {
    const release = this.release();
    if (!release || this.isDeletingAllTracks() || !this.hasTracks()) {
      return;
    }

    this.isDeletingAllTracks.set(true);
    this.deleteAllTracksError.set(null);
    this.releasesApi.deleteAllTracks(release.id).subscribe({
      next: () => {
        this.isDeletingAllTracks.set(false);
        this.isDeleteAllTracksOpen.set(false);
        this.toast.success('All tracks deleted successfully.');
        this.loadReleaseDetail();
      },
      error: (err: { error?: { message?: string } }) => {
        this.isDeletingAllTracks.set(false);
        const message = err.error?.message ?? 'Could not delete tracks. Please try again.';
        this.deleteAllTracksError.set(message);
        this.toast.error(message);
      },
    });
  }

  saveReleaseEdit(data: ReleaseDetailEditForm): void {
    const release = this.release();
    const bandId = release?.band?.bandId;
    if (!release || !bandId || this.isUpdatingRelease()) return;

    this.isUpdatingRelease.set(true);
    this.editError.set(null);
    this.releasesApi.updateReleaseTracks(bandId, release.id, data.tracks).subscribe({
      next: () => {
        this.isUpdatingRelease.set(false);
        this.isEditModalOpen.set(false);
        this.toast.success('Tracks updated successfully.');
        this.loadReleaseDetail();
      },
      error: (err: { error?: { message?: string } }) => {
        this.isUpdatingRelease.set(false);
        const message = err.error?.message ?? 'Could not update release. Please try again.';
        this.editError.set(message);
        this.toast.error(message);
      },
    });
  }

  private loadReleaseDetail(): void {
    const id = this.releaseId();
    if (!id) return;
    this.releasesApi.getReleaseDetail(id).subscribe({
      next: (data) => {
        this.release.set(data);
        this.loading.set(false);
      },
      error: (err: { status?: number }) => {
        this.loading.set(false);
        this.error.set(err.status === 404 ? 'Release not found.' : 'Something went wrong. Please try again.');
      },
    });
  }
}
