import { Component, input, output, signal } from '@angular/core';
import { BandDetailModel } from '../../../infrastructure/api/models';
import { StatusBadgeComponent } from '../status-badge/status-badge.component';

@Component({
  selector: 'dmc-band-hero',
  standalone: true,
  imports: [StatusBadgeComponent],
  templateUrl: './band-hero.component.html',
  styleUrl: './band-hero.component.scss',
})
export class BandHeroComponent {
  readonly band = input.required<BandDetailModel>();
  readonly isAdmin = input(false);
  readonly edit = output<void>();
  readonly deleteBand = output<void>();

  readonly fallback = 'images/grindLogo.jpg';

  readonly lightboxUrl = signal<string | null>(null);

  isCustomImage(url: string | null): boolean {
    return !!url && url !== this.fallback;
  }

  openLightbox(url: string | null): void {
    if (this.isCustomImage(url)) {
      this.lightboxUrl.set(url);
    }
  }

  closeLightbox(): void {
    this.lightboxUrl.set(null);
  }

  onImgError(event: Event): void {
    const el = event.target as HTMLImageElement;
    el.src = this.fallback;
  }
}
