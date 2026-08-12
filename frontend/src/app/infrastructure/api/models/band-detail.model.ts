import type { BandMemberModel } from './band-member.model';
import type { BandReleaseModel } from './band-release.model';

export interface BandDetailModel {
  id: string;
  name: string;
  country: string;
  location: string | null;
  status: string;
  genre: string | null;
  formationYear: number | null;
  description: string | null;
  logoImageUrl: string | null;
  bandImageUrl: string | null;
  musicUrlPortal: string | null;
  bandContact: string | null;
  releases: BandReleaseModel[];
  members: BandMemberModel[];
}
