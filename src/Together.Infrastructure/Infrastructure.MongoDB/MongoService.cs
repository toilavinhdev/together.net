using MongoDB.Driver;

namespace Infrastructure.MongoDB;

public interface IMongoService
{
    IMongoDatabase Database();

    MongoClient Client();

    IMongoCollection<T> Collection<T>() where T : BaseDocument;
}

public class MongoService : IMongoService
{
    private readonly IMongoDatabase _database;

    private readonly MongoClient _client;

    public MongoService(string connectionString, string databaseName)
    {
        var settings = MongoClientSettings.FromConnectionString(connectionString);
        _client = new MongoClient(settings);
        _database = _client.GetDatabase(databaseName);
    }

    public IMongoDatabase Database() => _database;
    
    public MongoClient Client() => _client;
    
    public IMongoCollection<T> Collection<T>() where T : BaseDocument 
        => _database.GetCollection<T>(UnderscoreCase(typeof(T).Name));
    
    private static string UnderscoreCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return default!;
        var inspect = input.Select(
            (x, idx) => idx > 0 && char.IsUpper(x) 
                ? $"_{x}" 
                : string.IsNullOrWhiteSpace(x.ToString()) 
                    ? ""
                    : x.ToString());
        return string.Concat(inspect).ToLower();
    }
}