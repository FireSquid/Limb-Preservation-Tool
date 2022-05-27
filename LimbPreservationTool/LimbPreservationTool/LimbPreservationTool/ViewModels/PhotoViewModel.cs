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
using LimbPreservationTool.CustomComponents;
using Xamarin.CommunityToolkit.Extensions;

namespace LimbPreservationTool.ViewModels
{
    public class PhotoViewModel : BaseViewModel
    {


        public PhotoViewModel()
        {
            Title = "About";
            PictureStatus = "No Picture Found";
            photo = null;
            Canvas = new Renderers.NormalRenderer();
            //Receiver = new TouchReceiver();
            Highlighter = new Renderers.PathRenderer();
            TakePhotoCommand = new Command(async () => await TakePhoto());
            ExaminePhotoCommand = new Command(async () => await ExamineHighlight());
            HighlightCommand = new Command(async () => await StartHighlight());
            SaveHighlightCommand = new Command(async () => await SaveHighlight());
            RedoHighlightCommand = new Command(() => RedoHighlight());
            PreviewCommand = new Command(() => SetPreview());
            SaveWoundDataCommand = new Command(async () => await SaveWoundData());
            EnablePicture();
        }

        private async Task SaveWoundData()
        {
            await Shell.Current.GoToAsync($"//{nameof(WoundSavePage)}");
        }

        public async Task<bool> TakePhoto()
        {
            DisableSave();
            try
            {
                // Attempt to take the picture
                photo = await MediaPicker.CapturePhotoAsync();

                Console.WriteLine(photo.FileName.ToString());
            }
            catch (FeatureNotSupportedException e)
            {
                System.Diagnostics.Debug.WriteLine($"Feature Not Supported {e.Message}");
                return false;
            }
            catch (PermissionException e)
            {
                System.Diagnostics.Debug.WriteLine($"Permission Denied: {e.Message}");
                return false;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"CapturePhotoAsync THREW: {e.Message}");
                return false;
            }

            if (photo == null)
            {
                System.Diagnostics.Debug.WriteLine("photo is null");
                return false;
            }
            // Load the picture from a stream and set as the image source
            photoStream = await photo.OpenReadAsync();
            Image tmp = new Image() { Source = photo.FullPath };
            Console.WriteLine(tmp.Aspect);
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
            EnableHighlight();
            EnableExamine();
            //SetProperty(ref pictureInputAllowed, false);

            //PR.ImageBitmap = scanBitmap.Copy();
            //photoStream = await PhotoRotator(photoStream);
            //LastPhoto = ImageSource.FromStream(() => photoStream);

            //using (var stream = await photo.OpenReadAsync())
            //BeginInvoke(()=>ExaminePhoto());
            return true;
        }


        //        public async Task ExaminePhoto()
        //        {
        //
        //            if (photo == null)
        //            {
        //                Console.Write("Has not taken a photo");
        //                return;
        //            }
        //            // photoStream = await photo.OpenReadAsync();
        //
        //            Console.WriteLine("#_#_#_#_#_#_#_#_# EXAMINING");
        //
        //            Stream e = await Doctor.GetInstance().Examine(photoStream);
        //            //examineEnabled = false;
        //            if (!e.Equals(Stream.Null))
        //            {
        //                e = await PhotoRotator(e);
        //                LastPhoto = ImageSource.FromStream(() => e);
        //
        //                //var bitmapStream = new MemoryStream();
        //
        //                //await photoStream.CopyToAsync(bitmapStream); //copying will reset neither streams' position
        //
        //                //bitmapStream.Seek(0, SeekOrigin.Begin);
        //                Console.WriteLine("Examine  f inished");
        //            }
        //        }
        //
        public async Task<bool> ExamineHighlight()
        {

            DisenableAll();
            if (scanBitmap == null)
            {

                Console.Write("Has not taken a photo as bitmap");
                return false;
            }
            if (blendBitmap == null)
            {
                blendBitmap = scanBitmap.Copy();
            }
            Stream highlightStream = SKImage.FromBitmap(blendBitmap).Encode().AsStream();
            Stream e = await Doctor.GetInstance().Examine(highlightStream);

            //examineEnabled = false;
            if (!e.Equals(Stream.Null))
            {
                LastPhoto = ImageSource.FromStream(() => e);

                Canvas.ImageBitmap = SKBitmap.Decode(e);
                Console.WriteLine("Examine finished");
            }
            blendBitmap = null;
            scanBitmap = null;
            EnablePicture();

            var db = (WoundDatabase.Database).GetAwaiter().GetResult();
            if (db.dataHolder.Size > 0)
            {
                EnableSave();
            }

            return true;
            //EraseAll();
        }
        //
        //        private async Task<Stream> PhotoRotator(Stream pS)
        //        {
        //
        //            var bitmapStream = new MemoryStream();
        //            await pS.CopyToAsync(bitmapStream); //copying will reset neither streams' position
        //            pS.Seek(0, SeekOrigin.Begin);
        //            bitmapStream.Seek(0, SeekOrigin.Begin);
        //            var scanBitmap = SKBitmap.Decode(bitmapStream);
        //            var rotated = new SKBitmap(scanBitmap.Height, scanBitmap.Width);
        //
        //            using (var surface = new SKCanvas(rotated))
        //            {
        //                surface.Translate(rotated.Width, 0);
        //                surface.RotateDegrees(90);
        //                surface.DrawBitmap(scanBitmap, 0, 0);
        //            }
        //            scanBitmap = rotated;
        //            SKImage image = SKImage.FromBitmap(scanBitmap);
        //            SKData encodedData = image.Encode(SKEncodedImageFormat.Png, 100);
        //            return encodedData.AsStream();
        //        }
        //
        public void EraseAll()
        {

            Canvas.ClearAll();
            Highlighter.ClearAll();
            scanBitmap = null;
            blendBitmap = null;
            DisenableAll();
            EnablePicture();

        }

