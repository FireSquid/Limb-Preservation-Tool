using LimbPreservationTool.Views;
using System;
using Xamarin.Forms;
using LimbPreservationTool.Services;
using Xamarin.Essentials;
using Xamarin.Forms.Xaml;
using SkiaSharp;

namespace LimbPreservationTool
{
    public partial class App : Application
    {

        public static IScaler scalerInterface { get; private set; }

        public static SKBitmap unexaminedImage { get; set; }
        public static SKBitmap examinedImage { get; set; }

        public App()
        {

            //if (Device.RuntimePlatform.Equals(Device.iOS))
            //{

            //    scalerInterface = DependencyService.Get<IScaler>();


            //}

            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new AppShell();
        }



        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }

}
