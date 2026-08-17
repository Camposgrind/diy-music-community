import { Component, input, inject, output } from '@angular/core';
import { NgClass } from '@angular/common';
import { Router } from '@angular/router';
import { BandReleaseModel } from '../../../infrastructure/api/models';
import { ReleaseStateService } from '../../release-detail/release-state.service';

@Component({
  selector: 'dmc-band-discography',
  standalone: true,
  imports: [NgClass],
  templateUrl: './band-discography.component.html',
  styleUrl: './band-discography.component.scss',
})
export class BandDiscographyComponent {
  private readonly router = inject(Router);
  private readonly releaseState = inject(ReleaseStateService);

  readonly releases = input.required<BandReleaseModel[]>();
  readonly bandId = input.required<string>();
  readonly isAdmin = input(false);
  readonly addRelease = output<void>();
  readonly editRelease = output<BandReleaseModel>();

  navigateToRelease(releaseId: string): void {
    this.releaseState.saveBandId(this.bandId());
    this.router.navigate(['/releases', releaseId]);
  }

  onEditRelease(event: Event, release: BandReleaseModel): void {
    event.stopPropagation();
    this.editRelease.emit(release);
  }

  typeClass(releaseType: string): string {
    switch (releaseType) {
      case 'Album':       return 'type--album';
      case 'EP':          return 'type--ep';
      case 'Demo':        return 'type--demo';
      case 'Split':       return 'type--split';
      case 'Compilation': return 'type--compilation';
      default:            return 'type--default';
    }
  }

  accentClass(releaseType: string): string {
    switch (releaseType) {
      case 'Album':       return 'accent--album';
      case 'EP':          return 'accent--ep';
      case 'Demo':        return 'accent--demo';
      case 'Split':       return 'accent--split';
      case 'Compilation': return 'accent--compilation';
      default:            return '';
    }
  }
}
