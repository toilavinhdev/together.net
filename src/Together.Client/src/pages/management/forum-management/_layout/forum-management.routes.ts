import { Routes } from '@angular/router';
import { ForumManagementComponent } from '@/pages/management/forum-management/_layout/forum-management.component';

export const routes: Routes = [
  {
    path: '',
    component: ForumManagementComponent,
    children: [
      {
        path: '',
        loadComponent: () =>
          import('../m-forum-list/m-forum-list.component').then(
            (o) => o.MForumListComponent,
          ),
      },
      {
        path: 'create',
        loadComponent: () =>
          import('../m-forum-detail/m-forum-detail.component').then(
            (o) => o.MForumDetailComponent,
          ),
      },
      {
        path: ':forumId/update',
        loadComponent: () =>
          import('../m-forum-detail/m-forum-detail.component').then(
            (o) => o.MForumDetailComponent,
          ),
      },
      {
        path: 'topic/create',
        loadComponent: () =>
          import('../m-topic-detail/m-topic-detail.component').then(
            (o) => o.MTopicDetailComponent,
          ),
      },
      {
        path: 'topic/:topicId/update',
        loadComponent: () =>
          import('../m-topic-detail/m-topic-detail.component').then(
            (o) => o.MTopicDetailComponent,
          ),
      },
    ],
  },
];
