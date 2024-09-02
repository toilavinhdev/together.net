import { Component, OnInit } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { MenuComponent } from '@/pages/management/_layout/_components/menu/menu.component';
import { HeaderComponent } from '@/pages/management/_layout/_components/header/header.component';
import { UserService } from '@/shared/services';
import { AsyncPipe, NgIf } from '@angular/common';
import { policies } from '@/shared/constants';
import { BaseComponent } from '@/core/abstractions';
import { Button } from 'primeng/button';
import { AuthLoaderComponent } from '@/shared/components/elements';

@Component({
  selector: 'together-management',
  standalone: true,
  imports: [
    RouterOutlet,
    MenuComponent,
    HeaderComponent,
    NgIf,
    AsyncPipe,
    Button,
    RouterLink,
    AuthLoaderComponent,
  ],
  templateUrl: './management.component.html',
})
export class ManagementComponent extends BaseComponent implements OnInit {
  constructor(protected userService: UserService) {
    super();
  }

  protected readonly policies = policies;

  status: 'idle' | 'loading' | 'finished' = 'finished';

  ngOnInit() {}
}
