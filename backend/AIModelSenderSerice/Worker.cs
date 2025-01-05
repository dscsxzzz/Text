using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace AIModelSenderSerice;

public class Worker : IHostedService
{
    private readonly FrontendSender _frontendSender;

    public Worker(FrontendSender frontendSender)
    {
        _frontendSender = frontendSender;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _frontendSender.CreateChannel();
        await _frontendSender.StartListeningToEventQueue();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
