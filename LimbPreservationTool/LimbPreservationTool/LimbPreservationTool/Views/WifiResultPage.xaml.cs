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
    public partial class WifiResultPage : ContentPage
    {
        WifiResultModel viewModel;


        public WifiResultPage()
        {
            InitializeComponent();
            viewModel = new WifiResultModel();
            this.BindingContext = viewModel;
        }

        public WifiResultPage(string ampInfo, string revInfo, Color ampColor, Color revColor)
        {
            InitializeComponent();
            viewModel = new WifiResultModel(ampInfo, revInfo, ampColor, revColor);
            this.BindingContext = viewModel;
        }
    }
}