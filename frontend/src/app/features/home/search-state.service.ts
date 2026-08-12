import { Injectable, signal } from '@angular/core';
import { GetBandsQuery } from '../../infrastructure/api/models';

export interface SearchState {
  query: GetBandsQuery;
  page: number;
}

@Injectable({ providedIn: 'root' })
export class SearchStateService {
  private readonly _state = signal<SearchState | null>(null);

  readonly state = this._state.asReadonly();

  save(state: SearchState): void {
    this._state.set(state);
  }

  clear(): void {
    this._state.set(null);
  }
}
