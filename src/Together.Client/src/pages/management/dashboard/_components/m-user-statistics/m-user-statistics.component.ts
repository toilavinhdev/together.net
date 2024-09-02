import { Component, computed, OnInit, signal } from '@angular/core';
import { ChartModule } from 'primeng/chart';
import { ContainerComponent } from '@/shared/components/elements';
import { BaseComponent } from '@/core/abstractions';
import { IDailyUserReportResponse } from '@/shared/entities/report.entities';
import { ReportService } from '@/shared/services';
import { takeUntil } from 'rxjs';
import { getErrorMessage } from '@/shared/utilities';
import { DatePipe } from '@angular/common';
import { DropdownModule } from 'primeng/dropdown';
import { FormsModule } from '@angular/forms';

enum EDailyUserReportType {
  Last7Days = 0,
  Last14Days,
  Last30Days,
}

const dailyUserReportOptions = [
  {
    label: '7 ngày gần đây',
    value: EDailyUserReportType.Last7Days,
  },
  {
    label: '14 ngày gần đây',
    value: EDailyUserReportType.Last14Days,
  },
  {
    label: '30 ngày gần đây',
    value: EDailyUserReportType.Last30Days,
  },
];

@Component({
  selector: 'together-m-user-statistics',
  standalone: true,
  imports: [ChartModule, ContainerComponent, DropdownModule, FormsModule],
  templateUrl: './m-user-statistics.component.html',
  providers: [DatePipe],
})
export class MUserStatisticsComponent extends BaseComponent implements OnInit {
  protected readonly dailyUserReportOptions = dailyUserReportOptions;

  protected readonly EDailyUserReportType = EDailyUserReportType;

  offset = signal<number>(6);

  endTime = signal<Date>(new Date());

  startTime = computed(() => {
    const agoTime = new Date(this.endTime());
    agoTime.setDate(this.endTime().getDate() - this.offset());
    return agoTime;
  });

  chartData: any;

  chartOptions: any;

  selectedDailyUserReportType: EDailyUserReportType =
    EDailyUserReportType.Last7Days;

  constructor(
    private reportService: ReportService,
    private datePipe: DatePipe,
  ) {
    super();
  }

  ngOnInit() {
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

  onSelectedDailyUserReportTypeChange(type: EDailyUserReportType) {
    switch (type) {
      case EDailyUserReportType.Last7Days:
        this.offset.set(6);
        break;
      case EDailyUserReportType.Last14Days:
        this.offset.set(13);
        break;
      case EDailyUserReportType.Last30Days:
        this.offset.set(29);
        break;
    }
    this.loadStatistics();
  }

  private loadStatistics() {
    this.reportService
      .getDailyUserReport({
        from: this.startTime().toISOString(),
        to: this.endTime().toISOString(),
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

  private setChartData(data: IDailyUserReportResponse[]) {
    this.chartData = {
      labels: data.map((daily) =>
        this.datePipe.transform(new Date(daily.day), 'dd/MM/yyyy'),
      ),
      datasets: [
        {
          label: 'Thành viên',
          data: data.map((daily) => daily.totalUser),
          borderWidth: 1,
        },
      ],
    };
  }
}
