using FluentValidation;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Exceptions;
using Infrastructure.SharedKernel.Mediator;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Domain;

namespace Service.Identity.Application.Features.FeatureRole.Command;

public sealed class UpdateRoleCommand : IBaseRequest
{
    public Guid Id { get; set; }
    
    public string Name { get; set; } = default!;
    
    public string? Description { get; set; }

    public List<string> Claims { get; set; } = default!;
    
    public class Validator : AbstractValidator<UpdateRoleCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Claims).NotEmpty();
        }
    }

    internal class Handler(IHttpContextAccessor httpContextAccessor, IdentityContext context) :
        BaseRequestHandler<UpdateRoleCommand>(httpContextAccessor)
    {
        protected override async Task HandleAsync(UpdateRoleCommand request, CancellationToken ct)
        {
            var role = await context.Roles.FirstOrDefaultAsync(r => r.Id == request.Id, ct);
            if (role is null) throw new TogetherException(ErrorCodes.Role.RoleNotFound);

            role.Name = request.Name;
            role.Description = request.Description;
            role.Claims = request.Claims;
            role.MarkUserModified(UserClaimsPrincipal.Id);
            
            context.Roles.Update(role);
            
            await context.SaveChangesAsync(ct);
        }
    }
}