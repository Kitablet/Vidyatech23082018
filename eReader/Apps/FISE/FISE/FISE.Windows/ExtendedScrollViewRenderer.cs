using Foundation = global::Windows.Foundation;
using Kitablet.Windows;
using Kitablet.ViewModels;
using Xamarin.Forms.Platform.WinRT;
using Xamarin.Forms;
using Windows.UI.Xaml.Controls;
using System;

[assembly: ExportRenderer(typeof(ExtendedScrollView), typeof(ExtendedScrollViewRenderer))]
namespace Kitablet.Windows
{
    public class ExtendedScrollViewRenderer : ViewRenderer<ScrollView, global::Windows.UI.Xaml.Controls.ScrollViewer>
    {
        VisualElement _currentView;

        public ExtendedScrollViewRenderer()
        {
            AutoPackage = false;
        }

        protected IScrollViewController Controller
        {
            get { return Element; }
        }

        public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            SizeRequest result = base.GetDesiredSize(widthConstraint, heightConstraint);
            result.Minimum = new Xamarin.Forms.Size(40, 40);
            return result;
        }

        protected override Foundation.Size ArrangeOverride(Foundation.Size finalSize)
        {
            if (Element == null)
                return finalSize;

            Control?.Arrange(new Foundation.Rect(0, 0, finalSize.Width, finalSize.Height));

            return finalSize;
        }

        protected override void Dispose(bool disposing)
        {
            if (Control != null)
            {
                Control.ViewChanged -= OnViewChanged;
            }

            base.Dispose(disposing);
        }

        protected override Foundation.Size MeasureOverride(Foundation.Size availableSize)
        {
            if (Element == null)
                return new global::Windows.Foundation.Size(0, 0);

            double width = Math.Max(0, Element.Width);
            double height = Math.Max(0, Element.Height);
            var result = new Foundation.Size(width, height);

            Control?.Measure(result);

            return result;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<ScrollView> e)
        {
            base.OnElementChanged(e);
            

            if (e.OldElement != null)
            {
                ((IScrollViewController)e.OldElement).ScrollToRequested -= OnScrollToRequested;
            }

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    SetNativeControl(new ScrollViewer { HorizontalScrollBarVisibility = ScrollBarVisibility.Auto, VerticalScrollBarVisibility = ScrollBarVisibility.Auto, ZoomMode=ZoomMode.Disabled });

                    Control.ViewChanged += OnViewChanged;
                }

                Controller.ScrollToRequested += OnScrollToRequested;

                UpdateOrientation();

                LoadContent();
            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == "Content")
                LoadContent();
            else if (e.PropertyName == Layout.PaddingProperty.PropertyName)
                UpdateMargins();
            else if (e.PropertyName == ScrollView.OrientationProperty.PropertyName)
                UpdateOrientation();
        }

        void LoadContent()
        {
            if (_currentView != null)
            {
                //_currentView.Cleanup();
            }
            _currentView = Element.Content;
            IVisualElementRenderer renderer = null;
            if (_currentView != null)
            {
                renderer = _currentView.GetOrCreateRenderer();
            }
            Control.Content = renderer != null ? renderer.ContainerElement : null;
            UpdateMargins();
        }

        void OnScrollToRequested(object sender, ScrollToRequestedEventArgs e)
        {
            double x = e.ScrollX, y = e.ScrollY;
            ScrollToMode mode = e.Mode;
            if (mode == ScrollToMode.Element)
            {
                Point pos = Controller.GetScrollPositionForElement((VisualElement)e.Element, e.Position);
                x = pos.X;
                y = pos.Y;
                mode = ScrollToMode.Position;
            }

            if (mode == ScrollToMode.Position)
            {
                Control.ChangeView(x, y, null, !e.ShouldAnimate);
            }
            Controller.SendScrollFinished();
        }

        void OnViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            Controller.SetScrolledPosition(Control.HorizontalOffset, Control.VerticalOffset);

            if (!e.IsIntermediate)
                Controller.SendScrollFinished();
        }

        void UpdateMargins()
        {
            var element = Control.Content as global::Windows.UI.Xaml.FrameworkElement;
            if (element == null)
                return;

            switch (Element.Orientation)
            {
                case ScrollOrientation.Horizontal:
                    element.Margin = new global::Windows.UI.Xaml.Thickness(Element.Padding.Left, 0, Element.Padding.Right, 0);
                    break;
                case ScrollOrientation.Vertical:
                    element.Margin = new global::Windows.UI.Xaml.Thickness(0, Element.Padding.Top, 0, Element.Padding.Bottom);
                    break;
                case ScrollOrientation.Both:
                    element.Margin = new global::Windows.UI.Xaml.Thickness(Element.Padding.Left, Element.Padding.Top, Element.Padding.Right, Element.Padding.Bottom);
                    break;
            }
        }

        void UpdateOrientation()
        {
            if (Element.Orientation == ScrollOrientation.Horizontal || Element.Orientation == ScrollOrientation.Both)
            {
                Control.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            }
            else
            {
                Control.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            }
        }
    }
}
