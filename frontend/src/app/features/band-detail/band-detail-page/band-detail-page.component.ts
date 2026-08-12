import { Component, inject, OnInit, signal, computed, ChangeDetectionStrategy } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { BandsApiService } from '../../../infrastructure/api/bands-api.service';
import { BandDetailModel } from '../../../infrastructure/api/models';
import { BackToResultsComponent } from '../back-to-results/back-to-results.component';
import { BandHeroComponent } from '../band-hero/band-hero.component';
import { BandDiscographyComponent } from '../band-discography/band-discography.component';
import { BandMembersComponent } from '../band-members/band-members.component';

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
  ],
  templateUrl: './band-detail-page.component.html',
  styleUrl: './band-detail-page.component.scss',
})
export class BandDetailPageComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly bandsApi = inject(BandsApiService);

  readonly band = signal<BandDetailModel | null>(null);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);

  readonly currentMembers = computed(() =>
    (this.band()?.members ?? []).filter((m) => m.isCurrent)
  );

  readonly pastMembers = computed(() =>
    (this.band()?.members ?? []).filter((m) => !m.isCurrent)
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

  retry(): void {
    this.error.set(null);
    this.loading.set(true);
    this.ngOnInit();
  }
}
