using LimbPreservationTool.Models;
using LimbPreservationTool.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace LimbPreservationTool.ViewModels
{
    public class DataDisplayViewModel : BaseViewModel
    {
        public DataDisplayViewModel()
        {
            BacktoHome = new Command(async () => await Shell.Current.GoToAsync($"//{nameof(HomePage)}"));
        }

        public ICommand BacktoHome { get; }

        private async Task UpdatePatientName(Guid patientID)
        {
            PatientName = "Patient Name: " + (await (await WoundDatabase.Database).GetPatient(patientID)).PatientName;
        }

        private DBWoundData _woundData;
        public DBWoundData WoundData
        {
            get => _woundData;
            set
            {
                System.Diagnostics.Debug.WriteLine("Setting Wound Data");
                SetProperty(ref _woundData, value);

                if (value != null)
                {
                    AsyncRunner.Run(UpdatePatientName(value.PatientID));

                    DataDate = new DateTime(value.Date).ToLongDateString();

                    ShowWifi = ((value.Wound >= 0) || (value.Ischemia >= 0) || (value.Infection >= 0));

                    ShowWound = (value.Size >= 0);
                    System.Diagnostics.Debug.WriteLine("Set Wound Data");
                }
            }
        }

        private bool _showWifi;
        public bool ShowWifi { get => _showWifi; set => SetProperty(ref _showWifi, value); }

        private bool _showWound;
        public bool ShowWound { get => _showWound; set => SetProperty(ref _showWound, value); }

        private string _patientName;
        public string PatientName { get => _patientName; set => SetProperty(ref _patientName, value); }

        private string _dataDate;
        public string DataDate { get => _dataDate; set => SetProperty(ref _dataDate, value); }
    }
}