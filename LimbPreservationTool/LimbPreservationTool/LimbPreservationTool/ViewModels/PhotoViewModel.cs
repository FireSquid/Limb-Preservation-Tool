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
using LimbPreservationTool.Views;
using System.Drawing;
using System.IO;
using SkiaSharp;
using LimbPreservationTool.Renderers;
using LimbPreservationTool.CustomeComponents;

namespace LimbPreservationTool.ViewModels
{
    public class PhotoViewModel : BaseViewModel
    {


        public PhotoViewModel()
        {
            Title = "About";
            PictureStatus = "No Picture Found";
            photo = null;
            Canvas = new Renderers.PathRenderer();
            //Receiver = new TouchReceiver();
            // Highlither = new Renderers.PorterDuffRenderer();
            TakePhotoCommand = new Command(async () => await TakePhoto());
            ExaminePhotoCommand = new Command(async () => await ExaminePhoto());
            HighlightCommand = new Command(() => Canvas.StartHighlight());
            DrawHighlightCommand = new Command(() => DrawHighlight());
            RedoHighlightCommand = new Command(() => RedoHighlight());
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
                scanBitmap = SKBitmap.Decode(bitmapStream);
                var rotated = new SKBitmap(scanBitmap.Height, scanBitmap.Width);

                using (var surface = new SKCanvas(rotated))
                {
                    surface.Translate(rotated.Width, 0);
                    surface.RotateDegrees(90);
                    surface.DrawBitmap(scanBitmap, 0, 0);
                }
                scanBitmap = rotated;

                Canvas.RendererSize = CanvasSize;
                Console.WriteLine("CanvasSize: " + CanvasSize.ToString());
                Canvas.ImageBitmap = scanBitmap.Copy();
                //PR.ImageBitmap = scanBitmap.Copy();
                //using (var stream = await photo.OpenReadAsync())
                //BeginInvoke(()=>ExaminePhoto());
            }
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

            Stream e = await Doctor.GetInstance().Examine(photoStream);
            //examineEnabled = false;
            if (!e.Equals(Stream.Null))
            {
                LastPhoto = ImageSource.FromStream(() => e);
                Console.WriteLine("Examine finished");
            }
        }

        public async Task ExamineHighlight()
        {
            if (scanBitmap == null)
            {

                Console.Write("Has not taken a photo as bitmap");
                return;
            }
            Stream highlightStream = SKImage.FromBitmap(blendBitmap).Encode().AsStream();
            Stream e = await Doctor.GetInstance().Examine(highlightStream);
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


        void DrawHighlight()
        {
            Console.WriteLine("!");
            Canvas.StartHighlight();
            //create overlay on canvas
            //Highlither.Src = Canvas.ImageBitmap.Copy();
            //await Shell.Current.GoToAsync($"//{nameof(HighlightPage)}");
        }

        void SaveHighlight()
        {
            //blend overlay bitmap with picture bitmap

        }

        void RedoHighlight()
        {
            Canvas.ClearPath();
            //Canvas.RendererSize = CanvasSize;
            //Canvas.ImageBitmap = scanBitmap.Copy();
        }

        public Renderers.PathRenderer Canvas { get; set; }
        //public Renderers.PorterDuffRenderer Highlither { get; set; }
        private SKBitmap scanBitmap;
        private SKBitmap blendBitmap;
        private FileResult photo { get; set; }
        private Stream photoStream { get; set; }

        private SKSize canvasSize;
        public SKSize CanvasSize { get => canvasSize; set => SetProperty(ref canvasSize, value); }
        //public SKSize HighlighterSize { get; set; }
        //public TouchReceiver Receiver { get; set; }

        public ICommand TakePhotoCommand { get; }

        public ICommand ExaminePhotoCommand { get; }
        public ICommand HighlightCommand { get; }
        public ICommand DrawHighlightCommand { get; }
        public ICommand SaveHighlightCommand { get; }
        public ICommand RedoHighlightCommand { get; }

        private ImageSource lastPhoto;
        public ImageSource LastPhoto { get => lastPhoto; set => SetProperty(ref lastPhoto, value); }

        private bool examineEnabled;
        public bool ExamineEnabled { get => examineEnabled; set => SetProperty(ref examineEnabled, value); }
        private string pictureStatus;
        public string PictureStatus { get => pictureStatus; set => SetProperty(ref pictureStatus, value); }
    }
}
