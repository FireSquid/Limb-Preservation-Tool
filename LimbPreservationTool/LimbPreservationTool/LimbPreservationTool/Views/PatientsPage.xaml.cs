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
                    infectionView.IsVisible = true;
                    await Task.Delay(3000);
                    if (deleteConfirm)
                    {
                        await (await WoundDatabase.Database).DeletePatient(patient);

                        await viewModel.UpdatePatientList();
                    }

                    infectionView.IsVisible = false;
                    deleteConfirm = false;
                    viewModel.ToggleDeleteMode();

                }
                else
                {
                    (await WoundDatabase.Database).dataHolder.PatientID = patient.PatientID;

                    await Navigation.PopModalAsync();
                }
            }
        }

        async void ConfirmDeletePatient(object sender, EventArgs e)
        {
            deleteConfirm = true;
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
        private void infectionInfoOn(object sender, EventArgs e)
        {


        }
        private void infectionViewOff(object sender, EventArgs e)
        {
            infectionView.IsVisible = false;

        }
    }
}
