﻿using Kitablet.CustomControls;
using Kitablet.Windows;
using System;
using Windows.UI.Xaml.Input;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WinRT;
[assembly: ExportRenderer(typeof(GestureFrame), typeof(CustomGestureRenderer))]
namespace Kitablet.Windows
{
    public delegate void ChangedEventHandler(object sender, EventArgs e);
    public class CustomGestureRenderer : FrameRenderer
    {
        public int X1 { get; set; }
        public int X2 { get; set; }
        public int Y1 { get; set; }
        public int Y2 { get; set; }

        public GestureFrame GestureFrame { get; set; }

        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);

            GestureFrame = (GestureFrame)e.NewElement;

            ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
            ManipulationStarted += GestureFrameRenderer_ManipulationStarted;
            ManipulationCompleted += GestureFrameRenderer_ManipulationCompleted;
        }

        private void GestureFrameRenderer_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            X2 = (int)e.Position.X;
            Y2 = (int)e.Position.Y;

            var xChange = X1 - X2;
            var yChange = Y1 - Y2;

            var xChangeSize = Math.Abs(xChange);
            var yChangeSize = Math.Abs(yChange);

            if (xChangeSize > yChangeSize)
            {
                // horizontal
                if (X1 > X2)
                {
                    // left
                    GestureFrame.OnSwipeLeft();
                }
                else
                {
                    // right
                    GestureFrame.OnSwipeRight();
                }
            }
        }

        private void GestureFrameRenderer_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            X1 = (int)e.Position.X;
            Y1 = (int)e.Position.Y;
        }
    }
}