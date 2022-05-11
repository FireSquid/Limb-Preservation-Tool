using LimbPreservationTool.ViewModels;
using LimbPreservationTool.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace LimbPreservationTool
{
    public partial class AppShell : Xamarin.Forms.Shell
    {

        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
        }

        private async void OnHomeItemClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//HomePage");
        }

        private async void OnWifiItemClicked(object sender, EventArgs e)
        {
            //App.Current.Resources["SharedWifiViewModel"] = new WifiViewModel();
            await Shell.Current.GoToAsync($"//{nameof(WifiPage)}");

        }

        private async void OnLogoutItemClicked(object sender, EventArgs e)
        {

            PhotoViewModel p = (PhotoViewModel)App.Current.Resources["SharedPhotoViewModel"];
            p.EraseAll();
            //var c = this.Resources["Clear"];
            await Shell.Current.GoToAsync("//LoginPage");
        }

    }
}
