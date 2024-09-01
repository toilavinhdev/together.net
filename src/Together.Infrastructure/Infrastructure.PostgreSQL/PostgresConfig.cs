namespace Infrastructure.PostgreSQL;

public sealed class PostgresConfig
{
    public string ConnectionString { get; set; } = default!;

    public string Schema { get; set; } = default!;
}