import { Component, OnInit } from '@angular/core';
import { BaseComponent } from '@/core/abstractions';
import { PostService } from '@/shared/services';
import {
  IListPostRequest,
  IPostViewModel,
} from '@/shared/entities/post.entities';
import { ActivatedRoute } from '@angular/router';
import { takeUntil } from 'rxjs';
import { getErrorMessage } from '@/shared/utilities';
import {
  IPaginatorChange,
  PaginatorComponent,
  PostComponent,
} from '@/shared/components/elements';
import { NgForOf } from '@angular/common';

@Component({
  selector: 'together-profile-post-list',
  standalone: true,
  imports: [PostComponent, PaginatorComponent, NgForOf],
  templateUrl: './profile-post-list.component.html',
})
export class ProfilePostListComponent extends BaseComponent implements OnInit {
  posts: IPostViewModel[] = [];

  params: IListPostRequest = {
    userId: '',
    pageIndex: 1,
    pageSize: 4,
  };

  totalRecord = 0;

  loading = false;

  constructor(
    private postService: PostService,
    private activatedRoute: ActivatedRoute,
  ) {
    super();
  }

  ngOnInit(): void {
    this.activatedRoute.paramMap
      .pipe(takeUntil(this.destroy$))
      .subscribe((paramMap) => {
        const userId = paramMap.get('userId');
        if (!userId) return;
        this.params.userId = userId;
        this.loadPosts();
      });
  }

  private loadPosts() {
    this.loading = true;
    this.postService
      .listPost(this.params)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: ({ result, pagination }) => {
          this.posts = result;
          this.loading = false;
          this.totalRecord = pagination.totalRecord;
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

  onPaginationChange(event: IPaginatorChange) {
    console.log(event);
    if (this.params.pageIndex === event.pageIndex) return;
    this.params.pageIndex = event.pageIndex;
    this.loadPosts();
  }

  protected readonly Array = Array;
}
