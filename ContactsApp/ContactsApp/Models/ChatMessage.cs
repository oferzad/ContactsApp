using System;
using System.Collections.Generic;
using System.Text;

namespace ContactsApp.Models
{
    public class ChatMessage
    {
        public string UserId { get; set; }
        public string Message { get; set; }
        public bool Recieved { get; set; } = true;
        public bool Sent
        {
            get
            {
                return !Recieved;
            }
        }
        public DateTime MessageDateTime { get; set; }
        public string GroupName { get; set; }
        public string Description
        {
            get
            {
                
                return $"{UserId} To {GroupName}: {Message}";
                
            }
        }
    }
}
