import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { IFilmCard } from '../models/i-film-card';

@Injectable({ providedIn: 'root' })
export class FilmService {
  private baseUrl = 'http://localhost:5201';

  constructor(private http: HttpClient) {}

  getHome(take = 20): Observable<IFilmCard[]> {
    return this.http.get<IFilmCard[]>(`${this.baseUrl}/api/Film/home?take=${take}`);
  }
}