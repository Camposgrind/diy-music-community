import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ReleaseDetailModel, ReleaseWriteRequest, TemporaryBandImageUploadResponse, ConfirmBandImageResponse } from './models';

@Injectable({ providedIn: 'root' })
export class ReleasesApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/releases`;
  private readonly bandsBaseUrl = `${environment.apiBaseUrl}/bands`;

  getReleaseDetail(id: string): Observable<ReleaseDetailModel> {
    return this.http.get<ReleaseDetailModel>(`${this.baseUrl}/${id}`);
  }

  createRelease(bandId: string, request: ReleaseWriteRequest): Observable<ReleaseDetailModel> {
    return this.http.post<ReleaseDetailModel>(`${this.bandsBaseUrl}/${bandId}/releases`, request);
  }

  updateRelease(bandId: string, releaseId: string, request: ReleaseWriteRequest): Observable<ReleaseDetailModel> {
    return this.http.put<ReleaseDetailModel>(`${this.bandsBaseUrl}/${bandId}/releases/${releaseId}`, request);
  }

  updateReleaseTracks(bandId: string, releaseId: string, tracks: { title: string }[]): Observable<ReleaseDetailModel> {
    return this.http.put<ReleaseDetailModel>(`${this.bandsBaseUrl}/${bandId}/releases/${releaseId}/tracks`, { tracks });
  }

  deleteRelease(releaseId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${releaseId}`);
  }

  deleteAllTracks(releaseId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${releaseId}/tracks`);
  }

  uploadTemporaryCover(releaseId: string, file: File): Observable<TemporaryBandImageUploadResponse> {
    const formData = new FormData(); formData.append('file', file); formData.append('imageType', 'ReleaseCover');
    return this.http.post<TemporaryBandImageUploadResponse>(`${this.baseUrl}/${releaseId}/images/temporary`, formData);
  }

  confirmCover(releaseId: string, temporaryFileId: string): Observable<ConfirmBandImageResponse> {
    return this.http.post<ConfirmBandImageResponse>(`${this.baseUrl}/${releaseId}/images/confirm`, { imageType: 'ReleaseCover', temporaryFileId });
  }
}
