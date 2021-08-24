using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;
using ContactsApp.Services;
using ContactsApp.Models;
using Xamarin.Essentials;

namespace ContactsApp.ViewModels
{
    class LoginViewModel : INotifyPropertyChanged
    {
        
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private string email;
        public string Email
        {
            get { return email; }
            set
            {
                email = value;
                OnPropertyChanged("Email");
            }
        }
        private string password;
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                OnPropertyChanged("Password");
            }
        }
        public ICommand SubmitCommand { protected set; get; }
        
        public LoginViewModel()
        {
            SubmitCommand = new Command(OnSubmit);
        }

        private string serverStatus;
        public string ServerStatus
        {
            get { return serverStatus; }
            set
            {
                serverStatus = value;
                OnPropertyChanged("ServerStatus");
            }
        }

        public async void OnSubmit()
        {
            ServerStatus = "מחבר לשרת...";
            await App.Current.MainPage.Navigation.PushModalAsync(new Views.ServerStatusPage(this));
            ContactsAPIProxy proxy = ContactsAPIProxy.CreateProxy();
            User user = await proxy.LoginAsync(Email, Password);
            if (user == null)
            {
                await App.Current.MainPage.Navigation.PopModalAsync();
                await App.Current.MainPage.DisplayAlert("שגיאה", "התחברות נכשלה, בדוק שם משתמש וסיסמה ונסה שוב", "בסדר");
            }
            else
            {
                ServerStatus = "קורא נתונים...";
                App theApp = (App)App.Current;
                theApp.CurrentUser = user;
                bool success = await LoadPhoneTypes(theApp);
                await App.Current.MainPage.Navigation.PopModalAsync();
                if (!success)
                {
                    await App.Current.MainPage.DisplayAlert("שגיאה", "קריאת נתונים נכשלה. נסה שוב מאוחר יותר", "בסדר");
                }
                else
                {
                    theApp.MainPage = new NavigationPage(new Views.ContactsList());
                    ((NavigationPage)theApp.MainPage).FlowDirection = FlowDirection.RightToLeft;
                }
            }
        }

        private async Task<bool> LoadPhoneTypes(App theApp)
        {
            ContactsAPIProxy proxy = ContactsAPIProxy.CreateProxy();
            theApp.PhoneTypes = await proxy.GetPhoneTypesAsync();
            return theApp.PhoneTypes != null;
        }
    }
}
