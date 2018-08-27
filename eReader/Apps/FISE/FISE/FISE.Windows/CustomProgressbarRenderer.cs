using Kitablet.Windows;
using Kitablet.ViewModels;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WinRT;

[assembly: ExportRenderer(typeof(CustomProgressbar), typeof(CustomProgressbarRenderer))]
namespace Kitablet.Windows
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

                color = Constant.Secondary1Color;
                color = color.Replace("#", "");
                Control.Background = new SolidColorBrush(ColorHelper.FromArgb(255,
                byte.Parse(color.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                byte.Parse(color.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                byte.Parse(color.Substring(4, 2), System.Globalization.NumberStyles.HexNumber)));
            }
        }
       
    }
}