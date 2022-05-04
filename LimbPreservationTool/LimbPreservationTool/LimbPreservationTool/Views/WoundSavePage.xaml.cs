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

        }
    }
}