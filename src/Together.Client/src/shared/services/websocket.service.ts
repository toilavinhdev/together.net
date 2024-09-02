import { Injectable } from '@angular/core';
import { webSocket } from 'rxjs/webSocket';
import { interval } from 'rxjs';
import { IWebSocketMessage } from '@/core/models/websocket.models';
import { websocketServerTarget } from '@/shared/constants';
import { AuthService } from '@/shared/services/auth.service';
import { environment } from '@/environments/environment';

@Injectable({
  providedIn: 'root',
})
export class WebSocketService {
  client$ = webSocket<IWebSocketMessage>(this.createUrl());

  constructor(private authService: AuthService) {
    this.keepConnect();
  }

  private createUrl() {
    const userId = this.authService.getClaims()!.id;
    return `${environment.wsUrl}?id=${userId}`;
  }

  private keepConnect() {
    interval(60 * 1000).subscribe(() => {
      this.client$.next({
        target: websocketServerTarget.Ping,
      });
    });
  }

  public disconnect() {
    this.client$.complete();
    this.client$.unsubscribe();
  }
}
