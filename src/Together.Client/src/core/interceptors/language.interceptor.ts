import { HttpInterceptorFn } from '@angular/common/http';
import { localStorageKeys } from '@/shared/constants';

export const languageInterceptor: HttpInterceptorFn = (req, next) => {
  const lang = localStorage.getItem(localStorageKeys.LANG) || 'vi-VN';

  return next(
    req.clone({
      setHeaders: {
        'Accept-Language': lang,
      },
    }),
  );
};
