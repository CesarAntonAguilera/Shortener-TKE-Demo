import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { ShortenUrlResponseDto } from '../models/shortenUrlResponseDto.model';
import { ShortenUrlRequestDto } from '../models/shortenUrlRequestDto.model';

@Injectable({ providedIn: 'root' })
export class UrlShortenerService {
  private readonly baseUrl = environment.apiBaseUrl;

  constructor(private http: HttpClient) {}

  shorten(longUrl: string): Observable<ShortenUrlResponseDto> {
    const body: ShortenUrlRequestDto = { longUrl };
    return this.http.post<ShortenUrlResponseDto>(`${this.baseUrl}/api/shorten`, body);
  }
}
