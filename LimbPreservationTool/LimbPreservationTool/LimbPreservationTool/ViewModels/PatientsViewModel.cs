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

            PatientsListSource = await db.GetPatientsList();

            foreach (var patient in PatientsListSource)
            {
                System.Diagnostics.Debug.WriteLine($"Deleting Patient: {patient.PatientName} - {patient.PatientID}");
                await db.DeletePatient(patient);
            }

            await db.CreatePatient("Alice");
            await db.CreatePatient("Bob");

            System.Diagnostics.Debug.WriteLine("Updating Patients List Source");

            PatientsListSource = await db.GetPatientsList();

            foreach (var patient in PatientsListSource)
            {
                System.Diagnostics.Debug.WriteLine($"Created Patient: {patient.PatientName} - {patient.PatientID}");
            }
        }

        private List<DBPatient> _patientsListSource;
        public List<DBPatient> PatientsListSource { get => _patientsListSource; set => SetProperty(ref _patientsListSource, value); }

        public string Name = "Patients";
    }
}