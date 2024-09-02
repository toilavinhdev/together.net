import { Routes } from '@angular/router';
import { authGuard, signedInGuard } from '@/shared/guards';

export const routes: Routes = [
  {
    path: '',
    loadChildren: () =>
      import('@/pages/main/_layout/main.routes').then((o) => o.routes),
    canActivate: [authGuard],
  },
  {
    path: 'auth',
    loadChildren: () =>
      import('@/pages/auth/_layout/auth.routes').then((o) => o.routes),
    canActivate: [signedInGuard],
  },
  {
    path: 'management',
    loadChildren: () =>
      import('@/pages/management/_layout/management.routes').then(
        (o) => o.routes,
      ),
    canActivate: [authGuard],
  },
];
