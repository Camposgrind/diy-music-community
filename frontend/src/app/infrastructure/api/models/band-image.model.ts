export type BandImageType = 'BandPhoto' | 'BandLogo';
export type ImageUploadType = BandImageType | 'ReleaseCover';

export interface TemporaryBandImageUploadResponse {
  temporaryFileId: string;
  originalFileName: string;
  sanitizedFileName: string;
  detectedContentType: string;
  extension: string;
  size: number;
  previewUrl: string | null;
}

export interface ConfirmBandImageResponse {
  bandId: string;
  imageType: ImageUploadType;
  imageUrl: string;
}
