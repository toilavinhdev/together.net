import { Component, OnInit } from '@angular/core';
import { BaseComponent } from '@/core/abstractions';
import { PostComponent } from '@/shared/components/elements';
import { NgClass } from '@angular/common';
import { IPostViewModel } from '@/shared/entities/post.entities';
import { PostService } from '@/shared/services';
import { takeUntil } from 'rxjs';
import { getErrorMessage } from '@/shared/utilities';
import {TranslateModule} from "@ngx-translate/core";

@Component({
  selector: 'together-latest-posts',
  standalone: true,
  imports: [PostComponent, NgClass, TranslateModule],
  templateUrl: './latest-posts.component.html',
})
export class LatestPostsComponent extends BaseComponent implements OnInit {
  latestPost: IPostViewModel[] = [];

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
      .listPost({ pageIndex: 1, pageSize: 5 })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: ({ result }) => {
          this.loading = false;
          this.latestPost = result;
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
