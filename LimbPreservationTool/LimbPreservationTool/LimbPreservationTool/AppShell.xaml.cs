using LimbPreservationTool.Models;
using LimbPreservationTool.ViewModels;
using LimbPreservationTool.Views;
using System;
using System.Threading.Tasks;
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
            CleanWifi();
            await Shell.Current.GoToAsync($"//{nameof(WifiPage)}");

        }

        private async void OnLogoutItemClicked(object sender, EventArgs e)
        {
            CleanAll();
            //var c = this.Resources["Clear"];
            WoundDatabase DB = (await WoundDatabase.Database);
            DB.dataHolder = DBWoundData.Create();   // Clear the selected patient when logging out
            await Shell.Current.GoToAsync("//LoginPage");
        }

        public async void CleanAll()
        {
            CleanPhoto();
            CleanWifi();
            await CleanDatabase();
        }

        public void CleanWifi()
        {

            (App.Current.Resources["SharedWifiViewModel"] as WifiViewModel).ClearStartInfo();
        }

        public void CleanPhoto()
        {
            PhotoViewModel p = (PhotoViewModel)App.Current.Resources["SharedPhotoViewModel"];
            p.EraseAll();
        }

        public async Task CleanDatabase()
        {

            (await WoundDatabase.Database).dataHolder = DBWoundData.Create(Guid.Empty);

        }

    }
}
