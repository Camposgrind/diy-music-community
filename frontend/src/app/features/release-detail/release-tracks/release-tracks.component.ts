import { Component, input, computed, output } from '@angular/core';
import { ReleaseTrackModel } from '../../../infrastructure/api/models';

@Component({
  selector: 'dmc-release-tracks',
  standalone: true,
  imports: [],
  templateUrl: './release-tracks.component.html',
  styleUrl: './release-tracks.component.scss',
})
export class ReleaseTracksComponent {
  readonly tracks = input.required<ReleaseTrackModel[]>();
  readonly isAdmin = input(false);
  readonly editTracks = output<void>();
  readonly deleteAllTracks = output<void>();

  readonly sortedTracks = computed(() =>
    [...this.tracks()].sort((a, b) => a.trackNumber - b.trackNumber)
  );
}
