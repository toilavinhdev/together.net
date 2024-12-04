import {
  HttpErrorResponse,
  HttpInterceptorFn,
  HttpRequest,
} from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService, CommonService } from '@/shared/services';
import {
  catchError,
  filter,
  finalize,
  of,
  switchMap,
  take,
  throwError,
} from 'rxjs';
import { getErrorMessage } from '@/shared/utilities';
import { environment } from '@/environments/environment';

const reqWithAT = (
  req: HttpRequest<unknown>,
  accessToken: string | null,
): HttpRequest<unknown> => {
  if (!accessToken) return req;
  if (req.url.startsWith(environment.host)) {
    return req.clone({
      setHeaders: {
        Authorization: `Bearer ${accessToken}`,
      },
      withCredentials: true,
    });
  }
  return req;
};

export const tokenInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const commonService = inject(CommonService);

  return next(reqWithAT(req, authService.getAT())).pipe(
    catchError((err) => {
      if (err.status === 401 && authService.getAT()) {
        if (!authService.refreshingToken$.value) {
          authService.refreshingToken$.next(true);
          authService.accessToken$.next(undefined);
          return authService.refreshToken().pipe(
            switchMap(({ accessToken, refreshToken }) => {
              authService.accessToken$.next(accessToken);
              authService.setToken(accessToken, refreshToken);
              return next(reqWithAT(req, accessToken));
            }),
            catchError((refreshTokenErr) => {
              if (
                refreshTokenErr instanceof HttpErrorResponse &&
                getErrorMessage(refreshTokenErr) === 'RefreshTokenFailed'
              ) {
                commonService.navigateToLogin();
                localStorage.clear();
                commonService.toast$.next({
                  type: 'info',
                  message: 'Phiên đăng nhập hết hạn',
                });
                return of();
              } else {
                return throwError(() => err);
              }
            }),
            finalize(() => {
              authService.refreshingToken$.next(false);
            }),
          );
        } else {
          return authService.accessToken$.pipe(
            filter((accessToken) => !!accessToken),
            take(1),
            switchMap((accessToken) => {
              return next(reqWithAT(req, accessToken!));
            }),
          );
        }
      }
      return throwError(() => err);
    }),
  );
};
