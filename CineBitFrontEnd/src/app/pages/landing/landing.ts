import { Component } from '@angular/core';
import { UiButton } from '../../shared/ui/ui-button/ui-button';
import { UiPage } from '../../shared/ui/ui-page/ui-page';
import { UiHeroBanner } from '../../shared/ui/ui-hero-banner/ui-hero-banner';
import { RouterLink } from '@angular/router';
import { RouterOutlet } from '@angular/router';



@Component({
  selector: 'landing',
  imports: [UiButton, UiPage, UiHeroBanner, RouterLink, RouterOutlet ],
  templateUrl: './landing.html',
  styleUrl: './landing.css',
})
export class Landing {

}
