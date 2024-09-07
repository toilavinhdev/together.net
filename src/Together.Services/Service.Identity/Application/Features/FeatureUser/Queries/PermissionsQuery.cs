using Infrastructure.SharedKernel.Mediator;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Domain;

namespace Service.Identity.Application.Features.FeatureUser.Queries;

public sealed class PermissionsQuery : IBaseRequest<List<string>>
{
    internal class Handler(IHttpContextAccessor httpContextAccessor, IdentityContext context) 
        : BaseRequestHandler<PermissionsQuery, List<string>>(httpContextAccessor)
    {
        protected override async Task<List<string>> HandleAsync(PermissionsQuery request, CancellationToken ct)
        {
            var roles = await context.UserRoles
                .Include(r => r.Role)
                .Where(ur => ur.UserId == UserClaimsPrincipal.Id)
                .Select(ur => ur.Role)
                .ToListAsync(ct);
            
            var permissions = roles
                .SelectMany(role => role.Claims)
                .Distinct()
                .OrderBy(claim => claim)
                .ToList();
            
            return permissions;
        }
    }
}