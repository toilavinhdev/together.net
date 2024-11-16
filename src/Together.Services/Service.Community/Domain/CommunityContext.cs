using Infrastructure.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Service.Community.Domain.Aggregates.ForumAggregate;
using Service.Community.Domain.Aggregates.PostAggregate;
using Service.Community.Domain.Aggregates.PrefixAggregate;
using Service.Community.Domain.Aggregates.ReplyAggregate;
using Service.Community.Domain.Aggregates.TopicAggregate;

namespace Service.Community.Domain;

public sealed class CommunityContext(DbContextOptions<CommunityContext> options, AppSettings appSettings) 
    : PostgresDbContext<CommunityContext>(options, appSettings.PostgresConfig)
{
    public DbSet<Forum> Forums  { get; init; } = default!;
    
    public DbSet<Topic> Topics  { get; init; } = default!;
    
    public DbSet<Prefix> Prefixes  { get; init; } = default!;
    
    public DbSet<Post> Posts  { get; init; } = default!;
    
    public DbSet<PostVote> PostVotes  { get; init; } = default!;

    public DbSet<PostReport> PostReports  { get; init; } = default!;
    
    public DbSet<Reply> Replies  { get; init; } = default!;
    
    public DbSet<ReplyVote> ReplyVotes  { get; init; } = default!;
}