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
    public partial class WoundDataPage : ContentPage
    {
        WoundDataViewModel viewModel;

        string groupName;
        Guid patientID;

        public WoundDataPage()
        {
            InitializeComponent();
            viewModel = new WoundDataViewModel();
            this.BindingContext = viewModel;            
        }

        public void SetGroupName(string gn)
        {
            groupName = gn;
        }

        public void SetPatientID(Guid pID)
        {
            patientID = pID;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await viewModel.Initialize(groupName, patientID);
        }

        async void OnWoundGroupSelected(object sender, SelectedItemChangedEventArgs e)
        {

        }
    }
}
