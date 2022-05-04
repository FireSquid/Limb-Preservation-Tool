using LimbPreservationTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace LimbPreservationTool.ViewModels
{
    public class NewPatientViewModel : BaseViewModel
    {
        public NewPatientViewModel()
        {
        }

        public async Task CreatePatient()
        {
            await (await WoundDatabase.Database).CreatePatient(PatientName);
        }


        private string _patientName;
        public string PatientName { get => _patientName; set => SetProperty(ref _patientName, value); }
    }
}