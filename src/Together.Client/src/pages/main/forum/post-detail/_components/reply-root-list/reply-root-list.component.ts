import { Component, OnInit } from '@angular/core';
import { BaseComponent } from '@/core/abstractions';
import { ReplyService } from '@/shared/services';
import {
  IListReplyRequest,
  IReplyViewModel,
} from '@/shared/entities/reply.entities';
import { ActivatedRoute } from '@angular/router';
import { takeUntil } from 'rxjs';
import { getErrorMessage } from '@/shared/utilities';
import { ReplyComponent } from '@/shared/components/elements';
import { SkeletonModule } from 'primeng/skeleton';
import { NgTemplateOutlet } from '@angular/common';

@Component({
  selector: 'together-reply-root-list',
  standalone: true,
  imports: [ReplyComponent, SkeletonModule, NgTemplateOutlet],
  templateUrl: './reply-root-list.component.html',
})
export class ReplyRootListComponent extends BaseComponent implements OnInit {
  replies: IReplyViewModel[] = [];

  params: IListReplyRequest = {
    pageIndex: 1,
    pageSize: 14,
  };

  status: 'idle' | 'loading' | 'finished' = 'idle';

  constructor(
    private replyService: ReplyService,
    private activatedRoute: ActivatedRoute,
  ) {
    super();
  }

  ngOnInit() {
    this.activatedRoute.paramMap
      .pipe(takeUntil(this.destroy$))
      .subscribe((paramMap) => {
        const postId = paramMap.get('postId');
        if (!postId) return;
        this.params.postId = postId;
        this.loadRootReplies();
      });
  }

  private loadRootReplies() {
    this.status = 'loading';
    this.replyService
      .listReply(this.params)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: ({ result }) => {
          this.status = 'finished';
          this.replies = result;
        },
        error: (err) => {
          this.status = 'finished';
          this.commonService.toast$.next({
            type: 'error',
            message: getErrorMessage(err),
          });
        },
      });
  }

  addRootReply(reply: IReplyViewModel) {
    this.replies = [reply, ...this.replies];
  }

  onDeleteReply(replyId: string) {
    this.replies = this.replies.filter((reply) => reply.id !== replyId);
  }

  protected readonly Array = Array;
}
