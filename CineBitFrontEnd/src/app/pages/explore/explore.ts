import { Component, OnInit, ElementRef, ViewChild, ChangeDetectorRef } from '@angular/core';
import { UiPage } from "../../shared/ui/ui-page/ui-page";
import { FormsModule } from '@angular/forms';
import { Searchbar } from "../searchbar/searchbar";
import { CardFilm } from "../card-film/card-film";
import { AiService } from '../../services/ai-service';
import { IFilmCard } from '../../models/i-film-card';
import { Router } from '@angular/router';
import { FilmService } from '../../services/film-service';
import { UiHeroBanner } from '../../shared/ui/ui-hero-banner/ui-hero-banner';
import { AiLoader } from '../ai-loader/ai-loader';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'explore',
  imports: [UiPage, FormsModule, Searchbar, CardFilm, UiHeroBanner, AiLoader],
  templateUrl: './explore.html',
  styleUrl: './explore.css',
})
export class Explore implements OnInit {

  query = '';
  movies: IFilmCard[] = [];
  loading = false;
  error: string | null = null;
  isAiThinking = false;
  loadingText = 'Analizzando la tua richiesta...';
  private loadingInterval: any;

  @ViewChild('row', { static: false }) rowRef!: ElementRef<HTMLElement>;
  //Questo "aggancia" l'elemento HTML con `#row` (il `<section>` dello scroll) alla variabile `rowRef`. 
  // Con `rowRef.nativeElement` puoi accedere direttamente al DOM. `static: false` significa che l'elemento 
  // viene cercato dopo che il template è renderizzato.
  constructor(
    private aiService: AiService,
    private filmService: FilmService,
    private router: Router,
    private cdr: ChangeDetectorRef,
    private authService: AuthService
  ) { }

    get user() {
      return this.authService.currentUser();
    }

  ngOnInit() {
    if (this.aiService.lastSearchResults.length > 0) {
    this.movies = this.aiService.lastSearchResults;
    this.query = this.aiService.lastSearchQuery;
  } else {
    this.loadHome();
  }
  }

  // caricamento iniziale veloce (API backend)
  loadHome() {
    this.loading = true;
    this.filmService.getHome(20).subscribe({
      next: (res) => {
        this.movies = res;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (e) => {
        this.error = 'Errore caricamento iniziale';
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  // ricerca tramite AI
 onSearch() {
    if (!this.query.trim()) return;

    this.isAiThinking = true; 
    this.error = null;

    this.startLoadingTexts();

    this.aiService.searchMovies(this.query).subscribe({
      next: (res: any) => {
        this.movies = res;
        this.aiService.lastSearchResults = res;
        
        this.stopLoading();
      },
      error: (e) => {
        console.log('Erorre ricerca ai:', e);
        this.error = '';
        this.stopLoading();
      }
    });
  }

private startLoadingTexts() {
  const texts = [
    "Preparando i Popcorn",
    "Caricando la bobina",
    "Proiezione in corso",
    "Ciak si gira"
  ];
  let i = 0;
  this.loadingText = texts[0];
  this.loadingInterval = setInterval(() => {
    i = (i + 1) % texts.length;
    this.loadingText = texts[i];
    this.cdr.detectChanges();
  }, 4000);
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
  goToProfile(): void {
    this.router.navigate(['/profile']);
  }

  private stopLoading() {
    this.isAiThinking = false;
    this.loading = false;
    if (this.loadingInterval) clearInterval(this.loadingInterval);
    this.cdr.detectChanges();
  }
}