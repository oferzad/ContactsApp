using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ContactsApp.Models;
using ContactsApp.Views;
using System.Collections.Generic;
namespace ContactsApp
{
    public partial class App : Application
    {
        public static bool IsDevEnv
        {
            get
            {
                return true; //change this before release!
            }
        }

        //The current logged in user
        public User CurrentUser { get; set; }

        //The list of phone types
        public List<PhoneType> PhoneTypes { get; set; }

        public App()
        {
            InitializeComponent();
            CurrentUser = null;
            PhoneTypes = new List<PhoneType>();
            //Too run the chat example
            CurrentUser = new User()
            {
                //Fill your chat identity
                //In real project it will be filled by the login process
                Email = "Ofer"
            };
            MainPage = new ChatView();
            //To run the contacts example
            //MainPage = new Login();

        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
