using LimbPreservationTool.Models;
using LimbPreservationTool.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LimbPreservationTool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WoundGroupPage : ContentPage
    {
        WoundGroupViewModel viewModel;

        DBPatient patient;

        public WoundGroupPage()
        {
            InitializeComponent();
            viewModel = new WoundGroupViewModel();
            this.BindingContext = viewModel;            
        }

        public void SetPatient(DBPatient p)
        {
            patient = p;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await viewModel.Initialize(patient);
        }

        async void OnWoundGroupSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                WoundDataPage newPage = new WoundDataPage();
                newPage.SetGroupName(((KeyValuePair<string, List<DBWoundData>>)e.SelectedItem).Key);
                newPage.SetPatientID(patient.PatientID);
                await Navigation.PushAsync(newPage);
            }
        }
    }
}
