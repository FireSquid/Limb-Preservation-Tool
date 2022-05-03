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
    public class NewUserViewModel : BaseViewModel
    {

        public NewUserViewModel()
        {
            ResetLoginMessage();
            LoginCommand = new Command(OnLoginClicked);
            CreatedAccountCommand = new Command(async () => await FinalizeAccount());
        }

        private string ischemiaGradeString;
        public string IschemiaGrade { get => ischemiaGradeString; set => SetProperty(ref ischemiaGradeString, value); }

        // grades to calculate ischemia
        private string toePressureGradeString;
        public string ToePressureGrade { get => toePressureGradeString; set => SetProperty(ref toePressureGradeString, value); }

        private async void OnLoginClicked(object obj)
        {
            LoginStatus = "Authenticating Login Information...";
            //if (await VerifyLoginEntry())
            if (true)
            {
                LoginStatus = "Login Successful";
                await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
            }
            else
            {
                LoginStatus = $"Login Failed - Invalid Username or Password";
            }
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

        async Task FinalizeAccount()
        {
            await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
        }

        public ICommand CreatedAccountCommand { get; }

        public Command LoginCommand { get; }

        private string usernameEntryField;
        public string UsernameEntryField { get => usernameEntryField; set => SetProperty(ref usernameEntryField, value); }

        private string passwordEntryField;
        public string PasswordEntryField { get => passwordEntryField; set => SetProperty(ref passwordEntryField, value); }

        private string loginStatus;
        public string LoginStatus { get => loginStatus; private set => SetProperty(ref loginStatus, value); }
    }
}
