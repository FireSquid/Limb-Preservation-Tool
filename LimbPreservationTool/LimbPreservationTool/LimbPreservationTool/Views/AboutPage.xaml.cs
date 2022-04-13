using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using LimbPreservationTool.ViewModels;

namespace LimbPreservationTool.Views
{
    public partial class AboutPage : ContentPage
    {

        AboutViewModel viewModel;

        public AboutPage()
        {
            InitializeComponent();
            viewModel = new AboutViewModel();
            this.BindingContext = viewModel;
        }
    }
}