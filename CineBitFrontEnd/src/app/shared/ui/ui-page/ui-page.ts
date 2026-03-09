import { Component } from '@angular/core';
import { Input } from '@angular/core';
import { IFilmCard } from '../../../models/i-film-card';
@Component({
  selector: 'ui-page',
  imports: [],
  templateUrl: './ui-page.html',
  styleUrl: './ui-page.css',
})
export class UiPage {
      @Input() showBrand = true; // se in una pagina non lo vuoi
      movies: IFilmCard[] = [];
      loading = false;
}
