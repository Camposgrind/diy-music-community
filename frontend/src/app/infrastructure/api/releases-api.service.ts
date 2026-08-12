import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ReleaseDetailModel } from './models';

@Injectable({ providedIn: 'root' })
export class ReleasesApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/releases`;

  getReleaseDetail(id: string): Observable<ReleaseDetailModel> {
    return this.http.get<ReleaseDetailModel>(`${this.baseUrl}/${id}`);
  }
}
