import { Injectable } from '@angular/core';
import { BaseService } from '@/core/abstractions';
import { map, Observable } from 'rxjs';
import {
  IDailyUserReportRequest,
  IDailyUserReportResponse,
  IPrefixReportResponse,
  IStatisticsResponse,
} from '@/shared/entities/report.entities';
import { IBaseResponse } from '@/core/models';

@Injectable({
  providedIn: 'root',
})
export class ReportService extends BaseService {
  constructor() {
    super();
    this.setEndpoint('community', 'report');
  }

  statistics(metrics?: string[]): Observable<IStatisticsResponse> {
    const url = this.createUrl('/statistics');
    return this.client
      .post<IBaseResponse<IStatisticsResponse>>(url, { metrics })
      .pipe(map((response) => response.data));
  }

  getPrefixReport(): Observable<IPrefixReportResponse[]> {
    const url = this.createUrl('/prefix');
    return this.client
      .get<IBaseResponse<IPrefixReportResponse[]>>(url)
      .pipe(map((response) => response.data));
  }

  getDailyUserReport(
    params: IDailyUserReportRequest,
  ): Observable<IDailyUserReportResponse[]> {
    const url = this.createUrl('/daily-user');
    return this.client
      .get<
        IBaseResponse<IDailyUserReportResponse[]>
      >(url, { params: this.createParams(params) })
      .pipe(map((response) => response.data));
  }
}
