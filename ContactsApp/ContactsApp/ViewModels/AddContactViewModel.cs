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
using Xamarin.Essentials;

namespace ContactsApp.ViewModels
{
    public static class ERROR_MESSAGES
    {
        public const string REQUIRED_FIELD = "זהו שדה חובה";
        public const string BAD_EMAIL = "מייל לא תקין";
    }
    public class AddContactViewModel : INotifyPropertyChanged
    {

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region פרטי שם
        private bool showNameError;

        public bool ShowNameError
        {
            get => showNameError;
            set
            {
                showNameError = value;
                OnPropertyChanged("ShowNameError");
            }
        }

        private string name;

        public string Name
        {
            get => name;
            set
            {
                name = value;
                ValidateName();
                OnPropertyChanged("Name");
            }
        }

        private string nameError;

        public string NameError
        {
            get => nameError;
            set
            {
                nameError = value;
                OnPropertyChanged("NameError");
            }
        }

        private void ValidateName()
        {
            this.ShowNameError = string.IsNullOrEmpty(Name);
        }
        #endregion

        #region משפחה שם
        private bool showLastNameError;

        public bool ShowLastNameError
        {
            get => showLastNameError;
            set
            {
                showLastNameError = value;
                OnPropertyChanged("ShowLastNameError");
            }
        }

        private string lastName;

        public string LastName
        {
            get => lastName;
            set
            {
                lastName = value;
                ValidateLastName();
                OnPropertyChanged("LastName");
            }
        }

        private string lastNameError;

        public string LastNameError
        {
            get => lastNameError;
            set
            {
                lastNameError = value;
                OnPropertyChanged("LastNameError");
            }
        }

        private void ValidateLastName()
        {
            this.ShowLastNameError = string.IsNullOrEmpty(LastName);
        }
        #endregion

        #region דואר אלקטרוני
        private bool showEmailError;

        public bool ShowEmailError
        {
            get => showEmailError;
            set
            {
                showEmailError = value;
                OnPropertyChanged("ShowEmailError");
            }
        }

        private string email;

        public string Email
        {
            get => email;
            set
            {
                email = value;
                ValidateEmail();
                OnPropertyChanged("Email");
            }
        }

        private string emailError;

        public string EmailError
        {
            get => emailError;
            set
            {
                emailError = value;
                OnPropertyChanged("EmailError");
            }
        }

