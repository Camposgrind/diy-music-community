import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class ReleaseStateService {
  private readonly _bandId = signal<string | null>(null);

  readonly bandId = this._bandId.asReadonly();

  saveBandId(id: string): void {
    this._bandId.set(id);
  }

  clear(): void {
    this._bandId.set(null);
  }
}
