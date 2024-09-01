using System.Text.Json;
using StackExchange.Redis;

namespace Infrastructure.Redis;

public interface IRedisService
{
    Task<string?> StringGetAsync(RedisKey key);
    
    Task<T?> StringGetAsync<T>(RedisKey key);
    
    Task<bool> StringSetAsync(RedisKey key, string value, TimeSpan? expiry = null);
    
    Task<bool> StringSetAsync<T>(RedisKey key, T value, TimeSpan? expiry = null);
    
    Task ListLeftPushAsync(RedisKey key, string value);

    Task<RedisValue?> ListLeftPopAsync(RedisKey key);

    Task ListRightPushAsync(RedisKey key, string value);

    Task<RedisValue?> ListRightPopAsync(RedisKey key);

    Task<RedisValue[]> ListRangeAsync(RedisKey key, long start, long stop);

    Task<bool> SetAddAsync(RedisKey key, string value);

    Task<bool> SetRemoveAsync(RedisKey key, string value);

    Task<bool> SetContainsAsync(RedisKey key, string value);

    Task<long> SetLengthAsync(RedisKey key);
    
    Task Transaction(RedisDatabase database, Func<ITransaction, Task> callback);
    
    Task<long> IncrementAsync(RedisKey key);

    Task<long> DecrementAsync(RedisKey key);

    Task<bool> ExistsAsync(RedisKey key);

    Task<bool> KeyDeleteAsync(RedisKey key);

    Task<List<string>> KeysByPatternAsync(RedisKey keyPrefix);
    
    Task<List<string>> KeysByPatternAsync(RedisDatabase database, string pattern);

    Task<long> PublishAsync<TEvent>(TEvent message) where TEvent : RedisPubSubEvent;

    Task SubscribeAsync<TEvent>(Func<TEvent, Task> callback) where TEvent : RedisPubSubEvent;
}

public class RedisService(IConnectionMultiplexer connection) : IRedisService
{
    private IDatabase Database(RedisDatabase db = RedisDatabase.Default) => connection.GetDatabase((int)db);
    
    private IServer Server()
    {
        foreach (var endPoint in connection.GetEndPoints())
        {
            var server = connection.GetServer(endPoint);
            if (!server.IsReplica) return server;
        }
        
        throw new RedisException("Master database not found");
    }

    private ISubscriber Subscriber() => connection.GetSubscriber();

    public async Task<string?> StringGetAsync(RedisKey key)
    {
        var value = await Database(key.Database).StringGetAsync(key.KeyName);
        return value.ToString();
    }
    
    public async Task<T?> StringGetAsync<T>(RedisKey key)
    {
        var value = await StringGetAsync(key);
        return string.IsNullOrEmpty(value) ? default : JsonSerializer.Deserialize<T>(value);
    }

    public async Task<bool> StringSetAsync(RedisKey key, string value, TimeSpan? expiry = null)
    {
        return await Database(key.Database).StringSetAsync(key.KeyName, value, expiry);
    }
    
    public async Task<bool> StringSetAsync<T>(RedisKey key, T value, TimeSpan? expiry = null)
    {
        return await Database(key.Database).StringSetAsync(key.KeyName, JsonSerializer.Serialize(value), expiry);
    }

    public async Task ListLeftPushAsync(RedisKey key, string value)
    {
        await Database(key.Database).ListLeftPushAsync(key.KeyName, value);
    }
    
    public async Task<RedisValue?> ListLeftPopAsync(RedisKey key)
    {
        return await Database(key.Database).ListLeftPopAsync(key.KeyName);
    }
    
    public async Task ListRightPushAsync(RedisKey key, string value)
    {
        await Database(key.Database).ListRightPushAsync(key.KeyName, value);
    }
    
    public async Task<RedisValue?> ListRightPopAsync(RedisKey key)
    {
        return await Database(key.Database).ListRightPopAsync(key.KeyName);
    }
    
    public async Task<RedisValue[]> ListRangeAsync(RedisKey key, long start, long stop)
    {
        return await Database(key.Database).ListRangeAsync(key.KeyName, start, stop);
    }
    
    public async Task<bool> SetAddAsync(RedisKey key, string value)
    {
        return await Database(key.Database).SetAddAsync(key.KeyName, value);
    }
    
    public async Task<bool> SetRemoveAsync(RedisKey key, string value)
    {
        return await Database(key.Database).SetRemoveAsync(key.KeyName, value);
    }
    
    public async Task<bool> SetContainsAsync(RedisKey key, string value)
    {
        return await Database(key.Database).SetContainsAsync(key.KeyName, value);
    }
    
    public async Task<long> SetLengthAsync(RedisKey key)
    {
        return await Database(key.Database).SetLengthAsync(key.KeyName);
    }
    
    public async Task Transaction(RedisDatabase database, Func<ITransaction, Task> callback)
    {
        var transaction = Database(database).CreateTransaction();
        await callback(transaction);
        await transaction.ExecuteAsync();
    }
    
    public async Task<long> IncrementAsync(RedisKey key)
    {
        return await Database(key.Database).StringIncrementAsync(key.KeyName);
    }

    public async Task<long> DecrementAsync(RedisKey key)
    {
        return await Database(key.Database).StringDecrementAsync(key.KeyName);
    }

    public async Task<bool> ExistsAsync(RedisKey key)
    {
        return await Database(key.Database).KeyExistsAsync(key.KeyName);
    }

    public async Task<bool> KeyDeleteAsync(RedisKey key)
    {
        return await Database(key.Database).KeyDeleteAsync(key.KeyName);
    }

    public async Task<List<string>> KeysByPatternAsync(RedisKey keyPrefix)
    {
        return await KeysByPatternAsync(keyPrefix.Database, keyPrefix.KeyName);
    }

    public async Task<List<string>> KeysByPatternAsync(RedisDatabase database, string pattern)
    {
        var asyncKeys = Server().KeysAsync(database: (int)database, pattern: pattern);
        var keys = new List<string>();
        await foreach (var key in asyncKeys) keys.Add(key.ToString());
        return keys;
    }

    public async Task<long> PublishAsync<TEvent>(TEvent message) where TEvent : RedisPubSubEvent
    {
        var channel = new RedisChannel(typeof(TEvent).Name, RedisChannel.PatternMode.Auto);
        return await Subscriber().PublishAsync(channel, JsonSerializer.Serialize(message));
    }

    public async Task SubscribeAsync<TEvent>(Func<TEvent, Task> callback) where TEvent : RedisPubSubEvent
    {
        var channel = new RedisChannel(typeof(TEvent).Name, RedisChannel.PatternMode.Auto);
        await Subscriber().SubscribeAsync(channel, (_, value) =>
        {
            var @event = JsonSerializer.Deserialize<TEvent>(value.ToString());
            if (@event is null) return;
            callback(@event);
        });
    }
}