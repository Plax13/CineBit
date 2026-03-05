import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { UiPage } from "../../shared/ui/ui-page/ui-page";
import { FormsModule } from '@angular/forms';
import { Searchbar } from "../searchbar/searchbar";
import { CardFilm } from "../card-film/card-film";
import { AiService } from '../../services/ai-service';
import { IFilmCard } from '../../models/i-film-card';
import { Router } from '@angular/router';
import { FilmService } from '../../services/film-service';

@Component({
  selector: 'explore',
  imports: [UiPage, FormsModule, Searchbar, CardFilm],
  templateUrl: './explore.html',
  styleUrl: './explore.css',
})
export class Explore implements OnInit {

  query = '';
  movies: IFilmCard[] = [];
  loading = false;
  error: string | null = null;

  @ViewChild('row', { static: false }) rowRef!: ElementRef<HTMLElement>;

  constructor(
    private aiService: AiService,
    private filmService: FilmService,
    private router: Router
  ) {}

  ngAfterViewInit() {
  this.loadHome();
  }

  ngOnInit() {
    this.loadHome();
  }

  // caricamento iniziale veloce (API backend)
  loadHome() {
    this.loading = true;

    this.filmService.getHome(20).subscribe({
      next: (res) => {
        this.movies = res;
        this.loading = false;
        
      },
      error: (e) => {
        console.log(e);
        this.error = 'Errore caricamento iniziale';
        this.loading = false;
      }
    });
  }

  // ricerca tramite AI
  onSearch() {
    if (!this.query.trim()) return;

    this.loading = true;
    this.error = null;

    this.aiService.searchMovies(this.query).subscribe({
      next: (res: any) => {
        this.movies = res;
        this.loading = false;
      },
      error: () => {
        this.error = 'Errore durante la ricerca';
        this.loading = false;
      }
    });
  }

  // scroll orizzontale
  scrollRow(dir: 1 | -1) {
    const el = this.rowRef?.nativeElement;
    if (!el) return;

    const amount = Math.round(el.clientWidth * 0.8);
    el.scrollBy({ left: dir * amount, behavior: 'smooth' });
  }

  // apertura dettaglio film
  onCardClick(id: number) {
    this.router.navigate(['/movie', id]);
  }
}