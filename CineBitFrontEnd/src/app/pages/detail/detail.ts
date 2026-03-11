import { Component, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { UiPage } from '../../shared/ui/ui-page/ui-page';
import { Location } from '@angular/common';
import { SafeUrlPipe } from '../../pipes/safe-url-pipe';
import { PreferitiService } from '../../services/preferiti-service';

@Component({
  selector: 'detail',
  standalone: true,
  imports: [CommonModule, UiPage, SafeUrlPipe],
  templateUrl: './detail.html',
  styleUrl: './detail.css',
})
export class Detail implements OnInit {
  id = signal<string>('');
  film = signal<any | null>(null);
  loading = signal<boolean>(true);
  error = signal<string | null>(null);

constructor(
  private route: ActivatedRoute, 
  private http: HttpClient, 
  private location: Location,
  private prefService: PreferitiService
) { }

  posterUrl() {
    const f = this.film();
    if (!f) return 'assets/placeholder.png';
    if (f.poster_path) return 'https://image.tmdb.org/t/p/w780' + f.poster_path;
    if (f.posterUrl) return f.posterUrl;
    return 'assets/placeholder.png';
  }

  // se genere arriva come stringa "Azione, Thriller, ..."
  splitList(value: any): string[] {
    if (!value) return [];
    if (Array.isArray(value)) return value;
    return String(value).split(',').map(s => s.trim()).filter(Boolean);
  }
  goBack() {
    this.location.back();
  }
  ngOnInit() {
    this.route.paramMap.subscribe((pm) => {
      const id = pm.get('id');
      if (!id) {
        this.error.set('ID mancante nella rotta');
        this.loading.set(false);
        return;
      }

      this.id.set(id);
      this.loading.set(true);
      this.error.set(null);
      this.film.set(null);

      this.http.get<any>(`http://localhost:5201/api/Film/${id}/dettagli`).subscribe({
        next: (res) => {
          console.log('DETAIL RES:', res);
          this.film.set(res);
          this.loading.set(false);
        },
        error: (err) => {
          console.log('DETAIL ERROR:', err);
          this.error.set('Errore nel caricamento dettagli (API/CORS/404)');
          this.loading.set(false);
        },
      });
    });
  }

  actorImageUrl(actor: any) {
    if (!actor || !actor.immagine) return 'assets/placeholder.png';
    return 'https://image.tmdb.org/t/p/w185' + actor.immagine;
  }

  backdropUrl() {
  const f = this.film();
  if (!f || !f.backdrop_path) return this.posterUrl(); // fallback sul poster se manca
  return 'https://image.tmdb.org/t/p/w1280' + f.backdrop_path;
}

guardaOra() {
  const f = this.film();
  if (!f) return;

  const titolo = encodeURIComponent(f.titolo ?? f.title);
  const providers = f.providers as any[] ?? [];

  // Cerchiamo se tra i provider c'è un "Big"
  const hasNetflix = providers.some(p => p.nome.toLowerCase().includes('netflix'));
  const hasDisney = providers.some(p => p.nome.toLowerCase().includes('disney'));
  const hasPrime = providers.some(p => p.nome.toLowerCase().includes('amazon'));

  if (hasNetflix) {
    window.open(`https://www.netflix.com/search?q=${titolo}`, '_blank');
  } else if (hasDisney) {
    window.open(`https://www.disneyplus.com/search?q=${titolo}`, '_blank');
  } else if (hasPrime) {
    window.open(`https://www.primevideo.com/search?phrase=${titolo}`, '_blank');
  } else {
    // Se non è sui "Big", usiamo la ricerca Google mirata che avevi prima
    const query = encodeURIComponent(`dove vedere ${f.titolo ?? f.title} in streaming ita`);
    window.open(`https://www.google.com/search?q=${query}`, '_blank');
  }
}

togglePreferito() {
  const f = this.film();
  if (!f) return;

  const dto = {
    tmdbId: parseInt(this.id()),
    idUtente: 1,                 
    titoloCache: f.titolo ?? f.title
  };

  this.prefService.aggiungi(dto).subscribe({
    next: (res) => alert("Film aggiunto ai preferiti! ❤️"),
    error: (err) => {
      if (err.status === 409) alert("Il film è già nei tuoi preferiti!");
      else alert("Errore nel salvataggio");
    }
  });
}

}