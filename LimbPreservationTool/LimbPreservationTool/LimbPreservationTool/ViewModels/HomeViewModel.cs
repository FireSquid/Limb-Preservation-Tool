using System;

using Xamarin.Forms;
using Xamarin.Essentials;

using System.Windows.Input;
using System.Threading.Tasks;
using LimbPreservationTool.Views;
using LimbPreservationTool.Models;

namespace LimbPreservationTool.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public HomeViewModel()
        {
            Title = "Home";
            ViewPatientWoundsPageCommand = new Command(async () => await ViewPatientsWoundsPage());
            TakeNewPhotoCommand = new Command(async () => await TakeNewPhoto());
            EnterAdditionalInfoCommand = new Command(async () => await EnterAdditionalWifiInfo());
            AboutCommand = new Command(async () => await AboutPageOpen());
            LogOutCommand = new Command(async () => await LogOutAction());
        }

        async Task ViewPatientsWoundsPage()
        {
            await Shell.Current.GoToAsync($"//{nameof(WoundGroupPage)}");
        }

        async Task TakeNewPhoto()
        {
            await Shell.Current.GoToAsync($"//{nameof(PhotoDescPage)}");
        }

        async Task EnterAdditionalWifiInfo()
        {
            await Shell.Current.GoToAsync($"//{nameof(WifiPage)}");
        }

        async Task AboutPageOpen()
        {
            await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");
        }
        async Task LogOutAction()
        {

            PhotoViewModel p = (PhotoViewModel)App.Current.Resources["sharedPhotoViewModel"];
            p.EraseAll();
            (await WoundDatabase.Database).dataHolder = DBWoundData.Create(Guid.Empty);
            await Shell.Current.GoToAsync("//LoginPage");
        }

        public ICommand ViewPatientWoundsPageCommand { get; }

        public ICommand TakeNewPhotoCommand { get; }

        public ICommand LogOutCommand { get; }

        public ICommand EnterAdditionalInfoCommand { get; }

        public ICommand AboutCommand { get; }
    }
}