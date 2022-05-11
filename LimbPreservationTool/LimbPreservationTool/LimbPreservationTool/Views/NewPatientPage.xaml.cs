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
    public partial class NewPatientPage : ContentPage
    {
        NewPatientViewModel viewModel;

        public NewPatientPage()
        {
            InitializeComponent();
            viewModel = new NewPatientViewModel();
            this.BindingContext = viewModel;
        }

        private async void OnCreatePatientClicked(object sender, EventArgs e)
        {
            await viewModel.CreatePatient();
            await Navigation.PopModalAsync();
        }
    }
}
