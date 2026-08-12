import { Component, inject, computed } from '@angular/core';
import { Router } from '@angular/router';
import { ReleaseStateService } from '../release-state.service';

@Component({
  selector: 'dmc-back-to-band',
  standalone: true,
  imports: [],
  templateUrl: './back-to-band.component.html',
  styleUrl: './back-to-band.component.scss',
})
export class BackToBandComponent {
  private readonly releaseState = inject(ReleaseStateService);
  private readonly router = inject(Router);

  readonly hasBand = computed(() => this.releaseState.bandId() !== null);

  goBack(): void {
    const bandId = this.releaseState.bandId();
    if (bandId) {
      this.router.navigate(['/bands', bandId]);
    } else {
      this.router.navigate(['/']);
    }
  }
}
