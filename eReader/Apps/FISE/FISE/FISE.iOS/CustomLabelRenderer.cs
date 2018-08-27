
using FISE.iOS;
using FISE.ViewModels;
using Foundation;
using System;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomLabel), typeof(CustomLabelRenderer))]
namespace FISE.iOS
{
    class CustomLabelRenderer : LabelRenderer
    {
        protected CustomLabel LineSpacingLabel { get; private set; }
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Label> e)
        {
            try
            {
                base.OnElementChanged(e);
                if (Control != null)
                {
                    var lineSpacingLabel = (CustomLabel)this.Element;
                    lineSpacingLabel.LineSpacing = 5;
                    var paragraphStyle = new NSMutableParagraphStyle()
                    {
                        LineSpacing = (nfloat)lineSpacingLabel.LineSpacing
                    };
                    var _string = new NSMutableAttributedString(lineSpacingLabel.Text);
                    var style = UIStringAttributeKey.ParagraphStyle;
                    var range = new NSRange(0, _string.Length);

                    _string.AddAttribute(style, paragraphStyle, range);

                    this.Control.AttributedText = _string;

                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
