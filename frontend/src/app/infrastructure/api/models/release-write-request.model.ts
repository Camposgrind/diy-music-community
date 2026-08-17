export interface ReleaseWriteRequest {
  title: string;
  releaseType: 'Demo' | 'EP' | 'Album' | 'Split' | 'Compilation';
  releaseDate: string | null;
  year: number | null;
  labelText: string | null;
  coverImageUrl: string | null;
  tracks: { title: string }[];
}
