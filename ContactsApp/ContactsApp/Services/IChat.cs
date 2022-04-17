using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ContactsApp.Services
{
    public interface IChatService
    {
        Task Connect(string [] groupNames);
        Task Disconnect(string [] groupNames);
        Task SendMessage(string userId, string message);
        Task SendMessageToGroup(string userId, string message, string groupName);
        void RegisterToReceiveMessage(Action<string, string> GetMessageAndUser);
        void RegisterToReceiveMessageFromGroup(Action<string, string, string> GetMessageAndUserFromGroup);
    }
    
}
