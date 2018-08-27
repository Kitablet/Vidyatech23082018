
using Kitablet.UWP;
using Kitablet.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(CustomLabel), typeof(CustomLabelRenderer))]
namespace Kitablet.UWP
{
    class CustomLabelRenderer : LabelRenderer
    {
        protected CustomLabel LineSpacingLabel { get; private set; }
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            try
            {
                base.OnElementChanged(e);
                if (Control != null)
                {
                    var lineSpacingLabel = (CustomLabel)this.Element;
                    Control.LineHeight = lineSpacingLabel.LineSpacing;
                }
            }
            catch(Exception ex)
            {

            }
           
        }
    }
}
