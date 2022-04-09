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
using System.Drawing;
using System.IO;
using SkiaSharp;
using SkiaSharp.Views.Forms;
#if __IOS__
using System.Drawing;
using UIKit;
using CoreGraphics;
#endif

#if __ANDROID__
using Android.Graphics;
#endif

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
                Console.WriteLine(photo.FileName.ToString());
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
                PictureStatus = $"Successfully obtained photo";
                photoStream = await PhotoRotator(photoStream);
                LastPhoto = ImageSource.FromStream(() => photoStream);

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
                e = await PhotoRotator(e);
                LastPhoto = ImageSource.FromStream(() => e);
                Console.WriteLine("Examine finished");
            }
        }

        private async Task<Stream> PhotoRotator(Stream pS)
        {

            var bitmapStream = new MemoryStream();
            await pS.CopyToAsync(bitmapStream); //copying will reset neither streams' position
            pS.Seek(0, SeekOrigin.Begin);
            bitmapStream.Seek(0, SeekOrigin.Begin);
            var scanBitmap = SKBitmap.Decode(bitmapStream);
            var rotated = new SKBitmap(scanBitmap.Height, scanBitmap.Width);

            using (var surface = new SKCanvas(rotated))
            {
                surface.Translate(rotated.Width, 0);
                surface.RotateDegrees(90);
                surface.DrawBitmap(scanBitmap, 0, 0);
            }
            scanBitmap = rotated;
            SKImage image = SKImage.FromBitmap(scanBitmap);
            SKData encodedData = image.Encode(SKEncodedImageFormat.Png, 100);
            return encodedData.AsStream();
        }


        private FileResult photo
        { get; set; }
        private Stream photoStream { get; set; }
        public ICommand TakePhotoCommand { get; }

        public ICommand ExaminePhotoCommand { get; }

        private ImageSource lastPhoto;
        public ImageSource LastPhoto { get => lastPhoto; set => SetProperty(ref lastPhoto, value); }

        private string pictureStatus;
        public string PictureStatus { get => pictureStatus; set => SetProperty(ref pictureStatus, value); }
    }
}