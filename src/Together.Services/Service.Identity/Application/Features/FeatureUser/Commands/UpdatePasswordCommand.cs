using FluentValidation;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Exceptions;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.Mediator;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Domain;

namespace Service.Identity.Application.Features.FeatureUser.Commands;

public sealed class UpdatePasswordCommand : IBaseRequest
{
    public string CurrentPassword { get; set; } = default!;

    public string NewPassword { get; set; } = default!;

    public string ConfirmNewPassword { get; set; } = default!;
    
    public class Validator : AbstractValidator<UpdatePasswordCommand>
    {
        public Validator()
        {
            RuleFor(x => x.CurrentPassword).NotEmpty();
            RuleFor(x => x.NewPassword).NotEmpty();
            RuleFor(x => x.ConfirmNewPassword)
                .NotEmpty()
                .Matches(x => x.NewPassword);
        }
    }
    
    internal class Handler(IHttpContextAccessor httpContextAccessor, IdentityContext context) : 
        BaseRequestHandler<UpdatePasswordCommand>(httpContextAccessor)
    {
        protected override async Task HandleAsync(UpdatePasswordCommand request, CancellationToken ct)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == UserClaimsPrincipal.Id, ct);
            if (user is null) throw new TogetherException(ErrorCodes.User.UserNotFound);

            if (user.PasswordHash != request.CurrentPassword.ToSha256()) 
                throw new TogetherException(ErrorCodes.User.IncorrectPassword);

            user.PasswordHash = request.NewPassword.ToSha256();
            user.MarkModified();

            context.Users.Update(user);

            await context.SaveChangesAsync(ct);
            
            Message = "Done";
        }
    }
}