import { Component } from '@angular/core';
import { UiPage } from '../../shared/ui/ui-page/ui-page';
import { UiButton } from '../../shared/ui/ui-button/ui-button';
import { RouterLink } from '@angular/router';
import { RouterOutlet } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-signup',
  imports: [UiPage, UiButton, FormsModule, RouterLink, RouterOutlet],
  templateUrl: './signup.html',
  styleUrl: './signup.css',
})
export class Signup {
    nome = ''
    email = ''
    password = ''
    confirmPassword=''
    acceptTerms = false


    get passwordsMatch(): boolean {
  return this.password === this.confirmPassword;
}

  get canSubmit(): boolean {
    return (
      this.acceptTerms &&
      this.passwordsMatch &&
      this.password.length >= 8 &&
      this.nome.length > 0 &&
      this.email.length > 0
    );
  }
}
