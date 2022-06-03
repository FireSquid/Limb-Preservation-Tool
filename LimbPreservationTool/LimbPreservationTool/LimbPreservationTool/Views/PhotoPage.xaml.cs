using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using LimbPreservationTool.ViewModels;
using System.Runtime.CompilerServices;

namespace LimbPreservationTool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PhotoPage : ContentPage
    {
        PhotoViewModel photoViewModel;

        public PhotoPage()
        {
            InitializeComponent();
            photoViewModel = new PhotoViewModel();
            photoViewModel.SetPhotoPage(this);
            this.BindingContext = photoViewModel;
        }

        private void OnSaveWifiData(object sender, EventArgs e)
        {
            var savePage = new WoundSavePage();
            Navigation.PushAsync(savePage);
        }

        public void PhotoErrorAlert()
        {
            DisplayAlert("Error Loading Image", "Please try taking the photo again or restarting the app", "OK");
        }
    }

}