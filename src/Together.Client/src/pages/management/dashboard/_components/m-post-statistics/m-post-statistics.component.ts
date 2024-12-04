import { Component, OnInit } from '@angular/core';
import { ContainerComponent } from '@/shared/components/elements';
import { ChartModule } from 'primeng/chart';
import { takeUntil } from 'rxjs';
import { getErrorMessage } from '@/shared/utilities';
import { ReportService } from '@/shared/services';
import { BaseComponent } from '@/core/abstractions';
import {
  IDailyPostReportResponse,
  IDailyUserReportResponse,
} from '@/shared/entities/report.entities';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'together-m-post-statistics',
  standalone: true,
  imports: [ContainerComponent, ChartModule],
  templateUrl: './m-post-statistics.component.html',
  providers: [DatePipe],
})
export class MPostStatisticsComponent extends BaseComponent implements OnInit {
  chartData: any;
  chartOptions: any;

  constructor(
    private reportService: ReportService,
    private datePipe: DatePipe,
  ) {
    super();
  }

  ngOnInit(): void {
    this.buildChart();
    this.loadStatistics();
  }

  private buildChart() {
    this.chartOptions = {
      plugins: {},
      scales: {
        y: {
          beginAtZero: true,
        },
        x: {},
      },
    };
  }

  private loadStatistics() {
    // Lấy thời gian đầu tháng
    const startOfMonth = new Date();
    startOfMonth.setDate(1);
    startOfMonth.setHours(0, 0, 0, 0);

    // Lấy thời gian cuối tháng
    const endOfMonth = new Date(
      startOfMonth.getFullYear(),
      startOfMonth.getMonth() + 1,
      0,
    );
    endOfMonth.setHours(23, 59, 59, 999);

    this.reportService
      .getDailyPostReport({
        from: startOfMonth.toISOString(),
        to: endOfMonth.toISOString(),
      })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data) => {
          this.setChartData(data);
        },
        error: (err) => {
          this.commonService.toast$.next({
            type: 'error',
            message: getErrorMessage(err),
          });
        },
      });
  }

  private setChartData(data: IDailyPostReportResponse[]) {
    this.chartData = {
      labels: data.map((daily) =>
        this.datePipe.transform(new Date(daily.day), 'dd/MM/yyyy'),
      ),
      datasets: [
        {
          label: 'Bài viết',
          data: data.map((daily) => daily.totalPost),
          borderWidth: 1,
        },
      ],
    };
  }
}
