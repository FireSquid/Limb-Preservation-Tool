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

        private async void OnLogoutItemClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
