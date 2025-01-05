using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedModels.Helpers;
using SharedModels.RabbitMQHelpers;
using SharedModels.Requests;

namespace AIModelSenderSerice;

public class FrontendSender : Hub
{
    private readonly IConnection _connection;
    private IChannel _channel;
    private readonly IHubContext<MessageHub> _hubContext;

    public FrontendSender(IConnection connection, IHubContext<MessageHub> hubContext)
    {
        _connection = connection;
        _hubContext = hubContext;
    }

    public async Task CreateChannel()
    {
        _channel = await _connection.CreateChannelAsync();
    }

    public async Task StartListeningToEventQueue()
    {
        await _channel.QueueDeclareAsync(queue: Queue.AIModelQueue.GetDescription(), durable: false, exclusive: false, autoDelete: false);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var messageBody = Encoding.UTF8.GetString(ea.Body.ToArray());
            var propertiesBody = ea.BasicProperties;
            var message = JsonSerializer.Deserialize<ModelRequest>(messageBody);

            Console.WriteLine($"Received input: {message.Input}, from {message.UserId}, Listening to queue: {propertiesBody.ReplyTo}");
            await StartListeningToTempQueue(propertiesBody.ReplyTo!);
        };

        await _channel.BasicConsumeAsync(queue: Queue.AIModelQueue.GetDescription(), autoAck: true, consumer: consumer);
    }

    private async Task StartListeningToTempQueue(string tempQueueName)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var processedMessage = Encoding.UTF8.GetString(ea.Body.ToArray());
            Console.WriteLine($"Received processed response: {processedMessage}");
            await SendToFrontend(processedMessage);
        };

        await _channel.BasicConsumeAsync(queue: tempQueueName, autoAck: true, consumer: consumer);
    }

    private async Task SendToFrontend(string message)
    {
        var response = JsonSerializer.Deserialize<ModelResponse>(message);
        if (response?.UserId != null)
        {
            await _hubContext.Clients.User(response.UserId.ToString()).SendAsync("ReceiveMessage", response.SummarizedText);
            Console.WriteLine($"Sent to user {response.UserId}: {response.SummarizedText}");
        }
        else
        {
            Console.WriteLine("Invalid response: UserId is missing.");
        }
    }

    public override Task OnConnectedAsync()
    {
        var userId = Context.GetHttpContext()?.Request.Query["userId"];
        if (!string.IsNullOrEmpty(userId))
        {
            Console.WriteLine($"User connected: {userId}");
        }
        return base.OnConnectedAsync();
    }
}
