using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

using Xamarin.Forms;
using Xamarin.Essentials;

using System.Windows.Input;
using System.Threading.Tasks;
using LimbPreservationTool.Models;
using System.IO;

namespace LimbPreservationTool.ViewModels
{
    public class PhotoViewModel : BaseViewModel
    {
        public PhotoViewModel()
        {
            Title = "About";
            PictureStatus = "No Picture Found";
            photo = null;
            TakePhotoCommand = new Command(async () => await TakePhoto());
            ExaminePhotoCommand = new Command(() => ExaminePhoto());
        }

        async Task TakePhoto()
        {
            try
            {
                // Attempt to take the picture
                photo = await MediaPicker.CapturePhotoAsync();
            }
            catch (FeatureNotSupportedException e)
            {
                System.Diagnostics.Debug.WriteLine($"Feature Not Supported {e.Message}");
            }
            catch (PermissionException e)
            {
                System.Diagnostics.Debug.WriteLine($"Permission Denied: {e.Message}");
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"CapturePhotoAsync THREW: {e.Message}");
            }

            if (photo != null)
            {
                // Load the picture from a stream and set as the image source
                photoStream = await photo.OpenReadAsync();
                LastPhoto = ImageSource.FromStream(() => photoStream);
                PictureStatus = $"Successfully obtained photo: {LastPhoto.ToString()}";
                //using (var stream = await photo.OpenReadAsync())
                //BeginInvoke(()=>ExaminePhoto());
            }
        }
        async void ExaminePhoto()
        {

            if (photo == null)
            {
                Console.Write("Has not taken a photo");
                return;
            }
            photoStream = await photo.OpenReadAsync();
            Task<string> e = Doctor.GetInstance().Examine(photoStream);
            await e;
            Console.WriteLine("Examine finished");
        }

        private FileResult photo { get; set; }
        private Stream photoStream { get; set; }
        public ICommand TakePhotoCommand { get; }

        public ICommand ExaminePhotoCommand { get; }

        private ImageSource lastPhoto;
        public ImageSource LastPhoto { get => lastPhoto; set => SetProperty(ref lastPhoto, value); }

        private string pictureStatus;
        public string PictureStatus { get => pictureStatus; set => SetProperty(ref pictureStatus, value); }
    }
}