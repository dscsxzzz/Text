using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SharedModels.Models;

namespace ConsoleApp1
{
    public class Class1 : Hub
    {
        private ILogger<Class1> logger;
        public Class1(ILogger<Class1> logger)
        {
            this.logger = logger;
        }

        public void log()
        {
            logger.LogInformation($"Started");
        }
        public async Task SendMessage(string user, string message)
        {
            logger.LogInformation($"Received message, {message}");
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
