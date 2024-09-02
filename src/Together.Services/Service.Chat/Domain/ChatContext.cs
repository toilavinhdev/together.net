using Infrastructure.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Service.Chat.Domain.Aggregates.ConversationAggregate;
using Service.Chat.Domain.Aggregates.MessageAggregate;

namespace Service.Chat.Domain;

public sealed class ChatContext(DbContextOptions<ChatContext> options, PostgresConfig config) 
    : PostgresDbContext<ChatContext>(options, config)
{
    public DbSet<Conversation> Conversations { get; init; }
    
    public DbSet<ConversationParticipant> ConversationParticipants { get; init; }
    
    public DbSet<Message> Messages { get; init; }
}