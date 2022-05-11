using LimbPreservationTool.Models;
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
        public HomePage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            WoundDatabase DB = (await WoundDatabase.Database);

            Guid patientID = DB.dataHolder.PatientID;

            if (patientID == null || patientID == Guid.Empty)
            {
                PatientsPage patientSelectionPage = new PatientsPage();
                await Navigation.PushModalAsync(patientSelectionPage);
            }
        }

        private void OnSwitchPatientClicked(object sender, EventArgs e)
        {
            PatientsPage patientSelectionPage = new PatientsPage();
            Navigation.PushModalAsync(patientSelectionPage);
        }
    }
}