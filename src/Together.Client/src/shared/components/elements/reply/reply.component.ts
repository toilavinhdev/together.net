import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  Output,
} from '@angular/core';
import { IReplyViewModel } from '@/shared/entities/reply.entities';
import { NgClass, NgForOf, NgIf } from '@angular/common';
import { BaseComponent } from '@/core/abstractions';
import {
  SanitizeHtmlPipe,
  ShortenNumberPipe,
  TimeAgoPipe,
} from '@/shared/pipes';
import {
  AvatarComponent,
  ReplyWriterComponent,
  VoteComponent,
} from '@/shared/components/elements';
import { ReplyService } from '@/shared/services';
import { takeUntil } from 'rxjs';
import { getErrorMessage } from '@/shared/utilities';
import { IVoteResponse } from '@/shared/entities/post.entities';
import { EVoteType } from '@/shared/enums';
import { FormsModule } from '@angular/forms';
import { EditorComponent } from '@/shared/components/controls';
import { Button } from 'primeng/button';

@Component({
  selector: 'together-reply',
  standalone: true,
  imports: [
    NgClass,
    SanitizeHtmlPipe,
    AvatarComponent,
    TimeAgoPipe,
    ShortenNumberPipe,
    NgIf,
    VoteComponent,
    NgForOf,
    ReplyWriterComponent,
    FormsModule,
    EditorComponent,
    Button,
  ],
  templateUrl: './reply.component.html',
})
export class ReplyComponent extends BaseComponent {
  @Output()
  deleteReply = new EventEmitter<string>();

  @Input()
  reply!: IReplyViewModel;

  children: IReplyViewModel[] = [];

  childrenNextPageIndex = 1;

  childrenHasNextPage = true;

  childrenPageSize = 1;

  childrenLoading = false;

  showEditor = false;

  body = '';

  constructor(
    private replyService: ReplyService,
    private cdk: ChangeDetectorRef,
  ) {
    super();
  }

  showChildren() {
    if (this.childrenLoading) return;
    this.loadChildren();
  }

  onAddChild(reply: IReplyViewModel) {
    this.children = [reply, ...this.children];
    // update paging
    this.cdk.detectChanges();
  }

  onVoteReply(data: IVoteResponse) {
    if (data.sourceId !== this.reply.id) return;
    // fixed no output
    if (!data.isVoted) {
      if (this.reply.voted === EVoteType.UpVote) this.reply.voteUpCount--;
      if (this.reply.voted === EVoteType.DownVote) this.reply.voteDownCount--;
    } else {
      if (this.reply.voted === undefined || this.reply.voted === null) {
        if (data.value === EVoteType.UpVote) this.reply.voteUpCount++;
        if (data.value === EVoteType.DownVote) this.reply.voteDownCount++;
      }
      if (this.reply.voted === EVoteType.UpVote) {
        this.reply.voteUpCount--;
        this.reply.voteDownCount++;
      }
      if (this.reply.voted === EVoteType.DownVote) {
        this.reply.voteUpCount++;
        this.reply.voteDownCount--;
      }
    }
    this.reply.voted = data.value;
  }

  private loadChildren() {
    if (!this.reply) return;
    this.childrenLoading = true;
    this.replyService
      .listReply({
        pageIndex: this.childrenNextPageIndex,
        pageSize: this.childrenPageSize,
        postId: this.reply.postId,
        parentId: this.reply.id,
      })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: ({ result, pagination }) => {
          this.childrenLoading = false;
          this.children =
            this.children.length === 0 ? result : [...this.children, ...result];
          if (pagination.hasNextPage) this.childrenNextPageIndex++;
          this.childrenHasNextPage = pagination.hasNextPage;
        },
        error: (err) => {
          this.childrenLoading = false;
          this.commonService.toast$.next({
            type: 'error',
            message: getErrorMessage(err),
          });
        },
      });
  }

  onDeleteSelf() {
    console.log(this.reply, this.children);
    this.deleteReply.emit(this.reply.id);
  }

  onDeleteChild(childId: string) {
    this.children = this.children.filter((child) => child.id !== childId);
  }

  protected readonly console = console;

  onUpdate() {
    this.replyService
      .updateReply({ id: this.reply.id, body: this.body })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.commonService.toast$.next({
            type: 'success',
            message: 'Cập nhật phản hồi thành công',
          });
          this.showEditor = false;
          this.reply.body = this.body;
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
