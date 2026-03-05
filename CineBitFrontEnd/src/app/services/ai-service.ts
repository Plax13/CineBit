import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AiService {
    private apiUrl = 'http://localhost:5201/api/moviesearch/search';
 
    constructor(private http: HttpClient) {}
 
      searchMovies(query: string): Observable<any> {
    return this.http.post<any>(this.apiUrl, { query });
  }
}
