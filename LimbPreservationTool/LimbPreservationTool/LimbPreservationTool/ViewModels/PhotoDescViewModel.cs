using System;

using Xamarin.Forms;
using Xamarin.Essentials;

using System.Windows.Input;
using System.Threading.Tasks;
using LimbPreservationTool.Views;

namespace LimbPreservationTool.ViewModels
{
    public class PhotoDescViewModel : BaseViewModel
    {
        // default constructor
        public PhotoDescViewModel()
        {
            TakeNewPhotoCommand = new Command(async () => await Shell.Current.GoToAsync($"//{nameof(PhotoPage)}"));
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