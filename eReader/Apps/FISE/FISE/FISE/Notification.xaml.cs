using Kitablet.CustomControls;
using Kitablet.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace Kitablet
{
    public partial class Notification : MR.Gestures.Grid
    {
        protected List<WebView> webviews;
        protected List<Image> images;
        protected int currentNavPage, index;
        public static List<UserEvent> UserEvents;
        public static List<int> NewEventID;
        MyTimer timer;
        private MainViewModel ViewModel
        {
            get { return BindingContext as MainViewModel; }
        }
        public Notification()
        {         
            InitializeComponent();
            BindingContext = new MainViewModel();
            this.webviews = new List<WebView>();
            this.images = new List<Image>();

            if (!HelperFunctions.CheckInternetConnection())
            {
                ActionTabPage.IsOfflineTextLabel.IsVisible = true;
                Label label = new Label();
                label.VerticalOptions = LayoutOptions.Center;
                label.HorizontalOptions = LayoutOptions.Center;
                label.HorizontalTextAlignment = TextAlignment.Center;
                label.VerticalTextAlignment = TextAlignment.Center;
                label.Text = "Event(s) not available offline.";
                label.FontSize = 30;
                label.SetDynamicResource(Label.TextColorProperty, "PrimaryColor");
                Grid.SetRow(label, 0);
                Grid.SetColumn(label, 0);
                this.Children.Add(label);
            }
            else
            {
                ActionTabPage.IsOfflineTextLabel.IsVisible = false;
                if (UserEvents != null)
                {
                    if (UserEvents.Count > 0)
                    {
                        this.SetView();
                        this.currentNavPage = -1;
                        this.Nav_Clicked(this.images.ElementAt(0), new EventArgs());
                        HideNavBar(true);
                        if (timer == null)
                        {
                            timer = new MyTimer(TimeSpan.FromSeconds(Constant.NumberOfSeconds), () =>
                            {
                                HideNavBar(false);
                            });
                            timer.Start();
                        }
                    }
                    else
                    {
                        Label label = new Label();
                        label.VerticalOptions = LayoutOptions.Center;
                        label.HorizontalOptions = LayoutOptions.Center;
                        label.HorizontalTextAlignment = TextAlignment.Center;
                        label.VerticalTextAlignment = TextAlignment.Center;
                        label.Text = "New event(s) not available.";
                        label.FontSize = 30;
                        label.SetDynamicResource(Label.TextColorProperty, "PrimaryColor");
                        Grid.SetRow(label, 0);
                        Grid.SetColumn(label, 0);
                        this.Children.Add(label);
                    }
                }
            }
        }
        ~Notification()
        {
            ActionTabPage.SendViewedEvents();
        }
        protected void SetNotification()
        {
            int count = 0;
            foreach(UserEvent User_Event in UserEvents)
            {
                if(!User_Event.IsView)
                {
                    count++;
                }
            }
            ActionTabPage.NotificationLabel.Text = count != 0 ? count.ToString() : string.Empty;
        }
        Grid navlayoutContainer;
        protected void SetView()
        {
            try
            {
                AbsoluteLayout pagelayout = new AbsoluteLayout();
                if (Xamarin.Forms.Device.OS == TargetPlatform.Android)
                {
                    Grid layout = new Grid();
                    layout.RowSpacing = 0;
                    layout.ColumnSpacing = 0;
                    layout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
                    layout.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                    for (int i = 0; i < UserEvents.Count; i++)
                    {
                        var htmlSource = new HtmlWebViewSource();
                        htmlSource.Html = UserEvents[i].Description;
                        WebView webview = new WebView();
                        webview.IsVisible = false;
                        webview.Source = htmlSource;
                        Grid.SetRow(webview, 0);
                        Grid.SetColumn(webview, 0);
                        layout.Children.Add(webview);
                        this.webviews.Add(webview);
                    }
                    AbsoluteLayout.SetLayoutBounds(layout, new Rectangle(0, 0, 1, 1));
                    AbsoluteLayout.SetLayoutFlags(layout, AbsoluteLayoutFlags.All);
                    pagelayout.Children.Add(layout);

                    Grid navlayout = new Grid();
                    navlayout.RowSpacing = 0;
                    navlayout.ColumnSpacing = 15;
                    navlayout.RowDefinitions.Add(new RowDefinition { Height = 25 });
                    //navlayout.Padding = new Thickness(0, 0, 0, 20);
                    navlayout.VerticalOptions = LayoutOptions.Center;
                    navlayout.HorizontalOptions = LayoutOptions.Center;
                    for (int i = 0; i < UserEvents.Count; i++)
                    {
                        Image image = new Image();
                        image.WidthRequest = 10;
                        image.HeightRequest = 10;
                        image.SetDynamicResource(Image.SourceProperty, "NavDotImage");
                        image.Aspect = Aspect.AspectFit;
                        image.VerticalOptions = LayoutOptions.Center;
                        image.HorizontalOptions = LayoutOptions.Center;
                        TapGestureRecognizer navTab = new TapGestureRecognizer();
                        navTab.Tapped += Nav_Clicked;
                        image.GestureRecognizers.Add(navTab);
                        Grid.SetRow(image, 0);
                        Grid.SetColumn(image, i);
                        navlayout.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                        navlayout.Children.Add(image);
                        this.images.Add(image);
                    }
                    navlayoutContainer = new Grid
                    {
                        RowSpacing = 0,
                        ColumnSpacing = 0,
                        VerticalOptions = LayoutOptions.Center
                    };
                    BoxView backgroundFade = new BoxView { Opacity = 0.5 };
                    backgroundFade.SetDynamicResource(BoxView.ColorProperty, "HeaderColor");
                    navlayoutContainer.Children.Add(backgroundFade);
                    navlayoutContainer.Children.Add(navlayout);
                    AbsoluteLayout.SetLayoutBounds(navlayout, new Rectangle(0.5, 1, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
                    AbsoluteLayout.SetLayoutFlags(navlayout, AbsoluteLayoutFlags.PositionProportional);
                    AbsoluteLayout.SetLayoutBounds(navlayoutContainer, new Rectangle(1, 1, 1, AbsoluteLayout.AutoSize));
                    AbsoluteLayout.SetLayoutFlags(navlayoutContainer, AbsoluteLayoutFlags.PositionProportional | AbsoluteLayoutFlags.WidthProportional);
                    GestureFrame gi = new GestureFrame
                    {
                        BackgroundColor = Color.Transparent
                    };
                    gi.SwipeLeft += (s, e) =>
                    {
                        Constant.UserActiveTime = DateTime.Now;
                        if (this.index < UserEvents.Count - 1)
                        {
                            this.Nav_Clicked(this.images.ElementAt(this.index + 1), new EventArgs());
                        }
                        ViewModel.SampleCommand.Execute("Swipe Left Detected");
                    };
                    gi.SwipeRight += (s, e) =>
                    {
                        Constant.UserActiveTime = DateTime.Now;
                        if (this.index > 0)
                        {
                            this.Nav_Clicked(this.images.ElementAt(this.index - 1), new EventArgs());
                        }
                        ViewModel.SampleCommand.Execute("Swipe Right Detected");
                    };
                    AbsoluteLayout.SetLayoutBounds(gi, new Rectangle(0, 0, 1, 1));
                    AbsoluteLayout.SetLayoutFlags(gi, AbsoluteLayoutFlags.All);
                    pagelayout.Children.Add(gi);
                    pagelayout.Children.Add(navlayoutContainer);
                }
                else
                {
                    Grid layout = new Grid();
                    layout.RowSpacing = 0;
                    layout.ColumnSpacing = 0;
                    layout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
                    layout.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                    for (int i = 0; i < UserEvents.Count; i++)
                    {
                        var htmlSource = new HtmlWebViewSource();
                        htmlSource.Html = UserEvents[i].Description;
                        WebView webview = new WebView();

                        webview.IsVisible = false;
                        webview.Source = htmlSource;
                        Grid.SetRow(webview, 0);
                        Grid.SetColumn(webview, 0);
                        layout.Children.Add(webview);
                        this.webviews.Add(webview);
                    }
                    AbsoluteLayout.SetLayoutBounds(layout, new Rectangle(0, 0, 1, 1));
                    AbsoluteLayout.SetLayoutFlags(layout, AbsoluteLayoutFlags.All);
                    pagelayout.Children.Add(layout);


                    Grid navlayout = new Grid();
                    navlayout.RowSpacing = 0;
                    navlayout.ColumnSpacing = 15;
                    navlayout.RowDefinitions.Add(new RowDefinition { Height = 25 });
                    //navlayout.Padding = new Thickness(0, 0, 0, 20);
                    navlayout.VerticalOptions = LayoutOptions.Center;
                    navlayout.HorizontalOptions = LayoutOptions.Center;
                    for (int i = 0; i < UserEvents.Count; i++)
                    {
                        Image image = new Image();
                        image.WidthRequest = 10;
                        image.HeightRequest = 10;
                        image.SetDynamicResource(Image.SourceProperty, "NavDotImage");
                        image.Aspect = Aspect.AspectFit;
                        image.VerticalOptions = LayoutOptions.Center;
                        image.HorizontalOptions = LayoutOptions.Center;
                        TapGestureRecognizer navTab = new TapGestureRecognizer();
                        navTab.Tapped += Nav_Clicked;
                        image.GestureRecognizers.Add(navTab);
                        Grid.SetRow(image, 0);
                        Grid.SetColumn(image, i);
                        navlayout.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                        navlayout.Children.Add(image);
                        this.images.Add(image);
                    }
                    navlayoutContainer = new Grid
                    {
                        RowSpacing = 0,
                        ColumnSpacing = 0,
                        VerticalOptions = LayoutOptions.Center
                    };
                    BoxView backgroundFade = new BoxView { Opacity = 0.5 };
                    backgroundFade.SetDynamicResource(BoxView.ColorProperty, "HeaderColor");
                    navlayoutContainer.Children.Add(backgroundFade);
                    navlayoutContainer.Children.Add(navlayout);
                    AbsoluteLayout.SetLayoutBounds(navlayout, new Rectangle(0.5, 1, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
                    AbsoluteLayout.SetLayoutFlags(navlayout, AbsoluteLayoutFlags.PositionProportional);
                    AbsoluteLayout.SetLayoutBounds(navlayoutContainer, new Rectangle(1, 1, 1, AbsoluteLayout.AutoSize));
                    AbsoluteLayout.SetLayoutFlags(navlayoutContainer, AbsoluteLayoutFlags.PositionProportional | AbsoluteLayoutFlags.WidthProportional);
                    GestureFrame gi = new GestureFrame
                    {
                        BackgroundColor = Color.Transparent
                    };
                    gi.SwipeLeft += (s, e) =>
                    {
                        Constant.UserActiveTime = DateTime.Now;
                        if (this.index < UserEvents.Count - 1)
                        {
                            this.Nav_Clicked(this.images.ElementAt(this.index + 1), new EventArgs());
                        }
                        ViewModel.SampleCommand.Execute("Swipe Left Detected");
                    };
                    gi.SwipeRight += (s, e) =>
                    {
                        Constant.UserActiveTime = DateTime.Now;
                        if (this.index > 0)
                        {
                            this.Nav_Clicked(this.images.ElementAt(this.index - 1), new EventArgs());
                        }
                        ViewModel.SampleCommand.Execute("Swipe Right Detected");
                    };
                    AbsoluteLayout.SetLayoutBounds(gi, new Rectangle(0, 0, 1, 1));
                    AbsoluteLayout.SetLayoutFlags(gi, AbsoluteLayoutFlags.All);
                    pagelayout.Children.Add(gi);
                    pagelayout.Children.Add(navlayoutContainer);
                }
                Grid.SetRow(pagelayout, 0);
                Grid.SetColumn(pagelayout, 0);
                this.Children.Add(pagelayout);
            }
            catch (Exception ex)
            {
            }
        }

        private void HideBar_Tapped(object sender, MR.Gestures.TapEventArgs e)
        {
            if (navlayoutContainer.IsVisible == false)
            {
                HideNavBar(true);
                AutoHide();
            }
            else
            {
                HideNavBar(false);
                timer.Stop();
            }
        }

        public void Nav_Clicked(object sender, EventArgs e)
        {
            try
            {
                Constant.UserActiveTime = DateTime.Now;
                ActionTabPage.CheckInternetConnectivity();
                AutoHide();
                this.index = this.images.IndexOf((Image)sender);
                if (this.currentNavPage != this.index)
                {
                    int counter = 0;
                    foreach (Image image in this.images)
                    {
                        if (image == (Image)sender)
                        {
                            image.Opacity = 1;
                            this.webviews[counter].Source = null;
                            var htmlSource = new HtmlWebViewSource();
                            htmlSource.Html = UserEvents[counter].Description;
                            this.webviews[counter].Source = htmlSource;
                        }
                        else
                        {
                            image.Opacity = 0.5;
                        }
                        counter++;
                    }
                    foreach (WebView webview in this.webviews)
                    {
                        webview.IsVisible = false;
                    }
                    this.webviews.ElementAt(this.index).Opacity = 0;
                    this.webviews.ElementAt(this.index).IsVisible = true;
                    this.webviews.ElementAt(this.index).FadeTo(1, 500, Easing.SinIn);
                    UserEvents.ElementAt(this.index).IsView = true;
                    this.SetNotification();
                    this.currentNavPage = this.index;
                }
            }
            catch (Exception ex)
            {

            }
        }



        private void HideNavBar(bool IsVisible)
        {
            if(navlayoutContainer != null)
                navlayoutContainer.IsVisible = IsVisible;
        }

        private void AutoHide()
        {
            if (timer == null)
            {
                timer = new MyTimer(TimeSpan.FromSeconds(Constant.NumberOfSeconds), () =>
                {
                    HideNavBar(false);
                });
                timer.Start();
            }
            else
            {
                timer.Stop();
                timer.Start();
            }
        }
    }
}
