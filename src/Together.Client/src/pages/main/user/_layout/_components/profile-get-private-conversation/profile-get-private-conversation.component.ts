import { Component, OnInit } from '@angular/core';
import { TooltipModule } from 'primeng/tooltip';
import { Button } from 'primeng/button';
import { BaseComponent } from '@/core/abstractions';
import { ActivatedRoute } from '@angular/router';
import { takeUntil } from 'rxjs';
import { ConversationService } from '@/shared/services';
import { getErrorMessage } from '@/shared/utilities';
import { EConversationType } from '@/shared/enums';

@Component({
  selector: 'together-profile-get-private-conversation',
  standalone: true,
  imports: [TooltipModule, Button],
  templateUrl: './profile-get-private-conversation.component.html',
})
export class ProfileGetPrivateConversationComponent
  extends BaseComponent
  implements OnInit
{
  userId = '';

  constructor(
    private activatedRoute: ActivatedRoute,
    private conversationService: ConversationService,
  ) {
    super();
  }

  ngOnInit() {
    this.activatedRoute.paramMap
      .pipe(takeUntil(this.destroy$))
      .subscribe((paramMap) => {
        this.userId = paramMap.get('userId') ?? '';
      });
  }

  onGetPrivateConversation() {
    if (!this.userId) return;
    this.conversationService
      .getConversation({ privateReceiverId: this.userId })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (conversation) => {
          if (conversation) {
            this.commonService.navigateToConversation(conversation.id);
          } else {
            this.conversationService
              .createConversation({
                type: EConversationType.Private,
                otherParticipantIds: [this.userId],
              })
              .pipe(takeUntil(this.destroy$))
              .subscribe({
                next: (newConversationId) => {
                  this.commonService.navigateToConversation(newConversationId);
                },
                error: (err) => {
                  this.commonService.toast$.next({
                    type: 'error',
                    message: getErrorMessage(err),
                  });
                },
              });
          }
        },
        error: (err) => {
          this.commonService.toast$.next({
            type: 'error',
            message: getErrorMessage(err),
          });
        },
      });
  }
}
