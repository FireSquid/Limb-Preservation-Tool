using LimbPreservationTool.Models;
using LimbPreservationTool.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LimbPreservationTool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DataDisplayPage : ContentPage
    {
        DataDisplayViewModel viewModel;

        public DataDisplayPage()
        {
            InitializeComponent();
            viewModel = new DataDisplayViewModel();
            this.BindingContext = viewModel;
        }

        public void SetWoundData(DBWoundData wd)
        {
            System.Diagnostics.Debug.WriteLine("Setting View Models Wound Data");
            viewModel.WoundData = wd;
        }

        private void OnDeleteWoundClicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var result = await this.DisplayAlert("Warning!", "Are you sure you want to delete this wound?", "Yes", "No");

                if (result)
                {
                    AsyncRunner.Run(WoundDatabase.Database.GetAwaiter().GetResult().DeleteWoundData(viewModel.WoundData));

                    System.Diagnostics.Debug.WriteLine("Deleted Wound Data");
                    Navigation.PopAsync();

                }
            });

            
        }
    }
}
