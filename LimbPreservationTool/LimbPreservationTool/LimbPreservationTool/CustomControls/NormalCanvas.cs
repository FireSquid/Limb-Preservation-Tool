using System;
using SkiaSharp;
using Xamarin.Forms;
using SkiaSharp.Views.Forms;
using LimbPreservationTool.CustomeComponents;

namespace LimbPreservationTool.CustomControls
{
    public class NormalCanvas : SKCanvasView
    {
        public static readonly BindableProperty RendererProperty = BindableProperty.Create(
            nameof(Renderer),
            typeof(Renderers.PathRenderer),
            typeof(NormalCanvas),
            null,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                ((NormalCanvas)bindable).RendererChanged(
                                    (Renderers.PathRenderer)oldValue, (Renderers.PathRenderer)newValue);
            }
        );

        // 2. Change the Renderer property
        public Renderers.PathRenderer Renderer
        {
            get { return (Renderers.PathRenderer)GetValue(RendererProperty); }
            set { SetValue(RendererProperty, value); }
        }

        public static readonly BindableProperty SizeProperty = BindableProperty.Create(
                        nameof(Size),
                        typeof(SKSize),
                        typeof(NormalCanvas),
                        null,
                        defaultBindingMode: BindingMode.TwoWay,
                        propertyChanged: (bindable, oldValue, newValue) => {; }
                        );
        public SKSize Size
        {
            get { return (SKSize)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        public NormalCanvas()
        {
            //not initializing an Renderer to force viewmodel to bind a Renderer
            //The new Renderer will be part of overriding OnPaintSurface method in SKCanvasView
            //base.GetI/
        }

        protected override void OnTouch(SKTouchEventArgs e)
        {
            //store the location of drawing and draw on image bitmap

            Renderer.Receiver.TouchReceive(e);
            base.OnTouch(e);

            InvalidateSurface();
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {

            Size = CanvasSize;// Size should be retrieved with view's eventhandler,yet it is set for convinence
            Renderer.PaintSurface(e.Surface, e.Info);
            base.OnPaintSurface(e);
        }

        void RendererChanged(Renderers.PathRenderer currentRenderer,
                    Renderers.PathRenderer newRenderer)
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
                //InvalidateSurface();
            }
        }

        //method used as property handler, will be  invoked when renderer changed
        void Renderer_RefreshRequested(object sender, EventArgs e)
        {

            InvalidateSurface();
        }

    }
}
