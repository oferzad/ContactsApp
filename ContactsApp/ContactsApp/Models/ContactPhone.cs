using System;
using System.Collections.Generic;


namespace ContactsApp.Models
{
    public partial class ContactPhone
    {
        public int PhoneId { get; set; }
        public int ContactId { get; set; }
        public int? PhoneTypeId { get; set; }
        public string PhoneNumber { get; set; }

        public virtual UserContact Contact { get; set; }
        public virtual PhoneType PhoneType { get; set; }
    }
}
