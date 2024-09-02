import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import {
  AvatarComponent,
  NotificationDropdownComponent,
  SvgIconComponent,
  UserDropdownComponent,
} from '@/shared/components/elements';
import { BaseComponent } from '@/core/abstractions';
import { NotificationService, UserService } from '@/shared/services';
import { AsyncPipe } from '@angular/common';
import { BadgeModule } from 'primeng/badge';

@Component({
  selector: 'together-navbar',
  standalone: true,
  imports: [
    RouterLink,
    SvgIconComponent,
    AvatarComponent,
    AsyncPipe,
    UserDropdownComponent,
    NotificationDropdownComponent,
    BadgeModule,
  ],
  templateUrl: './navbar.component.html',
})
export class NavbarComponent extends BaseComponent {
  constructor(
    protected userService: UserService,
    protected notificationService: NotificationService,
  ) {
    super();
  }
}
