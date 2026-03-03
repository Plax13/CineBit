import { Component } from '@angular/core';
import { UiButton } from '../../shared/ui/ui-button/ui-button';
import { UiPage } from '../../shared/ui/ui-page/ui-page';
import { UiHeroBanner } from '../../shared/ui/ui-hero-banner/ui-hero-banner';

@Component({
  selector: 'landing',
  imports: [UiButton, UiPage, UiHeroBanner],
  templateUrl: './landing.html',
  styleUrl: './landing.css',
})
export class Landing {

}
