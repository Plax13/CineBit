import { Routes } from '@angular/router';
import { Landing } from '../pages/landing/landing'
import { Explore } from '../pages/explore/explore';
import { Detail } from '../pages/detail/detail';

export const routes: Routes = [
  { path: '', component: Landing },
  { path: 'explore', component: Explore },
  { path: 'movie/:id', component: Detail },
  { path: '**', redirectTo: '' },
];