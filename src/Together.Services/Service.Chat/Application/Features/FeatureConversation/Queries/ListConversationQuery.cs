using System.Linq.Expressions;
using FluentValidation;
using Infrastructure.Redis;
using Infrastructure.SharedKernel.BusinessObjects;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.Mediator;
using Infrastructure.SharedKernel.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Service.Chat.Application.Features.FeatureConversation.Expresions;
using Service.Chat.Application.Features.FeatureConversation.Responses;
using Service.Chat.Domain;
using Service.Chat.Domain.Aggregates.ConversationAggregate;
using Service.Chat.Domain.Enums;

namespace Service.Chat.Application.Features.FeatureConversation.Queries;

public sealed class ListConversationQuery : IBaseRequest<ListConversationResponse>, IPaginationRequest
{
    public int PageIndex { get; set; }
    
    public int PageSize { get; set; }
    
    public class Validator : AbstractValidator<ListConversationQuery>
    {
        public Validator()
        {
            Include(new PaginationValidator());
        }
    }
    
    internal class Handler(IHttpContextAccessor httpContextAccessor, ChatContext context, IRedisService redisService) 
        : BaseRequestHandler<ListConversationQuery, ListConversationResponse>(httpContextAccessor)
    {
        protected override async Task<ListConversationResponse> HandleAsync(ListConversationQuery request, CancellationToken ct)
        {
            Expression<Func<Conversation, bool>> whereExpression = c => true;
            whereExpression = whereExpression.And(c => c.ConversationParticipants.Any(cp => cp.ParticipantId == UserClaimsPrincipal.Id));
            
            var queryable = context.Conversations.AsQueryable();
            
            var totalRecord = await queryable.LongCountAsync(ct);
            
            var conversations = await queryable
                .Include(c => c.ConversationParticipants)
                .Include(c => c.Messages)!
                .Where(whereExpression)
                .OrderByDescending(c => c.Messages!.Max(m => m.CreatedAt))
                .Paging(request.PageIndex, request.PageSize)
                .Select(ConversationExpressions.ConversationViewModelSelector(UserClaimsPrincipal))
                .ToListAsync(ct);

            foreach (var conversation in conversations)
            {
                if (conversation.Type == ConversationType.Private)
                {
                    var partner = await redisService
                        .StringGetAsync<IdentityPrivilege>(
                            RedisKeys.Identity<IdentityPrivilege>(conversation.Name!));
                    if (partner is null) continue;
                    conversation.Name = partner.UserName;
                    conversation.Image = partner.Avatar;
                    if (conversation.LastMessageAt is null) continue;
                    if (conversation.LastMessageByUserId == partner.Id)
                    {
                        conversation.LastMessageByUserName = partner.UserName;
                    }
                    else
                    {
                        var userNameSendLastMessage = await redisService
                            .StringGetAsync<IdentityPrivilege>(
                                RedisKeys.Identity<IdentityPrivilege>(conversation.LastMessageByUserId!));
                        if (userNameSendLastMessage is null) continue;
                        conversation.LastMessageByUserName = userNameSendLastMessage.UserName;
                    }
                }
            }
            
            return new ListConversationResponse
            {
                Pagination = new Pagination(request.PageIndex, request.PageSize, totalRecord),
                Result = conversations
            };
        }
    }
}