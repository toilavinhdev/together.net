import { Component, OnInit } from '@angular/core';
import { take, takeUntil, tap } from 'rxjs';
import { getErrorMessage } from '@/shared/utilities';
import { BaseComponent } from '@/core/abstractions';
import { UserService } from '@/shared/services';

@Component({
  selector: 'together-auth-loader',
  standalone: true,
  imports: [],
  template: '',
})
export class AuthLoaderComponent extends BaseComponent implements OnInit {
  constructor(private userService: UserService) {
    super();
  }

  ngOnInit() {
    this.getMe();
    this.getPermissions();
  }

  private getMe() {
    let hasData = false;
    this.userService.me$.pipe(take(1)).subscribe((user) => {
      hasData = !!user;
    });
    if (hasData) return;
    this.userService
      .getMe()
      .pipe(
        takeUntil(this.destroy$),
        tap(() => {
          this.commonService.spinning$.next(true);
        }),
      )
      .subscribe({
        next: (data) => {
          this.userService.me$.next(data);
        },
        error: (err) => {
          this.commonService.toast$.next({
            type: 'error',
            message: getErrorMessage(err),
          });
        },
        complete: () => {
          this.commonService.spinning$.next(false);
        },
      });
  }

  private getPermissions() {
    let hasData = false;
    this.userService.permissions$.pipe(take(1)).subscribe((permissions) => {
      hasData = permissions.length > 0;
    });
    if (hasData) return;
    this.userService
      .getPermissions()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data) => {
          this.userService.permissions$.next(data);
        },
        error: (err) => {
          this.commonService.toast$.next({
            type: 'error',
            message: getErrorMessage(err),
          });
        },
      });
  }
}
