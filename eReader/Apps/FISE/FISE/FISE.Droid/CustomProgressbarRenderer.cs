using Kitablet.Droid;
using Kitablet.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomProgressbar), typeof(CustomProgressbarRenderer))]
namespace Kitablet.Droid
{
    class CustomProgressbarRenderer : ProgressBarRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ProgressBar> e)
        {
            base.OnElementChanged(e);
            if(Control != null)
            {
                Control.ScaleY = 20;
                string color = Constant.Primarycolor;
                color = color.Replace("#", "");
                string color1 = Constant.Secondary1Color.Replace("#", "");
                //Control.ProgressTintList = Android.Content.Res.ColorStateList.ValueOf(Color.FromRgb(byte.Parse(color1.Substring(0, 2), System.Globalization.NumberStyles.HexNumber), byte.Parse(color1.Substring(2, 2), System.Globalization.NumberStyles.HexNumber), byte.Parse(color1.Substring(4, 2), System.Globalization.NumberStyles.HexNumber)).ToAndroid());
                Control.SetBackgroundColor(Color.FromRgb(byte.Parse(color1.Substring(0, 2), System.Globalization.NumberStyles.HexNumber), byte.Parse(color1.Substring(2, 2), System.Globalization.NumberStyles.HexNumber), byte.Parse(color1.Substring(4, 2), System.Globalization.NumberStyles.HexNumber)).ToAndroid());
                Control.ProgressDrawable.SetColorFilter(Color.FromRgb(byte.Parse(color.Substring(0, 2), System.Globalization.NumberStyles.HexNumber), byte.Parse(color.Substring(2, 2), System.Globalization.NumberStyles.HexNumber), byte.Parse(color.Substring(4, 2), System.Globalization.NumberStyles.HexNumber)).ToAndroid(), Android.Graphics.PorterDuff.Mode.SrcIn);
            }
        }
    }
}