import { Routes } from '@angular/router';
import { ConversationComponent } from '@/pages/main/conversation/_layout/conversation.component';

export const routes: Routes = [
  {
    path: '',
    component: ConversationComponent,
    children: [
      {
        path: ':conversationId',
        loadComponent: () =>
          import('../message-list/message-list.component').then(
            (o) => o.MessageListComponent,
          ),
      },
    ],
  },
];
