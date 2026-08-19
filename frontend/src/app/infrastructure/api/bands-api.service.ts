import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { BandDetailModel, BandListItemModel, BandWriteRequest, GetBandsQuery, PagedResult, BandImageType, TemporaryBandImageUploadResponse, ConfirmBandImageResponse } from './models';

@Injectable({ providedIn: 'root' })
export class BandsApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/bands`;

  getBands(query: GetBandsQuery): Observable<PagedResult<BandListItemModel>> {
    let params = new HttpParams()
      .set('page', query.page.toString())
      .set('pageSize', query.pageSize.toString());

    if (query.name) {
      params = params.set('name', query.name);
    }
    if (query.country) {
      params = params.set('country', query.country);
    }
    if (query.genreId) {
      params = params.set('genreId', query.genreId);
    }
    if (query.status) {
      params = params.set('status', query.status);
    }

    return this.http.get<PagedResult<BandListItemModel>>(this.baseUrl, { params });
  }

  getBandDetail(id: string): Observable<BandDetailModel> {
    return this.http.get<BandDetailModel>(`${this.baseUrl}/${id}`);
  }

  createBand(request: BandWriteRequest): Observable<BandDetailModel> {
    return this.http.post<BandDetailModel>(this.baseUrl, request);
  }

  updateBand(id: string, request: BandWriteRequest): Observable<BandDetailModel> {
    return this.http.put<BandDetailModel>(`${this.baseUrl}/${id}`, request);
  }

  deleteBand(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  uploadTemporaryBandImage(bandId: string, imageType: BandImageType, file: File): Observable<TemporaryBandImageUploadResponse> {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('imageType', imageType);
    return this.http.post<TemporaryBandImageUploadResponse>(`${this.baseUrl}/${bandId}/images/temporary`, formData);
  }

  confirmBandImage(bandId: string, imageType: BandImageType, temporaryFileId: string): Observable<ConfirmBandImageResponse> {
    return this.http.post<ConfirmBandImageResponse>(`${this.baseUrl}/${bandId}/images/confirm`, { imageType, temporaryFileId });
  }
}
