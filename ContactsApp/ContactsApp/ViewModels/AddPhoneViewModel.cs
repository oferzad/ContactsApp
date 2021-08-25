using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;
using ContactsApp.Services;
using ContactsApp.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace ContactsApp.ViewModels
{
    class AddPhoneViewModel : INotifyPropertyChanged
    {

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region מספר טלפון
        private bool showPhoneNumberError;

        public bool ShowPhoneNumberError
        {
            get => showPhoneNumberError;
            set
            {
                showPhoneNumberError = value;
                OnPropertyChanged("ShowPhoneNumberError");
            }
        }

        private string phoneNumber;

        public string PhoneNumber
        {
            get => phoneNumber;
            set
            {
                phoneNumber = value;
                ValidatePhoneNumber();
                OnPropertyChanged("PhoneNumber");
            }
        }

        private string phoneNumberError;

        public string PhoneNumberError
        {
            get => phoneNumberError;
            set
            {
                phoneNumberError = value;
                OnPropertyChanged("PhoneNumberError");
            }
        }

        private void ValidatePhoneNumber()
        {
            this.ShowPhoneNumberError = string.IsNullOrEmpty(PhoneNumber);
        }
        #endregion
        #region סוג טלפון
        private PhoneType phoneType;

        public PhoneType PhoneType
        {
            get => phoneType;
            set
            {
                phoneType = value;
                OnPropertyChanged("PhoneType");
            }
        }

        public List<PhoneType> PhoneTypes
        {
            get
            {
                App app = (App)App.Current;
                return app.PhoneTypes;
            }
        }
        #endregion

        //This is a reference to the phone list of the contact who initiated the page
        ObservableCollection<ContactPhone> contactPhones;
        public AddPhoneViewModel(ObservableCollection<ContactPhone> contactPhones)
        {
            this.contactPhones = contactPhones;
            PhoneNumberError = ERROR_MESSAGES.REQUIRED_FIELD;
            PhoneType = PhoneTypes[0];
        }

        //The following command cancel the addition and close the page
        public ICommand CancelCommand => new Command(OnCancel);
        public async void OnCancel()
        {
            await App.Current.MainPage.Navigation.PopModalAsync();
        }

        //The following command save the phone and close the page
        public ICommand SaveCommand => new Command(OnSave);
        public async void OnSave()
        {
            //Validate form first
            ValidatePhoneNumber();
            if (!ShowPhoneNumberError)
            {
                ContactPhone phone = new ContactPhone()
                {
                    PhoneNumber = this.PhoneNumber,
                    PhoneType = this.PhoneType,
                    PhoneTypeId = this.PhoneType.TypeId
                };
                this.contactPhones.Add(phone);
                await App.Current.MainPage.Navigation.PopModalAsync();
            }
        }

    }
}
