using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
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
    private ILogger<FrontendSender> _logger;
    private readonly IHubContext<MessageHub> _hubContext;

    public FrontendSender(IConnection connection, IHubContext<MessageHub> hubContext, ILogger<FrontendSender> logger)
    {
        _connection = connection;
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task CreateChannel()
    {
        try
        {
            _logger.LogInformation("Attempting to create channel...");
            _channel = await _connection.CreateChannelAsync();
            _logger.LogInformation("Channel created successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create channel.");
            throw;
        }
    }

    public async Task StartListeningToEventQueue()
    {
        if (_channel == null)
        {
            _logger.LogError("Cannot start listening: channel is null.");
            return;
        }

        _logger.LogInformation("Started listening to the event queue.");

        await _channel.QueueDeclareAsync(queue: Queue.UserQueue.GetDescription(), durable: false, exclusive: false, autoDelete: false);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var messageBody = Encoding.UTF8.GetString(ea.Body.ToArray());
            var propertiesBody = ea.BasicProperties;
            var message = JsonSerializer.Deserialize<ModelRequest>(messageBody);

            _logger.LogInformation($"Received input: {message.Input}, from {message.UserId}, Listening to queue: {propertiesBody.ReplyTo}");
            await StartListeningToTempQueue(propertiesBody.ReplyTo!);
        };

        await _channel.BasicConsumeAsync(queue: Queue.UserQueue.GetDescription(), autoAck: true, consumer: consumer);
    }

    private async Task StartListeningToTempQueue(string tempQueueName)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var processedMessage = Encoding.UTF8.GetString(ea.Body.ToArray());
            _logger.LogInformation($"Received processed response: {processedMessage}");
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
            _logger.LogInformation($"Sent to user {response.UserId}: {response.SummarizedText}");
        }
        else
        {
            _logger.LogInformation("Invalid response: UserId is missing.");
        }
    }

    public override Task OnConnectedAsync()
    {
        var userId = Context.GetHttpContext()?.Request.Query["userId"];
        if (!string.IsNullOrEmpty(userId))
        {
            _logger.LogInformation($"User connected: {userId}");
        }
        return base.OnConnectedAsync();
    }
}
