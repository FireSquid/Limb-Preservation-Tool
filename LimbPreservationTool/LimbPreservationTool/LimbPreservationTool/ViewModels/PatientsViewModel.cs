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
        }

        public async Task Initialize()
        {
            WoundDatabase db = await WoundDatabase.Database;

            await db.DeleteAllPatients();

            await db.CreatePatient("Alice Johnson");
            await db.CreatePatient("Bob Smith");

            List<DBPatient> PatientsList = await db.GetPatientsList();

            await db.DeleteAllWoundData();

            await db.SetWoundData(DBWoundData.Create().SetBase(PatientsList[0].PatientID, "Wound One").SetDate(DateTime.Today));
            await db.SetWoundData(DBWoundData.Create().SetBase(PatientsList[0].PatientID, "Wound One").SetDate(DateTime.Today.AddDays(-5)));
            await db.SetWoundData(DBWoundData.Create().SetBase(PatientsList[0].PatientID, "Wound Two").SetDate(DateTime.Today.AddDays(-3)));

            await db.SetWoundData(DBWoundData.Create().SetBase(PatientsList[1].PatientID, "Wound One").SetDate(DateTime.Today.AddDays(-4)));

            PatientsListSource = PatientsList;
        }

        private List<DBPatient> _patientsListSource;
        public List<DBPatient> PatientsListSource { get => _patientsListSource; set => SetProperty(ref _patientsListSource, value); }

        public string Name = "Patients";
    }
}