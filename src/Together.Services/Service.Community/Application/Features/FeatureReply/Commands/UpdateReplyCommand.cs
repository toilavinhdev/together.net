using FluentValidation;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Exceptions;
using Infrastructure.SharedKernel.Mediator;
using Microsoft.EntityFrameworkCore;
using Service.Community.Domain;

namespace Service.Community.Application.Features.FeatureReply.Commands;

public sealed class UpdateReplyCommand : IBaseRequest
{
    public Guid Id { get; set; }
    
    public string Body { get; set; } = default!;
    
    public class Validator : AbstractValidator<UpdateReplyCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
    
    internal class Handler(IHttpContextAccessor httpContextAccessor, CommunityContext context)
        : BaseRequestHandler<UpdateReplyCommand>(httpContextAccessor)
    {
        protected override async Task HandleAsync(UpdateReplyCommand request, CancellationToken ct)
        {
            var reply = await context.Replies.FirstOrDefaultAsync(r => r.Id == request.Id, ct);
            if (reply is null) throw new TogetherException(ErrorCodes.Reply.ReplyNotFound);

            reply.Body = request.Body;
            reply.MarkUserCreated(UserClaimsPrincipal.Id);

            context.Replies.Update(reply);

            await context.SaveChangesAsync(ct);

            Message = "Updated";
        }
    }
}