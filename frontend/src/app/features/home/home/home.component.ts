import { Component, inject, OnInit, signal } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { BandsSearchFormComponent, BandSearchFilters } from '../bands-search-form/bands-search-form.component';
import { BandsResultsComponent } from '../bands-results/bands-results.component';
import {
  BandsApiService,
  CountriesService,
  GenresApiService,
  BandListItemModel,
  PagedResult,
  GetBandsQuery,
  GenreModel,
} from '../../../infrastructure/api';

@Component({
  selector: 'dmc-home',
  standalone: true,
  imports: [BandsSearchFormComponent, BandsResultsComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})
export class HomeComponent implements OnInit {
  private readonly bandsApi = inject(BandsApiService);
  private readonly countriesService = inject(CountriesService);
  private readonly genresApi = inject(GenresApiService);

  readonly countries = signal<string[]>([]);
  readonly genres = signal<GenreModel[]>([]);
  readonly genresError = signal<string | null>(null);
  readonly results = signal<PagedResult<BandListItemModel> | null>(null);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  readonly toastMessage = signal<string | null>(null);

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
        this.genresError.set('Could not load genres — is the backend running?');
      },
    });
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
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.fetchBands();
  }

  dismissToast(): void {
    this.toastMessage.set(null);
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
