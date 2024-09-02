import { Routes } from '@angular/router';
import { AuthComponent } from './auth.component';

export const routes: Routes = [
  {
    path: '',
    component: AuthComponent,
    children: [
      {
        path: '',
        redirectTo: 'sign-in',
        pathMatch: 'full',
      },
      {
        path: 'sign-in',
        loadComponent: () =>
          import('../sign-in/sign-in.component').then((o) => o.SignInComponent),
      },
      {
        path: 'sign-up',
        loadComponent: () =>
          import('../sign-up/sign-up.component').then((o) => o.SignUpComponent),
      },
      {
        path: 'forgot-password',
        loadComponent: () =>
          import('../forgot-password/forgot-password.component').then(
            (o) => o.ForgotPasswordComponent,
          ),
      },
      {
        path: 'forgot-password/submit/:userId/:token',
        loadComponent: () =>
          import(
            '../forgot-password-submit/forgot-password-submit.component'
          ).then((o) => o.ForgotPasswordSubmitComponent),
      },
    ],
  },
];
