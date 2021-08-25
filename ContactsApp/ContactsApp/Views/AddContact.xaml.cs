﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ContactsApp.Models;
using ContactsApp.ViewModels;

namespace ContactsApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddContact : ContentPage
    {
        public AddContact(AddContactViewModel context)
        {
            this.BindingContext = context;
            InitializeComponent();
        }

        private void buttonPhones_Clicked(object sender, EventArgs e)
        {

        }
    }
}