using System;
using SkiaSharp;
using Xamarin.Forms;
using SkiaSharp.Views.Forms;

namespace LimbPreservationTool.CustomControls
{
    public class SKRendererControl : SKCanvasView
    {
        public static readonly BindableProperty RendererProperty = BindableProperty.Create(
            nameof(Renderer),
            typeof(Renderers.IRenderers),
            typeof(SKRendererControl),
            null,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                ((SKRendererControl)bindable).RendererChanged(
                                    (Renderers.IRenderers)oldValue, (Renderers.IRenderers)newValue);
            }
        );

        // 2. Change the Renderer property
        public Renderers.IRenderers Renderer
        {
            get { return (Renderers.IRenderers)GetValue(RendererProperty); }
            set { SetValue(RendererProperty, value); }
        }

        public SKRendererControl()
        {
            //not initializing an Renderer to force viewmodel to bind a Renderer
            //The new Renderer will be part of overriding OnPaintSurface method in SKCanvasView

        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            Renderer.PaintSurface(e.Surface, e.Info);
            base.OnPaintSurface(e);
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


        //method used as property handler, will be  invoked when renderer changed
        void Renderer_RefreshRequested(object sender, EventArgs e)
        {

            InvalidateSurface();
        }

    }
}
