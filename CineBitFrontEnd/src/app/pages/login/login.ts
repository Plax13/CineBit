import { Component } from '@angular/core';
import { UiPage } from "../../shared/ui/ui-page/ui-page";
import { UiButton } from '../../shared/ui/ui-button/ui-button';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';


@Component({
  selector: 'login',
  imports: [UiPage, UiButton, FormsModule, RouterLink ],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
    email = ''
    password = ''
    errore = '';

  constructor(private auth: AuthService, private router: Router) {}

  onSubmit() {
    const ok = this.auth.login(this.email, this.password);
    if (ok) {
      this.router.navigate(['explore']); // ← vai alla home
    } else {
      this.errore = 'Email o password errati';
    }
  }
}