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
            // viewModel = new WifiViewModel();
            // this.BindingContext = viewModel;
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

            var temp = e.NewTextValue.ToCharArray();

            for (int i = 0; i < temp.Length; i++)
            {
                if (string.Equals(temp[i], '.') || string.Equals(temp[i], ','))
                {
                    ((Entry)sender).Text = "";
                    instructionText.Text = $"Please only enter valid values.  Refer to the previous page for more details.";
                    instructionText.TextColor = Color.Black;
                    return;
                }
            }

            if (!int.TryParse(e.NewTextValue, out int value))
            {
                // only non numeric character allowed is negative integers
                if (string.Equals(temp[0], '-'))
                {
                    return;
                }
                ((Entry)sender).Text = e.OldTextValue;
                return;
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