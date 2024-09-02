import { Routes } from '@angular/router';
import { RoleManagementComponent } from '@/pages/management/role-management/_layout/role-management.component';

export const routes: Routes = [
  {
    path: '',
    component: RoleManagementComponent,
    children: [
      {
        path: '',
        loadComponent: () =>
          import('../m-role-list/m-role-list.component').then(
            (o) => o.MRoleListComponent,
          ),
      },
      {
        path: 'create',
        loadComponent: () =>
          import('../m-role-detail/m-role-detail.component').then(
            (o) => o.MRoleDetailComponent,
          ),
      },
      {
        path: ':roleId/update',
        loadComponent: () =>
          import('../m-role-detail/m-role-detail.component').then(
            (o) => o.MRoleDetailComponent,
          ),
      },
      {
        path: 'assign',
        loadComponent: () =>
          import('../m-role-assign/m-role-assign.component').then(
            (o) => o.MRoleAssignComponent,
          ),
      },
    ],
  },
];
