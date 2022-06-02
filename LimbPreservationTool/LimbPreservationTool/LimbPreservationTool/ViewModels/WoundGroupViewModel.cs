using LimbPreservationTool.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using LimbPreservationTool.Views;
using System.Windows.Input;



namespace LimbPreservationTool.ViewModels
{
    public class WoundGroupViewModel : BaseViewModel
    {
        public WoundGroupViewModel()
        {
            BacktoHome = new Command(async () => await Shell.Current.GoToAsync($"//{nameof(HomePage)}"));
        }
        public ICommand BacktoHome { get; }

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