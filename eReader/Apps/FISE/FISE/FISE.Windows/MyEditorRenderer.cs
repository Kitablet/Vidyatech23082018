using Kitablet.Windows;
using Kitablet.ViewModels;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WinRT;

[assembly: ExportRenderer(typeof(MyEditor), typeof(MyEditorRenderer))]
namespace Kitablet.Windows
{
    public class MyEditorRenderer : EditorRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.BorderThickness = new global::Windows.UI.Xaml.Thickness(0);
                Control.Background = new SolidColorBrush(Colors.Transparent);
                Control.TextWrapping = global::Windows.UI.Xaml.TextWrapping.Wrap;                
                Control.GotFocus += (sender, evt) => {
                    Control.Background = new SolidColorBrush(Colors.Transparent);
                };
            }
        }
    }
}
