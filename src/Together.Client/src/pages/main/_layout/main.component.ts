import { Component, OnDestroy, OnInit } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import {
  BreadcrumbComponent,
  NavbarComponent,
} from '@/pages/main/_layout/components';
import { BaseComponent } from '@/core/abstractions';
import { WebSocketService } from '@/shared/services';
import {
  AuthLoaderComponent,
  NotificationAlertComponent,
} from '@/shared/components/elements';

@Component({
  selector: 'together-main',
  standalone: true,
  imports: [
    RouterLink,
    NavbarComponent,
    RouterOutlet,
    BreadcrumbComponent,
    NotificationAlertComponent,
    AuthLoaderComponent,
  ],
  templateUrl: './main.component.html',
})
export class MainComponent extends BaseComponent implements OnInit, OnDestroy {
  constructor(private webSocketService: WebSocketService) {
    super();
  }

  ngOnInit() {
    this.webSocketService.client$.subscribe();
  }
}
