using Kitablet.UWP;
using Kitablet.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(CustomBookTitle), typeof(CustomBookTitleRenderer))]
namespace Kitablet.UWP
{
    class CustomBookTitleRenderer : LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            try
            {
                base.OnElementChanged(e);
                if (Control != null)
                {
                    Control.MinHeight = 40;
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
