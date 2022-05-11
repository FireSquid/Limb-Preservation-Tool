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
        }

        public ICommand BacktoHome { get; }

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

        private List<DBPatient> _patientsListSource;
        public List<DBPatient> PatientsListSource { get => _patientsListSource; set => SetProperty(ref _patientsListSource, value); }

        public string Name = "Patients";
    }
}