        async Task<bool> StartHighlight()
        {

            if (Canvas.ImageBitmap == null) { return false; }
            //var page = Activator.CreateInstance<HighlightPage>();
            //page.BindingContext = this;
            Highlighter.PreviewMode = false;
            await Shell.Current.GoToAsync($"//{nameof(HighlightPage)}");
            RedoHighlight();
            Highlighter.StartHighlight();
            Highlighter.Src = Canvas.ImageBitmap.Copy();
            if (Highlighter.Src == null)
            {

                Console.WriteLine("Src is null");
                return false;
            }
            return true;
            //Highlighter.Src = Canvas.ImageBitmap.Copy();
        }

        async Task<bool> SaveHighlight()
        {

            //Highlightable = false;//this won't update the property here 
            //Canvas.ImageBitmap = Highlighter.Save().Copy();
            await Shell.Current.GoToAsync($"//{nameof(PhotoPage)}");

            if (!Highlighter.Receiver.Fresh())
            {
                Canvas.ImageBitmap = Highlighter.PorterDuff();
            }
            blendBitmap = Canvas.ImageBitmap.Copy();
            DisableHighlight();
            return true;
            //blend overlay bitmap with picture bitmap
            //Highlightable = false;//this won't update the property possibily due to 2 way binding
        }

        void RedoHighlight()
        {
            Highlighter.ClearPath();

            Highlighter.PreviewMode = false;
            //Canvas.RendererSize = CanvasSize;
            //Canvas.ImageBitmap = scanBitmap.Copy();
        }

        void SetPreview()
        {
            Highlighter.PreviewMode = !Highlighter.PreviewMode;

        }


        void DisenableAll()
        {
            pictureInputAllowed = false;
            highlightInputAllowed = false;
            examineInputAllowed = false;
            saveDataAllowed = false;
            OnPropertyChanged("PictureInputAllowed");
            OnPropertyChanged("HighlightInputAllowed");
            OnPropertyChanged("ExamineInputAllowed");
            OnPropertyChanged("SaveDataAllowed");
        }

        void EnablePicture()
        {
            pictureInputAllowed = true;
            OnPropertyChanged("PictureInputAllowed");
        }

        void EnableHighlight()
        {
            highlightInputAllowed = true;
            OnPropertyChanged("HighlightInputAllowed");
        }

        void EnableExamine()
        {
            examineInputAllowed = true;
            OnPropertyChanged("ExamineInputAllowed");
        }

        void EnableSave()
        {
            saveDataAllowed = true;
            OnPropertyChanged("SaveDataAllowed");
        }

        void DisableHighlight()
        {
            highlightInputAllowed = false;
            OnPropertyChanged("HighlightInputAllowed");
        }

        void DisableExamine()
        {
            examineInputAllowed = false;
            OnPropertyChanged("ExamineInputAllowed");
        }

        void DisableSave()
        {
            saveDataAllowed = false;
            OnPropertyChanged("SaveDataAllowed");
        }


        public Renderers.NormalRenderer Canvas { get; set; }
        public Renderers.PathRenderer Highlighter { get; set; }
        private SKBitmap scanBitmap;
        private SKBitmap blendBitmap;
        private FileResult photo { get; set; }
        private Stream photoStream { get; set; }
        private SKSize canvasSize;
        public SKSize CanvasSize { get => canvasSize; set => SetProperty(ref canvasSize, value); }
        //public SKSize HighlighterSize { get; set; }
        //public TouchReceiver Receiver { get; set; }

        private ImageSource lastPhoto;
        public ImageSource LastPhoto { get => lastPhoto; set => SetProperty(ref lastPhoto, value); }
        private string pictureStatus;
        public string PictureStatus { get => pictureStatus; set => SetProperty(ref pictureStatus, value); }

        public ICommand TakePhotoCommand { get; }
        public ICommand ExaminePhotoCommand { get; }
        public ICommand HighlightCommand { get; }
        public ICommand SaveHighlightCommand { get; }
        public ICommand RedoHighlightCommand { get; }
        public ICommand PreviewCommand { get; }
        public ICommand SaveWoundDataCommand { get; }

        public bool PictureInputAllowed
        {
            get
            {
                return pictureInputAllowed;
            }

        }
        private bool pictureInputAllowed = true;
        public bool HighlightInputAllowed
        {
            get
            {
                return highlightInputAllowed;
            }

        }

        private bool highlightInputAllowed = false;

        public bool ExamineInputAllowed
        {
            get
            {
                return examineInputAllowed;
            }

        }

        private bool examineInputAllowed = false;

        public bool SaveDataAllowed
        {
            get
            {
                return saveDataAllowed;
            }

        }

        private bool saveDataAllowed = false;


    }
}
