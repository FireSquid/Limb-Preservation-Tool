using System;

using Xamarin.Forms;
using Xamarin.Essentials;

using System.Windows.Input;
using System.Threading.Tasks;
using LimbPreservationTool.Views;

namespace LimbPreservationTool.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public HomeViewModel()
        {
            Title = "Home";
            ViewPatientsPageCommand = new Command(async () => await ViewPatientsPage());
            TakeNewPhotoCommand = new Command(async () => await TakeNewPhoto());
            EnterAdditionalInfoCommand = new Command(async () => await EnterAdditionalWifiInfo());
            AboutCommand = new Command(async () => await AboutPageOpen());
        }

        async Task ViewPatientsPage()
        {
            await Shell.Current.GoToAsync($"//{nameof(PatientsPage)}");
        }

        async Task TakeNewPhoto()
        {
            await Shell.Current.GoToAsync($"//{nameof(PhotoPage)}");
        }

        async Task EnterAdditionalWifiInfo()
        {
            await Shell.Current.GoToAsync($"//{nameof(WifiPage)}");
        }

        async Task AboutPageOpen()
        {
            await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");
        }

        public ICommand ViewPatientsPageCommand { get; }

        public ICommand TakeNewPhotoCommand { get; }

        public ICommand EnterAdditionalInfoCommand { get; }

        public ICommand AboutCommand { get; }
    }
}