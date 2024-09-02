import { Routes } from '@angular/router';
import { MainComponent } from './main.component';

export const routes: Routes = [
  {
    path: '',
    component: MainComponent,
    children: [
      {
        path: '',
        loadChildren: () =>
          import('../forum/_layout/forum.routes').then((o) => o.routes),
      },
      {
        path: 'conversations',
        loadChildren: () =>
          import('../conversation/_layout/conversation.routes').then(
            (o) => o.routes,
          ),
      },
      {
        path: 'notifications',
        loadChildren: () =>
          import('../notification/_layout/notification.routes').then(
            (o) => o.routes,
          ),
      },
      {
        path: 'settings',
        loadChildren: () =>
          import('../setting/_layout/setting.routes').then((o) => o.routes),
      },
      {
        path: 'user/:userId',
        loadChildren: () =>
          import('../user/_layout/user.routes').then((o) => o.routes),
      },
    ],
  },
];
