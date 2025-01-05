using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SharedModels.RabbitMQHelpers;

public class RabbitMqChannelManager
{
    private readonly IConnection _connection;
    public IChannel Channel;

    public RabbitMqChannelManager(IConnection connection)
    {
        _connection = connection;
    }

    public async Task CreateChannel()
    {
        Channel = await _connection.CreateChannelAsync();
        await Channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);
    }

    public async Task<string> DeclareTempQueue()
    {
        var response = await Channel.QueueDeclareAsync();
        return response.QueueName;
    }

    public AsyncEventingBasicConsumer CreateConsumer()
    {
        return new AsyncEventingBasicConsumer(Channel);
        
    }

    public async Task Dispose()
    {
        try
        {
            await Channel.CloseAsync();
        }
        catch(Exception e)
        {
            throw;
        }
    }
}
