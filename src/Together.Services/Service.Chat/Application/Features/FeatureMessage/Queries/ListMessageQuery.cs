using System.Linq.Expressions;
using FluentValidation;
using Grpc.Net.Client;
using Infrastructure.Redis;
using Infrastructure.SharedKernel.BusinessObjects;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Exceptions;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.Mediator;
using Infrastructure.SharedKernel.Protos.Identity;
using Infrastructure.SharedKernel.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Service.Chat.Application.Features.FeatureMessage.Responses;
using Service.Chat.Domain;
using Service.Chat.Domain.Aggregates.MessageAggregate;
using Service.Chat.Domain.Enums;

namespace Service.Chat.Application.Features.FeatureMessage.Queries;

public sealed class ListMessageQuery : IBaseRequest<ListMessageResponse>, IPaginationRequest
{
    public int PageIndex { get; set; }
    
    public int PageSize { get; set; }
    
    public Guid ConversationId { get; set; }
    
    public class Validator : AbstractValidator<ListMessageQuery>
    {
        public Validator()
        {
            Include(new PaginationValidator());
            RuleFor(x => x.ConversationId).NotEmpty();
        }
    }

    internal class Handler(IHttpContextAccessor httpContextAccessor, 
        ChatContext context, 
        AppSettings appSettings,
        IRedisService redisService)
        : BaseRequestHandler<ListMessageQuery, ListMessageResponse>(httpContextAccessor)
    {
        protected override async Task<ListMessageResponse> HandleAsync(ListMessageQuery request, CancellationToken ct)
        {
            var conversation= await context.Conversations.FirstOrDefaultAsync(c => c.Id == request.ConversationId, ct);
            if (conversation is null) throw new TogetherException(ErrorCodes.Conversation.ConversationNotFound);
            
            var extra = new Dictionary<string, object>();
            extra.Add("conversationType", conversation.Type);

            Expression<Func<Message, bool>> whereExpression = m => true;
            whereExpression = whereExpression.And(m => m.DeletedAt == null);
            whereExpression = whereExpression.And(m => m.ConversationId == request.ConversationId);

            var queryable = context.Messages
                .Where(whereExpression)
                .AsQueryable();

            var totalRecord = await queryable.LongCountAsync(ct);

            var messages = await queryable
                .OrderByDescending(m => m.CreatedAt)
                .Paging(request.PageIndex, request.PageSize)
                .Select(m => new MessageViewModel
                {
                    Id = m.Id,
                    SubId = m.SubId,
                    ConversationId = m.ConversationId,
                    Text = m.Text,
                    CreatedAt = m.CreatedAt,
                    CreatedById = m.CreatedById,
                    CreatedByUserName = string.Empty, // must attach
                    CreatedByAvatar = string.Empty // must attach
                })
                .ToListAsync(ct);
            
            // Attach user info
            foreach (var message in messages)
            {
                var cachedUser = await redisService.StringGetAsync<IdentityPrivilege>(
                    RedisKeys.Identity<IdentityPrivilege>(message.CreatedById));
                if (cachedUser is not null)
                {
                    message.CreatedByUserName = cachedUser.UserName;
                    message.CreatedByAvatar = cachedUser.Avatar;
                }
                else
                {
                    var channelOptions = new GrpcChannelOptions { HttpHandler = new HttpClientHandler { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator } };
                    var channel = GrpcChannel.ForAddress(appSettings.GrpcEndpoints.ServiceIdentity, channelOptions);
                    var client = new UserGrpcServerService.UserGrpcServerServiceClient(channel);
                    var response = await client.GetGeneralUserGrpcAsync(
                        new GetGeneralUserGrpcRequest
                        {
                            Id = message.CreatedById.ToString()
                        }, cancellationToken: ct);
                    message.CreatedByUserName = response.Data.UserName;
                    message.CreatedByAvatar = response.Data.Avatar;
                }
            }

            switch (conversation.Type)
            {
                case ConversationType.Group:
                    extra.Add("conversationName", conversation.Name!);
                    break;
                case ConversationType.Private:
                    var receiver = await context.ConversationParticipants
                        .FirstOrDefaultAsync(cp => 
                            cp.ConversationId == request.ConversationId && 
                            cp.ParticipantId != UserClaimsPrincipal.Id, 
                            ct);
                    if (receiver is null) throw new NullReferenceException();
                    var cachedUser = await redisService.StringGetAsync<IdentityPrivilege>(
                        RedisKeys.Identity<IdentityPrivilege>(receiver.ParticipantId));
                    if (cachedUser is null) break;
                    extra.Add("conversationName", cachedUser!.UserName);
                    extra.Add("conversationImage", cachedUser.Avatar ?? string.Empty);
                    extra.Add("userId", cachedUser.Id);
                    var userOnline = await redisService.SetContainsAsync(RedisKeys.SocketOnlineUsers(), cachedUser.Id.ToString());
                    extra.Add("userOnline", userOnline);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new ListMessageResponse
            {
                Pagination = new Pagination(request.PageIndex, request.PageSize, totalRecord),
                Result = messages,
                Extra = extra
            };
        }
    }
}