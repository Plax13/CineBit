import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AiService {
  private apiUrl = 'http://localhost:5201/api/moviesearch/search';

  lastSearchQuery: string = '';
  lastSearchResults: any[] = [];

  constructor(private http: HttpClient) { }

  searchMovies(query: string): Observable<any> {
    this.lastSearchQuery = query;
    return this.http.post<any>(this.apiUrl, { query });
  }
}
