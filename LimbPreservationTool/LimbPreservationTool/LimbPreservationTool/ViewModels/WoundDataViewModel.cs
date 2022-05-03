using LimbPreservationTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LimbPreservationTool.ViewModels
{
    public class WoundDataViewModel : BaseViewModel
    {
        public WoundDataViewModel()
        {
        }

        public async Task Initialize(string groupName, Guid patientID)
        {
            WoundDatabase db = await WoundDatabase.Database;

            WoundDataListSource = (await db.GetAllPatientWoundData(patientID))[groupName].ConvertAll(wd => new WoundDataDisplay(wd));
        }


        public List<WoundDataDisplay> _woundDataListSource;
        public List<WoundDataDisplay> WoundDataListSource { get => _woundDataListSource; set => SetProperty(ref _woundDataListSource, value); }
    }

    public class WoundDataDisplay 
    {
        private DBWoundData _data;
        public DBWoundData data { get => _data; set => _data = value; }


        private DateTime _date;
        public DateTime date { get => _date; set => _date = value; }

        public WoundDataDisplay(DBWoundData wd)
        {
            data = wd;
            date = new DateTime(wd.Date);
        }
    }
}