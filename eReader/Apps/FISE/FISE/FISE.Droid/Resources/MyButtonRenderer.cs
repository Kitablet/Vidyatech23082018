
using Xamarin.Forms;
using Kitablet.ViewModels;
using Kitablet.Droid;
using Xamarin.Forms.Platform.Android;
using System;

[assembly: ExportRenderer(typeof(MyButton), typeof(MyButtonRenderer))]
namespace Kitablet.Droid
{
    class MyButtonRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);
            try
            {
                if (Control != null)
                {
                    Control.Elevation = 0;
                }
            }
            catch(Exception ex){}
        }
    }
}