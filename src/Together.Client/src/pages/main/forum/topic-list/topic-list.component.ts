import { Component, OnInit } from '@angular/core';
import { BaseComponent } from '@/core/abstractions';
import { ForumService } from '@/shared/services/forum.service';
import { takeUntil } from 'rxjs';
import { AsyncPipe, NgIf } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ShortenNumberPipe } from '@/shared/pipes';
import { getErrorMessage } from '@/shared/utilities';
import { SkeletonModule } from 'primeng/skeleton';
import { PostComponent } from '@/shared/components/elements';

@Component({
  selector: 'together-topic-list',
  standalone: true,
  imports: [
    AsyncPipe,
    RouterLink,
    NgIf,
    ShortenNumberPipe,
    SkeletonModule,
    PostComponent,
  ],
  templateUrl: './topic-list.component.html',
})
export class TopicListComponent extends BaseComponent implements OnInit {
  protected readonly Array = Array;

  loading = false;

  constructor(protected forumService: ForumService) {
    super();
  }

  ngOnInit() {
    this.commonService.breadcrumb$.next([]);
    this.loadData();
  }

  private loadData() {
    this.loading = true;
    this.forumService
      .listForum(true)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data) => {
          this.loading = false;
          this.forumService.forums$.next(data);
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
