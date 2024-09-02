import { Component, OnInit, ViewChild } from '@angular/core';
import { AvatarComponent } from '@/shared/components/elements';
import { TimeAgoPipe } from '@/shared/pipes';
import {
  IListNotificationRequest,
  INotificationViewModel,
} from '@/shared/entities/notification.entities';
import { BaseComponent } from '@/core/abstractions';
import { takeUntil } from 'rxjs';
import { NotificationService } from '@/shared/services';
import { AsyncPipe, NgForOf } from '@angular/common';
import { Menu, MenuModule } from 'primeng/menu';
import { PrimeTemplate } from 'primeng/api';
import { Ripple } from 'primeng/ripple';
import { TranslateModule } from '@ngx-translate/core';
import { getErrorMessage } from '@/shared/utilities';
import { SkeletonModule } from 'primeng/skeleton';
import { ENotificationStatus, ENotificationType } from '@/shared/enums';

@Component({
  selector: 'together-notification-dropdown',
  standalone: true,
  imports: [
    AvatarComponent,
    TimeAgoPipe,
    AsyncPipe,
    MenuModule,
    PrimeTemplate,
    Ripple,
    TranslateModule,
    SkeletonModule,
    NgForOf,
  ],
  templateUrl: './notification-dropdown.component.html',
})
export class NotificationDropdownComponent
  extends BaseComponent
  implements OnInit
{
  @ViewChild('menu', { static: true })
  private menuComponent!: Menu;

  notificationParams: IListNotificationRequest = {
    pageIndex: 1,
    pageSize: 6,
  };

  status: 'idle' | 'loading ' | 'finished' = 'idle';

  protected readonly Array = Array;

  protected readonly ENotificationType = ENotificationType;

  protected readonly ENotificationStatus = ENotificationStatus;

  constructor(protected notificationService: NotificationService) {
    super();
  }

  ngOnInit() {
    this.notificationService
      .getUnreadCountNotification()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (count) => {
          this.notificationService.unreadCount$.next(count);
        },
      });
  }

  private loadNotifications() {
    this.status = 'loading ';
    this.notificationService
      .listNotification(this.notificationParams)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: ({ result }) => {
          this.notificationService.notifications$.next(result);
          this.status = 'finished';
        },
        error: (err) => {
          this.commonService.toast$.next({
            type: 'error',
            message: getErrorMessage(err),
          });
        },
      });
  }

  navigateToSource(notification: INotificationViewModel) {
    switch (notification.type) {
      case ENotificationType.VotePost:
        this.commonService.navigateToPost(notification.directObjectId);
        break;
      case ENotificationType.VoteReply:
        this.commonService.navigateToPost(notification.prepositionalObjectId!, {
          replyId: notification.directObjectId,
        });
        break;
    }
    if (notification.status == ENotificationStatus.Unread) {
      this.markRead(notification.id);
    }
    this.menuComponent.hide();
  }

  toggle(event: Event) {
    if (this.status !== 'finished') {
      this.loadNotifications();
    }
    this.menuComponent.toggle(event);
  }

  markRead(id: string) {
    this.notificationService
      .markReadNotification(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          const notifications = this.notificationService.notifications$.value;
          notifications.forEach((notification) => {
            if (notification.id == id) {
              notification.status = ENotificationStatus.Read;
            }
          });
          this.notificationService.notifications$.next(notifications);
          this.notificationService.unreadCount$.next(
            this.notificationService.unreadCount$.value - 1,
          );
        },
        error: () => {},
      });
  }
}
