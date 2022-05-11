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
    public partial class PatientsPage : ContentPage
    {
        PatientsViewModel viewModel;

        public PatientsPage()
        {
            InitializeComponent();
            viewModel = new PatientsViewModel();
            this.BindingContext = viewModel;            
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await viewModel.Initialize();
        }

        async void OnPatientSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                DBPatient patient = (e.SelectedItem as DBPatient);
                (await WoundDatabase.Database).dataHolder.PatientID = patient.PatientID;
            }

            await Navigation.PopModalAsync();
        }

        private async void OnAddNewPatientClicked(object sender, EventArgs e)
        {
            NewPatientPage newPage = new NewPatientPage();
            await Navigation.PushModalAsync(newPage);
        }
    }
}
