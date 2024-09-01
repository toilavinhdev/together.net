using System.Linq.Expressions;
using FluentValidation;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.Mediator;
using Infrastructure.SharedKernel.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Application.Features.FeatureRole.Responses;
using Service.Identity.Domain;
using Service.Identity.Domain.Aggregates.RoleAggregate;

namespace Service.Identity.Application.Features.FeatureRole.Queries;

public sealed class ListRoleQuery : IBaseRequest<ListRoleResponse>, IPaginationRequest
{
    public int PageIndex { get; set; }
    
    public int PageSize { get; set; }
    
    public Guid? UserId { get; set; }
    
    public class Validator : AbstractValidator<ListRoleQuery>
    {
        public Validator()
        {
            Include(new PaginationValidator());
        }
    }
    
    internal class Handler(IHttpContextAccessor httpContextAccessor, IdentityContext context) :
        BaseRequestHandler<ListRoleQuery, ListRoleResponse>(httpContextAccessor)
    {
        protected override async Task<ListRoleResponse> HandleAsync(ListRoleQuery request, CancellationToken ct)
        {
            Expression<Func<Role, bool>> whereExpression = role => true;

            if (request.UserId is not null)
            {
                whereExpression = whereExpression.And(r => r.UserRoles!.Any(ur => ur.UserId == request.UserId));
            }

            var query = context.Roles
                .Where(whereExpression)
                .AsQueryable();

            var totalRecord = await query.LongCountAsync(ct);

            var data = await query
                .Select(role => role.MapTo<RoleViewModel>())
                .ToListAsync(ct);

            return new ListRoleResponse
            {
                Pagination = new Pagination(request.PageIndex, request.PageSize, totalRecord),
                Result = data
            };
        }
    }
}