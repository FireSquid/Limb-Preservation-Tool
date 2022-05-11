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

        public WoundGroupPage()
        {
            InitializeComponent();
            viewModel = new WoundGroupViewModel();
            this.BindingContext = viewModel;            
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            WoundDatabase DB = (await WoundDatabase.Database);

            Guid patientID = DB.dataHolder.PatientID;
            
            if (patientID != null && patientID != Guid.Empty)
            {
                DBPatient patient = await DB.GetPatient(patientID);
                await viewModel.Initialize(patient);
            }
            else
            {
                PatientsPage patientSelectionPage = new PatientsPage();
                await Navigation.PushModalAsync(patientSelectionPage);
            }            
        }

        async void OnWoundGroupSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                Guid patientID = (await WoundDatabase.Database).dataHolder.PatientID;

                if (patientID != null && patientID != Guid.Empty)
                {
                    WoundDataPage newPage = new WoundDataPage();
                    newPage.SetGroupName(((KeyValuePair<string, List<DBWoundData>>)e.SelectedItem).Key);
                    newPage.SetPatientID(patientID);
                    await Navigation.PushAsync(newPage);
                }                    
            }
        }
    }
}
