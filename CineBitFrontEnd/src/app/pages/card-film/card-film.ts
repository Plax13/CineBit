
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { IFilmCard } from '../../models/i-film-card';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'card-film',
  imports: [],
  templateUrl: './card-film.html',
  styleUrl: './card-film.css',
})
export class CardFilm {
  @Input() film!: IFilmCard;
  @Output() cliccata = new EventEmitter<number>();
 
  onClick() {
      console.log('CLICK CARD:', this.film.id, this.film.title);
  this.cliccata.emit(this.film.id);
  }
}
