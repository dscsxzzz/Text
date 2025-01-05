using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace SharedModels.RabbitMQHelpers;
public class RabbitMqSender
{
    private readonly IChannel _channel;
    private readonly string _queueName;

    public RabbitMqSender(IChannel channel, string queueName)
    {
        _channel = channel;
        _queueName = queueName;
    }

    public async Task SendMessageAsync(RabbitMqMessage message)
    {
        await _channel.BasicPublishAsync(
            exchange: "",
            routingKey: _queueName,
            mandatory: true,
            basicProperties: message.Properties,
            body: message.Body
        );
    }
}

