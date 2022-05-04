using System;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Collections.Generic;

namespace LimbPreservationTool.CustomComponents
{
    public class TouchReceiver
    {

        public event EventHandler RefreshRequested;
        public bool highlightmode = false;
        public SKSize CanvasSize { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        Dictionary<long, SKPath> inProgressPaths;
        List<SKPath> completedPaths;
        SKPaint paint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Blue,
            StrokeWidth = 10,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round
        };
        public TouchReceiver()
        {
            inProgressPaths = new Dictionary<long, SKPath>();
            completedPaths = new List<SKPath>();
        }
        public void RemoveAll()
        {
            inProgressPaths = new Dictionary<long, SKPath>();
            completedPaths = new List<SKPath>();
        }
        public void DrawAllPath(SKCanvas canvas, SKPaint paint)
        {
            //Test drawing  normal path
            //using (SKPath pipePath = new SKPath())
            //{
            //    pipePath.MoveTo(50, 50);
            //    pipePath.CubicTo(0, 1.25f * info.Height,
            //                     info.Width - 0, 1.25f * info.Height,
            //                     info.Width - 50, 50);

            //    canvas.DrawPath(pipePath, paint);
            //}
            foreach (SKPath path in completedPaths)
                Console.WriteLine("completed:" + path.ToString());

            foreach (SKPath path in inProgressPaths.Values)
                Console.WriteLine("inProgress:" + path.ToString());
            //SKCanvas canvas = surface.Canvas;
            foreach (SKPath path in completedPaths)
            {
                canvas.DrawPath(path, paint);
            }

            foreach (SKPath path in inProgressPaths.Values)
            {
                canvas.DrawPath(path, paint);
            }

        }
        SKPoint ConvertToPixel(SKPoint pt)
        {
            return new SKPoint((float)(CanvasSize.Width * pt.X / Width),
                               (float)(CanvasSize.Height * pt.Y / Height));
        }

        public bool Fresh()
        {

            return inProgressPaths.Count == 0 && completedPaths.Count == 0;



        }




        public void TouchReceive(SKTouchEventArgs e)
        {
            if (!highlightmode)
            {
                return;
            }
            switch (e.ActionType)
            {
                case SKTouchAction.Pressed:
                    if (!inProgressPaths.ContainsKey(e.Id))
                    {
                        Console.WriteLine("Pressed");
                        SKPath path = new SKPath();
                        path.MoveTo(e.Location);
                        inProgressPaths.Add(e.Id, path);
                    }
                    break;

                case SKTouchAction.Moved:
                    if (inProgressPaths.ContainsKey(e.Id))
                    {

                        Console.WriteLine("Moved");
                        SKPath path = inProgressPaths[e.Id];
                        path.LineTo(e.Location);
                        inProgressPaths[e.Id] = path;
                    }
                    break;

                case SKTouchAction.Released:
                    if (inProgressPaths.ContainsKey(e.Id))
                    {

                        Console.WriteLine("Released");
                        completedPaths.Add(inProgressPaths[e.Id]);

                        inProgressPaths.Remove(e.Id);
                        RefreshRequested?.Invoke(this, EventArgs.Empty);
                    }
                    break;

                case SKTouchAction.Cancelled:
                    if (inProgressPaths.ContainsKey(e.Id))
                    {

                        Console.WriteLine("Cancelled");
                        inProgressPaths.Remove(e.Id);
                    }
                    break;
            }

        }
    }
}
