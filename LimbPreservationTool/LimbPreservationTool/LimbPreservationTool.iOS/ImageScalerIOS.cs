using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AppCenter;
using Xamarin.Forms;

using System.Drawing;
using UIKit;
using CoreGraphics;
using LimbPreservationTool;
using LimbPreservationTool.Services;
namespace LimbPreservationTool.iOS
{
    public class ImageScalerIOS : IScaler
    {
        static ImageScalerIOS s;
        private ImageScalerIOS()
        {
        }
        public ImageScalerIOS GetInstance()
        {
            if (s == null)
                s = new ImageScalerIOS();

            return s;

        }
        public Stream ResizeImage(byte[] imageData, float width, float height)
        {
            return ResizeImageIOS(imageData, width, height);

        }

        private Stream ResizeImageIOS(byte[] imageData, float width, float height)
        {
            UIImage originalImage = ImageFromByteArray(imageData);
            UIImageOrientation orientation = originalImage.Orientation;

            //create a 24bit RGB image
            using (CGBitmapContext context = new CGBitmapContext(IntPtr.Zero,
                                                 (int)width, (int)height, 8,
                                                 4 * (int)width, CGColorSpace.CreateDeviceRGB(),
                                                 CGImageAlphaInfo.PremultipliedFirst))
            {

                RectangleF imageRect = new RectangleF(0, 0, width, height);

                // draw the image
                context.DrawImage(imageRect, originalImage.CGImage);

                UIKit.UIImage resizedImage = UIKit.UIImage.FromImage(context.ToImage(), 0, orientation);

                // save the image as a jpeg
                return new MemoryStream(resizedImage.AsPNG().ToArray());
            }
        }


        public UIKit.UIImage ImageFromByteArray(byte[] data)
        {
            if (data == null)
            {
                return null;
            }

            UIKit.UIImage image;
            try
            {
                image = new UIKit.UIImage(Foundation.NSData.FromArray(data));
            }
            catch (Exception e)
            {
                Console.WriteLine("Image load failed: " + e.Message);
                return null;
            }
            return image;
        }


    }
}