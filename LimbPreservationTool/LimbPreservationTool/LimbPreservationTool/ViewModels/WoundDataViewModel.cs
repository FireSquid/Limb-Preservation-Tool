using LimbPreservationTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microcharts;
using SkiaSharp.Views.Forms;
using SkiaSharp;
using Xamarin.Forms;

namespace LimbPreservationTool.ViewModels
{
    public class WoundDataViewModel : BaseViewModel
    {
        private int _sectionCount = 4;

        private static int _currentSection = 0;
        private Dictionary<String,List<ChartEntry>> _gradeSections;
        public WoundDataViewModel()
        {
            NextChartCommand = new Command(() => NextChart());
            PreviousChartCommand = new Command(() => PreviousChart());
        }

        public async Task<bool> Initialize(string groupName, Guid patientID)
        {
            Title = groupName;

            WoundDatabase db = await WoundDatabase.Database;

            var patientData = await db.GetAllPatientWoundData(patientID);

            if (patientData.ContainsKey(groupName))
            {
                WoundDataListSource = patientData[groupName].ConvertAll(wd => new WoundDataDisplay(wd));

                //Enumerable.Range(0, 4).ToList().ForEach(e => {_gradeSections.Add(new List<ChartEntry>){ }; });
                List<ChartEntry> e = new List<ChartEntry>();
                //patientData[groupName].ForEach(f => { e.Add(new ChartEntry(f.Wound + f.Infection + f.Ischemia) { Label = "Date" , ValueLabel ="120",TextColor = Extensions.ToSKColor(Color.Black)}); });
                //WoundEntryChart = new LineChart { Entries = e, BackgroundColor = Extensions.ToSKColor(Color.Transparent),
                //    Margin=20,LabelOrientation=Orientation.Horizontal,ValueLabelOrientation=Orientation.Horizontal, LabelTextSize = 40 };
                //WoundEntryChart.MaxValue = 9; 
                //WoundEntryChart.MinValue = 3; 

                _currentSection = 0;
                _gradeSections = new  WoundDataChartList( patientData[groupName]).GetAllChartList();
                UpdateCurrentChart();

                return true;
            }

            return false;
        }

        public void UpdateCurrentChart()
        {

            CurrentSectionName = _gradeSections.Keys.ToList()[_currentSection];
            WoundEntryChart = new LineChart { Entries = _gradeSections[_gradeSections.Keys.ToList()[_currentSection]], BackgroundColor = Extensions.ToSKColor(Color.Transparent),
                    Margin=20,LabelOrientation=Orientation.Horizontal,ValueLabelOrientation=Orientation.Horizontal, LabelTextSize = 40 };

        }
         
        public void NextChart()
        {
            _currentSection += 1;
            _currentSection %= _sectionCount;
            UpdateCurrentChart();
        }

        public void PreviousChart()
        {
            _currentSection -= 1;
            if (_currentSection < 0)
                _currentSection = 3;
            UpdateCurrentChart();
        }

        public List<WoundDataDisplay> _woundDataListSource;
        public List<WoundDataDisplay> WoundDataListSource { get => _woundDataListSource; set => SetProperty(ref _woundDataListSource, value); }

        private LineChart _woundEntryChart;
        public LineChart WoundEntryChart { get => _woundEntryChart; set => SetProperty(ref _woundEntryChart, value); }

        public ICommand PreviousChartCommand{ get; }
        public ICommand NextChartCommand { get; }

        private String _currentSectionName;
        public String CurrentSectionName { get => _currentSectionName; set => SetProperty(ref _currentSectionName, value); }
    }
    
    

    public class WoundDataChartList {
        private readonly List<ChartEntry> Wound; 
        private readonly List<ChartEntry> Ischimia;
        private readonly List<ChartEntry> Area;
        private readonly List<ChartEntry> FootInfection; 
        public WoundDataChartList(List<DBWoundData> l) {
            Wound = new List<ChartEntry>();
            Ischimia = new List<ChartEntry>();
            Area = new List<ChartEntry>();
            FootInfection = new List<ChartEntry>(); 
            l.ForEach(e=> { 
                 Wound.Add(new ChartEntry(e.Wound) {Label =new DateTime(e.Date).ToShortDateString()}); 
                 Ischimia.Add(new ChartEntry(e.Ischemia) { Label =new DateTime(e.Date).ToShortDateString()});
                 Area.Add(new ChartEntry(e.Size) { Label =new DateTime(e.Date).ToShortDateString()});
                 FootInfection.Add(new ChartEntry(e.Infection) { Label =new DateTime(e.Date).ToShortDateString()});
            });

             
        }
        public Dictionary<String,List<ChartEntry>> GetAllChartList() {
            Dictionary<String, List<ChartEntry>> all = new Dictionary<string, List<ChartEntry>>();
            all.Add("Wound",new List<ChartEntry>(Wound));
            all.Add("Ischimia",new List<ChartEntry>(Ischimia));
            all.Add("Area",new List<ChartEntry>(Area));
            all.Add("FootInfection",new List<ChartEntry>(FootInfection));
            return all;
        }
        
        private SKColor WoundColorGradient(float max, float min,float current)
        {
            if(max < min)
            {
                Console.WriteLine("max value for gradient is less than min.");
                return new SKColor();
            }
            var colors = new SKColor[] {
                new SKColor(0, 0, 255),
                new SKColor(0, 255, 0)
            };
            var shader = SKShader.CreateLinearGradient(
                new SKPoint(0, 0),
                new SKPoint(255, 255),
                colors,
                null,
                SKShaderTileMode.Clamp);
        
            // use the shader
            var paint = new SKPaint {
                Shader = shader
            };
            float ratio =   (current - min) / (max - min);

            return  new SKColor();
        }
    
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