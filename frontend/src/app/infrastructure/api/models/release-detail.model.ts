import { ReleaseBandModel } from './release-band.model';
import { ReleaseTrackModel } from './release-track.model';

export interface ReleaseDetailModel {
  id: string;
  title: string;
  releaseType: string;
  releaseDate: string | null;
  year: number | null;
  labelText: string | null;
  coverImageUrl: string | null;
  band: ReleaseBandModel | null;
  formats: string[];
  tracks: ReleaseTrackModel[];
}
