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
            MainPage = new Login();
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
