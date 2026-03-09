import { Component } from '@angular/core';
import { UiPage } from '../../shared/ui/ui-page/ui-page';
import { UiButton } from '../../shared/ui/ui-button/ui-button';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-signup',
  imports: [UiPage, UiButton, FormsModule, RouterLink],
  templateUrl: './signup.html',
  styleUrl: './signup.css',
})
export class Signup {
  nome = '';
  cognome = '';  
  email = '';
  password = '';
  confirmPassword = '';
  acceptTerms = false;

  constructor(private auth: AuthService, private router: Router) {}

  get passwordsMatch(): boolean {
    return this.password === this.confirmPassword;
  }

  get canSubmit(): boolean {
    return (
      this.acceptTerms &&
      this.passwordsMatch &&
      this.password.length >= 8 &&
      this.nome.length > 0 &&
      this.cognome.length > 0 &&  // ← aggiunto!
      this.email.length > 0
    );
  }

  onSubmit() {
    this.auth.register(this.nome, this.cognome, this.email, this.password).subscribe({  // ← aggiunto cognome!
      next: () => {
        this.router.navigate(['login']);
      },
      error: (err) => {
        console.error('Errore:', err.error);  // ← messaggio dettagliato
      }
    });
  }
}