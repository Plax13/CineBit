import { Routes } from '@angular/router';
import { Landing } from '../pages/landing/landing'
import { Explore } from '../pages/explore/explore';
import { Detail } from '../pages/detail/detail';
import { Login } from '../pages/login/login';
import { Signup } from '../pages/signup/signup';
import { Profile } from '../pages/profile/profile';

export const routes: Routes = [
  { path: '', component: Landing },
  { path: 'explore', component: Explore },
  { path: 'movie/:id', component: Detail },
  { path: 'login', component: Login },
  { path: 'signup', component: Signup },
  { path: 'profile', component: Profile},

  { path: '**', redirectTo: '' },

];
