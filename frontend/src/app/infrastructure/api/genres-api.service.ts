import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { GenreModel } from './models';

@Injectable({ providedIn: 'root' })
export class GenresApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/genres`;

  getGenres(): Observable<GenreModel[]> {
    return this.http.get<GenreModel[]>(this.baseUrl);
  }
}
