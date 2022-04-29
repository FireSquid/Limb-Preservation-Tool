using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

using LimbPreservationTool.Views;



namespace LimbPreservationTool.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = "About";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://aka.ms/xamarin-quickstart"));
            BacktoHome = new Command(async () => await Shell.Current.GoToAsync($"//{nameof(HomePage)}"));
        }

        public ICommand OpenWebCommand { get; }

        public ICommand BacktoHome { get; }
    }
}