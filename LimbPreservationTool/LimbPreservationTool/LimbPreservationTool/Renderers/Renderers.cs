using System;
using SkiaSharp;
namespace LimbPreservationTool.Renderers
{
    public class PathRenderer : IRenderers
    {
        public event EventHandler RefreshRequested;

        private SKColor fillColor { get; set; } = new SKColor(160, 160, 160);
        public SKColor FillColor
        {
            get => fillColor;
            set
            {
                if (fillColor != value)
                {
                    fillColor = value;
                    RefreshRequested?.Invoke(this, EventArgs.Empty); //"this" is the method passed in
                }
            }
        }

        private SKBitmap imageBitmap { get; set; }
        public SKBitmap ImageBitmap
        {
            get => imageBitmap;
            set
            {
                //changes in bitmap means rerendering of the renderer
                imageBitmap = value;
                RefreshRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        public SKSize rendererSize { get; set; }
        public SKSize RendererSize { get => rendererSize; set => rendererSize = value; }

        public void PaintSurface(SKSurface surface, SKImageInfo info)
        {

            SKCanvas canvas = surface.Canvas;
            canvas.Clear();

            if (imageBitmap != null)
            {
                canvas.DrawBitmap(imageBitmap, 0, 0);
            }
        }
    }
}
