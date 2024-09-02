import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  OnInit,
  Output,
  SimpleChanges,
  ViewChild,
} from '@angular/core';
import { AsyncPipe, NgClass, NgIf } from '@angular/common';
import { BaseComponent } from '@/core/abstractions';
import { EVoteType } from '@/shared/enums';
import { ShortenNumberPipe } from '@/shared/pipes';
import { PostService, ReplyService, UserService } from '@/shared/services';
import { Observable, take, takeUntil } from 'rxjs';
import { getErrorMessage } from '@/shared/utilities';
import { IVoteResponse } from '@/shared/entities/post.entities';
import { Menu, MenuModule } from 'primeng/menu';
import { ConfirmationService, MenuItem } from 'primeng/api';
import { Ripple } from 'primeng/ripple';
import { TranslateModule } from '@ngx-translate/core';
import { Router } from '@angular/router';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { policies } from '@/shared/constants';

@Component({
  selector: 'together-vote',
  standalone: true,
  imports: [
    NgClass,
    ShortenNumberPipe,
    MenuModule,
    Ripple,
    TranslateModule,
    ConfirmDialogModule,
    NgIf,
    AsyncPipe,
  ],
  templateUrl: './vote.component.html',
  providers: [ConfirmationService],
})
export class VoteComponent extends BaseComponent implements OnInit, OnChanges {
  @ViewChild('menu', { static: true }) menu!: Menu;

  @Output()
  expandReplyWriter = new EventEmitter<void>();

  @Output()
  voteResponse = new EventEmitter<IVoteResponse>();

  @Output()
  deleteResponse = new EventEmitter<string>();

  @Output()
  updateResponse = new EventEmitter();

  @Input()
  sourceId = '';

  @Input()
  userId = '';

  @Input()
  voteFor: 'post' | 'reply' = 'post';

  @Input()
  voteUpCount = 0;

  @Input()
  voteDownCount = 0;

  @Input()
  replyCount = 0;

  @Input()
  voted?: EVoteType;

  protected readonly EVoteType = EVoteType;

  items: MenuItem[] = [];

  constructor(
    private postService: PostService,
    private replyService: ReplyService,
    private router: Router,
    private primeNGConfirmationService: ConfirmationService,
    protected userService: UserService,
  ) {
    super();
  }

  ngOnInit() {}

  ngOnChanges(changes: SimpleChanges) {
    if ('voteFor' in changes) {
      this.items = [
        {
          id: 'Update',
          label: this.voteFor === 'post' ? 'Update post' : 'Update reply',
          icon: 'pi pi-pencil',
          command: () => {
            this.onUpdate();
          },
        },
        {
          id: 'Delete',
          label: this.voteFor === 'post' ? 'Delete post' : 'Delete reply',
          icon: 'pi pi-trash',
          command: () => {
            this.menu.visible = false;
            setTimeout(() => {
              this.primeNGConfirmationService.confirm({
                message:
                  this.voteFor === 'post'
                    ? 'Bạn có muốn xóa bài viết này không?'
                    : 'Bạn có muốn xóa phản hồi này không?',
                header: 'Xác nhận xóa',
                icon: 'pi pi-exclamation-triangle',
                acceptIcon: 'none',
                rejectIcon: 'none',
                acceptLabel: 'Xác nhận',
                rejectLabel: 'Hủy',
                rejectButtonStyleClass: 'p-button-text',
                acceptButtonStyleClass: 'p-button-danger',
                accept: () => {
                  this.onDelete();
                },
                reject: () => {},
              });
            }, 100);
          },
        },
        {
          id: 'Report',
          label: 'Report',
          icon: 'pi pi-flag',
          command: () => {
            this.onReport();
          },
        },
      ];
    }
  }

  onVote(type: EVoteType) {
    if (!this.sourceId) return;
    this.getObservable(type)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data) => {
          this.voteResponse.emit(data);
        },
        error: (err) => {
          this.commonService.toast$.next({
            type: 'error',
            message: getErrorMessage(err),
          });
        },
      });
  }

  private onUpdate() {
    if (this.voteFor === 'post') {
      this.commonService.navigateToUpdatePost(this.sourceId);
    } else if (this.voteFor === 'reply') {
      this.updateResponse.emit();
    }
  }

  private onDelete() {
    if (this.voteFor === 'post') {
      this.postService
        .deletePost(this.sourceId)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.deleteResponse.emit(this.sourceId);
            this.commonService.breadcrumb$
              .pipe(take(1))
              .subscribe((breadcrumb) => {
                this.router
                  .navigate(breadcrumb[0].routerLink as string[])
                  .then();
              });
            this.commonService.toast$.next({
              type: 'success',
              message: 'Đã xóa bài viết',
            });
          },
          error: (err) => {
            this.commonService.toast$.next({
              type: 'error',
              message: getErrorMessage(err),
            });
          },
        });
    } else if (this.voteFor === 'reply') {
      this.replyService
        .deleteReply(this.sourceId)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.deleteResponse.emit(this.sourceId);
            this.commonService.toast$.next({
              type: 'success',
              message: 'Đã xóa phản hồi',
            });
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

  private onReport() {
    this.commonService.toast$.next({
      type: 'info',
      message: 'Tính năng đang phát triển',
    });
  }

  private getObservable(type: EVoteType): Observable<IVoteResponse> {
    if (this.voteFor === 'post') {
      return this.postService.votePost({
        postId: this.sourceId,
        type: type,
      });
    } else if (this.voteFor === 'reply') {
      return this.replyService.voteReply({
        replyId: this.sourceId,
        type: type,
      });
    }
    return new Observable<IVoteResponse>();
  }

  protected readonly policies = policies;
}
