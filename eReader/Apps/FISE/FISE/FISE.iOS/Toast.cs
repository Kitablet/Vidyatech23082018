using Foundation;
using FISE.iOS;
using UIKit;
using CoreGraphics;
using System;

[assembly: Xamarin.Forms.Dependency(typeof(MessageIOS))]
namespace FISE.iOS
{
    public class MessageIOS : IMessage
    {
        private readonly UIView _view;
        private readonly UILabel _label;
        private const int Margin = 30;
        private const int Height = 40;
        private const int Width = 400;
        private NSTimer _timer;

        public void LongAlert(string message)
        {
            ShowAlert(message);
        }

        public void ShortAlert(string message)
        {
            ShowAlert(message);
        }
        public void ShowAlert(string message)
        {
            var toast = new MessageIOS();
            toast.Show(UIApplication.SharedApplication.KeyWindow.RootViewController.View, message);
        }
        public MessageIOS()
        {
            _view = new UIView(new CGRect(0, 0, 0, 0))
            {
                BackgroundColor = UIColor.FromRGB(0, 175, 240)
            };
            _view.Layer.CornerRadius = (nfloat)20.0;

            _label = new UILabel(new CGRect(0, 0, 0, 0))
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.White
            };
            _view.AddSubview(_label);

        }

        public void Show(UIView parent, string message)
        {
            if (_timer != null)
            {
                _timer.Invalidate();
                _view.RemoveFromSuperview();
            }

            _view.Alpha = (nfloat)0.7;

            _view.Frame = new CGRect(
                (parent.Bounds.Width - Width) / 2,
                parent.Bounds.Height - Height - Margin,
                Width,
                Height);

            _label.Frame = new CGRect(0, 0, Width, Height);
            _label.Text = message;

            parent.AddSubview(_view);

            var wait = 10;
            _timer = NSTimer.CreateRepeatingScheduledTimer(TimeSpan.FromMilliseconds(100), delegate {
                if (_view.Alpha <= 0)
                {
                    _timer.Invalidate();
                    _view.RemoveFromSuperview();
                }
                else
                {
                    if (wait > 0)
                    {
                        wait--;
                    }
                    else
                    {
                        _view.Alpha -= (nfloat)0.05;
                    }
                }
            });
        }
    }
}