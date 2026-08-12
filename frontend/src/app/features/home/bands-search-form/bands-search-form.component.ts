import { Component, input, output, effect } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { GenreModel } from '../../../infrastructure/api/models';

export interface BandSearchFilters {
  name: string;
  country: string;
  genreId: string;
}

@Component({
  selector: 'dmc-bands-search-form',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './bands-search-form.component.html',
  styleUrl: './bands-search-form.component.scss',
})
export class BandsSearchFormComponent {
  readonly countries = input<string[]>([]);
  readonly genres = input<GenreModel[]>([]);
  readonly genresError = input<string | null>(null);
  readonly initialFilters = input<BandSearchFilters | null>(null);

  readonly search = output<BandSearchFilters>();
  readonly reset = output<void>();

  readonly form = new FormGroup({
    name: new FormControl(''),
    country: new FormControl(''),
    genreId: new FormControl(''),
  });

  constructor() {
    effect(() => {
      const filters = this.initialFilters();
      if (filters) {
        this.form.patchValue(
          { name: filters.name, country: filters.country, genreId: filters.genreId },
          { emitEvent: false }
        );
      }
    });
  }

  onSearch(): void {
    const value = this.form.getRawValue();
    this.search.emit({
      name: value.name ?? '',
      country: value.country ?? '',
      genreId: value.genreId ?? '',
    });
  }

  onReset(): void {
    this.form.reset({ name: '', country: '', genreId: '' });
    this.reset.emit();
  }
}
