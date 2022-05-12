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
            Patient = "Patient Name: ";
            ViewPatientWoundsPageCommand = new Command(async () => await ViewPatientsWoundsPage());
            TakeNewPhotoCommand = new Command(async () => await TakeNewPhoto());
            EnterAdditionalInfoCommand = new Command(async () => await EnterAdditionalWifiInfo());
            AboutCommand = new Command(async () => await AboutPageOpen());
            LogOutCommand = new Command(async () => await LogOutAction());
            // setPatientName();
        }

        internal async Task setPatientName()
        {
            WoundDatabase DB = (await WoundDatabase.Database);
            Guid patientID = DB.dataHolder.PatientID;
            DBPatient patient = await DB.GetPatient(patientID);
            Patient = "Patient Name: " + patient.PatientName;
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

            (AppShell.Current as AppShell).CleanWifi();
            await Shell.Current.GoToAsync($"//{nameof(WifiPage)}");
        }

        async Task AboutPageOpen()
        {
            await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");
        }
        async Task LogOutAction()
        {
            (AppShell.Current as AppShell).CleanAll();
            await Shell.Current.GoToAsync("//LoginPage");
        }

        private string patientName;
        public string Patient { get => patientName; set => SetProperty(ref patientName, value); }

        public ICommand ViewPatientWoundsPageCommand { get; }

        public ICommand TakeNewPhotoCommand { get; }

        public ICommand LogOutCommand { get; }

        public ICommand EnterAdditionalInfoCommand { get; }

        public ICommand AboutCommand { get; }
    }
}