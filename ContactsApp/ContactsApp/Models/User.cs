using System;
using System.Collections.Generic;


namespace ContactsApp.Models
{
    public partial class User
    {
        public User()
        {
            UserContacts = new List<UserContact>();
        }

        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserPswd { get; set; }

        public virtual List<UserContact> UserContacts { get; set; }
    }
}
