using Kitablet.Windows;
using Kitablet.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WinRT;

[assembly: ExportRenderer(typeof(CustomBookTitle), typeof(CustomBookTitleRenderer))]
namespace Kitablet.Windows
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
