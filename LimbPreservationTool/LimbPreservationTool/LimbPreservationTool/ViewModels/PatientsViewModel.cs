using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

using LimbPreservationTool.Models;
using System.Threading.Tasks;

namespace LimbPreservationTool.ViewModels
{
    public class PatientsViewModel : BaseViewModel
    {

        public PatientsViewModel()
        {
            Title = "Patients";
        }

        public async Task Initialize()
        {
            WoundDatabase db = await WoundDatabase.Database;

            /* CAUTION - Clears Entire Database
            await db.DeleteAllWoundData();
            await db.DeleteAllPatients();
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
        }

        private async Task UpdatePatientList()
        {
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
    }
}