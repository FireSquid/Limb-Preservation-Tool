using System;
using LimbPreservationTool.CustomeComponents;
using SkiaSharp;
namespace LimbPreservationTool.Renderers
{

    public class NormalRenderer : IRenderers
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

        private SKSize rendererSize { get; set; }
        public SKSize RendererSize { get => rendererSize; set { rendererSize = value; } }


        public NormalRenderer()
        {

            imageBitmap = null;
        }


        public void PaintSurface(SKSurface surface, SKImageInfo info)
        {
            SKCanvas canvas = surface.Canvas;
            canvas.Clear(SKColors.White);

            if (imageBitmap != null)
            {

                canvas.DrawBitmap(imageBitmap, info.Rect);
            }
        }
    }

    public class PathRenderer : IRenderers
    {
        private SKBitmap src { get; set; }
        public SKBitmap Src
        {
            get => src; set
            {
                src = new SKBitmap();
                Console.WriteLine("Seting src");

                Console.WriteLine(value.Info.BytesSize);
                if (value == null)
                {
                    Console.WriteLine("but its null");
                }

                Console.WriteLine(src.Info.BytesSize);
                value.CopyTo(src);
                Console.WriteLine(src.Info.BytesSize);
                RefreshRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        private SKBitmap dest { get; set; }
        public SKBitmap Dest
        {
            get => dest; set
            {
                dest = value;
                RefreshRequested?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler RefreshRequested;

        private TouchReceiver receiver { get; set; }
        public TouchReceiver Receiver
        {
            get => receiver;
            set
            {
                receiver = value;

                RefreshRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        public void StartHighlight()
        {
            receiver.highlightmode = true;
            RefreshRequested?.Invoke(this, EventArgs.Empty);
        }
        public void EndHighlight()
        {
            receiver.highlightmode = false;
        }
        public SKBitmap PorterDuff()
        {

            SKBitmap tmp = src.Copy();
            using (SKCanvas view = new SKCanvas(tmp))
            {

                using (SKPaint paint = new SKPaint
                {
                    //TextSize = 64.0f,
                    //IsAntialias = true,
                    //Color = new SKColor(255, 255, 0).WithAlpha(0x50),
                    //Style = SKPaintStyle.Fill
                    Style = SKPaintStyle.Stroke,
                    Color = SKColors.Blue,
                    StrokeWidth = 50,
                    StrokeCap = SKStrokeCap.Round,
                    StrokeJoin = SKStrokeJoin.Round


                })
                {



                    //paint.BlendMode = SKBlendMode.SrcIn;
                    paint.BlendMode = SKBlendMode.SrcATop;
                    //paint.BlendMode = SKBlendMode.SrcIn;
                    receiver.DrawAllPath(view, paint);

                    //canvas.DrawBitmap(matteBitmap, info.Rect);
                    //canvas.DrawBitmap(imageBitmap, info.Rect, paint);
                    //canvas.DrawBitmap(imageBitmap, info.Rect);
                    // canvas.DrawPaint(paint);
                }

            }

            return tmp.Copy();
        }

        public void ClearPath()
        {
            Receiver.RemoveAll();
            RefreshRequested?.Invoke(this, EventArgs.Empty);
        }

        public PathRenderer()
        {

            //src = null;
            receiver = new TouchReceiver();
        }

        public void PaintSurface(SKSurface surface, SKImageInfo info)
        {

            SKCanvas canvas = surface.Canvas;
            var file = surface.Snapshot();
            canvas.Clear();

            Console.WriteLine(" try DrawSrc");

            if (src != null)
            {

                Console.WriteLine(src.Info.BytesSize);
                Console.WriteLine("DrawSrc");
                SKBitmap tmp = src.Copy();
                canvas.DrawBitmap(tmp, info.Rect);


                using (SKPaint paint = new SKPaint
                {
                    //TextSize = 64.0f,
                    //IsAntialias = true,
                    //Color = new SKColor(255, 255, 0).WithAlpha(0x50),
                    //Style = SKPaintStyle.Fill
                    Style = SKPaintStyle.Stroke,
                    Color = SKColors.Blue,
                    StrokeWidth = 50,
                    StrokeCap = SKStrokeCap.Round,
                    StrokeJoin = SKStrokeJoin.Round


                })
                {

                    using (SKCanvas view = new SKCanvas(tmp))
                    {

                        //canvas.Clear();
                        receiver.DrawAllPath(canvas, paint);
                        //receiver.DrawAllPath(canvas,info, paint);

                        //canvas.DrawBitmap(matteBitmap, info.Rect);
                        //paint.BlendMode = SKBlendMode.DstIn;
                        //canvas.DrawBitmap(imageBitmap, info.Rect, paint);
                        //canvas.DrawBitmap(imageBitmap, info.Rect);
                        // canvas.DrawPaint(paint);
                    }

                }

            }
        }
    }
}
