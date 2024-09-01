using FluentValidation;
using Infrastructure.Redis;
using Infrastructure.SharedKernel.BusinessObjects;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Enums;
using Infrastructure.SharedKernel.Exceptions;
using Infrastructure.SharedKernel.Mediator;
using Infrastructure.SharedKernel.Utilities;
using Infrastructure.SharedKernel.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Domain;

namespace Service.Identity.Application.Features.FeatureUser.Commands;

public sealed class UpdateProfileCommand : IBaseRequest
{
    public string UserName { get; set; } = default!;
    
    public Gender Gender { get; set; }
    
    public string? FullName { get; set; }
    
    public string? Biography { get; set; }
    
    public class Validator : AbstractValidator<UpdateProfileCommand>
    {
        public Validator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty()
                .Matches(RegexUtils.UserNameRegex);
            RuleFor(x => x.Gender).NotNull();
        }
    }

    internal class Handler(IHttpContextAccessor httpContextAccessor, IdentityContext context, IRedisService redisService) :
        BaseRequestHandler<UpdateProfileCommand>(httpContextAccessor)
    {
        protected override async Task HandleAsync(UpdateProfileCommand request, CancellationToken ct)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == UserClaimsPrincipal.Id, ct);
            if (user is null) throw new TogetherException(ErrorCodes.User.UserNotFound);

            if (request.UserName != user.UserName && await context.Users.AnyAsync(u => u.UserName == request.UserName, ct))
                throw new TogetherException(ErrorCodes.User.UserNameAlreadyExists);
            
            user.UserName = request.UserName;
            user.Gender = request.Gender;
            user.FullName = request.FullName;
            user.Biography = request.Biography;
            user.MarkModified();

            context.Users.Update(user);

            await context.SaveChangesAsync(ct);

            // Update cache
            var cachedIdentityPrivilege = await redisService.StringGetAsync<IdentityPrivilege>(
                RedisKeys.Identity<IdentityPrivilege>(user.Id));
            if (cachedIdentityPrivilege is not null)
            {
                cachedIdentityPrivilege.UserName = user.UserName;
                cachedIdentityPrivilege.Gender = user.Gender;
                cachedIdentityPrivilege.FullName = user.FullName;
                cachedIdentityPrivilege.Biography = user.Biography;
                cachedIdentityPrivilege.CreatedAt = user.CreatedAt;
                await redisService.StringSetAsync(
                    RedisKeys.Identity<IdentityPrivilege>(user.Id), 
                    cachedIdentityPrivilege);
            }

            Message = "Done";
        }
    }
}