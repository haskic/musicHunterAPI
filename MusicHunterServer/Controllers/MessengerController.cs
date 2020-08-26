using Google.Apis.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using MusicHunterServer.Data;
using MusicHunterServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
        
        public async Task GetNotification(string message)
        {
            _logger.LogError("Getnotification message received = " + message);
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
        //public async Task SendMessage(Message message)
        //{


        //}
        public void Connect(string UserHash)
        {
            var userId = Context.ConnectionId;
            if (!Users.Any(user => user.ConnectionId == userId))
            {
                Users.Add(new MessengerUser() { ConnectionId = Context.ConnectionId, Hash = UserHash });
                Clients.Caller.SendAsync("connected", "hello new user");
            }

        }
        public async Task SendMessage(string hashSender,string hashReciver, Message message)
        {

            message.CreatedAt = DateTime.Now;
            var userReceiver = Users.FirstOrDefault(user => user.Hash == hashReciver);
            if (message.ConversationId == 0)
            {
                int conversationId = IsConversationExist(hashReciver, hashSender);
                if (conversationId == 0)
                {
                    
                    Conversation newConversation = new Conversation() { CreatedAt = DateTime.Now, CreatorHash = hashSender, UpdatedAt = DateTime.Now, IsDuo = true, Title = "Personal Chat" };
                    _dbContext.Conversations.Add(newConversation);
                    _dbContext.SaveChanges();
                    Participant participant1 = new Participant()
                    {
                        ConversationId = newConversation.ConversationId,
                        Hash = hashSender
                    };
                    Participant participant2 = new Participant()
                    {
                        ConversationId = newConversation.ConversationId,
                        Hash = hashReciver
                    };

                    message.ConversationId = newConversation.ConversationId;
                    _dbContext.Participants.Add(participant1);
                    _dbContext.Participants.Add(participant2);
                    _dbContext.Messages.Add(message);
                    _dbContext.SaveChanges();

                }
            }
            else
            {
                _dbContext.Messages.Add(message);
                _dbContext.SaveChanges();
            }
            //_dbContext.ConversationRelations.FirstOrDefault(item => item.)

            if (userReceiver != null)
            {
                //await Clients.Client(userReceiver.ConnectionId).SendAsync("GetMessage", message);
                await Clients.Client(userReceiver.ConnectionId).SendAsync("GetNotification", message);


            }



        }
        public int IsConversationExist(string hashUser1,string hashUser2)
        {

            var conversationList = _dbContext.Participants.GroupBy(item => new { item.ConversationId, item.Hash }).Where(p => p.Key.Hash == hashUser1 || p.Key.Hash == hashUser2).Take(2);
            
            if (conversationList.ElementAt(0).Key.ConversationId == conversationList.ElementAt(1).Key.ConversationId)
            {
                return conversationList.ElementAt(0).Key.ConversationId;
            }
            return 0;
        }
        

    }
}
