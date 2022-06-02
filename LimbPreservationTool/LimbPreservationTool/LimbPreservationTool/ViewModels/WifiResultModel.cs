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
        // default constructor
        public WifiResultModel()
        {
            Title = "No results available, please fill out all fields on the previous page";
            EnterAdditionalInfoCommand = new Command(async () => await EnterAdditionalWifiInfo());
            BacktoHome = new Command(async () => await Shell.Current.GoToAsync($"//{nameof(HomePage)}"));

            AmputationInfo = "";
            AmputationColor = Color.Transparent;

            RevascInfo = "";
            RevascColor = Color.Transparent;
        }

        // constructor that passes in information from question page
        public WifiResultModel(string ampInfo, string revInfo, Color ampColor, Color revColor)
        {
            Title = "Wifi results";
            EnterAdditionalInfoCommand = new Command(async () => await EnterAdditionalWifiInfo());
            BacktoHome = new Command(async () => await Shell.Current.GoToAsync($"//{nameof(HomePage)}"));
            if (ampInfo == "VeryLow")
            {
                ampInfo = "Very Low";
            }

            if (revInfo == "VeryLow")
            {
                revInfo = "Very Low";
            }
            AmputationInfo = $"Your estimate risk for amputation at 1 year is: \n" + ampInfo;
            AmputationColor = ampColor;

            RevascInfo = $"Your estimate likelihood of benefit of/requirement for revascularization (assuming your infection can first be controlled) is: \n" + revInfo;
            RevascColor = revColor;
        }

        async Task EnterAdditionalWifiInfo()
        {
            await Shell.Current.GoToAsync($"//{nameof(WifiPage)}");
        }

        public ICommand EnterAdditionalInfoCommand { get; }

        public ICommand SaveWifiData { get; }

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