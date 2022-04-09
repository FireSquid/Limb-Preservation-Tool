using System;
using SkiaSharp;
using Xamarin.Forms;
using SkiaSharp.Views.Forms;

namespace LimbPreservationTool.CustomControls
{
    public class HighlightCanvas : SKCanvasView
    {
        public static readonly BindableProperty RendererProperty = BindableProperty.Create(
            nameof(Renderer),
            typeof(Renderers.IRenderers),
            typeof(HighlightCanvas),
            null,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                ((HighlightCanvas)bindable).RendererChanged(
                                    (Renderers.IRenderers)oldValue, (Renderers.IRenderers)newValue);
            }
        );

        public Renderers.IRenderers Renderer
        {
            get { return (Renderers.IRenderers)GetValue(RendererProperty); }
            set { SetValue(RendererProperty, value); }
        }

        SKSize size;


        public HighlightCanvas()
        {
            size = CanvasSize;

        }
        void RendererChanged(Renderers.IRenderers currentRenderer,
                            Renderers.IRenderers newRenderer)
        {
            if (currentRenderer != newRenderer)
            {
                // detach the event from old renderer
                if (currentRenderer != null)
                    currentRenderer.RefreshRequested -= Renderer_RefreshRequested;

                // attach the event to new renderer
                if (newRenderer != null)
                    newRenderer.RefreshRequested += Renderer_RefreshRequested;

                // refresh the contrl
                InvalidateSurface();
            }
        }
        void Renderer_RefreshRequested(object sender, EventArgs e)
        {
            InvalidateSurface();
        }
    }
}
