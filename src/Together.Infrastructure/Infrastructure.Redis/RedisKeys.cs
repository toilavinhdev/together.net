namespace Infrastructure.Redis;


public class RedisKey
{
    public RedisDatabase Database { get; init; }

    public string KeyName { get; init; } = default!;
}

public static class RedisKeys
{
    public static RedisKey Default<T>(object key) => new()
    {
        Database = RedisDatabase.Default,
        KeyName = $"{typeof(T).Name.ToKebabCase()}:{key}"
    };
    
    public static RedisKey Identity<T>(object key) => new()
    {
        Database = RedisDatabase.Identity,
        KeyName = $"{typeof(T).Name.ToKebabCase()}:{key}"
    };
    
    public static RedisKey IdentityForgotPasswordToken(object key) => new()
    {
        Database = RedisDatabase.Identity,
        KeyName = $"forgot-passwd-token:{key}"
    };
    
    public static RedisKey IdentityNewMember() => new()
    {
        Database = RedisDatabase.Identity,
        KeyName = "new-member"
    };
    
    public static RedisKey Community<T>(object key) => new()
    {
        Database = RedisDatabase.Community,
        KeyName = $"{typeof(T).Name.ToKebabCase()}:{key}"
    };
    
    public static RedisKey CommunityPostView(object key) => new()
    {
        Database = RedisDatabase.Community,
        KeyName = $"post-view:{key}"
    };
    
    public static RedisKey CommunityPostViewer(object key1, object key2) => new()
    {
        Database = RedisDatabase.Community,
        KeyName = $"post-viewer:{key1}:{key2}"
    };
    
    public static RedisKey Chat<T>(object key) => new()
    {
        Database = RedisDatabase.Chat,
        KeyName = $"{typeof(T).Name.ToKebabCase()}:{key}"
    };
    
    public static RedisKey Notification<T>(object key) => new()
    {
        Database = RedisDatabase.Notification,
        KeyName = $"{typeof(T).Name.ToKebabCase()}:{key}"
    };

    public static RedisKey SocketOnlineUsers() => new()
    {
        Database = RedisDatabase.Socket,
        KeyName = "online-users"
    };
    
    private static string ToKebabCase(this string input)
    {
        if (string.IsNullOrEmpty(input)) return default!;
        var inspect = input.Select(
            (x, idx) => idx > 0 && char.IsUpper(x) 
                ? $"-{x}" 
                : string.IsNullOrWhiteSpace(x.ToString()) 
                    ? ""
                    : x.ToString());
        return string.Concat(inspect).ToLower();
    }
}