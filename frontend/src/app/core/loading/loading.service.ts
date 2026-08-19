import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class LoadingService {
  private readonly pendingRequests = signal(0);
  readonly isLoading = this.pendingRequests.asReadonly();

  begin(): void {
    this.pendingRequests.update((count) => count + 1);
  }

  end(): void {
    this.pendingRequests.update((count) => Math.max(0, count - 1));
  }
}
