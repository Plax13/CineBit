import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class PreferitiService {
  private apiUrl = 'http://localhost:5201/api/Preferiti';

  constructor(private http: HttpClient) {}

  aggiungi(dto: any): Observable<any> {
    return this.http.post(this.apiUrl, dto);
  }

  getPerUtente(idUtente: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/utente/${idUtente}`);
  }
}
