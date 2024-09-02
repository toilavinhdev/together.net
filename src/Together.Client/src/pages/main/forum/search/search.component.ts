import { Component, OnDestroy, OnInit } from '@angular/core';
import { BaseComponent } from '@/core/abstractions';
import { ActivatedRoute, Router } from '@angular/router';
import { Button } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { TabViewModule } from 'primeng/tabview';
import { debounceTime, Subject, takeUntil } from 'rxjs';
import { IPostViewModel } from '@/shared/entities/post.entities';
import { PostService, UserService } from '@/shared/services';
import { getErrorMessage, windowScrollToTop } from '@/shared/utilities';
import {
  AvatarComponent,
  IPaginatorChange,
  PaginatorComponent,
  PostComponent,
} from '@/shared/components/elements';
import {
  AsyncPipe,
  NgClass,
  NgForOf,
  NgIf,
  NgTemplateOutlet,
} from '@angular/common';
import { IUserViewModel } from '@/shared/entities/user.entities';
import { SkeletonModule } from 'primeng/skeleton';
import { policies } from '@/shared/constants';

@Component({
  selector: 'together-search',
  standalone: true,
  imports: [
    Button,
    InputTextModule,
    ReactiveFormsModule,
    TranslateModule,
    FormsModule,
    TabViewModule,
    PostComponent,
    NgForOf,
    PaginatorComponent,
    NgIf,
    AvatarComponent,
    NgClass,
    NgTemplateOutlet,
    SkeletonModule,
    AsyncPipe,
  ],
  templateUrl: './search.component.html',
})
export class SearchComponent
  extends BaseComponent
  implements OnInit, OnDestroy
{
  posts: IPostViewModel[] = [];

  users: IUserViewModel[] = [];

  search = '';

  search$ = new Subject<string>();

  pageIndex = 1;

  pageSize = 8;

  totalRecord = 0;

  tabIndex = 0;

  status: 'idle' | 'loading' | 'success' | 'failed' = 'idle';

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private postService: PostService,
    protected userService: UserService,
  ) {
    super();
  }

  ngOnInit() {
    this.commonService.breadcrumb$.next([
      {
        title: 'Search',
      },
    ]);
    this.search$
      .pipe(takeUntil(this.destroy$), debounceTime(400))
      .subscribe(() => {
        this.onSearch();
      });
  }

  private initSearch() {
    const queryParams = this.activatedRoute.snapshot.queryParams;
    this.search = queryParams['q'];
    this.tabIndex = parseInt(queryParams['tab'] ?? 0);
  }

  onTabChange(idx: number) {
    this.tabIndex = idx;
    this.pageIndex = 1;
    this.totalRecord = 0;
    this.onSearch();
    this.router
      .navigate([], {
        queryParamsHandling: 'merge',
        queryParams: {
          tab: idx,
        },
      })
      .then();
  }

  onSearch() {
    if (!this.search) return;
    if (this.tabIndex === 0) {
      this.loadPosts();
    } else if (this.tabIndex === 1) {
      this.loadUsers();
    }
    this.router
      .navigate([], {
        queryParamsHandling: 'merge',
        queryParams: {
          q: this.search,
        },
      })
      .then();
  }

  onPaginationChange(event: IPaginatorChange) {
    windowScrollToTop();
    this.pageIndex = event.pageIndex;
    this.onSearch();
  }

  loadPosts() {
    this.status = 'loading';
    this.postService
      .listPost({
        pageIndex: this.pageIndex,
        pageSize: this.pageSize,
        search: this.search,
      })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: ({ result, pagination }) => {
          this.status = 'success';
          this.posts = result;
          this.totalRecord = pagination.totalRecord;
        },
        error: (err) => {
          this.status = 'failed';
          this.commonService.toast$.next({
            type: 'error',
            message: getErrorMessage(err),
          });
        },
      });
  }

  loadUsers() {
    this.status = 'loading';
    this.userService
      .listUser({
        pageIndex: this.pageIndex,
        pageSize: this.pageSize,
        search: this.search,
      })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: ({ result, pagination }) => {
          this.status = 'success';
          this.users = result;
          this.totalRecord = pagination.totalRecord;
        },
        error: (err) => {
          this.status = 'failed';
          this.commonService.toast$.next({
            type: 'error',
            message: getErrorMessage(err),
          });
        },
      });
  }

  protected readonly Array = Array;
  protected readonly policies = policies;
}
