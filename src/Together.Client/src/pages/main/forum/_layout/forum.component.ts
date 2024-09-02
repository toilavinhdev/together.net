import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import {
  LatestPostsComponent,
  StatisticComponent,
} from '@/pages/main/forum/_layout/components';

@Component({
  selector: 'together-forum',
  standalone: true,
  imports: [RouterOutlet, LatestPostsComponent, StatisticComponent],
  templateUrl: './forum.component.html',
})
export class ForumComponent {}
