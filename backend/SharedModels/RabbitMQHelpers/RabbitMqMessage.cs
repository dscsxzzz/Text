using RabbitMQ.Client;
using System;
using System.Text;

public class RabbitMqMessage
{
    public byte[] Body { get; set; }

    public BasicProperties Properties { get; set; }

    public RabbitMqMessage(string message, BasicProperties basicProperties)
    {
        Body = Encoding.UTF8.GetBytes(message);
        Properties = basicProperties;
    }
}
