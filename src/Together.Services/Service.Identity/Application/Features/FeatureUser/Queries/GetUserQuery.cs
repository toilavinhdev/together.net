using FluentValidation;
using Infrastructure.Redis;
using Infrastructure.SharedKernel.BusinessObjects;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Exceptions;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.Mediator;
using Infrastructure.SharedKernel.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Application.Features.FeatureUser.Responses;
using Service.Identity.Domain;

namespace Service.Identity.Application.Features.FeatureUser.Queries;

public sealed class GetUserQuery(Guid userId) : IBaseRequest<GetUserResponse>
{
    private Guid UserId { get; set; } = userId;
    
    public class Validator : AbstractValidator<GetUserQuery>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
        }
    }

    internal class Handler(IHttpContextAccessor httpContextAccessor, IRedisService redisService, IdentityContext context) :
        BaseRequestHandler<GetUserQuery, GetUserResponse>(httpContextAccessor)
    {
        protected override async Task<GetUserResponse> HandleAsync(GetUserQuery request, CancellationToken ct)
        {
            var cachedUser = await redisService.StringGetAsync<IdentityPrivilege>(
                RedisKeys.Identity<IdentityPrivilege>(request.UserId));
        
            if (cachedUser is null)
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, ct);
                if (user is null) throw new TogetherException(ErrorCodes.User.UserNotFound);
                cachedUser = user.MapTo<IdentityPrivilege>();
                await redisService.StringSetAsync(RedisKeys.Identity<IdentityPrivilege>(user.Id), cachedUser);
            }

            return new GetUserResponse
            {
                Id = cachedUser.Id,
                SubId = cachedUser.SubId,
                UserName = cachedUser.UserName,
                IsOfficial = cachedUser.IsOfficial,
                Gender = cachedUser.Gender,
                FullName = cachedUser.FullName,
                Biography = cachedUser.Biography,
                Avatar = cachedUser.Avatar,
                PostCount = 0,
                ReplyCount = 0,
                CreatedAt = cachedUser.CreatedAt
            };
        }
    }
}