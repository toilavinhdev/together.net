import { Component, OnInit, ViewChild } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { AvatarComponent } from '@/shared/components/elements';
import { Menu, MenuModule } from 'primeng/menu';
import { MenuItem, PrimeTemplate } from 'primeng/api';
import { Ripple } from 'primeng/ripple';
import { takeUntil } from 'rxjs';
import { BaseComponent } from '@/core/abstractions';
import { AuthService, UserService, WebSocketService } from '@/shared/services';
import { getErrorMessage } from '@/shared/utilities';
import { TranslateModule } from '@ngx-translate/core';
import { LanguageSwitchModalComponent } from '@/shared/components/elements/user-dropdown/_components/language-switch-modal/language-switch-modal.component';
import { policies } from '@/shared/constants';
import { Router } from '@angular/router';

@Component({
  selector: 'together-user-dropdown',
  standalone: true,
  imports: [
    AsyncPipe,
    AvatarComponent,
    MenuModule,
    PrimeTemplate,
    Ripple,
    TranslateModule,
    LanguageSwitchModalComponent,
  ],
  templateUrl: './user-dropdown.component.html',
})
export class UserDropdownComponent extends BaseComponent implements OnInit {
  items: MenuItem[] = [
    {
      separator: true,
    },
    {
      label: 'Profile',
      icon: 'pi pi-user',
      command: () => {
        this.userService.me$.pipe(takeUntil(this.destroy$)).subscribe((me) => {
          if (!me) return;
          this.commonService.navigateToProfile(me.id);
        });
      },
    },
    {
      label: 'Settings',
      icon: 'pi pi-cog',
      routerLink: '/settings',
    },
    {
      label: 'Switch language',
      icon: 'pi pi-language',
      command: () => {
        this.languageSwitchModal.show();
      },
    },
    {
      id: policies.Management.Access,
      label: 'Managements',
      icon: 'pi pi-th-large',
      command: () => {
        this.commonService.navigateToAdminPage();
      },
      visible: false,
    },
    {
      label: 'Logout',
      icon: 'pi pi-sign-out',
      command: () => {
        this.onLogout();
      },
    },
  ];

  @ViewChild('menu', { static: true })
  menuComponent!: Menu;

  @ViewChild('languageSwitchModal', { static: true })
  languageSwitchModal!: LanguageSwitchModalComponent;

  constructor(
    protected userService: UserService,
    private authService: AuthService,
    private router: Router,
    private webSocketService: WebSocketService,
  ) {
    super();
  }

  ngOnInit() {
    this.visibleAccessManagement();
  }

  private visibleAccessManagement() {
    this.userService
      .hasPermission$(policies.Management.Access)
      .pipe(takeUntil(this.destroy$))
      .subscribe((value) => {
        this.items = this.items.map((item) =>
          item.id === policies.Management.Access
            ? {
                ...item,
                visible:
                  value && this.router.url.split('/')[1] !== 'management',
              }
            : item,
        );
      });
  }

  private onLogout() {
    this.commonService.spinning$.next(true);
    this.authService
      .logout()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.commonService.spinning$.next(false);
          this.webSocketService.disconnect();
          this.authService.removeToken();
          localStorage.clear();
          this.commonService.navigateToLogin();
        },
        error: (err) => {
          this.commonService.spinning$.next(false);
          this.commonService.toast$.next({
            type: 'error',
            message: getErrorMessage(err),
          });
        },
      });
  }
}
