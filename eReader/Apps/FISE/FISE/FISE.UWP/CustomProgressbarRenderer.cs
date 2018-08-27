
using Kitablet;
using Kitablet.UWP;
using Kitablet.ViewModels;
using System;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(CustomProgressbar), typeof(CustomProgressbarRenderer))]
namespace Kitablet.UWP
{
    class CustomProgressbarRenderer : ProgressBarRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ProgressBar> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
            {
                Control.Height = 20;
                string color = Constant.Primarycolor;
                color = color.Replace("#", "");
                Control.Foreground = new SolidColorBrush(ColorHelper.FromArgb(255,
                byte.Parse(color.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                byte.Parse(color.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                byte.Parse(color.Substring(4, 2), System.Globalization.NumberStyles.HexNumber)));
            }
        }
       
    }
}