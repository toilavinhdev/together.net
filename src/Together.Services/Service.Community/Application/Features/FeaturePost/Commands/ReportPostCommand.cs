using FluentValidation;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Exceptions;
using Infrastructure.SharedKernel.Mediator;
using Microsoft.EntityFrameworkCore;
using Service.Community.Domain;
using Service.Community.Domain.Aggregates.PostAggregate;
using Service.Community.Domain.Enums;

namespace Service.Community.Application.Features.FeaturePost.Commands;

public sealed class ReportPostCommand : IBaseRequest
{
    public Guid PostId { get; set; }

    public PostReportType Type { get; set; }

    public string? Description { get; set; }

    public class Validator : AbstractValidator<ReportPostCommand>
    {
        public Validator()
        {
            RuleFor(x => x.PostId).NotEmpty();
            RuleFor(x => x.Type).NotNull();
        }
    }

    internal class Handler(IHttpContextAccessor httpContextAccessor, CommunityContext context)
        : BaseRequestHandler<ReportPostCommand>(httpContextAccessor)
    {
        protected override async Task HandleAsync(ReportPostCommand request, CancellationToken ct)
        {
            var post = await context.Posts
                .Where(x => x.Id == request.PostId && x.Status != PostStatus.Deleted)
                .Select(x => new
                {
                    x.CreatedById
                })
                .FirstOrDefaultAsync(ct);
            if (post is null) throw new TogetherException(ErrorCodes.Post.PostNotFound);

            var report = new PostReport
            {
                Id = Guid.NewGuid(),
                ReportByUserId = UserClaimsPrincipal.Id,
                Type = request.Type,
                Status = PostReportStatus.Pending,
                Description = request.Description
            };
            report.MarkCreated();
            await context.PostReports.AddAsync(report, ct);
            await context.SaveChangesAsync(ct);
        }
    }
}