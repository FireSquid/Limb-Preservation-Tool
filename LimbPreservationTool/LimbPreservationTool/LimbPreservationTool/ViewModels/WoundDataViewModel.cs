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
        private int _sectionCount  ;

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
                _sectionCount = _gradeSections.Count; 
                UpdateCurrentChart();

                return true;
            }

            return false;
        }

        public void UpdateCurrentChart()
        {

            CurrentSectionName = _gradeSections.Keys.ToList()[_currentSection];
            WoundEntryChart = new LineChart { Entries = _gradeSections[_gradeSections.Keys.ToList()[_currentSection]], BackgroundColor = Extensions.ToSKColor(Color.Transparent),
                    Margin=30,LabelOrientation=Orientation.Horizontal,ValueLabelOrientation=Orientation.Horizontal, LabelTextSize = 40f };
                

        }
         
        public void NextChart()
        {
            _currentSection += 1;
            _currentSection %= _sectionCount;
            UpdateCurrentChart();
            Console.WriteLine(CurrentSectionName);
        }

        public void PreviousChart()
        {
            _currentSection -= 1;
            if (_currentSection < 0)
                _currentSection = _sectionCount - 1;
            UpdateCurrentChart();
            Console.WriteLine(CurrentSectionName);

        }

        public List<WoundDataDisplay> _woundDataListSource;
        public List<WoundDataDisplay> WoundDataListSource { get => _woundDataListSource; set => SetProperty(ref _woundDataListSource, value); }

        private LineChart _woundEntryChart;
        public LineChart WoundEntryChart { get => _woundEntryChart; set => SetProperty(ref _woundEntryChart, value); }

        public ICommand PreviousChartCommand{ get; }
        public ICommand NextChartCommand { get; }
 private String _currentSectionName; public String CurrentSectionName { get => _currentSectionName; set => SetProperty(ref _currentSectionName, value); } } 
    public class WoundDataChartList { 
        //private readonly List<ChartEntry> Wound; 
        //private readonly List<ChartEntry> Ischimia;
        private readonly List<ChartEntry> _area; 
        //private readonly List<ChartEntry> FootInfection;
        private float _sizeRange = (float)5.0; 
        private SKSurface palette; 
        private SKImageInfo _paletteInfo = new SKImageInfo(256, 256);
        private SKImageInfo _pixelInfo = new SKImageInfo(1, 1);

        public WoundDataChartList(List<DBWoundData> l) {
            //Wound = new List<ChartEntry>();
            //Ischimia = new List<ChartEntry>();
            _area = new List<ChartEntry>();
            //FootInfection = new List<ChartEntry>(); 
            using ( palette = SKSurface.Create(_paletteInfo))
            {
                SKCanvas canvas = palette.Canvas;

                canvas.Clear(SKColors.White);
                var colors = new SKColor[] {
                new SKColor(0, 255, 0),
                new SKColor(255, 0, 0)
            };
                var shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, 0),
                    new SKPoint(255, 255),
                    colors,
                    null,
                    SKShaderTileMode.Clamp);

                // use the shader
                var paint = new SKPaint
                {
                    Shader = shader
                };
                canvas.DrawPaint(paint);
            }
            l.ForEach(e=> { 
                Console.WriteLine( new DateTime(e.Date).ToShortDateString());
                 //Wound.Add(new ChartEntry(e.Wound) {Label =new DateTime(e.Date).ToShortDateString(),ValueLabel=e.Wound.ToString(), TextColor = Extensions.ToSKColor(Color.Black) });
                 //Ischimia.Add(new ChartEntry(e.Ischemia) { Label =new DateTime(e.Date).ToShortDateString(),ValueLabel=e.Ischemia.ToString(), TextColor = Extensions.ToSKColor(Color.Black)});
                if (e.Size >= 0)
                {

                    float percentage=0; 
                    if(_area.Count> 0)
                    {
                        float    diff = e.Size- _area.Last().Value;
                        percentage = diff / _area.Last().Value;
                     _area.Add(new ChartEntry(e.Size) { Label =new DateTime(e.Date).ToShortDateString(),ValueLabel= AreaText(e.Size.ToString("0.00"),percentage),TextColor=Extensions.ToSKColor(Color.Black) 
                     ,Color=AreaGradientColor(_area.Last().Value,e.Size,_sizeRange)  } );
                    }
                    else
                    {
                     _area.Add(new ChartEntry(e.Size) { Label =new DateTime(e.Date).ToShortDateString(),ValueLabel= AreaText(e.Size.ToString("0.00"),percentage),TextColor=Extensions.ToSKColor(Color.Black) 
                     ,Color=AreaGradientColor(e.Size-(float)0.5,e.Size,_sizeRange)} );

                    }

                }
                 //FootInfection.Add(new ChartEntry(e.Infection) { Label =new DateTime(e.Date).ToShortDateString(),ValueLabel=e.Infection.ToString(),TextColor = Extensions.ToSKColor(Color.Black)});
            });

             
        }

        private string AreaText(String size,float percentage)
        {
            size+="(" + (percentage*100).ToString("0.0") + "%)";
            return size;
        }
        private SKColor AreaGradientColor(float previous,float current, float diffrange)
        {

            return Extensions.ToSKColor(Color.Black);
            SKBitmap bitmap = new SKBitmap(_pixelInfo);
            IntPtr pixelBuffer = bitmap.GetPixels();
            int x = 128;
            int y= 128;
            var diff = previous - current;
            diff = diff / diffrange ;
            diff = diff < -1 ? -1 : diff;
            diff = diff > 1 ? 1 : diff;
            x += (int)(diff * 127); 
            y += (int)(diff * 127); 

            palette.ReadPixels(_pixelInfo, pixelBuffer, _pixelInfo.RowBytes,x,y);
            return bitmap.GetPixel(0, 0); 
        }
        public Dictionary<String,List<ChartEntry>> GetAllChartList() {
            Dictionary<String, List<ChartEntry>> all = new Dictionary<string, List<ChartEntry>>();
            //all.Add("Wound",new List<ChartEntry>(Wound));
            //all.Add("Ischemia",new List<ChartEntry>(Ischimia));
            all.Add("Area\n (inches^2)",new List<ChartEntry>(_area));
            //all.Add("Foot Infection",new List<ChartEntry>(FootInfection));
            return all;
        }
        
        private SKColor WoundColorGradient(float max, float min,float current)
        {
            if(max < min)
            {
                Console.WriteLine("max value for gradient is less than min.");
                return new SKColor();
            }
            float ratio =   (current - min) / (max - min);

            return  new SKColor();
        }
    
    }

    public class WoundDataDisplay
    {
        private DBWoundData _data;
        public DBWoundData Data { get => _data; set => _data = value; }


        private string _dateString;
        public string dateString { get => _dateString; set => _dateString = value; }


        private DateTime _date;
        public DateTime date { get => _date; set => _date = value; }

        public WoundDataDisplay(DBWoundData wd)
        {
            Data = wd;
            date = new DateTime(wd.Date);
            dateString = date.ToLongDateString();
        }
    }
}