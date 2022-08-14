using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using _4AutoMarket.Models;
using Microsoft.EntityFrameworkCore;

namespace _4AutoMarket.Classes
{
    public static class UserHandler
    {
        public static Dictionary<string, UserChat> Users = new Dictionary<string, UserChat>();
        public static List<string> ConnectingOperators = new List<string>();
        public static string CurrentUserConnectionId { get; set; }
    }
    public class UserChat
    {
        public UserChat()
        {
            messages = new List<ChatMessage>();
        }
        public string UserName { get; set; }
        public string OperatorName { get; set; }
        public List<ChatMessage> messages { get; set; }
    }
    public class ChatMessage
    {
        public string Message { get; set; }
        public bool IsOperator { get; set; }
        public bool IsNew { get; set; }
        public DateTime Sended { get; set; }
    }
    public class ChatHub : Hub
    {
        private UserDBContext DBContext;
        public ChatHub(UserDBContext dBContext)
        {
            DBContext = dBContext;
        }
        public async Task GetMessage(string name, string message)
        {
            string connectionId = Context.ConnectionId;
            if (Context.User.IsInRole("Admin") && !UserHandler.Users.Keys.Any(_ => _ == connectionId))
            {
                UserHandler.Users.Add(Context.ConnectionId, new UserChat());
                UserHandler.ConnectingOperators.Remove(connectionId);
                await sendAutoResponse(Context.ConnectionId);
            }

            UserHandler.Users[connectionId].messages.Add(new ChatMessage() { IsNew = true, IsOperator = false, Message = message, Sended = DateTime.Now });
            if (string.IsNullOrEmpty(UserHandler.Users[connectionId].UserName)) UserHandler.Users[connectionId].UserName = string.IsNullOrEmpty(name) ? "User" : name;
            if (UserHandler.ConnectingOperators != null && UserHandler.ConnectingOperators.Count > 0)
            {
                if (!string.IsNullOrEmpty(UserHandler.CurrentUserConnectionId) && UserHandler.CurrentUserConnectionId == connectionId)
                {
                    UserHandler.Users[connectionId].messages.Last().IsNew = false;
                }
                await this.Clients.Clients(UserHandler.ConnectingOperators).SendAsync("operatorHub", connectionId, UserHandler.Users[connectionId].UserName, message);
            }
            if (!Context.User.IsInRole("Admin") && UserHandler.Users[connectionId].messages.Count == 2)
            {
                await sendAutoResponse(Context.ConnectionId);
            }
        }
        public override async Task OnConnectedAsync()// new connection
        {
            if (Context.User.IsInRole("Admin"))
            {
                UserHandler.ConnectingOperators.Add(Context.ConnectionId);
            }
            else
            {
                UserHandler.Users.Add(Context.ConnectionId, new UserChat());
            }
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception)// close connections
        {
            if (Context.User.IsInRole("Admin"))
            {
                if (UserHandler.ConnectingOperators.Any(_ => _ == Context.ConnectionId)) UserHandler.ConnectingOperators.Remove(Context.ConnectionId);
                else
                {
                    UserHandler.Users.Remove(Context.ConnectionId);
                    await this.Clients.Clients(UserHandler.ConnectingOperators).SendAsync("closeConnection", Context.ConnectionId);
                }
            }
            else
            {
                UserHandler.Users.Remove(Context.ConnectionId);
                await this.Clients.Clients(UserHandler.ConnectingOperators).SendAsync("closeConnection", Context.ConnectionId);
            }
            await base.OnDisconnectedAsync(exception);
        }
        private async Task<int> SaveInDB(UserChat userChat)
        {
            if (userChat != null && userChat.messages != null && userChat.messages.Count > 0)
            {
                LiveChatSession temp = new LiveChatSession() { OperatorName = userChat.OperatorName, UserName = userChat.UserName, Added = DateTime.Now };
                foreach (var mess in userChat.messages)
                {
                    temp.messages.Add(new LiveChatMessage() { IsNew = mess.IsNew, IsOperator = mess.IsOperator, Sended = mess.Sended, Message = mess.Message });
                }
                DBContext.LiveChatSessions.Add(temp);
                return await DBContext.SaveChangesAsync();
            }
            return 0;
        }
        private async Task sendAutoResponse(string connectionId)
        {
            var gs = await DBContext.GlobalSetings.FirstOrDefaultAsync();
            if (gs != null && !string.IsNullOrEmpty(gs.LiveChat_AutoResponse))
            {
                string ON = string.IsNullOrEmpty(gs.LiveChat_OperatorName) ? "Operator" : gs.LiveChat_OperatorName;
                UserHandler.Users[connectionId].messages.Add(new ChatMessage() { IsNew = false, IsOperator = true, Message = gs.LiveChat_AutoResponse, Sended = DateTime.Now });
                await this.Clients.Client(connectionId).SendAsync("newMessage", ON, gs.LiveChat_AutoResponse);
            }
            return;
        }
    }
}
