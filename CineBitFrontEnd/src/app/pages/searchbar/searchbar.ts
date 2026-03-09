import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AiService} from '../../services/ai-service';
import { Movie } from '../../models/movie';


@Component({
  selector: 'searchbar',
  imports: [FormsModule],
  templateUrl: './searchbar.html',
  styleUrl: './searchbar.css',
})
export class Searchbar {
  @Input() query: string = '';
  @Output() queryChange = new EventEmitter<string>();
  @Output() searchEvent = new EventEmitter<void>();
 
  onInputChange(value: string) {
    this.query = value;           // aggiorna interno
    this.queryChange.emit(value); // aggiorna genitore
  }
 
  onSearch() {
    console.log('Bottone cliccato', this.query);
    this.searchEvent.emit();
  }
}
