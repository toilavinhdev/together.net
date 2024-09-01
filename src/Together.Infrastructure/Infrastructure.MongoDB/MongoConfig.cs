namespace Infrastructure.MongoDB;

public sealed class MongoConfig
{
    public string ConnectionString { get; set; } = default!;

    public string DatabaseName { get; set; } = default!;
}