import { Injectable } from '@angular/core';
import { BaseService } from '@/core/abstractions';
import {
  IListNotificationRequest,
  IListNotificationResponse,
  INotificationViewModel,
} from '@/shared/entities/notification.entities';
import { IBaseResponse } from '@/core/models';
import { BehaviorSubject, map, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class NotificationService extends BaseService {
  notifications$ = new BehaviorSubject<INotificationViewModel[]>([]);

  unreadCount$ = new BehaviorSubject<number>(0);

  constructor() {
    super();
    this.setEndpoint('notification', 'notification');
  }

  listNotification(params: IListNotificationRequest) {
    const url = this.createUrl('/list');
    return this.client
      .get<
        IBaseResponse<IListNotificationResponse>
      >(url, { params: this.createParams(params) })
      .pipe(map((response) => response.data));
  }

  getUnreadCountNotification(): Observable<number> {
    const url = this.createUrl('/unread-count');
    return this.client
      .get<IBaseResponse<number>>(url, {})
      .pipe(map((response) => response.data));
  }

  markReadNotification(id: string) {
    const url = this.createUrl('/mark-read');
    return this.client.post<IBaseResponse>(url, { id });
  }

  markAllReadNotification() {
    const url = this.createUrl('/mark-all-read');
    return this.client.post<IBaseResponse>(url, {});
  }
}
