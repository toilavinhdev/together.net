import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import {
  LatestPostsComponent, PostRankComponent,
  StatisticComponent,
} from '@/pages/main/forum/_layout/components';

@Component({
  selector: 'together-forum',
  standalone: true,
  imports: [
    RouterOutlet,
    LatestPostsComponent,
    StatisticComponent,
    PostRankComponent,
  ],
  templateUrl: './forum.component.html',
})
export class ForumComponent {}
