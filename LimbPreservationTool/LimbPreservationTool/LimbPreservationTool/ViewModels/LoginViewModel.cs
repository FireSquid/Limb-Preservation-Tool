using LimbPreservationTool.Models;
using LimbPreservationTool.Views;
using System;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

using System.Windows.Input;


namespace LimbPreservationTool.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {

        public LoginViewModel()
        {
            ResetLoginMessage();
            LoginCommand = new Command(OnLoginClicked);
            NewUserCommand = new Command(async () => await NewUserPage());
        }

        private async void OnLoginClicked(object obj)
        {
            LoginStatus = "Authenticating Login Information...";
            try
            {
                if (await VerifyLoginEntry())
                {
                    LoginStatus = "Login Successful";
                    await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
                }
                else
                {
                    LoginStatus = $"Login Failed - Invalid Username or Password";
                }
            }
            catch (Exception ex)
            {
                LoginStatus = "Could Not Connect to the Server";
            }
        }

        private async Task<bool> VerifyLoginEntry()
        {
            try
            {
                return await Authentication.AttemptAuthentication(UsernameEntryField, PasswordEntryField);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ResetLoginMessage()
        {
            LoginStatus = "Awaiting Login...";
        }

        public void ClearLoginInformation()
        {
            UsernameEntryField = "";
            PasswordEntryField = "";
        }

        async Task NewUserPage()
        {
            await Shell.Current.GoToAsync("//NewUserPage");
        }

        public ICommand NewUserCommand { get; }

        public Command LoginCommand { get; }

        private string usernameEntryField;
        public string UsernameEntryField { get => usernameEntryField; set => SetProperty(ref usernameEntryField, value); }

        private string passwordEntryField;
        public string PasswordEntryField { get => passwordEntryField; set => SetProperty(ref passwordEntryField, value); }

        private string loginStatus;
        public string LoginStatus { get => loginStatus; private set => SetProperty(ref loginStatus, value); }
    }
}
