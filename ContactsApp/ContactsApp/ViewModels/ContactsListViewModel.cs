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

namespace ContactsApp.ViewModels
{
    class ContactsListViewModel : INotifyPropertyChanged
    {

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private List<UserContact> allContacts;
        private ObservableCollection<UserContact> filteredContacts;
        public ObservableCollection<UserContact> FilteredContacts
        {
            get
            {
                return this.filteredContacts;
            }
            set
            {
                if (this.filteredContacts != value)
                {

                    this.filteredContacts = value;
                    OnPropertyChanged("FilteredContacts");
                }
            }
        }

        private string searchTerm;
        public string SearchTerm
        {
            get
            {
                return this.searchTerm;
            }
            set
            {
                if (this.searchTerm != value)
                {

                    this.searchTerm = value;
                    OnTextChanged(value);
                    OnPropertyChanged("SearchTerm");
                }
            }
        }

        public ContactsListViewModel()
        {
            this.SearchTerm = String.Empty;
            InitContacts();

        }

        public List<PhoneType> PhoneTypes
        {
            get
            {
                return ((App)Application.Current).PhoneTypes;
            }
        }

        private void InitContacts()
        {
            IsRefreshing = true;
            App theApp = (App)App.Current;
            this.allContacts = theApp.CurrentUser.UserContacts;

            
            //Copy list to the filtered list
            this.FilteredContacts = new ObservableCollection<UserContact>(this.allContacts.OrderBy(uc => uc.LastName));
            SearchTerm = String.Empty;
            IsRefreshing = false;
        }

        //Commands

        #region Search
        public void OnTextChanged(string search)
        {
            //Filter the list of contacts based on the search term
            if (this.allContacts == null)
                return;
            if (String.IsNullOrWhiteSpace(search) || String.IsNullOrEmpty(search))
            {
                foreach (UserContact uc in this.allContacts)
                {
                    if (!this.FilteredContacts.Contains(uc))
                        this.FilteredContacts.Add(uc);
                    
                    
                }
            }
            else
            {
                foreach (UserContact uc in this.allContacts)
                {
                    string contactString = $"{uc.FirstName}|{uc.LastName}|{uc.Email}";

                    if (!this.FilteredContacts.Contains(uc) &&
                        contactString.Contains(search))
                        this.FilteredContacts.Add(uc);
                    else if (this.FilteredContacts.Contains(uc) &&
                        !contactString.Contains(search))
                        this.FilteredContacts.Remove(uc);
                }
            }

            this.FilteredContacts = new ObservableCollection<UserContact>(this.FilteredContacts.OrderBy(uc => uc.LastName));
        }
        #endregion
        #region Refresh
        private bool isRefreshing;
        public bool IsRefreshing
        {
            get => isRefreshing;
            set
            {
                if (this.isRefreshing != value)
                {
                    this.isRefreshing = value;
                    OnPropertyChanged(nameof(IsRefreshing));
                }
            }
        }
        public ICommand RefreshCommand => new Command(OnRefresh);
        public void OnRefresh()
        {
            InitContacts();
        }
        #endregion
        #region Delete Contact
        public ICommand DeleteContact => new Command<UserContact>(OnDeleteContact);
        public async void OnDeleteContact(UserContact uc) 
        {
            ContactsAPIProxy proxy = ContactsAPIProxy.CreateProxy();
            bool success = await proxy.RemoveContact(uc);
            if (success)
            {
                this.allContacts.Remove(uc);
                this.FilteredContacts.Remove(uc);
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("שגיאה", "שגיאה במחיקת איש קשר", "בסדר", FlowDirection.RightToLeft);
            }
        }
        #endregion
        #region Add New Contact
        public ICommand AddContact => new Command(OnAddContact);
        public async void OnAddContact()
        {
            App theApp = (App)App.Current;
            AddContactViewModel vm = new AddContactViewModel();
            vm.ContactUpdatedEvent += OnContactAdded;
            Page p = new Views.AddContact(vm);
            await theApp.MainPage.Navigation.PushAsync(p);
        }
        //This event is fired by the AddContact view model object and send the old contact to be removed and new contact to be added
        public void OnContactAdded(UserContact newUc, UserContact oldUc)
        {
            //Change the Phone type objects to be the same instance of the PhoneTypes in App level
            //This is a must in order to send the server the same objects
            foreach (ContactPhone cp in newUc.ContactPhones)
                cp.PhoneType = PhoneTypes.Where(pt => pt.TypeId == cp.PhoneTypeId).FirstOrDefault();

            //Add the new contact, remove the old one from both lists and refresh the filtered list
            this.allContacts.Remove(oldUc);
            this.allContacts.Add(newUc);
            this.FilteredContacts.Remove(oldUc);
            OnTextChanged(SearchTerm);
        }

        #endregion
        #region Update Existing Contact
        public ICommand UpdateContact => new Command<UserContact>(OnUpdateContact);
        public async void OnUpdateContact(UserContact uc)
        {
            if (uc != null)
            {
                App theApp = (App)App.Current;
                AddContactViewModel vm = new AddContactViewModel(uc);
                vm.ContactUpdatedEvent += OnContactAdded;
                Page p = new Views.AddContact(vm);
                await theApp.MainPage.Navigation.PushAsync(p);
                if (ClearSelection != null)
                    ClearSelection();
            }
        }

        public event Action ClearSelection;
        #endregion
    }
}
