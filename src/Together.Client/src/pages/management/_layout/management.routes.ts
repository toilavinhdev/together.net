import { Routes } from '@angular/router';
import { ManagementComponent } from './management.component';

export const routes: Routes = [
  {
    path: '',
    component: ManagementComponent,
    children: [
      {
        path: '',
        loadComponent: () =>
          import('../dashboard/dashboard.component').then(
            (o) => o.DashboardComponent,
          ),
      },
      {
        path: 'forum',
        loadChildren: () =>
          import('../forum-management/_layout/forum-management.routes').then(
            (o) => o.routes,
          ),
      },
      {
        path: 'prefix',
        loadChildren: () =>
          import('../prefix-management/_layout/prefix-management.routes').then(
            (o) => o.routes,
          ),
      },
      {
        path: 'post',
        loadChildren: () =>
          import('../post-management/_layout/post-management.routes').then(
            (o) => o.routes,
          ),
      },
      {
        path: 'user',
        loadChildren: () =>
          import('../user-management/_layout/user-management.routes').then(
            (o) => o.routes,
          ),
      },
      {
        path: 'role',
        loadChildren: () =>
          import('../role-management/_layout/role-management.routes').then(
            (o) => o.routes,
          ),
      },
    ],
  },
];
