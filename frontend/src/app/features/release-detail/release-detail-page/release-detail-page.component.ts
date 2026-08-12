import {
  Component,
  inject,
  OnInit,
  signal,
  computed,
  ChangeDetectionStrategy,
} from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ReleasesApiService } from '../../../infrastructure/api/releases-api.service';
import { ReleaseDetailModel } from '../../../infrastructure/api/models';
import { BackToBandComponent } from '../back-to-band/back-to-band.component';
import { ReleaseHeroComponent } from '../release-hero/release-hero.component';
import { ReleaseTracksComponent } from '../release-tracks/release-tracks.component';

@Component({
  selector: 'dmc-release-detail-page',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterLink, BackToBandComponent, ReleaseHeroComponent, ReleaseTracksComponent],
  templateUrl: './release-detail-page.component.html',
  styleUrl: './release-detail-page.component.scss',
})
export class ReleaseDetailPageComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly releasesApi = inject(ReleasesApiService);

  readonly release = signal<ReleaseDetailModel | null>(null);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);

  readonly hasTracks = computed(() => (this.release()?.tracks.length ?? 0) > 0);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.error.set('Invalid release identifier.');
      this.loading.set(false);
      return;
    }
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

  retry(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) return;
    this.loading.set(true);
    this.error.set(null);
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
