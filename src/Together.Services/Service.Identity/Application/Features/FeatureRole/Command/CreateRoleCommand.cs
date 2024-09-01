using FluentValidation;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Exceptions;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.Mediator;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Application.Features.FeatureRole.Responses;
using Service.Identity.Domain;
using Service.Identity.Domain.Aggregates.RoleAggregate;

namespace Service.Identity.Application.Features.FeatureRole.Command;

public sealed class CreateRoleCommand : IBaseRequest<CreateRoleResponse>
{
    public string Name { get; set; } = default!;
    
    public string? Description { get; set; }

    public List<string> Claims { get; set; } = default!;
    
    public class Validator : AbstractValidator<CreateRoleCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Claims).NotEmpty();
        }
    }
    
    internal class Handler(IHttpContextAccessor httpContextAccessor, IdentityContext context) : 
        BaseRequestHandler<CreateRoleCommand, CreateRoleResponse>(httpContextAccessor)
    {
        protected override async Task<CreateRoleResponse> HandleAsync(CreateRoleCommand request, CancellationToken ct)
        {
            if (await context.Roles.AnyAsync(r => r.Name.ToUpper() == request.Name, ct))
                throw new TogetherException(ErrorCodes.Role.RoleNameAlreadyExists);

            var role = new Role
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                IsDefault = false,
                Description = request.Description,
                Claims = request.Claims
            };
            role.MarkUserCreated(UserClaimsPrincipal.Id);
            
            await context.Roles.AddAsync(role, ct);
            
            await context.SaveChangesAsync(ct);

            return role.MapTo<CreateRoleResponse>();
        }
    }
}