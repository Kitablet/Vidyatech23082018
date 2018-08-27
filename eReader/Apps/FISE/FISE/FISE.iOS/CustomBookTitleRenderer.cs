
using FISE.iOS;
using FISE.ViewModels;
using Foundation;
using System;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomBookTitle), typeof(CustomBookTitleRenderer))]
namespace FISE.iOS
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
                    var titleLabel = (CustomBookTitle)this.Element;
                    var paragraphStyle = new NSMutableParagraphStyle()
                    {
                        LineSpacing = 3
                    };
                    var _string = new NSMutableAttributedString(titleLabel.Text);
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
