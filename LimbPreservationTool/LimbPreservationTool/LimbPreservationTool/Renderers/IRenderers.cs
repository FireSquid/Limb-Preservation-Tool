using System;
using System.ComponentModel;
using SkiaSharp;
namespace LimbPreservationTool.Renderers
{
    public interface IRenderers
    {
        event EventHandler RefreshRequested;
        SKColor FillColor { get; set; }
        void PaintSurface(SKSurface surface, SKImageInfo info);
    }
}
