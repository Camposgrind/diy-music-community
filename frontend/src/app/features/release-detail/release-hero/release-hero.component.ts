import { Component, input, signal } from '@angular/core';
import { ReleaseDetailModel } from '../../../infrastructure/api/models';

@Component({
  selector: 'dmc-release-hero',
  standalone: true,
  imports: [],
  templateUrl: './release-hero.component.html',
  styleUrl: './release-hero.component.scss',
})
export class ReleaseHeroComponent {
  readonly release = input.required<ReleaseDetailModel>();

  readonly fallback = 'images/grindLogo.jpg';
  readonly lightboxUrl = signal<string | null>(null);

  isCustomImage(url: string | null | undefined): boolean {
    return !!url && url !== this.fallback;
  }

  openLightbox(url: string | null | undefined): void {
    if (this.isCustomImage(url)) {
      this.lightboxUrl.set(url!);
    }
  }

  closeLightbox(): void {
    this.lightboxUrl.set(null);
  }

  onImgError(event: Event): void {
    const el = event.target as HTMLImageElement;
    el.src = this.fallback;
  }

  typeClass(type: string): string {
    return `type--${type.toLowerCase()}`;
  }
}
