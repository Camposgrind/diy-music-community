export interface BandWriteRequest {
  name: string;
  country: string;
  genreId: string;
  status: 'Active' | 'SplitUp' | 'OnHold';
  location?: string | null;
  formationYear?: number | null;
  splitUpYear?: number | null;
  description?: string | null;
  logoImageUrl?: string | null;
  bandImageUrl?: string | null;
  musicUrlPortal?: string | null;
  bandContact?: string | null;
}
