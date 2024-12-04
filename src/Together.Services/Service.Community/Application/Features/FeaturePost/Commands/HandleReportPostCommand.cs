using FluentValidation;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Exceptions;
using Infrastructure.SharedKernel.Mediator;
using Microsoft.EntityFrameworkCore;
using Service.Community.Domain;
using Service.Community.Domain.Enums;

namespace Service.Community.Application.Features.FeaturePost.Commands;

public class HandleReportPostCommand : IBaseRequest
{
    public Guid PostReportId { get; init; }

    public bool IsApprove { get; init; }

    public class Validator : AbstractValidator<HandleReportPostCommand>
    {
        public Validator()
        {
            RuleFor(x => x.PostReportId).NotEmpty();
            RuleFor(x => x.IsApprove).NotEmpty();
        }
    }

    internal class Handler(IHttpContextAccessor httpContextAccessor, CommunityContext context)
        : BaseRequestHandler<HandleReportPostCommand>(httpContextAccessor)
    {
        protected override async Task HandleAsync(HandleReportPostCommand request, CancellationToken ct)
        {
            var report = await context.PostReports
                .FirstOrDefaultAsync(x => x.Id == request.PostReportId, ct);
            if (report is null) throw new TogetherException(ErrorCodes.Post.PostReportNotFound);

            var post = await context.Posts
                .FirstOrDefaultAsync(x => x.Id == report.PostId || x.Status != PostStatus.Deleted, ct);
            if (post is null) throw new TogetherException(ErrorCodes.Post.PostNotFound);

            report.Status = request.IsApprove ? PostReportStatus.Approved : PostReportStatus.Rejected;
            report.MarkModified();
            context.PostReports.Update(report);

            if (report.Status == PostReportStatus.Approved)
            {
                post.Status = PostStatus.Deleted;
                context.Posts.Update(post);
            }

            await context.SaveChangesAsync(ct);
        }
    }
}