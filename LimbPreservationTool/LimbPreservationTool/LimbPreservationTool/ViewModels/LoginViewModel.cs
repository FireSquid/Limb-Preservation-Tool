using LimbPreservationTool.Models;
using LimbPreservationTool.Views;
using System;
using System.Collections.Generic;
using System.Text;
<<<<<<< HEAD
using System.ComponentModel;
using LimbPreservationTool.Models;
=======
using System.Threading.Tasks;
>>>>>>> dev
using Xamarin.Forms;

namespace LimbPreservationTool.ViewModels
{
    public class LoginViewModel : BaseViewModel
<<<<<<< HEAD
    {
        public event PropertyChangedEventHandler PropertyChanged;
        //void OnPropertyChanged(string name){
        //	PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));	
        //}
        public Command LoginCommand { get; }
        private String _userID;
        private String _userPassword;
        private static Doctor um;
        public String UserID
        {
            get { return _userID; }
            set
            {
                if (_userID == value) { return; }
                _userID = value;
                OnPropertyChanged(nameof(UserID));
            }
        }
        public String UserPassword
        {
            get { return _userPassword; }
            set
            {
                if (_userPassword == value) { return; }
                _userPassword = value;
                OnPropertyChanged(nameof(UserPassword));
            }
        }
=======
    {      
>>>>>>> dev

        public LoginViewModel()
        {
            ResetLoginMessage();
            LoginCommand = new Command(OnLoginClicked);
        }

        private async void OnLoginClicked(object obj)
        {
<<<<<<< HEAD
            if (um == null)
            {
                //This is where the sqlite call will happen 
                um = await Doctor.CreateInstance(_userID, _userPassword);
            }

            // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
            await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");
=======
            LoginStatus = "Authenticating Login Information...";
            if (await VerifyLoginEntry())
            {
                LoginStatus = "Login Successful";
                await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
            }
            else
            {
                LoginStatus = $"Login Failed - Invalid Username or Password";
            }
>>>>>>> dev
        }

        private async Task<bool> VerifyLoginEntry()
        {
            return await Authentication.AttemptAuthentication(UsernameEntryField, PasswordEntryField);
        }

        public void ResetLoginMessage()
        {
            LoginStatus = "Awaiting Login";
        }

        public void ClearLoginInformation()
        {
            UsernameEntryField = "";
            PasswordEntryField = "";
        }

        public Command LoginCommand { get; }

        private string usernameEntryField;
        public string UsernameEntryField { get => usernameEntryField; set => SetProperty(ref usernameEntryField, value); }

        private string passwordEntryField;
        public string PasswordEntryField { get => passwordEntryField; set => SetProperty(ref passwordEntryField, value); }

        private string loginStatus;
        public string LoginStatus { get => loginStatus; private set => SetProperty(ref loginStatus, value); }
    }
}
