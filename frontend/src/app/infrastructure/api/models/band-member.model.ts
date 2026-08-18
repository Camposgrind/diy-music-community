import type { BandMemberOtherBandModel } from './band-member-other-band.model';

export interface BandMemberModel {
  id: string;
  name: string;
  instrument: string | null;
  startYear: number | null;
  endYear: number | null;
  isCurrent: boolean;
  isLastKnownLineup: boolean;
  otherBands: BandMemberOtherBandModel[];
}
