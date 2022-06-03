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
    public partial class PhotoDescPage : ContentPage
    {
        PhotoDescViewModel viewModel;


        public PhotoDescPage()
        {
            InitializeComponent();
            viewModel = new PhotoDescViewModel();
            this.BindingContext = viewModel;
        }
    }
}