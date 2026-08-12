import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./features/home/home/home.component').then((m) => m.HomeComponent),
  },
  {
    path: 'bands/:id',
    loadComponent: () =>
      import('./features/band-detail/band-detail-page/band-detail-page.component').then(
        (m) => m.BandDetailPageComponent
      ),
  },
  {
    path: 'releases/:id',
    loadComponent: () =>
      import('./features/release-detail/release-detail-page/release-detail-page.component').then(
        (m) => m.ReleaseDetailPageComponent
      ),
  },
  { path: '**', redirectTo: '' },
];
