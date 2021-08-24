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
        public void OnDeleteContact(UserContact uc) { }
        #endregion
        #region Add New Contact
        public ICommand AddContact => new Command(OnAddContact);
        public void OnAddContact()
        {
            //App theApp = (App)App.Current;
            //Page p = new Views.AddEvent(this, this.allEvents);

            //await theApp.MainPage.Navigation.PushAsync(p);
        }
        
        #endregion
        #region Update Existing Contact
        public ICommand UpdateContact => new Command<UserContact>(OnUpdateContact);
        public void OnUpdateContact(UserContact uc)
        {
            //App theApp = (App)App.Current;
            //Page p = new Views.AddEvent(this, this.allEvents, ev);
            //await theApp.MainPage.Navigation.PushAsync(p);
            if (ClearSelection != null)
                ClearSelection();
        }

        public event Action ClearSelection;
        #endregion
    }
}
