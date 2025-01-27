using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RabbitMQ.Client;
using SharedModels.RabbitMQHelpers;
using SharedModels.Requests;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;



namespace AIModelReceiverService;
public class FrontendReceiver : Hub
{
    private readonly IConnection _connection;
    private readonly ILogger<FrontendReceiver> _logger; 
    public IChannel _channel;

    public FrontendReceiver(IConnection connection, ILogger<FrontendReceiver> logger)
    {
        _connection = connection;
        _logger = logger;

        _channel = _connection.CreateChannelAsync().Result;
        _logger.LogInformation("Channel created successfully.");
    }

    public async Task<IChannel> CreateChannel()
    {
        _channel = await _connection.CreateChannelAsync();
        return _channel;
    }

    public async Task HandleFrontendRequest(Guid userId, string input)
    {
        _logger.LogInformation($"Received request from UserId: {userId.ToString()}, Input: {input}");
        var tempQueue = (await _channel.QueueDeclareAsync(exclusive:false)).QueueName;
        var req = new ModelRequest()
        {
            UserId = userId,
            Input = input,
        };
        var message = JsonSerializer.Serialize(req);
        var messageBody = Encoding.UTF8.GetBytes(message);

        var correlationId = Guid.NewGuid().ToString();

        var properties = new BasicProperties()
        {
            CorrelationId = correlationId,
            ReplyTo = tempQueue
        };

        await _channel.QueueDeclareAsync(queue: Queue.AIModelQueue.GetDescription(), durable: false, exclusive: false, autoDelete: false);
        await _channel.QueueDeclareAsync(queue: Queue.UserQueue.GetDescription(), durable: false, exclusive: false, autoDelete: false);
        await _channel.BasicPublishAsync(
            exchange: "",
            routingKey: Queue.AIModelQueue.GetDescription(),
            mandatory: true,
            basicProperties: properties,
            body: messageBody
        );
        await _channel.BasicPublishAsync(
            exchange: "",
            routingKey: Queue.UserQueue.GetDescription(),
            mandatory: true,
            basicProperties: properties,
            body: messageBody
        );

        _logger.LogInformation($"Request sent to AI model queue: {input}, ReplyQueue: {tempQueue}");
    }
}
