using System;
using System.Collections.Generic;


namespace ContactsApp.Models
{
    public partial class PhoneType
    {
        public PhoneType()
        {
            ContactPhones = new List<ContactPhone>();
        }

        public int TypeId { get; set; }
        public string TypeName { get; set; }

        public virtual List<ContactPhone> ContactPhones { get; set; }
    }
}
