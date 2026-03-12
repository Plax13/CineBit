import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../environment/environment';
import { tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  isLogged = false;
  currentUser = signal<any>(null);

  constructor(private http: HttpClient) {
    const savedUser = localStorage.getItem('cinebit_session');
    if (savedUser) {
      this.currentUser.set(JSON.parse(savedUser));
      this.isLogged = true;
    }
  }

  login(email: string, password: string) {
    return this.http.post(`${environment.apiUrl}/api/Auth/login`, { email, password })
      .pipe(
        tap((user: any) => {
          this.isLogged = true;
          this.currentUser.set(user);
          localStorage.setItem('cinebit_session', JSON.stringify(user));
        })
      );
  }

  register(nome: string, cognome:string, email: string, password: string) {
    return this.http.post(`${environment.apiUrl}/api/Auth/register`, { nome, cognome, email, password });
  }

  logout() {
    this.isLogged = false;
    this.currentUser.set(null);
    localStorage.removeItem('cinebit_session');
  }
}