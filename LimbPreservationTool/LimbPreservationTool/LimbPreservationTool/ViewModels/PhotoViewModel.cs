using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using Xamarin.Forms;
using Xamarin.Essentials;

using System.Windows.Input;
using System.Threading.Tasks;

namespace LimbPreservationTool.ViewModels
{
    public class PhotoViewModel : BaseViewModel
    {
        public PhotoViewModel()
        {
            Title = "About";
            PictureStatus = "No Picture Found";
            TakePhotoCommand = new Command(async () => await GetImageFromCamera());
        }

        private async Task<ImageSource> TakePhoto()
        {
            var photo = await Plugin.Media.CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions() { });

            if (photo != null)
            {
                ImageSource nextPhoto = ImageSource.FromStream(() => { return photo.GetStream(); });
                
                return nextPhoto;
            }
            return null;
        }

        private async Task GetImageFromCamera()
        {
            ImageSource newImage = await TakePhoto();

            if (newImage != null)
            {
                PictureStatus = $"Found Picture, {newImage.ToString()}";
                LastPhoto = newImage;
            }
            
        }

        public ICommand TakePhotoCommand { get; }

        private ImageSource lastPhoto;
        public ImageSource LastPhoto { get => lastPhoto; set => SetProperty(ref lastPhoto, value); }

        private string pictureStatus;
        public string PictureStatus { get => pictureStatus; set => SetProperty(ref pictureStatus, value); }
    }
}