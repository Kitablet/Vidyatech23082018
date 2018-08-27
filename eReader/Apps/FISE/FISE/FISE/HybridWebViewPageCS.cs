using Xamarin.Forms;

namespace Kitablet
{
    public class HybridWebViewPageCS : ContentPage
    {
        public class ExtendedWebView : WebView { }
        public HybridWebViewPageCS()
        {
            var hybridWebView = new HybridWebView
            {
                Uri = "local.html",
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

           // hybridWebView.RegisterAction(data => DisplayAlert("Alert", "Hello " + data, "OK"));
            Content = hybridWebView;
        }
    }
}