        private void ValidateEmail()
        {
            this.ShowEmailError = string.IsNullOrEmpty(Email);
            if (!this.ShowEmailError)
            {
                if (!Regex.IsMatch(this.Email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"))
                {
                    this.ShowEmailError = true;
                    this.EmailError = ERROR_MESSAGES.BAD_EMAIL;
                }
            }
            else
                this.EmailError = ERROR_MESSAGES.REQUIRED_FIELD;
        }
        #endregion

        #region מקור התמונה
        private string contactImgSrc;

        public string ContactImgSrc
        {
            get => contactImgSrc;
            set
            {
                contactImgSrc = value;
                OnPropertyChanged("ContactImgSrc");
            }
        }
        private const string DEFAULT_PHOTO_SRC = "defaultPhoto.jpg";
        #endregion
        #region רשימת טלפונים
        ObservableCollection<Models.ContactPhone> contactPhones;
        public ObservableCollection<Models.ContactPhone> ContactPhones
        {
            get
            {
                return this.contactPhones;
            }
            set
            {
                if (value != this.contactPhones)
                {
                    this.contactPhones = value;
                    OnPropertyChanged("ContactPhones");
                }
            }
        }
        #endregion

        //This contact is a reference to the updated or new created contact
        private UserContact theContact;
        //For adding a new contact, uc will be null
        //For updates the user contact object should be sent to the constructor
        public AddContactViewModel(UserContact uc = null)
        {
            //create a new user contact if this is an add operation
            if (uc == null)
            {
                App theApp = (App)App.Current;
                uc = new UserContact()
                {
                    UserId = theApp.CurrentUser.Id,
                    FirstName = "",
                    LastName = "",
                    Email = "",
                    ContactPhones = new List<Models.ContactPhone>()
                };

                //Setup default image photo
                this.ContactImgSrc = DEFAULT_PHOTO_SRC;
            }
            else
            {
                //set the path url to the contact photo
                ContactsAPIProxy proxy = ContactsAPIProxy.CreateProxy();
                this.ContactImgSrc = proxy.GetBasePhotoUri() + uc.ContactId + ".jpg";
            }

            this.theContact = uc;
            this.NameError = ERROR_MESSAGES.REQUIRED_FIELD;
            this.LastNameError = ERROR_MESSAGES.REQUIRED_FIELD;
            this.EmailError = ERROR_MESSAGES.BAD_EMAIL;

            this.ShowNameError = false;
            this.ShowLastNameError = false;
            this.ShowEmailError = false;
            this.ContactPhones = new ObservableCollection<Models.ContactPhone>(uc.ContactPhones);
            this.SaveDataCommand = new Command(() => SaveData());
            this.Name = uc.FirstName;
            this.LastName = uc.LastName;
            this.Email = uc.Email;
        }

        //This function validate the entire form upon submit!
        private bool ValidateForm()
        {
            //Validate all fields first
            ValidateLastName();
            ValidateName();
            ValidateEmail();

            //check if any validation failed
            if (ShowLastNameError ||
                ShowNameError || ShowEmailError)
                return false;
            return true;
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

        //This event is fired after the new contact is generated in the system so it can be added to the list of contacts
        public event Action<UserContact, UserContact> ContactUpdatedEvent;

        //The command for saving the contact
        public Command SaveDataCommand { protected set; get; }
        private async void SaveData()
        {
            if (ValidateForm())
            {

                this.theContact.FirstName = this.Name;
                this.theContact.LastName = this.LastName;
                this.theContact.Email = this.Email;
                this.theContact.ContactPhones = this.ContactPhones.ToList();
                

                ServerStatus = "מתחבר לשרת...";
                await App.Current.MainPage.Navigation.PushModalAsync(new Views.ServerStatusPage(this));
                ContactsAPIProxy proxy = ContactsAPIProxy.CreateProxy();
                UserContact newUC = await proxy.UpdateContact(this.theContact);
                if (newUC == null)
                {
                    await App.Current.MainPage.DisplayAlert("שגיאה", "שמירת איש הקשר נכשלה", "בסדר");
                    await App.Current.MainPage.Navigation.PopModalAsync();
                }
                else
                {
                    if (this.imageFileResult != null)
                    {
                        ServerStatus = "מעלה תמונה...";
                        bool success = await proxy.UploadImage(new FileInfo()
                        {
                            Name = this.imageFileResult.FullPath
                        }, $"{newUC.ContactId}.jpg"); 
                    }
                    ServerStatus = "שומר נתונים...";
                    //if someone registered to get the contact added event, fire the event
                    if (this.ContactUpdatedEvent != null)
                    {
                        this.ContactUpdatedEvent(newUC, this.theContact);
                    }
                    //close the message and add contact windows!
                    
                    await App.Current.MainPage.Navigation.PopAsync();
                    await App.Current.MainPage.Navigation.PopModalAsync();
                }
            }
            else
                await App.Current.MainPage.DisplayAlert("שמירת נתונים", " יש בעיה עם הנתונים בדוק ונסה שוב", "אישור", FlowDirection.RightToLeft);
        }

        //The following command deletes a phone from the observable phone list
        public ICommand DeletePhoneCommand => new Command<Models.ContactPhone>(OnDeletePhone);
        public async void OnDeletePhone(Models.ContactPhone phone)
        {
            //check if the phone has an id. if so, it is not new and should be deleted from server
            if (phone.PhoneId > 0)
            {
                ContactsAPIProxy proxy = ContactsAPIProxy.CreateProxy();
                bool success = await proxy.RemoveContactPhone(phone);
                if (success)
                {
                    this.ContactPhones.Remove(phone);
                    this.theContact.ContactPhones.Remove(phone);
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("שגיאה", "שגיאה במחיקת טלפון", "בסדר", FlowDirection.RightToLeft);
                }
            }
            else //delete phone locally
                this.ContactPhones.Remove(phone);
        }

        //The following command open a page to add a new phone
        public ICommand AddPhoneCommand => new Command(OnAddPhone);
        public async void OnAddPhone()
        {
            Page p = new Views.AddPhone();
            p.BindingContext = new AddPhoneViewModel(ContactPhones);
            await App.Current.MainPage.Navigation.PushModalAsync(p);
        }

        ///The following command handle the pick photo button
        FileResult imageFileResult;
        public event Action<ImageSource> SetImageSourceEvent;
        public ICommand PickImageCommand => new Command(OnPickImage);
        public async void OnPickImage()
        {
            FileResult result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions()
            {
                Title = "בחר תמונה"
            });

            if (result != null)
            {
                this.imageFileResult = result;
                
                var stream = await result.OpenReadAsync();
                ImageSource imgSource  = ImageSource.FromStream(() => stream);
                if (SetImageSourceEvent != null)
                    SetImageSourceEvent(imgSource);
            }
        }

        ///The following command handle the take photo button
        public ICommand CameraImageCommand => new Command(OnCameraImage);
        public async void OnCameraImage()
        {
            var result = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions()
            {
                Title = "צלם תמונה"
            });

            if (result != null)
            {
                this.imageFileResult = result;
                var stream = await result.OpenReadAsync();
                ImageSource imgSource = ImageSource.FromStream(() => stream);
                if (SetImageSourceEvent != null)
                    SetImageSourceEvent(imgSource);
            }
        }


    }
}
