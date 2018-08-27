
using Kitablet.Windows;
using Kitablet.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WinRT;

[assembly: ExportRenderer(typeof(CustomLabel), typeof(CustomLabelRenderer))]
namespace Kitablet.Windows
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
