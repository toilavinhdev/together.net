namespace Service.Identity.Application.Features.FeatureReport.Responses;

public class DailyUserReportResponse
{
    public DateTimeOffset Day { get; set; }
    
    public long TotalNewUser { get; set; }
    
    public long TotalUser { get; set; }
}