import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { BandListItemModel, GetBandsQuery, PagedResult } from './models';

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
}
