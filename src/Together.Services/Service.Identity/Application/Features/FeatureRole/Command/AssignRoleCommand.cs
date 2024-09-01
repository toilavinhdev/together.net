using FluentValidation;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Exceptions;
using Infrastructure.SharedKernel.Mediator;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Domain;
using Service.Identity.Domain.Aggregates.UserAggregate;

namespace Service.Identity.Application.Features.FeatureRole.Command;

public sealed class AssignRoleCommand : IBaseRequest
{
    public Guid UserId { get; set; }

    public List<Guid> RoleIds { get; set; } = default!;
    
    public class Validator : AbstractValidator<AssignRoleCommand>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.RoleIds).NotEmpty();
        }
    }
    
    internal class Handler(IHttpContextAccessor httpContextAccessor, 
        IdentityContext context) :
        BaseRequestHandler<AssignRoleCommand>(httpContextAccessor)
    {
        protected override async Task HandleAsync(AssignRoleCommand request, CancellationToken ct)
        {
            var user = await context.Users
                .Include(u => u.UserRoles)!
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == request.UserId, ct);
            if (user is null) throw new TogetherException(ErrorCodes.User.UserNotFound);

            if (user.UserRoles!.Count > 0) context.UserRoles.RemoveRange(user.UserRoles);

            var userRoles = new List<UserRole>();

            foreach (var roleId in request.RoleIds)
            {
                var role = await context.Roles.FirstOrDefaultAsync(r => r.Id == roleId, ct);
                if (role is null) throw new TogetherException(ErrorCodes.Role.RoleNotFound, roleId.ToString());
                
                userRoles.Add(new UserRole
                {
                    UserId = request.UserId,
                    RoleId = roleId
                });
            }

            user.UserRoles = userRoles;
            user.MarkModified();
            
            context.Users.Update(user);
            
            await context.SaveChangesAsync(ct);
        }
    }
}