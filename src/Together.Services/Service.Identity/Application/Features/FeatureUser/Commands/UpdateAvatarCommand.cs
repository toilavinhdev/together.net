using FluentValidation;
using Infrastructure.Redis;
using Infrastructure.SharedKernel.BusinessObjects;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Exceptions;
using Infrastructure.SharedKernel.Mediator;
using Infrastructure.SharedKernel.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Domain;

namespace Service.Identity.Application.Features.FeatureUser.Commands;

public sealed class UpdateAvatarCommand : IBaseRequest
{
    public string Url { get; set; } = default!;
    
    public class Validator : AbstractValidator<UpdateAvatarCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Url).NotEmpty();
        }
    }
    
    public class Handler(IHttpContextAccessor httpContextAccessor,
        IdentityContext context,
        IRedisService redisService) 
        : BaseRequestHandler<UpdateAvatarCommand>(httpContextAccessor)
    {
        protected override async Task HandleAsync(UpdateAvatarCommand request, CancellationToken ct)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == UserClaimsPrincipal.Id, ct);
            if (user is null) throw new TogetherException(ErrorCodes.User.UserNotFound);

            user.Avatar = request.Url;
            user.MarkModified();

            context.Users.Update(user);

            await context.SaveChangesAsync(ct);

            var cachedUser = await redisService.StringGetAsync<IdentityPrivilege>(
                RedisKeys.Identity<IdentityPrivilege>(user.Id));
            if (cachedUser is not null)
            {
                cachedUser.Avatar = user.Avatar;
                await redisService.StringSetAsync(RedisKeys.Identity<IdentityPrivilege>(cachedUser.Id), cachedUser);
            }
        }
    }
}