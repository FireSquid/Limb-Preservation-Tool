using LimbPreservationTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microcharts;
using SkiaSharp.Views.Forms;
using SkiaSharp;
using Xamarin.Forms;

namespace LimbPreservationTool.ViewModels
{
    public class WoundDataViewModel : BaseViewModel
    {
        public WoundDataViewModel()
        {

        }

        public async Task<bool> Initialize(string groupName, Guid patientID)
        {
            Title = groupName;

            WoundDatabase db = await WoundDatabase.Database;

            var patientData = await db.GetAllPatientWoundData(patientID);

            if (patientData.ContainsKey(groupName))
            {
                WoundDataListSource = patientData[groupName].ConvertAll(wd => new WoundDataDisplay(wd));
                List<ChartEntry> e = new List<ChartEntry>();
                patientData[groupName].ForEach(f => { e.Add(new ChartEntry(f.Wound + f.Infection + f.Ischemia) { Label = "Date" }); });
                WoundEntryChart = new LineChart { Entries = e, BackgroundColor = Extensions.ToSKColor((Color)App.Current.Resources["MainBG"]) };
                return true;
            }

            return false;
        }


        public List<WoundDataDisplay> _woundDataListSource;
        public List<WoundDataDisplay> WoundDataListSource { get => _woundDataListSource; set => SetProperty(ref _woundDataListSource, value); }

        private LineChart _woundEntryChart;
        public LineChart WoundEntryChart { get => _woundEntryChart; set => SetProperty(ref _woundEntryChart, value); }
    }


    public class WoundDataDisplay
    {
        private DBWoundData _data;
        public DBWoundData data { get => _data; set => _data = value; }


        private string _dateString;
        public string dateString { get => _dateString; set => _dateString = value; }


        private DateTime _date;
        public DateTime date { get => _date; set => _date = value; }

        public WoundDataDisplay(DBWoundData wd)
        {
            data = wd;
            date = new DateTime(wd.Date);
            dateString = date.ToLongDateString();
        }
    }
}