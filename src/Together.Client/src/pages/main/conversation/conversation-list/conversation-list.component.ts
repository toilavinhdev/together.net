import { Component, OnInit } from '@angular/core';
import { BaseComponent } from '@/core/abstractions';
import {
  ConversationService,
  UserService,
  WebSocketService,
} from '@/shared/services';
import { IListConversationRequest } from '@/shared/entities/conversation.entities';
import { filter, take, takeUntil } from 'rxjs';
import { getErrorMessage } from '@/shared/utilities';
import { AvatarComponent } from '@/shared/components/elements';
import { TimeAgoPipe } from '@/shared/pipes';
import { AsyncPipe, NgClass, NgIf } from '@angular/common';
import { websocketClientTarget } from '@/shared/constants';
import { NavigationEnd, Router } from '@angular/router';

@Component({
  selector: 'together-conversation-list',
  standalone: true,
  imports: [AvatarComponent, TimeAgoPipe, NgIf, NgClass, AsyncPipe],
  templateUrl: './conversation-list.component.html',
})
export class ConversationListComponent extends BaseComponent implements OnInit {
  params: IListConversationRequest = {
    pageIndex: 1,
    pageSize: 12,
  };

  selectedConversationId = '';

  constructor(
    protected conversationService: ConversationService,
    protected userService: UserService,
    private router: Router,
    private webSocketService: WebSocketService,
  ) {
    super();
  }

  ngOnInit() {
    this.routerTracking();
    this.listenWebSocket();
    this.loadConversations();
  }

  private routerTracking() {
    this.selectedConversationId = this.router.url.split('/')[2];
    this.router.events
      .pipe(
        takeUntil(this.destroy$),
        filter((event) => event instanceof NavigationEnd),
      )
      .subscribe((event: any) => {
        this.selectedConversationId = event.url.split('/')[2];
      });
  }

  private loadConversations() {
    this.conversationService
      .listConversation(this.params)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: ({ result }) => {
          this.conversationService.conversations$.next(result);
        },
        error: (err) => {
          this.commonService.toast$.next({
            type: 'error',
            message: getErrorMessage(err),
          });
        },
      });
  }

  private listenWebSocket() {
    this.webSocketService.client$
      .pipe(
        takeUntil(this.destroy$),
        filter(
          (message) => message.target === websocketClientTarget.ReceivedMessage,
        ),
      )
      .subscribe({
        next: (socket) => {
          this.conversationService.conversations$.pipe(take(1)).subscribe({
            next: (conversations) => {
              // update conversation
              let existed = conversations.find(
                (c) => c.id === socket.message.conversationId,
              );
              // nếu chưa đã có conversation thì get api
              if (!existed) {
              } else {
                this.conversationService.conversations$.next([
                  {
                    ...existed,
                    lastMessageText: socket.message.text,
                    lastMessageAt: socket.message.createdAt,
                    lastMessageByUserId: socket.message.createdBy,
                    lastMessageByUserName: socket.message.createdByUserName,
                  },
                  ...conversations.filter((c) => c.id !== existed.id),
                ]);
              }
            },
          });
        },
        error: () => {},
      });
  }
}
