import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PreferitiService } from '../../services/preferiti-service';
import { UiPage } from '../../shared/ui/ui-page/ui-page';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-profile',
  imports: [CommonModule, UiPage, RouterLink],
  templateUrl: './profile.html',
  styleUrl: './profile.css',
})

export class Profile implements OnInit {
  listaPreferiti = signal<any[]>([]);
  loading = signal<boolean>(true);
  consigliati = signal<any[]>([]);

  constructor(private prefService: PreferitiService,
    public auth: AuthService
  ) { }

  ngOnInit() {
    const user = this.auth.currentUser();
    const userId = user?.id || user?.id_utente;

    if (userId) {
      this.prefService.getPreferiti(userId).subscribe({
        next: (data) => {
          this.listaPreferiti.set(data);
          this.loading.set(false);
          this.caricaConsigliati(userId);
        },
        error: (err) => {
          console.error("Errore nel recupero preferiti:", err);
          this.loading.set(false);
        }
      });
    } else {
      this.loading.set(false);
    }
  }

  generiStat = computed(() => {
    const preferiti = this.listaPreferiti();
    const conteggio: { [key: string]: number } = {};

    const coloriGeneri: { [key: string]: { start: string, end: string } } = {
      'Azione': { start: '#f97316', end: '#dc2626' },
      'Avventura': { start: '#4ade80', end: '#059669' },
      'Animazione': { start: '#f472b6', end: '#9333ea' },
      'Commedia': { start: '#facc15', end: '#f97316' },
      'Dramma': { start: '#b91c1c', end: '#7f1d1d' },
      'Fantascienza': { start: '#2563eb', end: '#1e3a8a' },
      'Horror': { start: '#4b5563', end: '#000000' },
      'default': { start: '#55BBEF', end: '#2B5E77' }
    };

    preferiti.forEach((film) => {
      const generiStr = film.genereCache;
      if (generiStr) {
        const generi = generiStr.split(',').map((g: string) => g.trim());
        generi.forEach((g: string) => {
          if (g) conteggio[g] = (conteggio[g] || 0) + 1;
        });
      }
    });

    return Object.entries(conteggio)
      .map(([nome, count]) => ({
        nome,
        count,
        coloreStart: coloriGeneri[nome]?.start || coloriGeneri['default'].start,
        coloreEnd: coloriGeneri[nome]?.end || coloriGeneri['default'].end
      }))
      .sort((a, b) => b.count - a.count)
      .slice(0, 5);
  });

  posterUrl(path: string | null) {
    return path ? 'https://image.tmdb.org/t/p/w500' + path : 'assets/placeholder.png';
  }

  caricaConsigliati(userId: number) {
  this.prefService.getConsigliati(userId).subscribe({
    next: (data) => this.consigliati.set(data),
    error: (err) => console.error("Errore consigliati:", err)
  });
}

nomiGeneriTop = computed(() => {
    const stats = this.generiStat();
    if (stats.length === 0) return 'i tuoi gusti';

    const nomi = stats.slice(0, 3).map(s => s.nome);

    if (nomi.length === 1) return nomi[0];
    
    const ultimoNome = nomi.pop();
    return `${nomi.join(', ')} e ${ultimoNome}`;
  });

}
