import { Component, EventEmitter, Input, Output } from '@angular/core';
import { BaseComponent } from '@/core/abstractions';
import { NgClass } from '@angular/common';
import { EditorModule } from 'primeng/editor';
import { Button } from 'primeng/button';
import { ReplyService, UserService } from '@/shared/services';
import { take, takeUntil } from 'rxjs';
import { getErrorMessage } from '@/shared/utilities';
import { FormsModule } from '@angular/forms';
import { IReplyViewModel } from '@/shared/entities/reply.entities';

@Component({
  selector: 'together-reply-writer',
  standalone: true,
  imports: [NgClass, EditorModule, Button, FormsModule],
  templateUrl: './reply-writer.component.html',
})
export class ReplyWriterComponent extends BaseComponent {
  @Input()
  postId = '';

  @Input()
  parentId?: string;

  @Input()
  showOnClose = true;

  @Output()
  response = new EventEmitter<IReplyViewModel>();

  body = '';

  expand = false;

  loading = false;

  constructor(
    private replyService: ReplyService,
    private userService: UserService,
  ) {
    super();
  }

  toggle() {
    this.expand = !this.expand;
  }

  onSubmit() {
    if (!this.body || !this.postId) return;
    this.loading = true;
    this.replyService
      .createReply({
        parentId: this.parentId,
        postId: this.postId,
        body: this.body,
      })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data) => {
          this.loading = false;
          this.expand = false;
          this.userService.me$.pipe(take(1)).subscribe((user) => {
            if (!user) return;
            this.response.emit({
              id: data.id,
              subId: data.subId,
              postId: data.postId,
              parentId: data.parentId,
              body: data.body,
              createdAt: data.createdAt,
              createdById: user.id,
              createdByUserName: user.userName,
              createdByAvatar: user.avatar,
              voteUpCount: 0,
              voteDownCount: 0,
              voted: undefined,
              childCount: 0,
            } as IReplyViewModel);
          });
        },
        error: (err) => {
          this.loading = false;
          this.commonService.toast$.next({
            type: 'error',
            message: getErrorMessage(err),
          });
        },
      });
  }
}
