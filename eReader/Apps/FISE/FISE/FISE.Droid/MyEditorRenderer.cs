using Xamarin.Forms;
using Kitablet.ViewModels;
using Kitablet.Droid;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(MyEditor), typeof(MyEditorRenderer))]
namespace Kitablet.Droid
{
    public class MyEditorRenderer : EditorRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
            {
                Control.SetBackgroundColor(global::Android.Graphics.Color.Transparent);
            }
        }
    }
}