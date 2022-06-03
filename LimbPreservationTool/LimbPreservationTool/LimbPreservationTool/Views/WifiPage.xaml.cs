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
            //viewModel = new WifiViewModel();
            //this.BindingContext = viewModel;
        }

        private void woundInfoOn(object sender, EventArgs e)
        {
            woundView.IsVisible = true;
            overlayCover.IsVisible = true;

        }
        private void woundInfoOff(object sender, EventArgs e)
        {
            woundView.IsVisible = false;
            overlayCover.IsVisible = false;

        }

        private void infectionInfoOn(object sender, EventArgs e)
        {
            infectionView.IsVisible = true;
            overlayCover.IsVisible = true;

        }
        private void infectionViewOff(object sender, EventArgs e)
        {
            infectionView.IsVisible = false;
            overlayCover.IsVisible = false;

        }

        private void ischemiaInfoOn(object sender, EventArgs e)
        {
            ischemiaView.IsVisible = true;
            overlayCover.IsVisible = true;

        }
        private void ischemiaViewOff(object sender, EventArgs e)
        {
            ischemiaView.IsVisible = false;
            overlayCover.IsVisible = false;

        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            //lets the Entry be empty
            if (string.IsNullOrEmpty(e.NewTextValue)) return;

            if (!int.TryParse(e.NewTextValue, out int value))
            {
                ((Entry)sender).Text = e.OldTextValue;
            }

            int checkRange = 0;
            checkRange = int.Parse(e.NewTextValue);
            if (checkRange > 3 || checkRange < -1)
            {
                ((Entry)sender).Text = e.OldTextValue;
            }
        }
    }
}