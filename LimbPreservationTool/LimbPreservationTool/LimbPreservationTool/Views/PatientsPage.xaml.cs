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
        bool deleteConfirm = false;

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

                if (viewModel.PatientDeleteMode)
                {

                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        var result = await this.DisplayAlert("Warning!", "Are you sure you want to delete this patient?", "Yes", "No");

                        if (result)
                        {
                            await (await WoundDatabase.Database).DeletePatient(patient);

                            await viewModel.UpdatePatientList();
                        }
                    });

                    deleteConfirm = false;
                    viewModel.ToggleDeleteMode();

                }
                else
                {
                    (await WoundDatabase.Database).dataHolder.PatientID = patient.PatientID;

                    MessagingCenter.Send(this, "popModalToHome");
                    await Navigation.PopModalAsync();
                }
                 ((ListView)sender).SelectedItem = null;
            }
        }

        async void OnAddNewPatientClicked(object sender, EventArgs e)
        {
            NewPatientPage newPage = new NewPatientPage();
            await Navigation.PushModalAsync(newPage);
        }

        private void onToggleDeleteModeClicked(object sender, EventArgs e)
        {
            viewModel.ToggleDeleteMode();
        }
    }
}
