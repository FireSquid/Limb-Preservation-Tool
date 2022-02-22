using LimbPreservationTool.Views;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace LimbPreservationTool.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {      

        public LoginViewModel()
        {
            LoginStatus = "Awaiting Login";
            LoginCommand = new Command(OnLoginClicked);
        }

        private async void OnLoginClicked(object obj)
        {
            if (VerifyLoginEntry())
            {
                LoginStatus = "Login Successful";
                await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
            }

            LoginStatus = "Login Failed - Invalid Username or Password";
        }

        private bool VerifyLoginEntry()
        {
            if (UsernameEntryField == "Admin" && PasswordEntryField == "12345")
            {
                return true;
            }

            return false;
        }

        public Command LoginCommand { get; }

        private string usernameEntryField;
        public string UsernameEntryField { get => usernameEntryField; set => SetProperty(ref usernameEntryField, value); }

        private string passwordEntryField;
        public string PasswordEntryField { get => passwordEntryField; set => SetProperty(ref passwordEntryField, value); }

        private string loginStatus;
        public string LoginStatus { get => loginStatus; set => SetProperty(ref loginStatus, value); }
    }
}
