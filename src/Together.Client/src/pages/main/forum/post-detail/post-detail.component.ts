import { Component, OnInit } from '@angular/core';
import { BaseComponent } from '@/core/abstractions';
import { PostService, UserService } from '@/shared/services';
import { ActivatedRoute } from '@angular/router';
import { takeUntil } from 'rxjs';
import {
  IGetPostResponse,
  IVoteResponse,
} from '@/shared/entities/post.entities';
import { getErrorMessage, windowScrollToTop } from '@/shared/utilities';
import {
  AvatarComponent,
  PrefixComponent,
  ReplyWriterComponent,
  VoteComponent,
} from '@/shared/components/elements';
import { AsyncPipe, NgIf } from '@angular/common';
import {
  SanitizeHtmlPipe,
  ShortenNumberPipe,
  TimeAgoPipe,
} from '@/shared/pipes';
import { ReplyRootListComponent } from '@/pages/main/forum/post-detail/_components';
import { EVoteType } from '@/shared/enums';
import { TranslateModule } from '@ngx-translate/core';
import { policies } from '@/shared/constants';
import { SkeletonModule } from 'primeng/skeleton';

@Component({
  selector: 'together-post-detail',
  standalone: true,
  imports: [
    PrefixComponent,
    AsyncPipe,
    NgIf,
    AvatarComponent,
    TimeAgoPipe,
    ShortenNumberPipe,
    SanitizeHtmlPipe,
    ReplyWriterComponent,
    ReplyRootListComponent,
    VoteComponent,
    TranslateModule,
    SkeletonModule,
  ],
  templateUrl: './post-detail.component.html',
})
export class PostDetailComponent extends BaseComponent implements OnInit {
  post!: IGetPostResponse;

  postId = '';

  status: 'idle' | 'loading' | 'finished' = 'idle';

  constructor(
    private postService: PostService,
    private activatedRoute: ActivatedRoute,
    protected userService: UserService,
  ) {
    super();
  }

  ngOnInit() {
    this.loadPost();
  }

  private loadPost() {
    this.activatedRoute.paramMap
      .pipe(takeUntil(this.destroy$))
      .subscribe((paramMap) => {
        const postId = paramMap.get('postId');
        if (!postId) return;
        this.postId = postId;
        this.status = 'loading';
        this.postService
          .getPost(postId)
          .pipe(takeUntil(this.destroy$))
          .subscribe({
            next: (data) => {
              this.status = 'finished';
              this.post = data;
              this.commonService.breadcrumb$.next([
                {
                  title: data.forumName,
                  routerLink: ['/', 'topics', data.topicId],
                },
                {
                  title: data.topicName,
                },
              ]);
              windowScrollToTop();
            },
            error: (err) => {
              this.status = 'finished';
              this.commonService.toast$.next({
                type: 'error',
                message: getErrorMessage(err),
              });
            },
          });
      });
  }

  onVotePost(data: IVoteResponse) {
    if (data.sourceId !== this.post.id) return;
    if (!data.isVoted) {
      if (this.post.voted === EVoteType.UpVote) this.post.voteUpCount--;
      if (this.post.voted === EVoteType.DownVote) this.post.voteDownCount--;
    } else {
      if (this.post.voted === undefined || this.post.voted === null) {
        if (data.value === EVoteType.UpVote) this.post.voteUpCount++;
        if (data.value === EVoteType.DownVote) this.post.voteDownCount++;
      }
      if (this.post.voted === EVoteType.UpVote) {
        this.post.voteUpCount--;
        this.post.voteDownCount++;
      }
      if (this.post.voted === EVoteType.DownVote) {
        this.post.voteUpCount++;
        this.post.voteDownCount--;
      }
    }
    this.post.voted = data.value;
  }

  protected readonly policies = policies;
}
