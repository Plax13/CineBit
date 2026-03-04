import { Component } from '@angular/core';
import { UiPage } from "../../shared/ui/ui-page/ui-page";
import { UiButton } from '../../shared/ui/ui-button/ui-button';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { RouterOutlet } from '@angular/router';


@Component({
  selector: 'login',
  imports: [UiPage, UiButton, FormsModule, RouterLink, RouterOutlet],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
    email = ''
    password = ''
}
