using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using LimbPreservationTool.ViewModels;



namespace LimbPreservationTool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WifiPage : ContentPage
    {
        WifiViewModel viewModel;
        public WifiPage()
        {
            InitializeComponent();
            viewModel = new WifiViewModel();
            this.BindingContext = viewModel;
        }
    }
}