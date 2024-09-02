import { Component } from '@angular/core';
import { BaseComponent } from '@/core/abstractions';
import {
  AvatarComponent,
  NotificationDropdownComponent,
  UserDropdownComponent,
} from '@/shared/components/elements';
import { AsyncPipe } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { UserService } from '@/shared/services';
import { Button } from 'primeng/button';

@Component({
  selector: 'together-header',
  standalone: true,
  imports: [
    AvatarComponent,
    UserDropdownComponent,
    AsyncPipe,
    TranslateModule,
    Button,
    NotificationDropdownComponent,
  ],
  templateUrl: './header.component.html',
})
export class HeaderComponent extends BaseComponent {
  constructor(protected userService: UserService) {
    super();
  }
}
