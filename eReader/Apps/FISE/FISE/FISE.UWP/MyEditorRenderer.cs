using Kitablet.UWP;
using Kitablet.ViewModels;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(MyEditor), typeof(MyEditorRenderer))]
namespace Kitablet.UWP
{
    public class MyEditorRenderer : EditorRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.BorderThickness = new Windows.UI.Xaml.Thickness(0);
                Control.Background = new SolidColorBrush(Colors.Transparent);
                Control.TextWrapping = Windows.UI.Xaml.TextWrapping.Wrap;
                Control.GotFocus += (sender, evt) => {
                    ((TextBox)sender).Background = new SolidColorBrush(Colors.Transparent);
                };
                Control.LostFocus += (sender, evt) => {
                    ((TextBox)sender).Background = new SolidColorBrush(Colors.Transparent);
                };
            }
        }
    }
}
