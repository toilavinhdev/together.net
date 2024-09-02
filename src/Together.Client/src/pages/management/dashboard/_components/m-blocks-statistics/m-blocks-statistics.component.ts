import { Component, OnInit } from '@angular/core';
import { BaseComponent } from '@/core/abstractions';
import { ReportService } from '@/shared/services';
import { takeUntil } from 'rxjs';
import { getErrorMessage } from '@/shared/utilities';
import { NgTemplateOutlet } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { ShortenNumberPipe } from '@/shared/pipes';
import { IStatisticsResponse } from '@/shared/entities/report.entities';

@Component({
  selector: 'together-m-blocks-statistics',
  standalone: true,
  imports: [NgTemplateOutlet, TranslateModule, ShortenNumberPipe],
  templateUrl: './m-blocks-statistics.component.html',
})
export class MBlocksStatisticsComponent
  extends BaseComponent
  implements OnInit
{
  private metrics = [
    'totalUser',
    'totalUserToday',
    'totalPost',
    'totalPostToday',
    'totalReply',
    'totalReplyToday',
  ];

  statistics: any;

  constructor(private reportService: ReportService) {
    super();
  }

  ngOnInit() {
    this.commonService.title$.next('Thống kê diễn đàn');
    this.loadStatistic();
  }

  private loadStatistic() {
    this.reportService
      .statistics(this.metrics)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data) => {
          this.statistics = data;
        },
        error: (err) => {
          this.commonService.toast$.next({
            type: 'error',
            message: getErrorMessage(err),
          });
        },
      });
  }
}
