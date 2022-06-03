using System;

using Xamarin.Forms;
using Xamarin.Essentials;

using System.Windows.Input;
using System.Threading.Tasks;
using LimbPreservationTool.Views;

namespace LimbPreservationTool.ViewModels
{
    public class WifiDescViewModel : BaseViewModel
    {
        // default constructor
        public WifiDescViewModel()
        {
            TakeNewPhotoCommand = new Command(async () => await Shell.Current.GoToAsync($"//{nameof(WifiPage)}"));
            BacktoHome = new Command(async () => await Shell.Current.GoToAsync($"//{nameof(HomePage)}"));
        }

        async Task TakeNewPhoto()
        {
            await Shell.Current.GoToAsync($"//{nameof(PhotoPage)}");
        }

        public ICommand TakeNewPhotoCommand { get; }

        public ICommand BacktoHome { get; }
    }
}