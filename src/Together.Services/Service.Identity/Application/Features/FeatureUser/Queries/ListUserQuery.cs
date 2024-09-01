using System.Linq.Expressions;
using FluentValidation;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.Mediator;
using Infrastructure.SharedKernel.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Application.Features.FeatureUser.Responses;
using Service.Identity.Domain;
using Service.Identity.Domain.Aggregates.UserAggregate;

namespace Service.Identity.Application.Features.FeatureUser.Queries;

public sealed class ListUserQuery : IBaseRequest<ListUserResponse>, IPaginationRequest
{
    public int PageIndex { get; set; }
    
    public int PageSize { get; set; }
    
    public string? Search { get; set; }
    
    public Guid? RoleId { get; set; }
    
    public class Validator : AbstractValidator<ListUserQuery>
    {
        public Validator()
        {
            Include(new PaginationValidator());
        }
    }
    
    internal class Handler(IHttpContextAccessor httpContextAccessor, IdentityContext context) 
        : BaseRequestHandler<ListUserQuery, ListUserResponse>(httpContextAccessor)
    {
        protected override async Task<ListUserResponse> HandleAsync(ListUserQuery request, CancellationToken ct)
        {
            var queryable = context.Users.AsQueryable();
            
            Expression<Func<User, bool>> whereExpression = u => true;

            if (!string.IsNullOrEmpty(request.Search))
            {
                whereExpression = whereExpression.And(u => u.UserName.Contains(request.Search.ToLower()));
            }

            if (request.RoleId is not null)
            {
                queryable = queryable.Include(u => u.UserRoles);
                whereExpression = whereExpression.And(u => u.UserRoles!.Any(ur => ur.RoleId == request.RoleId));
            }

            queryable = queryable.Where(whereExpression);

            var totalRecord = await queryable.LongCountAsync(ct);

            var users = await queryable
                .Paging(request.PageIndex, request.PageSize)
                .Select(u => u.MapTo<UserViewModel>())
                .ToListAsync(ct);

            return new ListUserResponse
            {
                Pagination = new Pagination(request.PageIndex, request.PageSize, totalRecord),
                Result = users
            };
        }
    }
}