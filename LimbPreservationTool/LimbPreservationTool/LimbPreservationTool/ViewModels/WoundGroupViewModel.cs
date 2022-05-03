using LimbPreservationTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LimbPreservationTool.ViewModels
{
    public class WoundGroupViewModel : BaseViewModel
    {
        public WoundGroupViewModel()
        {
        }

        public async Task Initialize(DBPatient patient)
        {
            Title = patient.PatientName;

            WoundDatabase db = await WoundDatabase.Database;

            WoundGroupListSource = (await db.GetAllPatientWoundData(patient.PatientID)).ToList();

            foreach (var woundGroup in WoundGroupListSource)
            {
                System.Diagnostics.Debug.WriteLine($"{patient.PatientName} Group: {woundGroup.Key} - {woundGroup.Value.Count}");
            }
        }

        private List<KeyValuePair<string, List<DBWoundData>>> _woundGroupSource;
        public List<KeyValuePair<string, List<DBWoundData>>> WoundGroupListSource { get => _woundGroupSource; set => SetProperty(ref _woundGroupSource, value); }
    }
}