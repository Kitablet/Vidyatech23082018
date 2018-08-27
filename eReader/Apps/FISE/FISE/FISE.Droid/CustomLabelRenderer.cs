using Kitablet.Droid;
using Kitablet.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomLabel), typeof(CustomLabelRenderer))]
namespace Kitablet.Droid
{
    class CustomLabelRenderer : LabelRenderer
    {
        protected CustomLabel LineSpacingLabel { get; private set; }
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            try
            {
                base.OnElementChanged(e);
                if (e.OldElement == null)
                {
                    this.LineSpacingLabel = (CustomLabel)this.Element;
                }
                double lineSpacing = this.LineSpacingLabel.LineSpacing;
                lineSpacing = 1.2;
                this.Control.SetLineSpacing(1f, (float)lineSpacing);
                this.UpdateLayout();
            }
            catch(Exception ex) { }
        }
    }
}

