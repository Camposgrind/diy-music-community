import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, shareReplay } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class CountriesService {
  private readonly http = inject(HttpClient);
  private countries$: Observable<string[]> | null = null;

  getCountries(): Observable<string[]> {
    if (!this.countries$) {
      this.countries$ = this.http
        .get<string[]>('data/countries.json')
        .pipe(shareReplay(1));
    }
    return this.countries$;
  }
}
