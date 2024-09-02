export interface IStatisticsResponse {
  totalUser?: number;
  totalTopic?: number;
  totalPost?: number;
  totalReply?: number;
  totalOnlineUser?: number;
  newMember?: number;
}

export interface IPrefixReportResponse {
  id: string;
  name: string;
  foreground: string;
  background: string;
  totalPost: number;
  percentage: number;
}

export interface IDailyUserReportRequest {
  from: string;
  to: string;
}

export interface IDailyUserReportResponse {
  day: string;
  totalNewUser: number;
  totalUser: number;
}
