import { Component, input } from '@angular/core';
import { NgClass } from '@angular/common';
import { BandReleaseModel } from '../../../infrastructure/api/models';

@Component({
  selector: 'dmc-band-discography',
  standalone: true,
  imports: [NgClass],
  templateUrl: './band-discography.component.html',
  styleUrl: './band-discography.component.scss',
})
export class BandDiscographyComponent {
  readonly releases = input.required<BandReleaseModel[]>();

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
