using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ContactsApp.Services
{
    public class ChatService : IChatService
    {
        private const string CLOUD_URL = "TBD"; //API url when going on the cloud
        private const string DEV_ANDROID_EMULATOR_URL = "http://10.0.2.2:21604/chat"; //chat url when using emulator on android
        private const string DEV_ANDROID_PHYSICAL_URL = "http://192.168.1.14:21604/chat"; //chat url when using physucal device on android
        private const string DEV_WINDOWS_URL = "http://localhost:21604/chat"; //API url when using windoes on development
        
        private readonly HubConnection hubConnection;
        public ChatService()
        {
            string chatUrl = GetChatUrl();
            hubConnection = new HubConnectionBuilder().WithUrl(chatUrl).Build();
           
        }

        private string GetChatUrl()
        {
            if (App.IsDevEnv)
            {
                if (Device.RuntimePlatform == Device.Android)
                {
                    if (DeviceInfo.DeviceType == DeviceType.Virtual)
                    {
                        return DEV_ANDROID_EMULATOR_URL;
                    }
                    else
                    {
                        return DEV_ANDROID_PHYSICAL_URL;
                    }
                }
                else
                {
                    return DEV_WINDOWS_URL;
                }
            }
            else
            {
                return CLOUD_URL;
            }
        }

        //Connect gets a list of groups the user belongs to!
        public async Task Connect(string [] groups)
        {
            await hubConnection.StartAsync();
            await hubConnection.InvokeAsync("OnConnect", groups);
        }

        //Use this method when the chat is finished so the connection will not stay open
        public async Task Disconnect(string [] groups)
        {
            await hubConnection.InvokeAsync("OnDisconnect", groups);
            await hubConnection.StopAsync();

        }

        //This message send message to all clients!
        public async Task SendMessage(string userId, string message)
        {
            
            await hubConnection.InvokeAsync("SendMessage", userId, message);
            
        }

        //This methid send a message to specific group
        public async Task SendMessageToGroup(string userId, string message, string groupName)
        {

            await hubConnection.InvokeAsync("SendMessageToGroup", userId, message, groupName);
            
        }

        //this method register a method to be called upon receiving a message
        public void RegisterToReceiveMessage(Action<string, string> GetMessageAndUser)
        {
            hubConnection.On("ReceiveMessage", GetMessageAndUser);
        }
        //this method register a method to be called upon receiving a message from specific group
        public void RegisterToReceiveMessageFromGroup(Action<string, string, string> GetMessageAndUserFromGroup)
        {
            hubConnection.On("ReceiveMessageFromGroup", GetMessageAndUserFromGroup);
        }
    }
}
