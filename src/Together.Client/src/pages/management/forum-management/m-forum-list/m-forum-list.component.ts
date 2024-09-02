import { Component, computed, OnInit, signal } from '@angular/core';
import { BaseComponent } from '@/core/abstractions';
import { ForumService, TopicService, UserService } from '@/shared/services';
import { TabViewModule } from 'primeng/tabview';
import { TranslateModule } from '@ngx-translate/core';
import { takeUntil } from 'rxjs';
import { getErrorMessage } from '@/shared/utilities';
import { IForumViewModel } from '@/shared/entities/forum.entities';
import {
  TableCellDirective,
  TableColumnDirective,
  TableComponent,
} from '@/shared/components/elements';
import { ITopicViewModel } from '@/shared/entities/topic.entities';
import { AsyncPipe, JsonPipe, NgIf } from '@angular/common';
import { Button } from 'primeng/button';
import { RouterLink } from '@angular/router';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService } from 'primeng/api';
import { policies } from '@/shared/constants';

@Component({
  selector: 'together-m-forum-list',
  standalone: true,
  imports: [
    TabViewModule,
    TranslateModule,
    TableComponent,
    TableColumnDirective,
    JsonPipe,
    TableCellDirective,
    Button,
    RouterLink,
    ConfirmDialogModule,
    NgIf,
    AsyncPipe,
  ],
  templateUrl: './m-forum-list.component.html',
  providers: [ConfirmationService],
})
export class MForumListComponent extends BaseComponent implements OnInit {
  forums = signal<IForumViewModel[]>([]);

  topics = computed(() => this.forums().flatMap((f) => f.topics));

  loading = false;

  constructor(
    private forumService: ForumService,
    private topicService: TopicService,
    private primeConfirmationService: ConfirmationService,
    protected userService: UserService,
  ) {
    super();
  }

  ngOnInit() {
    this.commonService.title$.next('Danh sách diễn đàn');
    this.loadForums();
  }

  private loadForums() {
    this.loading = true;
    this.forumService
      .listForum(true)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data) => {
          this.loading = false;
          this.forums.set(data);
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

  confirmDeleteForum(event: Event, id: string, name: string) {
    this.primeConfirmationService.confirm({
      target: event.target as EventTarget,
      message: `Dữ liệu của diễn đàn '${name}' khi xóa sẽ không thể hoàn tác`,
      header: 'Xác nhận xóa',
      icon: 'pi pi-exclamation-triangle',
      acceptIcon: 'none',
      rejectIcon: 'none',
      rejectButtonStyleClass: 'p-button-text',
      accept: () => {
        this.forumService
          .deleteForum(id)
          .pipe(takeUntil(this.destroy$))
          .subscribe({
            next: () => {
              this.commonService.toast$.next({
                type: 'success',
                message: 'Xóa diễn đàn thành công',
              });
              this.forums.update((forum) => forum.filter((f) => f.id !== id));
            },
            error: (err) => {
              this.commonService.toast$.next({
                type: 'error',
                message: getErrorMessage(err),
              });
            },
          });
      },
      reject: () => {},
    });
  }

  confirmDeleteTopic(event: Event, id: string, name: string) {
    this.primeConfirmationService.confirm({
      target: event.target as EventTarget,
      message: `Dữ liệu của chủ đề '${name}' khi xóa sẽ không thể hoàn tác`,
      header: 'Xác nhận xóa',
      icon: 'pi pi-exclamation-triangle',
      acceptIcon: 'none',
      rejectIcon: 'none',
      rejectButtonStyleClass: 'p-button-text',
      accept: () => {
        this.topicService
          .deleteTopic(id)
          .pipe(takeUntil(this.destroy$))
          .subscribe({
            next: () => {
              this.commonService.toast$.next({
                type: 'success',
                message: 'Xóa chủ đề thành công',
              });
              this.forums.update((forum) =>
                forum.map((forum) => ({
                  ...forum,
                  topics: forum.topics?.filter((topic) => topic.id !== id),
                })),
              );
            },
            error: (err) => {
              this.commonService.toast$.next({
                type: 'error',
                message: getErrorMessage(err),
              });
            },
          });
      },
      reject: () => {},
    });
  }

  protected readonly policies = policies;
}
