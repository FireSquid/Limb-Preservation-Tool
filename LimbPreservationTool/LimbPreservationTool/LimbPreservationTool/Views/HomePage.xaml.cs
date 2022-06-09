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
    public partial class HomePage : ContentPage
    {
        private bool isAppearing = false;

        public HomeViewModel viewModel;
        public HomePage()
        {
            InitializeComponent();
            viewModel =  new HomeViewModel();
            this.BindingContext = viewModel;

            MessagingCenter.Subscribe<PatientsPage>(this, "popModalToHome", async (sender) => await PageUpdate());
        }

        protected override async void OnAppearing()
        {
            await PageUpdate();
        }

        private async Task PageUpdate()
        {
            if (isAppearing) return;
            isAppearing = true;

            base.OnAppearing();

            WoundDatabase DB = (await WoundDatabase.Database);

            Guid patientID = DB.dataHolder.PatientID;

            if (patientID == Guid.Empty)
            {
                PatientsPage patientSelectionPage = new PatientsPage();
                await Navigation.PushModalAsync(patientSelectionPage);
                isAppearing = false;
                return;
            }

            await viewModel.setPatientName();

            isAppearing = false;
        }

        private void OnSwitchPatientClicked(object sender, EventArgs e)
        {
            PatientsPage patientSelectionPage = new PatientsPage();
            Navigation.PushModalAsync(patientSelectionPage);
        }


    }
}