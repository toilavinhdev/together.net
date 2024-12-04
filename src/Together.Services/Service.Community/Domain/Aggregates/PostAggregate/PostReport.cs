using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.PostgreSQL;
using Service.Community.Domain.Enums;

namespace Service.Community.Domain.Aggregates.PostAggregate;

public class PostReport : TimeTrackingEntity
{
    public Guid PostId { get; set; }
    [ForeignKey(nameof(PostId))]
    public Post Post { get; set; } = default!;

    public Guid ReportByUserId { get; set; }

    public PostReportType Type { get; set; }

    public PostReportStatus Status { get; set; }

    [StringLength(200)]
    public string? Description { get; set; }
}