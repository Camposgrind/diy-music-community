import { ChangeDetectionStrategy, Component, inject, OnInit, signal } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { BandsSearchFormComponent, BandSearchFilters } from '../bands-search-form/bands-search-form.component';
import { BandsResultsComponent } from '../bands-results/bands-results.component';
import { BandCreateModalComponent } from '../band-create-modal/band-create-modal.component';
import {
  BandsApiService,
  CountriesService,
  GenresApiService,
  BandListItemModel,
  PagedResult,
  GetBandsQuery,
  GenreModel,
  BandWriteRequest,
} from '../../../infrastructure/api';
import { SearchStateService } from '../search-state.service';
import { AuthService } from '../../../core/auth/auth.service';
import { ToastService } from '../../../core/toast/toast.service';

@Component({
  selector: 'dmc-home',
  standalone: true,
  imports: [BandsSearchFormComponent, BandsResultsComponent, BandCreateModalComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HomeComponent implements OnInit {
  private readonly bandsApi = inject(BandsApiService);
  private readonly countriesService = inject(CountriesService);
  private readonly genresApi = inject(GenresApiService);
  private readonly searchState = inject(SearchStateService);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly toast = inject(ToastService);

  readonly countries = signal<string[]>([]);
  readonly genres = signal<GenreModel[]>([]);
  readonly genresError = signal<string | null>(null);
  readonly results = signal<PagedResult<BandListItemModel> | null>(null);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  readonly toastMessage = signal<string | null>(null);
  readonly savedFilters = signal<BandSearchFilters | null>(null);
  readonly isCreateModalOpen = signal(false);
  readonly isCreatingBand = signal(false);
  readonly createBandError = signal<string | null>(null);
  readonly isAdmin = this.auth.isAdmin;

  private currentFilters: BandSearchFilters = { name: '', country: '', genreId: '' };
  private currentPage = 1;
  private readonly pageSize = 20;

  ngOnInit(): void {
    this.countriesService.getCountries().subscribe({
      next: (data) => this.countries.set(data),
      error: (err: unknown) => {
        console.error('Failed to load countries:', err);
      },
    });

    this.genresApi.getGenres().subscribe({
      next: (data) => this.genres.set(data),
      error: (err: unknown) => {
        console.error('Failed to load genres:', err);
        this.genresError.set('Could not load genres \u2014 is the backend running?');
      },
    });

    const saved = this.searchState.state();
    if (saved) {
      this.currentPage = saved.page;
      this.currentFilters = {
        name: saved.query.name ?? '',
        country: saved.query.country ?? '',
        genreId: saved.query.genreId ?? '',
      };
      this.savedFilters.set(this.currentFilters);
      this.fetchBands();
    }
  }

  onSearch(filters: BandSearchFilters): void {
    this.currentFilters = filters;
    this.currentPage = 1;
    this.fetchBands();
  }

  onReset(): void {
    this.currentFilters = { name: '', country: '', genreId: '' };
    this.currentPage = 1;
    this.results.set(null);
    this.error.set(null);
    this.toastMessage.set(null);
    this.savedFilters.set(null);
    this.searchState.clear();
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.fetchBands();
  }

  dismissToast(): void {
    this.toastMessage.set(null);
  }

  openCreateModal(): void {
    this.createBandError.set(null);
    this.isCreateModalOpen.set(true);
  }

  closeCreateModal(): void {
    if (!this.isCreatingBand()) {
      this.isCreateModalOpen.set(false);
      this.createBandError.set(null);
    }
  }

  createBand(request: BandWriteRequest): void {
    if (this.isCreatingBand()) return;

    this.isCreatingBand.set(true);
    this.createBandError.set(null);
    this.bandsApi.createBand(request).subscribe({
      next: (band) => {
        this.isCreatingBand.set(false);
        this.isCreateModalOpen.set(false);
        this.toast.success('Band created successfully.');
        void this.router.navigate(['/bands', band.id]);
      },
      error: (err: HttpErrorResponse) => {
        this.isCreatingBand.set(false);
        const message = err.error?.message ?? 'Could not create band. Please try again.';
        this.createBandError.set(message);
        this.toast.error(message);
      },
    });
  }

  private fetchBands(): void {
    this.loading.set(true);
    this.error.set(null);
    this.toastMessage.set(null);

    const query: GetBandsQuery = {
      page: this.currentPage,
      pageSize: this.pageSize,
    };

    if (this.currentFilters.name) {
      query.name = this.currentFilters.name;
    }
    if (this.currentFilters.country) {
      query.country = this.currentFilters.country;
    }
    if (this.currentFilters.genreId) {
      query.genreId = this.currentFilters.genreId;
    }

    this.searchState.save({ query, page: this.currentPage });

    this.bandsApi.getBands(query).subscribe({
      next: (result) => {
        this.results.set(result);
        this.loading.set(false);
      },
      error: (err: HttpErrorResponse) => {
        this.loading.set(false);

        if (err.status === 422 && err.error?.code === 'Band.TooManyResults') {
          this.toastMessage.set(err.error.message);
          this.results.set(null);
        } else {
          this.error.set(err.error?.message ?? 'An unexpected error occurred. Please try again.');
        }
      },
    });
  }
}
