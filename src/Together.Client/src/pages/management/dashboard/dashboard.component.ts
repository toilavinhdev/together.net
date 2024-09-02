import { Component, OnInit } from '@angular/core';
import { BaseComponent } from '@/core/abstractions';
import {
  MBlocksStatisticsComponent,
  MPrefixStatisticsComponent,
  MUserStatisticsComponent,
} from '@/pages/management/dashboard/_components';

@Component({
  selector: 'together-dashboard',
  standalone: true,
  imports: [
    MPrefixStatisticsComponent,
    MUserStatisticsComponent,
    MBlocksStatisticsComponent,
  ],
  templateUrl: './dashboard.component.html',
})
export class DashboardComponent extends BaseComponent implements OnInit {
  ngOnInit() {
    this.commonService.title$.next('Thống kê diễn đàn');
  }
}
