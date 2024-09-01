using FluentValidation;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Enums;
using Infrastructure.SharedKernel.Exceptions;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.Mediator;
using Infrastructure.SharedKernel.Utilities;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Application.Features.FeatureUser.Responses;
using Service.Identity.Domain;
using Service.Identity.Domain.Aggregates.UserAggregate;

namespace Service.Identity.Application.Features.FeatureUser.Commands;

public sealed class SignUpCommand : IBaseRequest<SignUpResponse>
{
    public string UserName { get; set; } = default!;

    public string Email { get; set; } = default!;
    
    public Gender Gender { get; set; }
    
    public string Password { get; set; } = default!;
    
    public class Validator : AbstractValidator<SignUpCommand>
    {
        public Validator()
        {
            RuleFor(x => x.UserName).NotEmpty().Matches(RegexUtils.UserNameRegex);
            RuleFor(x => x.Email).NotEmpty().Matches(RegexUtils.EmailRegex);
            RuleFor(x => x.Gender).NotNull();
            RuleFor(x => x.Password).NotNull();
        }
    }
    
    internal class Handler(IHttpContextAccessor httpContextAccessor, IdentityContext context) 
        : BaseRequestHandler<SignUpCommand, SignUpResponse>(httpContextAccessor)
    {
        protected override async Task<SignUpResponse> HandleAsync(SignUpCommand request, CancellationToken ct)
        {
            var userNameExists = await context.Users.AnyAsync(x => x.UserName == request.UserName, ct);
            if (userNameExists) throw new TogetherException(ErrorCodes.User.UserNameAlreadyExists);
            
            var emailExists = await context.Users.AnyAsync(x => x.Email == request.Email, ct);
            if (emailExists) throw new TogetherException(ErrorCodes.User.UserEmailAlreadyExists);
            
            var memberRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Member", ct);
            if (memberRole is null) throw new ApplicationException("Member role has not been initialized");

            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = request.UserName,
                Email = request.Email,
                IsOfficial = false,
                Status = UserStatus.Active,
                Gender = request.Gender,
                PasswordHash = request.Password.ToSha256(),
                UserRoles = [new UserRole{ RoleId = memberRole.Id }]
            };
            
            user.MarkCreated();

            await context.Users.AddAsync(user, ct);
            
            await context.SaveChangesAsync(ct);

            Message = "Sign up account successfully";

            return user.MapTo<SignUpResponse>();
        }
    }
}