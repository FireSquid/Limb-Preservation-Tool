using LimbPreservationTool.Models;
using LimbPreservationTool.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LimbPreservationTool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WoundSavePage : ContentPage
    {
        WoundSaveViewModel viewModel;


        public WoundSavePage()
        {
            InitializeComponent();
            viewModel = new WoundSaveViewModel();
            this.BindingContext = viewModel;
        }

        async void OnSavePatientSelected(object sender, SelectedItemChangedEventArgs e)
        {
            viewModel.Patient = e.SelectedItem as DBPatient;
        }

        private void OnWoundGroupSelected(object sender, SelectedItemChangedEventArgs e)
        {
            viewModel.WoundData = DBWoundData.Create().SetBase(viewModel.Patient.PatientID, (e.SelectedItem as WoundGroup).Name);
        }

        private void OnSaveWoundGroupClicked(object sender, EventArgs e)
        {
            viewModel.CreateNewWound();
        }

        async void OnConfirmSaveDataClicked(object sender, EventArgs e)
        {
            await viewModel.ConfirmSaveData();
            await Navigation.PopAsync();
        }
    }

}