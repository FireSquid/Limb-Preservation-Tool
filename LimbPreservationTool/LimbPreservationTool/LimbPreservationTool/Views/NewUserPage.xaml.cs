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
    public partial class NewUserPage : ContentPage
    {
        NewUserViewModel viewModel;
        public NewUserPage()
        {
            InitializeComponent();
            viewModel = new NewUserViewModel();
            this.BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // viewModel.ResetLoginMessage();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            // viewModel.ClearLoginInformation();
        }
    }
}