namespace Service.Community.Application.Features.FeatureReport.Responses;

public class PrefixReportResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public string Foreground { get; set; } = default!;

    public string Background { get; set; } = default!;
    
    public long TotalPost { get; set; }
    
    public double Percentage { get; set; }
}