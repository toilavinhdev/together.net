import { Routes } from '@angular/router';
import { SettingComponent } from '@/pages/main/setting/_layout/setting.component';

export const routes: Routes = [
  {
    path: '',
    component: SettingComponent,
    children: [
      {
        path: '',
        redirectTo: 'update-password',
        pathMatch: 'full',
      },
      {
        path: 'update-password',
        loadComponent: () =>
          import('../update-password/update-password.component').then(
            (o) => o.UpdatePasswordComponent,
          ),
      },
    ],
  },
];
