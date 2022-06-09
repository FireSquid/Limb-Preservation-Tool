using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

using Xamarin.Forms;

using LimbPreservationTool.Views;
using LimbPreservationTool.Models;
using System.Threading.Tasks;

namespace LimbPreservationTool.ViewModels
{
    public class PatientsViewModel : BaseViewModel
    {

        public PatientsViewModel()
        {
            Title = "Patients";
            BacktoHome = new Command(async () => await Shell.Current.GoToAsync($"//{nameof(HomePage)}"));

            PatientDeleteMode = false;
        }

        public ICommand BacktoHome { get; }

        public async Task Initialize()
        {
            System.Diagnostics.Debug.WriteLine("Initializing Patient View");
            WoundDatabase db = await WoundDatabase.Database;

            /* CAUTION - Clears Entire Database
            await db.ClearDatabase();
            */

            /*
            await db.CreatePatient("Alice Johnson");
            await db.CreatePatient("Bob Smith");
            await db.CreatePatient("Alice Peterson");
            await db.CreatePatient("Greg Williams");
            await db.CreatePatient("Sarah Mason");
            */

            List<DBPatient> PatientsList = await db.GetPatientsList();            

            /*
            await db.SetWoundData(DBWoundData.Create().SetBase(PatientsList[0].PatientID, "Wound One").SetDate(DateTime.Today));
            await db.SetWoundData(DBWoundData.Create().SetBase(PatientsList[0].PatientID, "Wound One").SetDate(DateTime.Today.AddDays(-5)));
            await db.SetWoundData(DBWoundData.Create().SetBase(PatientsList[0].PatientID, "Wound Two").SetDate(DateTime.Today.AddDays(-3)));

            await db.SetWoundData(DBWoundData.Create().SetBase(PatientsList[1].PatientID, "Wound One").SetDate(DateTime.Today.AddDays(-4)));
            */

            PatientsListSource = PatientsList;

            System.Diagnostics.Debug.WriteLine("Finished Initializing");
        }

        public async Task UpdatePatientList()
        {
            System.Diagnostics.Debug.WriteLine("Updating Patient List");
            if (PatientEntry != null)
            {
                System.Diagnostics.Debug.WriteLine($"Updating List");
                var patients = (await (await WoundDatabase.Database).GetClosestPatient(PatientEntry));
                patients.Sort((pA, pB) => WoundDatabase.LevenshteinDist(pA.PatientName, PatientEntry) - WoundDatabase.LevenshteinDist(pB.PatientName, PatientEntry));
                PatientsListSource = patients;
            }
            else
            {
                PatientsListSource = await (await WoundDatabase.Database).GetPatientsList();
            }
            System.Diagnostics.Debug.WriteLine("Finished Updating Patient List");
        }

        public void ToggleDeleteMode()
        {
            PatientDeleteMode = !PatientDeleteMode;
        }

        private void UpdatePatientOptionColor(bool setting)
        {
            PatientOptionColor = (setting) ? Color.Red : Color.Black;
            ToggleDeleteColor = (setting) ? Color.Red : Color.FromHex("2196F3");
        }

        private List<DBPatient> _patientsListSource;
        public List<DBPatient> PatientsListSource { get => _patientsListSource; set => SetProperty(ref _patientsListSource, value); }

        public string Name = "Patients";

        private string _patientEntry;
        public string PatientEntry { 
            get => _patientEntry;
            set
            { 
                if (!value.Equals(_patientEntry))
                {                    
                    SetProperty(ref _patientEntry, value);
                    AsyncRunner.Run(UpdatePatientList());
                }                
            } 
        }

        private Color _patientOptionColor;
        public Color PatientOptionColor { get => _patientOptionColor; set => SetProperty(ref _patientOptionColor, value); }

        private Color _toggleDeleteColor;
        public Color ToggleDeleteColor { get => _toggleDeleteColor; set => SetProperty(ref _toggleDeleteColor, value); }

        private bool _patientDeleteMode;

        public bool PatientDeleteMode { 
            get => _patientDeleteMode; 
            private set {
                SetProperty(ref _patientDeleteMode, value);
                UpdatePatientOptionColor(value);
            } 
        }
    }
}