using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;
using ContactsApp.Services;
using ContactsApp.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace ContactsApp.ViewModels
{
    public class ChatViewModel : INotifyPropertyChanged
    {

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private string message;

        public string Message
        {
            get => message;
            set
            {
                message = value;
                OnPropertyChanged("Message");
            }
        }

        private string group;

        public string Group
        {
            get => group;
            set
            {
                group = value;
                OnPropertyChanged("Group");
            }
        }

        public ObservableCollection<ChatMessage> Messages { get; set; }
        //Declare the chat service to be allive as long as the chat view is on!
        //In other cases the live connection should be alive while the app in on
        //in such cases it will be declared in the App class
        private IChatService chatService;
        private User user;
        public ObservableCollection<string> Groups { get; set; }
        public ChatViewModel()
        {
            Group = "All";
            Messages = new ObservableCollection<ChatMessage>();
            Groups = new ObservableCollection<string>();
            Groups.Add("All");
            Groups.Add("Group1");
            Groups.Add("Group2");
            App app = (App)Application.Current;
            user = app.CurrentUser;
            chatService = new ChatService();
            chatService.RegisterToReceiveMessage(ReceiveMessage);
            chatService.RegisterToReceiveMessageFromGroup(ReceiveMessageFromGroup);
            Message = String.Empty;
            ConnectToChatService();
        }

        private async void ConnectToChatService()
        {
            //connect to server and register to all groups
            await chatService.Connect(Groups.ToArray());
        }
        private void ReceiveMessage(string userId, string message)
        {
            ChatMessage chatMessage = new ChatMessage()
            {
                UserId = userId,
                Message = message,
                Recieved = true,
                MessageDateTime = DateTime.Now,
                GroupName = "All"
            };
            if (chatMessage.UserId == user.Email)
                chatMessage.Recieved = false;
            Messages.Add(chatMessage);
        }

        private void ReceiveMessageFromGroup(string userId, string message, string groupName)
        {
            ChatMessage chatMessage = new ChatMessage()
            {
                UserId = userId,
                Message = message,
                Recieved = true,
                MessageDateTime = DateTime.Now,
                GroupName = groupName
            };

            if (chatMessage.UserId == user.Email)
                chatMessage.Recieved = false;
            Messages.Add(chatMessage);
        }

        public ICommand SendMessage => new Command(OnSendMessage);
        public async void OnSendMessage()
        {
            //ChatMessage message = new ChatMessage()
            //{
            //    UserId = this.user.Email,
            //    Message = this.Message,
            //    Recieved = false,
            //    MessageDateTime = DateTime.Now,
            //    GroupName = this.Group
            //};
            //Messages.Add(message);
            if (group == "All")
                await chatService.SendMessage(user.Email, Message);
            else
                await chatService.SendMessageToGroup(user.Email, Message, Group);
        }
    }
}
