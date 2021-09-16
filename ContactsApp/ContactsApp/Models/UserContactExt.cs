using System;
using System.Collections.Generic;
using System.Text;
using ContactsApp.Services;

namespace ContactsApp.Models
{
    public partial class UserContact
    {
        public string ImgSource 
        {
            get
            {
                ContactsAPIProxy proxy = ContactsAPIProxy.CreateProxy();
                //Create a source with cache busting!
                Random r = new Random();
                string source = $"{proxy.GetBasePhotoUri()}/{this.ContactId}.jpg?{r.Next()}";
                return source;
            }
        }
    }
}
