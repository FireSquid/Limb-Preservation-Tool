using LimbPreservationTool.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using LimbPreservationTool.Models;
using Xamarin.Forms;

namespace LimbPreservationTool.ViewModels
{
    public class LoginViewModel : BaseViewModel
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

        public LoginViewModel()
        {
            LoginCommand = new Command(OnLoginClicked);
        }

        private async void OnLoginClicked(object obj)
        {
            if (um == null)
            {
                //This is where the sqlite call will happen 
                um = await Doctor.CreateInstance(_userID, _userPassword);
            }

            // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
            await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");
        }
    }
}
