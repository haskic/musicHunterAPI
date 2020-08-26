using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace MusicHunterServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : Hub
    {
        public ILogger<Program> Logger { get; }
        public NotificationsController(ILogger<Program> logger)
        {
            Logger = logger;
        }

        public async Task GetNotification(string message)
        {
            Logger.LogError("Getnotification message received = " + message);
            await this.Clients.All.SendAsync("GetResponse", "Hello my friend asp.net core glad to see you");
            await this.Clients.All.SendAsync("GetLol", "Hello my friend asp.net core glad to see you");

            //if (UserContainer.getUserById(UserId) == null)
            //{
            //    string ar = $"Error, user was not founded. ID={UserId} doesn't exist.";
            //    await this.Clients.Client(connectionId).SendAsync("GetResponse", ar);
            //}
            //else
            //{
            //    string sshResponse = UserContainer.getUserById(UserId).SSHRunCommand(sshCommand);
            //    string ar = $"SSH COMMAND:{sshCommand} FOR USER ID={UserId} was execute.";
            //    await this.Clients.Client(connectionId).SendAsync("GetResponseCommand", ar, sshResponse);
            //}
        }
    }
}