using CoreGraphics;
using FISE.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using FISE.ViewModels;

[assembly: ExportRenderer(typeof(CustomProgressbar), typeof(CustomProgressBarRenderer))]
namespace FISE.iOS
{

    public class CustomProgressBarRenderer : ProgressBarRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ProgressBar> e)
        {
            base.OnElementChanged(e);
            Control.ProgressTintColor = Color.FromHex(Constant.Primarycolor).ToUIColor();
            Control.TrackTintColor = Color.FromHex(Constant.Secondary1Color).ToUIColor();
        }
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            var X = 1f;
            var Y = 10.0f;
            CGAffineTransform transform = CGAffineTransform.MakeScale(X, Y);
            this.Transform = transform;
        }
    }
}