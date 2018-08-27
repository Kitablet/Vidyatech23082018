using Kitablet.Droid;
using Kitablet.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(BookTitle), typeof(_CustomBookTitleRenderer))]
namespace Kitablet.Droid
{
    class _CustomBookTitleRenderer : LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            try
            {
                base.OnElementChanged(e);
                this.Control.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);
                this.UpdateLayout();
            }
            catch (Exception ex) { }
        }
    }
}

