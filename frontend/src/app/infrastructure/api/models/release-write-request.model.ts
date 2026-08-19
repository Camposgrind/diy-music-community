export interface ReleaseWriteRequest {
  title: string;
  releaseType: 'Demo' | 'EP' | 'Album' | 'Split' | 'Compilation';
  releaseDate: string | null;
  year: number | null;
  labelText: string | null;
  coverImageUrl: string | null;
  formats?: ReleaseFormat[];
  tracks: { title: string }[];
}

export type ReleaseFormat = 'Vinyl7' | 'Vinyl10' | 'Vinyl12' | 'VinylLatheCut' | 'VinylOther' | 'CD' | 'CDR' | 'DVD' | 'Cassette' | 'Digital';
