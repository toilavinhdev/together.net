import { Component, OnInit } from '@angular/core';
import { Button } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { BaseComponent } from '@/core/abstractions';
import {
  IListReplyRequest,
  IReplyViewModel,
} from '@/shared/entities/reply.entities';
import { ActivatedRoute, Router } from '@angular/router';
import { ReplyService } from '@/shared/services';
import { takeUntil } from 'rxjs';
import { ReplyComponent } from '@/shared/components/elements';
import { NgIf, NgTemplateOutlet } from '@angular/common';
import { SkeletonModule } from 'primeng/skeleton';
import { getErrorMessage } from '@/shared/utilities';
import { ENotificationType } from '@/shared/enums';

@Component({
  selector: 'together-reply-tree',
  standalone: true,
  imports: [
    Button,
    DialogModule,
    InputTextModule,
    ReplyComponent,
    NgTemplateOutlet,
    SkeletonModule,
    NgIf,
  ],
  templateUrl: './reply-tree.component.html',
  styles: ``,
})
export class ReplyTreeComponent extends BaseComponent implements OnInit {
  visible: boolean = false;

  parent?: IReplyViewModel;

  focusChild?: IReplyViewModel;

  children: IReplyViewModel[] = [];

  notificationType?: ENotificationType;

  request: IListReplyRequest = {
    pageIndex: 1,
    pageSize: 6,
    parentId: undefined,
    focusChildId: undefined,
  };

  status: 'idle' | 'loading' | 'finished' = 'idle';

  constructor(
    private activatedRoute: ActivatedRoute,
    private replyService: ReplyService,
    private router: Router,
  ) {
    super();
  }

  private showDialog() {
    this.visible = true;
  }

  ngOnInit() {
    this.activatedRoute.queryParams
      .pipe(takeUntil(this.destroy$))
      .subscribe((params) => {
        if (params['replyParentId'] || params['focusChildId']) {
          this.request.parentId = params['replyParentId'];
          this.request.focusChildId = params['focusChildId'];
          this.notificationType = Number.parseInt(params['notificationType']);
          this.showDialog();
          this.loadReply();
        }
      });
  }

  private loadReply() {
    this.status = 'loading';
    this.replyService
      .listReply(this.request)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: ({ result, parent, focusChild }) => {
          this.status = 'finished';
          this.children = result;
          this.parent = parent;
          this.focusChild = focusChild;
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

  protected readonly Array = Array;

  protected readonly ENotificationType = ENotificationType;

  onHide() {
    let currentUrl = this.router.url;
    let cleanUrl = currentUrl.split('?')[0];
    this.router.navigateByUrl(cleanUrl).then();
  }
}
