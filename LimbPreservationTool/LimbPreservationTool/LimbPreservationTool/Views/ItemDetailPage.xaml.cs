using LimbPreservationTool.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace LimbPreservationTool.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}