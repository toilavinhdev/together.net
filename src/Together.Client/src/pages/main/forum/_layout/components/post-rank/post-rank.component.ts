import { Component, OnInit } from '@angular/core';
import { BaseComponent } from '@/core/abstractions';
import { NgClass } from '@angular/common';
import { IPostViewModel } from '@/shared/entities/post.entities';
import { PostService } from '@/shared/services';
import { takeUntil } from 'rxjs';
import { getErrorMessage } from '@/shared/utilities';
import { PostComponent } from '@/shared/components/elements';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'together-post-rank',
  standalone: true,
  imports: [NgClass, PostComponent, TranslateModule],
  templateUrl: './post-rank.component.html',
})
export class PostRankComponent extends BaseComponent implements OnInit {
  posts: IPostViewModel[] = [];

  loading = false;

  constructor(private postService: PostService) {
    super();
  }

  ngOnInit() {
    this.loadData();
  }

  private loadData() {
    this.loading = true;
    this.postService
      .listPost({ pageIndex: 1, pageSize: 3, sort: '-VoteUpCount' })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: ({ result }) => {
          this.loading = false;
          this.posts = result;
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

  protected readonly Array = Array;
}
