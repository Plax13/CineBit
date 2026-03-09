import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../environment/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  isLogged = false;

  constructor(private http: HttpClient) {}

    login(email: string, password: string) {
    return this.http.post(`${environment.apiUrl}/api/Auth/login`, { email, password });
  }

  register(nome: string, cognome:string, email: string, password: string) {
    return this.http.post(`${environment.apiUrl}/api/Auth/register`, { nome,cognome, email, password });
  }

  logout() {
    this.isLogged = false;
  }
}