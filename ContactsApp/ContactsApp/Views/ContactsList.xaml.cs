using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ContactsApp.ViewModels;

namespace ContactsApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContactsList : ContentPage
    {
        public ContactsList()
        {
            ContactsListViewModel vm = new ContactsListViewModel();
            vm.ClearSelection += ClearSealection;
            this.BindingContext = vm;
            InitializeComponent();
        }

        public void ClearSealection()
        {
            collectionName.SelectedItem = null;
        }

        private void Logout_Clicked(object sender, EventArgs e)
        {
            App theApp = (App)App.Current;
            theApp.CurrentUser = null;
            theApp.MainPage = new Login();
        }
    }
}