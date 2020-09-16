using Google.Apis.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using MusicHunterServer.Data;
using MusicHunterServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Message = MusicHunterServer.Models.Message;

namespace MusicHunterServer.Controllers
{
    public class MessengerController : Hub
    {
        private readonly ILogger<Program> _logger;
        private readonly UserDbContext _dbContext;
        private List<MessengerUser> Users= new List<MessengerUser>();

        public MessengerController(ILogger<Program> logger, UserDbContext dbContext)
        {
            this._logger = logger;
            _dbContext = dbContext;
        }
        
        
        public void Connect(string UserHash)
        {
            var userId = Context.ConnectionId;
            _logger.LogWarning("User with hash = " + UserHash + " connecting...");
            if (!Users.Any(user => user.ConnectionId == userId))
            {
                Users.Add(new MessengerUser() { ConnectionId = Context.ConnectionId, Hash = UserHash });
                Clients.Caller.SendAsync("connected", "hello new user");
                _logger.LogWarning("User with hash = " + UserHash + " was connected");

            }

        }
        public async Task SendMessage(string hashSender,string hashReciver,Message message)
        {
            message.CreatedAt = DateTime.Now;
            message.SenderHash = hashSender;
            var userReceiver = Users.FirstOrDefault(user => user.Hash == hashReciver);
            if (message.ConversationId == 0)
            {
                _logger.LogWarning("ConversationId not founded in request body!");
                int conversationId = IsConversationExist(hashReciver, hashSender);
                if (conversationId == 0)
                {
                    _logger.LogWarning("Creating new converstion object ....");
                    Conversation newConversation = new Conversation();
                    newConversation.Title = "Conversation";
                    newConversation.IsDuo = true;
                    newConversation.UpdatedAt = DateTime.Now;
                    newConversation.CreatedAt = DateTime.Now;
                    newConversation.CreatorHash = hashSender;

                    _dbContext.Conversations.Add(newConversation);
                    await _dbContext.SaveChangesAsync();
                    Participant participant1 = new Participant();
                    participant1.ConversationId = newConversation.Id;
                    participant1.Hash = hashSender;

                    Participant participant2 = new Participant();
                    participant2.ConversationId = newConversation.Id;
                    participant2.Hash = hashReciver;

                    message.ConversationId = newConversation.Id;
                    _dbContext.Participants.Add(participant1);
                    _dbContext.Participants.Add(participant2);
                    _dbContext.Messages.Add(message);
                    await _dbContext.SaveChangesAsync();
                    _logger.LogWarning("New conversation was created with id = " + newConversation.Id + "  and message was successfullly sent");
                }
                else
                {
                    message.ConversationId = conversationId;
                    _dbContext.Messages.Add(message);
                    await _dbContext.SaveChangesAsync();
                }
                _logger.LogInformation("Message was send info: |Conversation was not founded in received message obj but was founded in DataBase|");
                await Clients.All.SendAsync("GetMessage", new { Text = message.Text, HashSender = hashSender});
            }
            else
            {
                _dbContext.Messages.Add(message);
                await _dbContext.SaveChangesAsync();
                _logger.LogWarning("ConversationId was found in body of request and message was sent!");
                await Clients.Client(userReceiver.ConnectionId).SendAsync("GetMessage", message.Text);
                _logger.LogInformation("Message was sent, info: |Conversation was found in received message|");

            }

            if (userReceiver != null)
            {
                await Clients.Client(userReceiver.ConnectionId).SendAsync("GetMessage", new { Text = message.Text, HashSender = hashSender });
                _logger.LogWarning("Notification to UserReceiver with connectionId = " + userReceiver.ConnectionId + " was sent");
                _logger.LogInformation("Message was sent, info: |Receiver user was Online on Zhakar servers.|");

            }

        }
        public int IsConversationExist(string hash1,string hash2)
        {
            var conversationList = _dbContext.Participants.FromSqlRaw("select * from Participants where ConversationId in (select O.ConversationId from (select count(T.ConversationID) as Counter, T.ConversationId from (select * from Participants where Hash like @hash1 or Hash like @hash2) as T group by T.ConversationId) as O where O.Counter = 2)",
                new SqlParameter("@hash1", hash1), new SqlParameter("@hash2", hash2));
            if (conversationList.Count() >= 2)
            {
                int conversationId = conversationList.ToList<Participant>().ElementAt(0).ConversationId;
                return conversationId;
            }
            return 0;
        }
        
        class ConversationHash
        {
            public int ConversationId { get; set; }
            public string Hash { get; set; }

        }

    }
}
