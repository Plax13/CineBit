import { Component, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { UiPage } from '../../shared/ui/ui-page/ui-page';
import { Location } from '@angular/common';


@Component({
  selector: 'detail',
  standalone: true,
  imports: [CommonModule, UiPage],
  templateUrl: './detail.html',
  styleUrl: './detail.css',
})
export class Detail implements OnInit {
  id = signal<string>('');
  film = signal<any | null>(null);
  loading = signal<boolean>(true);
  error = signal<string | null>(null);

  constructor(private route: ActivatedRoute, private http: HttpClient,private location: Location) {}
  // poster: usa TMDB se hai poster_path, altrimenti placeholder
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
}