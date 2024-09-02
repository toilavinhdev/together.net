import { Component, OnInit } from '@angular/core';
import { BaseComponent } from '@/core/abstractions';
import { ReportService } from '@/shared/services';
import { takeUntil } from 'rxjs';
import {
  ContainerComponent,
  PrefixComponent,
} from '@/shared/components/elements';
import { IPrefixReportResponse } from '@/shared/entities/report.entities';
import { ChartModule } from 'primeng/chart';

@Component({
  selector: 'together-m-prefix-statistics',
  standalone: true,
  imports: [ContainerComponent, PrefixComponent, ChartModule],
  templateUrl: './m-prefix-statistics.component.html',
})
export class MPrefixStatisticsComponent
  extends BaseComponent
  implements OnInit
{
  loading = false;

  chartData: any;

  charOptions: any;

  constructor(private reportService: ReportService) {
    super();
  }

  ngOnInit() {
    this.loadReport();
  }

  private loadReport() {
    this.reportService
      .getPrefixReport()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data) => {
          this.buildChart(data);
        },
      });
  }

  private buildChart(data: IPrefixReportResponse[]) {
    this.chartData = {
      labels: data.map((prefix) => prefix.name),
      datasets: [
        {
          data: data.map((prefix) => prefix.totalPost),
          backgroundColor: data.map((prefix) => prefix.background),
          hoverOffset: 10,
        },
      ],
    };

    this.charOptions = {
      plugins: {
        legend: {
          labels: {
            usePointStyle: true,
          },
        },
      },
    };
  }
}
