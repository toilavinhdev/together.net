import { Component, OnInit } from '@angular/core';
import { BaseComponent } from '@/core/abstractions';
import { NotificationService, WebSocketService } from '@/shared/services';
import { filter, takeUntil } from 'rxjs';
import { websocketClientTarget } from '@/shared/constants';
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { JsonPipe } from '@angular/common';
import { AvatarComponent } from '@/shared/components/elements';
import { TimeAgoPipe } from '@/shared/pipes';
import { ENotificationStatus, ENotificationType } from '@/shared/enums';
import { TranslateModule } from '@ngx-translate/core';
import { INotificationViewModel } from '@/shared/entities/notification.entities';

@Component({
  selector: 'together-notification-alert',
  standalone: true,
  imports: [
    ToastModule,
    JsonPipe,
    AvatarComponent,
    TimeAgoPipe,
    TranslateModule,
  ],
  templateUrl: './notification-alert.component.html',
  providers: [MessageService],
})
export class NotificationAlertComponent
  extends BaseComponent
  implements OnInit
{
  protected readonly ENotificationType = ENotificationType;

  constructor(
    private webSocketService: WebSocketService,
    private messageService: MessageService,
    private notificationService: NotificationService,
  ) {
    super();
  }

  id = 0;

  ngOnInit() {
    this.listenWebSocket();
  }

  private listenWebSocket() {
    this.webSocketService.client$
      .pipe(
        takeUntil(this.destroy$),
        filter(
          (socket) =>
            socket.target === websocketClientTarget.ReceivedNotification,
        ),
      )
      .subscribe({
        next: (socket) => {
          this.messageService.add({
            key: this.id.toString(),
            life: 4000,
            severity: 'secondary',
            closable: true,
            data: socket.message,
          });
          this.id++;
          this.notificationService.unreadCount$.next(
            this.notificationService.unreadCount$.value + 1,
          );
          const notifications = this.notificationService.notifications$.value;
          if (notifications.length) {
            this.notificationService.notifications$.next([
              {
                id: socket.message.notificationId,
                subId: 0,
                subject: {
                  id: socket.message.subjectId,
                  userName: socket.message.subjectUserName,
                  avatar: socket.message.subjectAvatar,
                  isOfficial: socket.message.subjectIsOfficial,
                },
                type: socket.message.notificationType,
                directObjectId: socket.message.directObjectId,
                status: ENotificationStatus.Unread,
                createdAt: socket.message.createdAt,
                indirectObjectId: socket.message.indirectObjectId,
                prepositionalObjectId: socket.message.prepositionalObjectId,
              } as INotificationViewModel,
              ...notifications,
            ]);
          }
        },
      });
  }

  navigateToSource(key: string) {
    this.messageService.clear(key);
  }
}
