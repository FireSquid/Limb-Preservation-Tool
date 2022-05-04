using LimbPreservationTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

using LimbPreservationTool.Models;
using System.Threading.Tasks;

namespace LimbPreservationTool.ViewModels
{
    public class WoundSaveViewModel : BaseViewModel
    {
        public WoundSaveViewModel()
        {
        }

        private async Task UpdatePatientList()
        {
            if (PatientName.Length > 0)
            {
                PatientsListIsVisible = true;
                System.Diagnostics.Debug.WriteLine($"Updating List");
                var patients = (await (await WoundDatabase.Database).GetClosestPatient(PatientName));
                patients.Sort((pA, pB) => WoundDatabase.LevenshteinDist(pA.PatientName, PatientName) - WoundDatabase.LevenshteinDist(pB.PatientName, PatientName));
                PatientsList = patients;
                foreach (var p in PatientsList)
                {
                    System.Diagnostics.Debug.WriteLine($"\t{p.PatientName}");
                }
            }
            else
            {
                PatientsListIsVisible = false;
            }
        }

        private string _patientName;
        public string PatientName { get => _patientName;
            set
            {
                SetProperty(ref _patientName, value);
                AsyncRunner.Run(UpdatePatientList());
            }
        }


        private List<DBPatient> _patientList;
        public List<DBPatient> PatientsList { get => _patientList; set => SetProperty(ref _patientList, value); }

        private bool _patientsListIsVisible;
        public bool PatientsListIsVisible { get => _patientsListIsVisible; set => SetProperty(ref _patientsListIsVisible, value); }
    }
}