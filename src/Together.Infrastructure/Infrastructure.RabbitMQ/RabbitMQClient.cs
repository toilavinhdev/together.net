using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Infrastructure.RabbitMQ;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public interface IRabbitMQClient
{
    void Publish<TEvent>(TEvent @event) where TEvent : RabbitMQEvent;

    void Subscribe<TEvent>(Func<TEvent, Task> callback) where TEvent : RabbitMQEvent;
}

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class RabbitMQClient : IRabbitMQClient
{
    private readonly IModel _channel;

    private readonly string _exchange;

    private readonly ILogger<RabbitMQClient> _logger;
    
    public RabbitMQClient(IConnectionFactory connectionFactory, RabbitMQConfig config, ILogger<RabbitMQClient> logger)
    {
        _logger = logger;
        _exchange = !string.IsNullOrEmpty(config.Exchange) ? config.Exchange : "default-event-bus.direct";
        var connection = connectionFactory.CreateConnection();
        _channel = connection.CreateModel();
        _channel.ExchangeDeclare(exchange: _exchange, type: ExchangeType.Direct);
    }
    
    public void Publish<TEvent>(TEvent @event) where TEvent : RabbitMQEvent
    {
        var routingKey = KebabCase(typeof(TEvent).Name);
        
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));

        var properties = _channel.CreateBasicProperties();
        properties.DeliveryMode = 1; // non-persistent (1) or persistent (2)
        
        _channel.BasicPublish(
            exchange: _exchange, 
            routingKey: routingKey, 
            basicProperties: null, 
            body: body);
        
        _logger.LogInformation("Published message to channel {routingKey}", routingKey);
    }

    public void Subscribe<TEvent>(Func<TEvent, Task> callback) where TEvent : RabbitMQEvent
    {
        var routingKey = KebabCase(typeof(TEvent).Name);
        
        // Tạo queue tạm thời (temporary + fresh + empty) với name random
        // Auto delete nếu client disconnect or không có channel nào binding với queue
        var queueName = _channel.QueueDeclare().QueueName;
        
        // Binding: ràng buộc queue gắn routingKey với exchange
        _channel.QueueBind(
            exchange: _exchange, 
            queue: queueName,
            routingKey: routingKey);
        
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.Received += async (_, ea) =>
        {
            _logger.LogInformation("Processing received message from channel {routingKey}", routingKey);
            
            var body = ea.Body.ToArray();
            var @event = JsonSerializer.Deserialize<TEvent>(Encoding.UTF8.GetString(body)) 
                          ?? throw new InvalidCastException();
            await callback(@event);
        };

        _channel.BasicConsume(
            queue: queueName, 
            autoAck: true, 
            consumer: consumer);
    }
    
    private static string KebabCase(string input)
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