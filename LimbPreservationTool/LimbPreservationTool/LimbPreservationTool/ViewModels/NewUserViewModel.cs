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
            CreatedAccountCommand = new Command(OnCreateUserClicked);
            BacktoLoginCommand = new Command(async () => await GoHome());
            CreationStatus = $"Please create a new account";
        }

        private async void OnCreateUserClicked(object obj)
        {
            if (await VerifyUserCreation())
            {
                CreationStatus = $"New User Created!";
                await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
            }
            else
            {
                CreationStatus = $"User Creation Failed - Username already exists";
            }
        }

        private async Task<bool> VerifyUserCreation()
        {
            CreationStatus = $"Verifying...";
            return await NewUser.AttemptCreation(Username, Password, Name, Email);
        }

        async Task GoHome()
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }

        private string usernameString;
        public string Username { get => usernameString; set => SetProperty(ref usernameString, value); }

        private string passwordString;
        public string Password { get => passwordString; set => SetProperty(ref passwordString, value); }

        private string nameString;
        public string Name { get => nameString; set => SetProperty(ref nameString, value); }

        private string emailString;
        public string Email { get => emailString; set => SetProperty(ref emailString, value); }

        private string creationStatus;
        public string CreationStatus { get => creationStatus; private set => SetProperty(ref creationStatus, value); }

        public ICommand CreatedAccountCommand { get; }
        public ICommand BacktoLoginCommand { get; }

    }
}
