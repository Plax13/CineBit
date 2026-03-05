import { Component, Input } from '@angular/core';

@Component({
  selector: 'ui-hero-banner',
  standalone: true,
  templateUrl: './ui-hero-banner.html',
})
export class UiHeroBanner {
  // usa \n per andare a capo
  @Input() rightText = 'I tuoi film preferiti\ncome vuoi, quando vuoi';
  @Input() leftText = 'Chiedi a CineBot\nun consiglio in base a cosa vuoi vedere';
}