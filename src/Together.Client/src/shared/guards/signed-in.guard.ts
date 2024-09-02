import { CanActivateFn } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService, CommonService } from '@/shared/services';

export const signedInGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const commonService = inject(CommonService);

  if (authService.getAT() && authService.getClaims()) {
    commonService.navigateToMain();
    return false;
  }

  return true;
};
