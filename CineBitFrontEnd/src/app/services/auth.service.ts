import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  isLogged = false;

  // Lista utenti registrati
  private utenti: { nome: string, email: string, password: string }[] = [];

  register(nome: string, email: string, password: string): void {
    this.utenti.push({ nome, email, password });
    console.log('Utente registrato:', { nome, email });
  }

  login(email: string, password: string): boolean {
    const trovato = this.utenti.find(u => u.email === email && u.password === password);
    if (trovato) {
      this.isLogged = true;
      return true;
    }
    return false;
  }

  logout() {
    this.isLogged = false;
  }
}