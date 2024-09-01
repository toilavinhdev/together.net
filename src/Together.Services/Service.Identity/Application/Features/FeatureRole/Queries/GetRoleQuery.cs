using FluentValidation;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Exceptions;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.Mediator;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Application.Features.FeatureRole.Responses;
using Service.Identity.Domain;

namespace Service.Identity.Application.Features.FeatureRole.Queries;

public sealed class GetRoleQuery(Guid id) : IBaseRequest<GetRoleResponse>
{
    private Guid Id { get; set; } = id;
    
    public class Validator : AbstractValidator<GetRoleQuery>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
    
    internal class Handler(IHttpContextAccessor httpContextAccessor, IdentityContext context) 
        : BaseRequestHandler<GetRoleQuery, GetRoleResponse>(httpContextAccessor)
    {
        protected override async Task<GetRoleResponse> HandleAsync(GetRoleQuery request, CancellationToken ct)
        {
            var role = await context.Roles
                .Where(r => r.Id == request.Id)
                .Select(r => r.MapTo<GetRoleResponse>())
                .FirstOrDefaultAsync(ct);

            if (role is null) throw new TogetherException(ErrorCodes.Role.RoleNotFound);

            return role;
        }
    }
}