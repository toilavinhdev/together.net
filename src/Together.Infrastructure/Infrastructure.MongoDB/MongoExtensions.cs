using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;

namespace Infrastructure.MongoDB;

public static class MongoExtensions
{
    public static ObjectId ToObjectId(this string id) => ObjectId.Parse(id);
    
    public static void AddMongoDb(this IServiceCollection services, MongoConfig config)
    {
        services.AddMongoDb(config.ConnectionString, config.DatabaseName);
    }
    
    private static void AddMongoDb(this IServiceCollection services, string connectionString, string databaseName)
    {
        services.AddSingleton<IMongoService>(new MongoService(connectionString, databaseName));
    }
}