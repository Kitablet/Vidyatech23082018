using Kitablet.Windows;
using Kitablet.ViewModels;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WinRT;

[assembly: ExportRenderer(typeof(MyEntry), typeof(MyEntryRenderer))]
namespace Kitablet.Windows
{
    public class MyEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.BorderThickness = new global::Windows.UI.Xaml.Thickness(0);
                Control.Background = new SolidColorBrush(Colors.Transparent);
                Control.BackgroundFocusBrush = new SolidColorBrush(Colors.Transparent);
            }
        }


    }
}
