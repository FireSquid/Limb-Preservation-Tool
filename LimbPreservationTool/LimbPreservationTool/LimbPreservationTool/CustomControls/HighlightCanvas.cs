using System;
using SkiaSharp;
using Xamarin.Forms;
using SkiaSharp.Views.Forms;
using LimbPreservationTool.CustomeComponents;
using LimbPreservationTool.Renderers;

namespace LimbPreservationTool.CustomControls
{
    public class HighlightCanvas : SKCanvasView
    {
        public static readonly BindableProperty RendererProperty = BindableProperty.Create(
            nameof(Renderer),
            typeof(PathRenderer),
            typeof(HighlightCanvas),
            null,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                ((HighlightCanvas)bindable).RendererChanged(
                                    (PathRenderer)oldValue, (PathRenderer)newValue);
            }
        );

        public Renderers.PathRenderer Renderer
        {
            get { return (PathRenderer)GetValue(RendererProperty); }
            set { SetValue(RendererProperty, value); }
        }

        SKSize size;

        public static readonly BindableProperty ReceiverProperty = BindableProperty.Create(
            nameof(Receiver),
            typeof(TouchReceiver),
            typeof(HighlightCanvas),
            null,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                ((HighlightCanvas)bindable).ReceiverChanged(
                                    (TouchReceiver)oldValue, (TouchReceiver)newValue);
            }
        );

        public TouchReceiver Receiver
        {
            get { return (TouchReceiver)GetValue(ReceiverProperty); }
            set { SetValue(ReceiverProperty, value); }
        }


        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {

            if (Renderer == null)
            {
                Console.WriteLine("Renderer is null but try painting");
                return;
            }
            Renderer.PaintSurface(e.Surface, e.Info);
            base.OnPaintSurface(e);
            InvalidateSurface();
        }

        protected override void OnTouch(SkiaSharp.Views.Forms.SKTouchEventArgs e)
        {
            //store the location of drawing and draw on image bitmap

            if (Receiver == null)
            {
                Console.WriteLine("Receiver is null but try touching");
                return;
            }
            Console.WriteLine("!");
            Renderer.Receiver.TouchReceive(e);
            base.OnTouch(e);
            e.Handled = true;
            InvalidateSurface();
        }

        public HighlightCanvas()
        {
            size = CanvasSize;
            Receiver = new TouchReceiver();
            EnableTouchEvents = true;
            //Touch += OnTouch;
            Receiver.Width = Width;
            Receiver.Height = Height;
            Receiver.CanvasSize = CanvasSize;

        }


        void ReceiverChanged(TouchReceiver currentReceiver,
                            TouchReceiver newReceiver)
        {
            if (currentReceiver != newReceiver)
            {
                // detach the event from old renderer
                if (currentReceiver != null)
                {

                    //                    Touch -= OnTouch;
                    currentReceiver.RefreshRequested -= Renderer_RefreshRequested;
                }


                // attach the event to new renderer
                if (newReceiver != null)
                {

                    //                    Touch += OnTouch;
                    newReceiver.RefreshRequested += Renderer_RefreshRequested;
                }

                // refresh the contrl
                InvalidateSurface();
            }
        }


        void RendererChanged(PathRenderer currentRenderer,
                            PathRenderer newRenderer)
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
