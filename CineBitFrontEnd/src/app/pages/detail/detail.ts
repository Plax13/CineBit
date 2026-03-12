import { Component, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { UiPage } from '../../shared/ui/ui-page/ui-page';
import { Location } from '@angular/common';
import { SafeUrlPipe } from '../../pipes/safe-url-pipe';
import { PreferitiService } from '../../services/preferiti-service';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { AiService } from '../../services/ai-service';
import { FilmService } from '../../services/film-service';
import { IFilmCard } from '../../models/i-film-card';
import { CardFilm } from '../card-film/card-film';

@Component({
  selector: 'detail',
  standalone: true,
  imports: [CommonModule, UiPage, SafeUrlPipe, CardFilm],
  templateUrl: './detail.html',
  styleUrl: './detail.css',
})

export class Detail implements OnInit {
  id = signal<string>('');
  film = signal<any | null>(null);
  loading = signal<boolean>(true);
  error = signal<string | null>(null);
  isPreferito = signal<boolean>(false);
  listaPreferiti = signal<any[]>([]);
  simili = signal<IFilmCard[]>([]);

  constructor(
    private route: ActivatedRoute,
    private http: HttpClient,
    private location: Location,
    private prefService: PreferitiService,
    public auth: AuthService,
    public router: Router,
    private aiService: AiService,
    private filmService: FilmService
  ) { }

  ngOnInit() {
    this.route.paramMap.subscribe((pm) => {
      const id = pm.get('id');
      if (!id) {
        this.error.set('ID mancante nella rotta');
        this.loading.set(false);
        return;
      }
      window.scrollTo({ top: 0, behavior: 'smooth' });

      this.id.set(id);
      this.loading.set(true);
      this.error.set(null);
      this.simili.set([]);

      this.http.get<any>(`http://localhost:5201/api/Film/${id}/dettagli`).subscribe({
        next: (res) => {
          this.film.set(res);
          this.loading.set(false);
          this.checkIfPreferito(id);
        },
        error: (err) => {
          this.error.set('Errore nel caricamento dettagli');
          this.loading.set(false);
        },
      });
      this.caricaSimili(id);
    });
  }

  checkIfPreferito(idFilm: string) {
    const userId = this.auth.currentUser()?.id;
    if (!userId) return;

    this.prefService.getPreferiti(userId).subscribe({
      next: (preferiti) => {
        const esiste = preferiti.some((p) => p.tmdbId == idFilm);
        this.isPreferito.set(esiste);
      },
      error: (err) => console.error("Errore nel check preferiti", err)
    });
  }

  togglePreferito() {
    const f = this.film();
    const user = this.auth.currentUser();
    const tmdbId = parseInt(this.id());

    if (!user || !user.id) {
      alert("Devi essere loggato!");
      return;
    }

    if (this.isPreferito()) {
      this.prefService.rimuovi(user.id, tmdbId).subscribe({
        next: () => {
          this.isPreferito.set(false);
        },
        error: (err) => {
          console.error(err);
          alert("Errore nella rimozione");
        }
      });
    } else {
      const dto = {
        tmdbId: tmdbId,
        idUtente: user.id,
        titoloCache: f.titolo ?? f.title,
        posterPathCache: f.poster_path,
        genereCache: f.genere ?? (Array.isArray(f.genres) ? f.genres.map((g: any) => g.name).join(', ') : '')
      };

      this.prefService.aggiungi(dto).subscribe({
        next: () => {
          this.isPreferito.set(true);

        },
        error: (err) => {
          if (err.status === 409) {
            // Se per qualche motivo il DB era già allineato, correggiamo il signal
            this.isPreferito.set(true);
          } else {
            alert("Errore nel salvataggio");
          }
        }
      });
    }
  }

  posterUrl() {
    const f = this.film();
    if (!f) return 'assets/placeholder.png';
    return f.poster_path ? 'https://image.tmdb.org/t/p/w780' + f.poster_path : 'assets/placeholder.png';
  }

  backdropUrl() {
    const f = this.film();
    return f?.backdrop_path ? 'https://image.tmdb.org/t/p/w1280' + f.backdrop_path : this.posterUrl();
  }

  actorImageUrl(actor: any) {
    return actor?.immagine ? 'https://image.tmdb.org/t/p/w185' + actor.immagine : 'assets/placeholder.png';
  }

  splitList(value: any): string[] {
    if (!value) return [];
    if (Array.isArray(value)) return value;
    return String(value).split(',').map((s) => s.trim()).filter(Boolean);
  }

  goBack() {
    this.location.back();
  }

  guardaOra() {
    const f = this.film();
    if (!f || !f.providers) return;

    const titolo = f.titolo ?? f.title;

    if (f.providers.length > 0) {
      const primoProvider = f.providers[0].nome.toLowerCase();

      if (primoProvider.includes('netflix')) {
        window.open(`https://www.netflix.com/search?q=${encodeURIComponent(titolo)}`, '_blank');
      }
      else if (primoProvider.includes('amazon') || primoProvider.includes('prime')) {
        window.open(`https://www.primevideo.com/search/ref=atv_nb_sr?phrase=${encodeURIComponent(titolo)}`, '_blank');
      }
      else if (primoProvider.includes('disney')) {
        window.open(`https://www.disneyplus.com/search`, '_blank');
      }
      else {
        window.open(`https://www.google.com/search?q=guarda+${encodeURIComponent(titolo)}+su+${encodeURIComponent(f.providers[0].nome)}`, '_blank');
      }
    } else {
      window.open(`https://www.google.com/search?q=dove+vedere+${encodeURIComponent(titolo)}+streaming+legale`, '_blank');
    }
  }

  resetToHome() {
    this.aiService.lastSearchResults = [];
    this.aiService.lastSearchQuery = '';

    this.router.navigate(['/explore']);
  }

  caricaSimili(id: string) {
  this.filmService.getSimili(id).subscribe({
    next: (res) => {
      this.simili.set(res);
    },
    error: (err) => console.error("Errore film simili", err)
  });
}
}