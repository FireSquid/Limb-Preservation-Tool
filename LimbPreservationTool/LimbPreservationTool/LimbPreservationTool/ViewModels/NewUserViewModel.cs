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
            //ResetLoginMessage();
            //LoginCommand = new Command(OnLoginClicked);
            CreateUserCommand = new Command(OnCreateUserClicked);
            //CreatedAccountCommand = new Command(async () => await FinalizeAccount());
        }

        private async void OnCreateUserClicked(object obj)
        {
            if (await VerifyUserCreation())
            {
                CreationStatus = "User Created";
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            }
            else
            {
                CreationStatus = $"User Creation Failed - Username already exists";
            }
        }

        private async Task<bool> VerifyUserCreation()
        {
            return await NewUser.AttemptCreation(Username, Password);
        }

        private string usernameString;
        public string Username { get => usernameString; set => SetProperty(ref usernameString, value); }

        private string passwordString;
        public string Password { get => passwordString; set => SetProperty(ref passwordString, value); }

        private string creationStatus;
        public string CreationStatus { get => creationStatus; private set => SetProperty(ref creationStatus, value); }
        /*async Task FinalizeAccount()
        {
            await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
        }*/
        public ICommand CreatedAccountCommand { get; }

        public Command CreateUserCommand { get; }
     
    }
}
