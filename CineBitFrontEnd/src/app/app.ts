import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Detail } from './pages/detail/detail';
import { Explore } from './pages/explore/explore';
import { Landing } from './pages/landing/landing';
import { UiButton } from './shared/ui/ui-button/ui-button';
import { UiPage } from './shared/ui/ui-page/ui-page';
import { RouterLink } from '@angular/router';



@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Detail, Explore, Landing, UiButton, UiPage, RouterLink  ],
  templateUrl: './app.html',
  styleUrl: './app.css'
  
})
export class App {

  
}
