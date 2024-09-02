import { Routes } from '@angular/router';
import { UserManagementComponent } from '@/pages/management/user-management/_layout/user-management.component';

export const routes: Routes = [
  {
    path: '',
    component: UserManagementComponent,
    children: [
      {
        path: '',
        loadComponent: () =>
          import('../m-user-list/m-user-list.component').then(
            (o) => o.MUserListComponent,
          ),
      },
    ],
  },
];
