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



namespace AIModelReceiverService;
public class FrontendReceiver : Hub
{
    private readonly IConnection _connection;
    public IChannel _channel;

    public FrontendReceiver(IConnection connection)
    {
        _connection = connection;
    }

    public async Task<IChannel> CreateChannel()
    {
        _channel = await _connection.CreateChannelAsync();
        return _channel;
    }

    public async Task HandleFrontendRequest(ModelRequest input)
    {
        Console.WriteLine($"Received request from UserId: {input.UserId}, Input: {input.Input}");
        var tempQueue = (await _channel.QueueDeclareAsync()).QueueName;

        var message = JsonSerializer.Serialize(input);
        var messageBody = Encoding.UTF8.GetBytes(message);

        var correlationId = new Guid().ToString();

        var properties = new BasicProperties()
        {
            CorrelationId = correlationId,
            ReplyTo = tempQueue
        };

        await _channel.QueueDeclareAsync(queue: Queue.AIModelQueue.GetDescription(), durable: false, exclusive: false, autoDelete: false);
        await _channel.BasicPublishAsync(
            exchange: "",
            routingKey: Queue.AIModelQueue.GetDescription(),
            mandatory: true,
            basicProperties: properties,
            body: messageBody
        );

        Console.WriteLine($"Request sent to AI model queue: {input}, ReplyQueue: {tempQueue}");
    }
}
