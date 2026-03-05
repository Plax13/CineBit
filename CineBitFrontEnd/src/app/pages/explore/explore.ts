import { Component } from '@angular/core';
import { UiPage } from "../../shared/ui/ui-page/ui-page";
import { FormsModule } from '@angular/forms';
import { Searchbar } from "../searchbar/searchbar";
import { CardFilm } from "../card-film/card-film";
import { AiService } from '../../services/ai-service';
import { IFilmCard } from '../../models/i-film-card';
import { ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'explore',
  imports: [UiPage, FormsModule, Searchbar, CardFilm],
  templateUrl: './explore.html',
  styleUrl: './explore.css',
})
export class Explore {
  query = '';
  movies: IFilmCard[] = [ { id: 1, title: 'Inception', release_date: '2010', poster_path: 'https://image.tmdb.org/t/p/w500/9gk7adHYeDvHkCSEqAvQNLV5Uge.jpg' },
  { id: 2, title: 'Interstellar', release_date: '2014', poster_path: 'https://image.tmdb.org/t/p/w500/gEU2QniE6E77NI6lCU6MxlNBvIx.jpg' },
  { id: 3, title: 'The Dark Knight', release_date: '2008', poster_path: 'https://image.tmdb.org/t/p/w500/qJ2tW6WMUDux911r6m7haRef0WH.jpg' }];
  loading = false;
  error: string | null = null;

  constructor(private aiService: AiService, private cd: ChangeDetectorRef) {}

  onSearch() {
    if (!this.query.trim()) return;

    this.loading = true;
    this.error = null;
    this.movies = [];

    this.aiService.searchMovies(this.query).subscribe({
      next: (res: any) => {
        this.movies = res;
        this.loading = false;
        this.cd.detectChanges();
      },
      error: (err) => {
        this.error = 'Errore durante la ricerca';
        this.loading = false;
      }
    });
  }
}