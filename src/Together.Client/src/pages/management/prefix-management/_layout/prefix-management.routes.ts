import { Routes } from '@angular/router';
import { PrefixManagementComponent } from '@/pages/management/prefix-management/_layout/prefix-management.component';

export const routes: Routes = [
  {
    path: '',
    component: PrefixManagementComponent,
    children: [
      {
        path: '',
        loadComponent: () =>
          import('../m-prefix-list/m-prefix-list.component').then(
            (o) => o.MPrefixListComponent,
          ),
      },
      {
        path: 'create',
        loadComponent: () =>
          import('../m-prefix-detail/m-prefix-detail.component').then(
            (o) => o.MPrefixDetailComponent,
          ),
      },
      {
        path: ':prefixId/update',
        loadComponent: () =>
          import('../m-prefix-detail/m-prefix-detail.component').then(
            (o) => o.MPrefixDetailComponent,
          ),
      },
    ],
  },
];
