import { Routes } from '@angular/router';
import { ForumComponent } from './forum.component';

export const routes: Routes = [
  {
    path: '',
    component: ForumComponent,
    children: [
      {
        path: '',
        loadComponent: () =>
          import('../topic-list/topic-list.component').then(
            (o) => o.TopicListComponent,
          ),
      },
      {
        path: 'topics/:topicId',
        loadComponent: () =>
          import('../post-list/post-list.component').then(
            (o) => o.PostListComponent,
          ),
      },
      {
        path: 'topics/:topicId/create-post',
        loadComponent: () =>
          import('../post-edit/post-edit.component').then(
            (o) => o.PostEditComponent,
          ),
      },
      {
        path: 'posts/:postId',
        loadComponent: () =>
          import('../post-detail/post-detail.component').then(
            (o) => o.PostDetailComponent,
          ),
      },
      {
        path: 'posts/:postId/update-post',
        loadComponent: () =>
          import('../post-edit/post-edit.component').then(
            (o) => o.PostEditComponent,
          ),
      },
      {
        path: 'search',
        loadComponent: () =>
          import('../search/search.component').then((o) => o.SearchComponent),
      },
    ],
  },
];
