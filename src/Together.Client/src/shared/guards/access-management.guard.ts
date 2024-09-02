import { CanActivateFn } from '@angular/router';
import { inject } from '@angular/core';
import { CommonService, UserService } from '@/shared/services';
import { policies } from '@/shared/constants';
import { tap } from 'rxjs';

export const accessManagementGuard: CanActivateFn = (route, state) => {
  const commonService = inject(CommonService);
  const userService = inject(UserService);

  return userService.hasPermission$(policies.Management.Access).pipe(
    tap((access) => {
      if (!access) {
        commonService.navigateToMain();
        commonService.toast$.next({
          type: 'info',
          message: 'Bạn không có quyền truy cập trang quản lý',
        });
      }
    }),
  );
};
