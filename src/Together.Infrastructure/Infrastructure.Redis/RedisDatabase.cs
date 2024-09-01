using System.ComponentModel;

namespace Infrastructure.Redis;

public enum RedisDatabase
{
    [Description("Default")]
    Default = 0,
    [Description("Service.Identity")]
    Identity = 1,
    [Description("Service.Community")]
    Community = 2,
    [Description("Service.Chat")]
    Chat = 3,
    [Description("Service.Notification")]
    Notification = 4,
    [Description("Service.Socket")]
    Socket = 5,
}