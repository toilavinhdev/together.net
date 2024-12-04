namespace Service.Community.Application.Features.FeatureReport.Responses;

public class DailyPostReportResponse
{
    public DateTimeOffset Day { get; set; }

    public long TotalPost { get; set; }
}