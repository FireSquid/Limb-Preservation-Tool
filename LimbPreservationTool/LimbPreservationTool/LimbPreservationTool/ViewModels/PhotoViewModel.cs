using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
<<<<<<< HEAD
using System.Threading;
=======
>>>>>>> dev

using Xamarin.Forms;
using Xamarin.Essentials;

using System.Windows.Input;
using System.Threading.Tasks;
<<<<<<< HEAD
using LimbPreservationTool.Models;
using System.Drawing;
=======
>>>>>>> dev
using System.IO;

namespace LimbPreservationTool.ViewModels
{
    public class PhotoViewModel : BaseViewModel
    {
        public PhotoViewModel()
        {
<<<<<<< HEAD
            Title = "About";
            PictureStatus = "No Picture Found";
            photo = null;
            TakePhotoCommand = new Command(async () => await TakePhoto());
            ExaminePhotoCommand = new Command(() => ExaminePhoto());
        }


        async Task TakePhoto()
        {
=======
            Title = "Photo";
            PictureStatus = "No Picture Found";
            TakePhotoCommand = new Command(async () => await TakePhoto());
        }

        async Task TakePhoto()
        {
            FileResult photo = null;
>>>>>>> dev
            try
            {
                // Attempt to take the picture
                photo = await MediaPicker.CapturePhotoAsync();
<<<<<<< HEAD
                Console.WriteLine(photo.FileName.ToString());
=======
>>>>>>> dev
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
<<<<<<< HEAD
                photoStream = await photo.OpenReadAsync();

                LastPhoto = ImageSource.FromStream(() => photoStream);
                PictureStatus = $"Successfully obtained photo";
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
            Console.WriteLine("#_#_#_#_#_#_#_#_# EXAMINING");
            Stream e = await Doctor.GetInstance().Examine(photoStream);
            if (!e.Equals(Stream.Null))
            {
                LastPhoto = ImageSource.FromStream(() => e);
                Console.WriteLine("Examine finished");
            }
        }

        private FileResult photo { get; set; }
        private Stream photoStream { get; set; }
        public ICommand TakePhotoCommand { get; }

        public ICommand ExaminePhotoCommand { get; }

=======
                var photoStream = await photo.OpenReadAsync();
                LastPhoto = ImageSource.FromStream(() => photoStream);
                PictureStatus = $"Successfully obtained photo: {LastPhoto.ToString()}";
            }
        }

        public ICommand TakePhotoCommand { get; }

>>>>>>> dev
        private ImageSource lastPhoto;
        public ImageSource LastPhoto { get => lastPhoto; set => SetProperty(ref lastPhoto, value); }

        private string pictureStatus;
        public string PictureStatus { get => pictureStatus; set => SetProperty(ref pictureStatus, value); }
    }
}