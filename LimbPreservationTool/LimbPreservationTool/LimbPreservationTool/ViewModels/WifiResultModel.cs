using System;

using Xamarin.Forms;
using Xamarin.Essentials;

using System.Windows.Input;
using System.Threading.Tasks;
using LimbPreservationTool.Views;

namespace LimbPreservationTool.ViewModels
{
    public class WifiResultModel : BaseViewModel
    {
        public WifiResultModel()
        {
            Title = "Wifi results";
            TakeNewPhotoCommand = new Command(async () => await TakeNewPhoto());
            EnterAdditionalInfoCommand = new Command(async () => await EnterAdditionalWifiInfo());
            AboutCommand = new Command(async () => await AboutPageOpen());

            // WifiStatus = $"Your estimate risk for amputation at 1 year is: \n" + amputationRisk + "\n" + "\nYour estimate requirement for revascularization is: \n" + revascularizationRisk;
            // longer message: "Your estimate likelihood of benefit of/requirement for revascularization (assuming your infection can first be controlled) is: 

            AmputationInfo = "Testing";
            AmputationColor = Color.Purple;

            RevascInfo = "More Testing";
            RevascColor = Color.Pink;
        }


        public WifiResultModel(string ampInfo, string revInfo, Color ampColor, Color revColor)
        {
            Title = "Wifi results";
            TakeNewPhotoCommand = new Command(async () => await TakeNewPhoto());
            EnterAdditionalInfoCommand = new Command(async () => await EnterAdditionalWifiInfo());
            AboutCommand = new Command(async () => await AboutPageOpen());
            BacktoHome = new Command(async () => await Shell.Current.GoToAsync($"//{nameof(HomePage)}"));

            // WifiStatus = $"Your estimate risk for amputation at 1 year is: \n" + amputationRisk + "\n" + "\nYour estimate requirement for revascularization is: \n" + revascularizationRisk;
            // longer message: "Your estimate likelihood of benefit of/requirement for revascularization (assuming your infection can first be controlled) is: 

            AmputationInfo = $"Your estimate risk for amputation at 1 year is: \n" + ampInfo;
            AmputationColor = ampColor;

            RevascInfo = $"Your estimate likelihood of benefit of/requirement for revascularization (assuming your infection can first be controlled) is: \n" + revInfo;
            RevascColor = revColor;
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

        public ICommand TakeNewPhotoCommand { get; }

        public ICommand EnterAdditionalInfoCommand { get; }

        public ICommand AboutCommand { get; }

        public ICommand BacktoHome { get; }


        private string amputationInfo;
        public string AmputationInfo { get => amputationInfo; private set => SetProperty(ref amputationInfo, value); }

        private Color amputationColor;
        public Color AmputationColor { get => amputationColor; private set => SetProperty(ref amputationColor, value); }

        private string revascInfo;
        public string RevascInfo { get => revascInfo; private set => SetProperty(ref revascInfo, value); }

        private Color revascColor;
        public Color RevascColor { get => revascColor; private set => SetProperty(ref revascColor, value); }
    }
}