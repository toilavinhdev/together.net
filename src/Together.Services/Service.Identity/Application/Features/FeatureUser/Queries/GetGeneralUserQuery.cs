using Infrastructure.Redis;
using Infrastructure.SharedKernel.BusinessObjects;
using Infrastructure.SharedKernel.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Domain;

namespace Service.Identity.Application.Features.FeatureUser.Queries;

public sealed class GetGeneralUserQuery(Guid userId) : IRequest<GeneralUser?>
{
    private Guid UserId { get; set; } = userId;

    internal class Handler(IRedisService redisService, IdentityContext context) 
        : IRequestHandler<GetGeneralUserQuery, GeneralUser?>
    {
        public async Task<GeneralUser?> Handle(GetGeneralUserQuery request, CancellationToken ct)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, ct);
            if (user is null) return null;
            
            await redisService.StringSetAsync(RedisKeys.Identity<IdentityPrivilege>(user.Id), user.MapTo<IdentityPrivilege>());

            return new GeneralUser
            {
                Id = user.Id,
                SubId = user.SubId,
                UserName = user.UserName,
                IsOfficial = user.IsOfficial,
                Avatar = user.Avatar
            };
        }
    }
}