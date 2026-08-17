import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ReleaseDetailModel, ReleaseWriteRequest } from './models';

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
}
