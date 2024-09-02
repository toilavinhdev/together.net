import { Component, OnInit } from '@angular/core';
import { BaseComponent } from '@/core/abstractions';
import { IStatisticsResponse } from '@/shared/entities/report.entities';
import { ReportService } from '@/shared/services';
import { takeUntil } from 'rxjs';
import { NgClass, NgIf } from '@angular/common';
import { ShortenNumberPipe } from '@/shared/pipes';
import { getErrorMessage } from '@/shared/utilities';
import { SkeletonModule } from 'primeng/skeleton';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'together-statistic',
  standalone: true,
  imports: [NgClass, ShortenNumberPipe, SkeletonModule, NgIf, TranslateModule],
  templateUrl: './statistic.component.html',
})
export class StatisticComponent extends BaseComponent implements OnInit {
  protected readonly Array = Array;

  statistic!: IStatisticsResponse;

  loading = false;

  private metrics = [
    'totalOnlineUser',
    'totalUser',
    'totalTopic',
    'totalPost',
    'totalReply',
    'newMember',
  ];

  constructor(private reportService: ReportService) {
    super();
  }

  ngOnInit() {
    this.loading = true;
    this.reportService
      .statistics(this.metrics)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data) => {
          this.loading = false;
          this.statistic = data;
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
