import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { BandMemberModel, MemberWriteRequest } from './models';

@Injectable({ providedIn: 'root' })
export class MembersApiService {
  private readonly http = inject(HttpClient);
  private readonly bandsBaseUrl = `${environment.apiBaseUrl}/bands`;

  createMember(bandId: string, request: MemberWriteRequest): Observable<BandMemberModel> {
    return this.http.post<BandMemberModel>(`${this.bandsBaseUrl}/${bandId}/members`, request);
  }

  updateMember(bandId: string, memberId: string, request: MemberWriteRequest): Observable<BandMemberModel> {
    return this.http.put<BandMemberModel>(`${this.bandsBaseUrl}/${bandId}/members/${memberId}`, request);
  }
}
