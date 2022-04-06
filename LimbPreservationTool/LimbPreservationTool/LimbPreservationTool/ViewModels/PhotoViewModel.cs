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
using LimbPreservationTool.Renderers;

namespace LimbPreservationTool.ViewModels
{
    public class PhotoViewModel : BaseViewModel
    {


        public PhotoViewModel()
        {
            Title = "About";
            PictureStatus = "No Picture Found";
            photo = null;
            PR = new Renderers.PathRenderer();
            TakePhotoCommand = new Command(async () => await TakePhoto());
            ExaminePhotoCommand = new Command(async () => await ExaminePhoto());
        }

        public async Task TakePhoto()
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
                LastPhoto = ImageSource.FromStream(() => photoStream);
                PictureStatus = $"Successfully obtained photo";
                var bitmapStream = new MemoryStream();
                await photoStream.CopyToAsync(bitmapStream); //copying will reset neither streams' position
                photoStream.Seek(0, SeekOrigin.Begin);
                bitmapStream.Seek(0, SeekOrigin.Begin);
                PR.ImageBitmap = SKBitmap.Decode(bitmapStream);

                //using (var stream = await photo.OpenReadAsync())
                //BeginInvoke(()=>ExaminePhoto());
            }
        }

        public void DrawHighlight(object colorArgument)
        {
            SKColor color = SKColor.Parse((string)colorArgument);
            PR.FillColor = color;
        }

        public async Task ExaminePhoto()
        {

            if (photo == null)
            {
                Console.Write("Has not taken a photo");
                return;
            }
            photoStream = await photo.OpenReadAsync();

            Console.WriteLine("#_#_#_#_#_#_#_#_# EXAMINING");

            if (Device.RuntimePlatform.Equals(Device.iOS))
            {

                //var ms = new MemoryStream();
                //photoStream.CopyTo(ms);
                //photoStream = App.scalerInterface.ResizeImage(ms.ToArray(), 1024, 2048);

                Console.WriteLine("Scaled with IOS");

            }

            Stream e = await Doctor.GetInstance().Examine(photoStream);
            //examineEnabled = false;
            if (!e.Equals(Stream.Null))
            {
                LastPhoto = ImageSource.FromStream(() => e);
                Console.WriteLine("Examine finished");
            }
        }
        public bool DecodeToBitMap(ref SKBitmap bitmap)
        {

            if (photoStream == null)
            {
                return false;
            }
            bitmap = SKBitmap.Decode(photoStream);

            return true;
        }


        //public static readonly BindableProperty RenderProperty = BindableProperty.Create(
        //       nameof(PR),
        //                   typeof(Renderers.PathRenderer),
        //                   typeof(SKRenderView),
        //                   null
        //                   );
        public Renderers.PathRenderer PR { get; set; }
        private SKBitmap scanBitmap;
        private FileResult photo { get; set; }
        private Stream photoStream { get; set; }



        public ICommand TakePhotoCommand { get; }

        public ICommand ExaminePhotoCommand { get; }

        private ImageSource lastPhoto;
        public ImageSource LastPhoto { get => lastPhoto; set => SetProperty(ref lastPhoto, value); }

        private bool examineEnabled;
        public bool ExamineEnabled { get => examineEnabled; set => SetProperty(ref examineEnabled, value); }
        private string pictureStatus;
        public string PictureStatus { get => pictureStatus; set => SetProperty(ref pictureStatus, value); }
    }
}
