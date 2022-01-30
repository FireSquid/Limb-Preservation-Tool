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
using System.IO;

namespace LimbPreservationTool.ViewModels
{
    public class PhotoViewModel : BaseViewModel
    {
        public PhotoViewModel()
        {
            Title = "About";
            PictureStatus = "No Picture Found";
            TakePhotoCommand = new Command(async () => await GetImageFromCamera());
            		    
            System.Diagnostics.Debug.WriteLine("Photo ViewModel\n");
        }

        //private async Task<ImageSource> TakePhoto()
        //{
        //    var photo = await Media.CrossMedia.Current.TakePhotoAsync(new Media.Abstractions.StoreCameraMediaOptions() { });

        //    if (photo != null)
        //    {
        //        ImageSource nextPhoto = ImageSource.FromStream(() => { return photo.GetStream(); });
        //        
        //        return nextPhoto;
        //    }
        //    return null;
        //}
		private ImageSource PhotoPath;
        async Task TakePhoto()
        {
            try
            {
                
                System.Diagnostics.Debug.WriteLine("Beging Taking Photo\n");
                //PermissionStatus status = await MediaPicker.RequestPermissionAsync<CalendarPermission>();
                var photo = await MediaPicker.CapturePhotoAsync();

                System.Diagnostics.Debug.WriteLine("Photo Capatured\n");
                await LoadPhotoAsync(photo);
                Console.WriteLine($"CapturePhotoAsync COMPLETED: {PhotoPath}");
            }
            catch (FeatureNotSupportedException fnsEx)
            {

                System.Diagnostics.Debug.WriteLine("Feature Not Supported\n");
                // Feature is not supported on the device
            }
            catch (PermissionException pEx)
            {

                System.Diagnostics.Debug.WriteLine("Permission Denied\n");
                // Permissions not granted
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine($"CapturePhotoAsync THREW: {ex.Message}");
            }
        }

        async Task LoadPhotoAsync(FileResult photo)
        {
            // canceled
            if (photo == null)
            {
                System.Diagnostics.Debug.WriteLine("Null Photo\n");
                PhotoPath = null;
                return;
            }
            // save the file into local storage

            Console.Write("Photo Taken");
            var newFile = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
            using (var stream = await photo.OpenReadAsync())
            using (var newStream = File.OpenWrite(newFile))
                await stream.CopyToAsync(newStream);

            PhotoPath = newFile;
        }

        private async Task GetImageFromCamera()
        {
            //ImageSource newImage = await TakePhoto();

            //if (newImage != null)
            //{
            //    PictureStatus = $"Found Picture, {newImage.ToString()}";
            //    LastPhoto = newImage;
            //}

			await TakePhoto();
		    LastPhoto = PhotoPath;	
            
        }

        public ICommand TakePhotoCommand { get; }

        private ImageSource lastPhoto;
        public ImageSource LastPhoto { get => lastPhoto; set => SetProperty(ref lastPhoto, value); }

        private string pictureStatus;
        public string PictureStatus { get => pictureStatus; set => SetProperty(ref pictureStatus, value); }
    }
}