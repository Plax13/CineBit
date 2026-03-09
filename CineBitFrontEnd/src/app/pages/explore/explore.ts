import { Component, OnInit, ElementRef, ViewChild, ChangeDetectorRef } from '@angular/core';
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
//Questo "aggancia" l'elemento HTML con `#row` (il `<section>` dello scroll) alla variabile `rowRef`. 
// Con `rowRef.nativeElement` puoi accedere direttamente al DOM. `static: false` significa che l'elemento 
// viene cercato **dopo** che il template è renderizzato.
  constructor(
    private aiService: AiService,
    private filmService: FilmService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  

  ngOnInit() {
    this.loadHome();
    
  }

  // caricamento iniziale veloce (API backend)
  loadHome() {
    this.loading = true;
    console.log('1. loadHome chiamato');

    this.filmService.getHome(20).subscribe({
      next: (res) => {
        console.log('2. dati ricevuti:', res);
        this.movies = res;
        console.log('3. movies aggiornato:', this.movies);
        this.loading = false;
        this.cdr.detectChanges();
        
      },
      error: (e) => {
        console.log(e);
        this.error = 'Errore caricamento iniziale';
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  // ricerca tramite AI
  onSearch() {
    if (!this.query.trim()) return;

    this.loading = true;
    this.error = null;
    console.log('1. onSearch chiamato, query:', this.query);

    this.aiService.searchMovies(this.query).subscribe({
      next: (res: any) => {
        console.log('2. risultati ricevuti:', JSON.stringify(res).slice(0, 300));
        this.movies = res;
        console.log('3. movies aggiornato:', this.movies);
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (e) => {
        console.log('ERRORE:', e);
        this.error = 'Errore caricamento iniziale';
        this.loading = false;
        this.cdr.detectChanges();
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
  goToProfile(): void {
  this.router.navigate(['/profilo']);
}
}