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
        }
    }
}