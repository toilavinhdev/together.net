import { environment } from '@/environments/environment';
import { Component, NgZone, OnInit } from '@angular/core';
import { BaseComponent } from '@/core/abstractions';
import { AuthService } from '@/shared/services';
import { NgClass } from '@angular/common';
import { takeUntil } from 'rxjs';
import { getErrorMessage } from '@/shared/utilities';

@Component({
  selector: 'together-external-auth',
  standalone: true,
  imports: [NgClass],
  templateUrl: './external-auth.component.html',
})
export class ExternalAuthComponent extends BaseComponent implements OnInit {
  constructor(
    private authService: AuthService,
    private ngZone: NgZone,
  ) {
    super();
  }

  ngOnInit() {
    this.buildGoogleAuth();
  }

  private buildGoogleAuth() {
    // @ts-ignore
    google.accounts.id.initialize({
      client_id: environment.googleClientId,
      callback: this.googleAuthCallBack.bind(this),
      auto_select: false,
      cancel_on_tap_outside: true,
    });
    // @ts-ignore
    google.accounts.id.renderButton(
      // @ts-ignore
      document.getElementById('google-btn'),
      {
        type: 'icon',
        theme: 'outline',
        size: 'large',
        height: 50,
        width: 490,
      },
    );
    // @ts-ignore
    google.accounts.id.prompt(() => {});
  }

  private async googleAuthCallBack(response: any) {
    const { credential } = response;
    this.commonService.spinning$.next(true);
    this.authService
      .externalAuth({ credential })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data) => {
          this.authService.setToken(data.accessToken, data.refreshToken);
          this.commonService.spinning$.next(false);
          this.ngZone.run(() => {
            this.commonService.toast$.next({
              type: 'success',
              message: 'Đăng nhập thành công',
            });
            this.commonService.navigateToMain();
          });
        },
        error: (err) => {
          this.commonService.spinning$.next(false);
          this.ngZone.run(() => {
            this.commonService.toast$.next({
              type: 'error',
              message: getErrorMessage(err),
            });
          });
        },
      });
  }
}
