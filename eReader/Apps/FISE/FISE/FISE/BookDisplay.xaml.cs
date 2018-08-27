using Kitablet.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamarin.Forms;

namespace Kitablet
{
    public partial class BookDisplay : ContentPage
    {
        public string id, EPUB_Name, BasePagePath;
        public bool IsLandscape, isReadAloud, isCompleted, isTouch, isPagerAllowed;
        public static bool isAutoPlay, isPlaying, isLoading;
        protected XDocument package;
        protected XElement metadata, manifest, spine;
        public int PageCount, currentPage, displayPageCount, ReadFrom, ReadPage, AudioPage, FileIndex;
        protected List<string> PageNames, PageNumber;
        public IAudio _audioPlayer;
        protected double Position;
        public DateTime StartTime, PageReadStart;
        public List<int> PageIndex;
        public List<AudioFile> AudioFiles = new List<AudioFile>();
        public TimeSpan AudioDuration;
        public List<HtmlWebViewSource> htmlSource = new List<HtmlWebViewSource>();
        public static string AudioState = string.Empty;
        MyTimer timer;
        Rectangle rect;

        public double ConstantWidth = 902, ConstantHeight = 676, ContainerScroll, ContainerPadding,
                        ContainerScale, ContainerWidth, ContainerHeight, ContainerScaleWidth,
                        ContainerScaleHeight, DeviceWidth, DeviceHeight, InputAvgWidth, InputAvgHeight,
                        InputWidth1, InputWidth2, PageWidth, PageHeight, pos_x = 0, pos_y = 0, scaleFactor;
        public List<double> InputHeight1 = new List<double>();
        public List<double> InputHeight2 = new List<double>();
        public bool isScrollVisible = false;

        Animation WaitAnimation;

        public class AudioFile
        {
            public string PageName { get; set; }
            public string FileName { get; set; }
            public TimeSpan Duration { get; set; }
            public bool IsCCFile { get; set; }
        }

        bool pageChangedOperation = false;

        public BookDisplay(string id, string E_Name, bool ReadAloud, bool IsLandscape, int ReadFrom, int PageDisplay, bool isPagerAllowed)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            this.StartTime = DateTime.Now;
            InitializeComponent();

            if (AppData.DeviceDetail.DeviceOS.Equals("Windows 8.1"))
            {
                ScrollAnimationControls.Padding = new Thickness(0, 0, 0, 70);
            }
            else
            {
                ScrollAnimationControls.Padding = new Thickness(0, 0, 0, 62);
            }

            Utils.AddUserSyncBook(id);

            this.id = id;
            this.EPUB_Name = E_Name;
            this.BasePagePath = Path.Combine(Constant.FileDirectoryStructure, this.EPUB_Name, "EPUB");
            this.isReadAloud = ReadAloud;
            this.ReadFrom = ReadFrom;
            BookDisplay.isAutoPlay = false;
            BookDisplay.isPlaying = false;
            this.IsLandscape = IsLandscape;
            this.displayPageCount = PageDisplay;
            this.isPagerAllowed = isPagerAllowed;
            this.DeviceWidth = Application.Current.MainPage.Width - 122;
            this.DeviceHeight = Application.Current.MainPage.Height - 30;

            IntializeBook();

            HideHeaderFooter(true);
            if (timer == null)
            {
                timer = new MyTimer(TimeSpan.FromSeconds(Constant.NumberOfSeconds), () =>
                {
                    HideHeaderFooter(false);
                });
                timer.Start();
            }
        }

        public void closeBtn_Clicked(object sender, EventArgs e)
        {
            Constant.UserActiveTime = DateTime.Now;
            this.PageLoader.IsVisible = true;
            Task.Run(() =>
            {
                Task.Delay(100).Wait();
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        DateTime EndTime = DateTime.Now;
                        Utils.SetBookReadingTime(this.id, this.StartTime, EndTime);
                        Utils.SetBookCurrentPage(this.id, this.currentPage);
                        if (this.PageIndex.Count > 0)
                        {
                            Utils.SetPageProgress(this.id, this.PageIndex, this.PageReadStart, EndTime);
                        }
                        WebViewContainer.Children.Clear();
                        BookPager.Children.Clear();
                        Navigation.PushAsync(new BookRead(this.id, false, ReadFrom));
                        Navigation.RemovePage(this);
                    }
                    catch (Exception ex)
                    {

                    }
                });
            });
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Constant.UserActiveTime = DateTime.Now;
            this.PageLoader.IsVisible = true;

            Task.Run(() =>
            {
                Task.Delay(100).Wait();
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        DateTime EndTime = DateTime.Now;
                        Utils.SetBookReadingTime(this.id, this.StartTime, EndTime);
                        Utils.SetBookCurrentPage(this.id, this.currentPage);
                        if (this.PageIndex.Count > 0)
                        {
                            Utils.SetPageProgress(this.id, this.PageIndex, this.PageReadStart, EndTime);
                        }
                        WebViewContainer.Children.Clear();
                        BookPager.Children.Clear();
                        Navigation.PushAsync(new BookRead(this.id, false, ReadFrom));
                        Navigation.RemovePage(this);
                    }
                    catch (Exception ex)
                    {

                    }
                });
            });
            return true;
        }
        bool isFirstLoading = false;
        public void IntializeBook()
        {
            isFirstLoading = true;
            try
            {
                UserBook UserBookElement = AppData.UserDetails.UserBooks.UserBook.Where(x => x.BookId.Equals(this.id)).FirstOrDefault();

                UserBookElement.IsReadLater = "false";
                UserProgressBook UserProgressBookElement = AppData.UserProgress.UserProgressBooks.UserProgressBook.Where(x => x.BookId.Equals(this.id)).FirstOrDefault();
                if (UserProgressBookElement != null)
                {
                    UserProgressBookElement.IsReadLater = "false";
                }
                AppData.UserDetails.LastAccessedBookId = this.id;

                if (!string.IsNullOrEmpty(UserBookElement.Bookmark.CurrentPage?.Trim()))
                {
                    this.ReadPage = Int32.Parse(UserBookElement.Bookmark.CurrentPage?.Trim());
                }
                else
                {
                    this.ReadPage = 0;
                }

                this.currentPage = this.ReadPage > 0 ? this.ReadPage - this.displayPageCount : 0;

                if ((Xamarin.Forms.Device.OS == TargetPlatform.Android) || (Device.OS == TargetPlatform.iOS))
                {
                    this.isTouch = true;
                    this.SetControlsVisibility(-1);
                }
                else
                {
                    this.isTouch = false;
                }

                XDocument audioXML = null;
                if (this.isReadAloud)
                {
                    _audioPlayer = DependencyService.Get<IAudio>();
                    this.AutoPlayBtn.IsVisible = true;
                    audioXML = XDocument.Parse(AppData.FileService.LoadText(Path.Combine(this.BasePagePath, "audio.xml")));
                }
                else
                {
                    this.AutoPlayBtn.IsVisible = false;
                }

                this.PageIndex = new List<int>();

                string path = Path.Combine(this.BasePagePath, "package.opf");
                this.package = XDocument.Parse(AppData.FileService.LoadText(path));

                if (this.package != null)
                {
                    this.metadata = this.package.Root.Descendants().Where(x => x.Name.LocalName == "metadata").ToList().FirstOrDefault();
                    this.manifest = this.package.Root.Descendants().Where(x => x.Name.LocalName == "manifest").ToList().FirstOrDefault();
                    this.spine = this.package.Root.Descendants().Where(x => x.Name.LocalName == "spine").ToList().FirstOrDefault();
                }
                if (this.metadata != null)
                {
                    foreach (XElement element in this.metadata.Elements())
                    {
                        if (element.Attribute("property") != null && element.Attribute("property").Value.Trim().Equals("media:height"))
                        {
                            string val = element.Value;
                            this.InputAvgHeight = !string.IsNullOrEmpty(val) ? Double.Parse(val) : 0;
                            continue;
                        }
                        if (element.Attribute("property") != null && element.Attribute("property").Value.Trim().Equals("media:width"))
                        {
                            string val = element.Value;
                            this.InputAvgWidth = !string.IsNullOrEmpty(val) ? Double.Parse(val) : 0;
                            continue;
                        }
                        if (element.Attribute("property") != null && element.Attribute("property").Value.Trim().Equals("media:Pagewidth1"))
                        {
                            string val = element.Value;
                            this.InputWidth1 = !string.IsNullOrEmpty(val) ? Double.Parse(val) : 0;
                            continue;
                        }
                        if (element.Attribute("property") != null && element.Attribute("property").Value.Trim().Equals("media:Pagewidth2"))
                        {
                            string val = element.Value;
                            this.InputWidth2 = !string.IsNullOrEmpty(val) ? Double.Parse(val) : 0;
                            continue;
                        }
                    }
                }
                if (this.spine != null)
                {
                    this.PageCount = this.spine.Elements().Count();
                    this.PageNames = new List<string>();
                    this.PageNumber = new List<string>();
                    if (this.manifest != null)
                    {
                        foreach (XElement element in this.spine.Elements())
                        {
                            this.PageNames.Add(this.manifest.Descendants().Where(e => e.Attribute("id").Value == element.Attribute("idref").Value).FirstOrDefault().Attribute("href").Value);
                            this.PageNumber.Add(element.Attribute("pageno") != null && !string.IsNullOrEmpty(element.Attribute("pageno").Value) ? element.Attribute("pageno").Value : "");
                            this.AudioFiles.Add(new AudioFile { PageName = this.PageNames.Last() });
                            double pgHt = -1;
                            double pg2Ht = -1;
                            if (element.Attribute("height1") != null)
                                pgHt = Double.Parse(element.Attribute("height1").Value);
                            if (element.Attribute("height2") != null)
                                pg2Ht = Double.Parse(element.Attribute("height2").Value);
                            this.InputHeight1.Add(pgHt);
                            this.InputHeight2.Add(pg2Ht);
                        }
                    }
                }
                //this.PageNames.RemoveAt(this.PageCount - 1);
                this.PageCount--;

                if (audioXML != null && audioXML.Root != null)
                {
                    for (int i = 0; i < this.AudioFiles.Count; i++)
                    {
                        XElement element = audioXML.Root.Descendants("File").Where(x => x.Attribute("Page").Value.Trim().ToLower().Equals(this.AudioFiles.ElementAt(i).PageName.ToLower())).FirstOrDefault();
                        if (element != null && !string.IsNullOrEmpty(element.Attribute("Name").Value.Trim()))
                        {
                            this.AudioFiles.ElementAt(i).FileName = element.Attribute("Name").Value.Trim();
                            this.AudioFiles.ElementAt(i).Duration = TimeSpan.Parse(element.Attribute("Duration").Value.Trim());
                            if (element.Attribute("IsCCFile") != null)
                                this.AudioFiles.ElementAt(i).IsCCFile = Boolean.Parse(element.Attribute("IsCCFile").Value.Trim());
                        }
                    }
                }

                if (!this.isPagerAllowed)
                {
                    singlePagerBtn.IsVisible = false;
                    doublePagerBtn.IsVisible = false;
                }
                this.setPager();

                if (this.currentPage == 0)
                {
                    this.LoadPage();
                    this.SetControlsVisibility(0);
                }
                else
                {
                    if (this.currentPage % 2 != 0 && this.displayPageCount == 2)
                    {
                        this.currentPage++;
                    }
                    this.NextPage();
                }

                if (this.PageCount <= this.displayPageCount)
                {
                    this.isCompleted = true;
                }
            }
            catch (Exception ex) { }
        }

        public void AutoPlayBtn_Clicked(object sender, EventArgs e)
        {
            clickedFromActionTab = true;
            Constant.UserActiveTime = DateTime.Now;
            AutoHide();
            if (!BookDisplay.isAutoPlay)
            {
                this.AutoPlayBtn.SetDynamicResource(Button.ImageProperty, "ReadAloudSoundImageBlack");
                this.singlePagerBtn.IsEnabled = false;
                this.doublePagerBtn.IsEnabled = false;
                this.singlePagerBtn.SetDynamicResource(Button.ImageProperty, "SinglePagerDeactiveImage");
                this.doublePagerBtn.SetDynamicResource(Button.ImageProperty, "DoublePagerDeactiveImage");
                BookDisplay.isAutoPlay = true;
            }
            else
            {
                this.AutoPlayBtn.SetDynamicResource(Button.ImageProperty, "ReadAloudMuteImageBlack");
                this.singlePagerBtn.IsEnabled = true;
                this.doublePagerBtn.IsEnabled = true;
                if (this.displayPageCount == 2)
                {
                    this.singlePagerBtn.SetDynamicResource(Button.ImageProperty, "SinglePagerImage");
                    this.doublePagerBtn.IsEnabled = false;
                }
                else if (this.displayPageCount == 1)
                {
                    this.doublePagerBtn.SetDynamicResource(Button.ImageProperty, "DoublePagerImage");
                    this.singlePagerBtn.IsEnabled = false;
                }
                BookDisplay.isAutoPlay = false;
            }
            this.OperateMediaFile();
        }

        double scrollAmount = 2d;

        bool lgPressUp = false;
        public void ScrollUpHandler()
        {
            this.pos_y += scrollAmount;
            if (this.pos_y < this.ContainerPadding)
            {
                AbsoluteLayout.SetLayoutBounds(this.WebViewContainer, new Rectangle(this.pos_x, this.pos_y, this.ContainerScaleWidth, this.PageHeight));
                UpBtn.Opacity = 1;
            }
            else
            {
                this.pos_y -= scrollAmount;
                UpBtn.Opacity = 0.5;
            }
            DownBtn.Opacity = 1.0;
        }

        public void ScrollDownHandler()
        {
            this.pos_y -= scrollAmount;
            if (this.pos_y > -(this.ContainerScroll - this.ContainerPadding))
            {
                AbsoluteLayout.SetLayoutBounds(this.WebViewContainer, new Rectangle(this.pos_x, this.pos_y, this.ContainerScaleWidth, this.PageHeight));
                DownBtn.Opacity = 1;
            }
            else
            {
                this.pos_y += scrollAmount;
                DownBtn.Opacity = 0.5;
            }
            UpBtn.Opacity = 1.0;
        }

        public void longPressUp()
        {
            ScrollUpHandler();
        }

        bool lgPressDown = false;
        public void longPressDown()
        {
            ScrollDownHandler();
        }

        public void UpPressing(object sender, EventArgs e)
        {
            lgPressUp = true;
            //longPressUp();
            var milliSeconds = TimeSpan.FromMilliseconds(1);

            Device.StartTimer(milliSeconds, () =>
            {

                // call your method to check for notifications here

                // Returning true means you want to repeat this timer

                longPressUp();
                return lgPressUp;
            });
        }

        public void UpPressed(object sender, EventArgs e)
        {
            lgPressUp = false;
        }

        public void DownPressing(object sender, EventArgs e)
        {
            lgPressDown = true;
            var milliSeconds = TimeSpan.FromMilliseconds(1);

            Device.StartTimer(milliSeconds, () =>
            {

                // call your method to check for notifications here

                // Returning true means you want to repeat this timer

                longPressDown();
                return lgPressDown;
            });

        }

        public void DownPressed(object sender, EventArgs e)
        {
            lgPressDown = false;
        }
        bool clickedFromActionTab = false;
        public void ZoomInBtn_Clicked(object sender, EventArgs e)
        {
            clickedFromActionTab = true;
            isScrollVisible = false;
            ScrollControls.IsVisible = false;
            AutoHide();
            Constant.UserActiveTime = DateTime.Now;

            MrScale += 0.10;
            ZoomOutBtn.IsEnabled = true;
            ZoomOutBtn.SetDynamicResource(Button.ImageProperty, "ZoomOutImage");
            if (MrScale < 2)
            {
                NavigationControls.IsVisible = true;
                WebViewContainer.Scale = MrScale;
            }
            else
            {
                ZoomInBtn.IsEnabled = false;
                ZoomInBtn.SetDynamicResource(Button.ImageProperty, "ZoomInDeactiveImage");
            }
        }

        public void ZoomOutBtn_Clicked(object sender, EventArgs e)
        {
            clickedFromActionTab = true;
            double zoomoutThreshold = 1;
            if (scaleFactor > this.ContainerScale)
                zoomoutThreshold = scaleFactor;
            Constant.UserActiveTime = DateTime.Now;
            AutoHide();
            ZoomInBtn.IsEnabled = true;
            ZoomInBtn.SetDynamicResource(Button.ImageProperty, "ZoomInImage");
            if (MrScale > zoomoutThreshold)
            {
                MrScale -= 0.10;
                if (MrScale <= zoomoutThreshold)
                {
                    MrTranslationX = 0;
                    MrTranslationY = 0;
                    if (this.ContainerScroll > 0)
                    {
                        isScrollVisible = true;
                        ScrollControls.IsVisible = true;
                    }
                    MrScale = zoomoutThreshold;
                    if (this.isTouch)
                    {
                        NavigationControls.IsVisible = false;
                    }
                    else
                    {
                        NavigationControls.IsVisible = true;
                    }
                    ZoomOutBtn.IsEnabled = false;
                    ZoomOutBtn.SetDynamicResource(Button.ImageProperty, "ZoomOutDeactiveImage");
                    this.RepositionContainer();
                }
                WebViewContainer.Scale = MrScale;
            }
            else
            {
                if (this.isTouch)
                {
                    NavigationControls.IsVisible = false;
                }
                else
                {
                    NavigationControls.IsVisible = true;
                }
                ZoomOutBtn.IsEnabled = false;
                ZoomOutBtn.SetDynamicResource(Button.ImageProperty, "ZoomOutDeactiveImage");
            }
        }

        public void LoadPage()
        {
            try
            {
                HideHeaderFooter(true);
                this.DefaultReadAloud();

                if (this.PageIndex.Count > 0)
                {
                    Utils.SetPageProgress(this.id, this.PageIndex, this.PageReadStart, DateTime.Now);
                }

                this.PageReadStart = DateTime.Now;

                if (this.displayPageCount == 1 && this.PageNames.ElementAt(this.currentPage).ToLower().Contains("blank"))
                {
                    this.currentPage++;
                }

                this.SetPageHeight(this.currentPage);

                this.SetDefaultPageAttribute();

                if (this.currentPage == this.PageCount)
                {
                    this.PageIndex.Add(this.currentPage + 1);

                    WebView view = this.AddPage(0, false);

                    if (this.isReadAloud)
                    {
                        this.AudioPage = 0;
                        BookDisplay.isLoading = true;
                        this.FileIndex = this.currentPage;
                        if (this.displayPageCount == 2 && string.IsNullOrEmpty(this.AudioFiles.ElementAt(this.FileIndex).FileName))
                        {
                            this.AudioPage = 1;
                            this.FileIndex = this.currentPage + 1;
                        }
                    }

                    this.WebViewContainer.Children.Add(view);

                    Label pager = new Label { Text = this.PageNumber.ElementAt(this.currentPage), WidthRequest = this.PageWidth, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 20 };
                    pager.SetDynamicResource(Label.StyleProperty, "LabelNormalRegular");
                    this.BookPager.Children.Add(pager, 0, 0);

                    this.currentPage++;
                    this.ReadPage = this.currentPage > this.ReadPage ? this.currentPage : this.ReadPage;

                    if (this.displayPageCount == 2)
                    {
                        this.WebViewContainer.Children.Add(new Grid { WidthRequest = this.PageWidth, HeightRequest = this.PageHeight, BackgroundColor = Color.Transparent });
                        Label Blankpager = new Label { WidthRequest = this.PageWidth, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 20 };
                        Blankpager.SetDynamicResource(Label.StyleProperty, "LabelNormalRegular");
                        this.BookPager.Children.Add(Blankpager, 1, 0);
                    }
                }
                else
                {
                    for (int i = 0; i < this.displayPageCount; i++)
                    {
                        if (this.currentPage != this.PageCount)
                        {
                            this.PageIndex.Add(this.currentPage + 1);

                            WebView view = this.AddPage(i, false);

                            if (this.isReadAloud && i == 0)
                            {
                                this.AudioPage = 0;
                                BookDisplay.isLoading = true;
                                this.FileIndex = this.currentPage;
                                if (this.displayPageCount == 2 && string.IsNullOrEmpty(this.AudioFiles.ElementAt(this.FileIndex).FileName))
                                {
                                    this.AudioPage = 1;
                                    this.FileIndex = this.currentPage + 1;
                                }
                            }

                            this.WebViewContainer.Children.Add(view);

                            Label pager = new Label { Text = this.PageNumber.ElementAt(this.currentPage), WidthRequest = this.PageWidth, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 20 };
                            pager.SetDynamicResource(Label.StyleProperty, "LabelNormalRegular");
                            this.BookPager.Children.Add(pager, i, 0);

                            this.currentPage++;
                            this.ReadPage = this.currentPage > this.ReadPage ? this.currentPage : this.ReadPage;
                        }
                        else
                        {
                            WebView view = this.AddPage(i, true);
                            this.WebViewContainer.Children.Add(view);
                            this.BookPager.Children.Add(new Label { WidthRequest = this.PageWidth, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 20 }, i, 0);
                        }
                    }
                }
                HideHeaderFooter(false);
            }
            catch (Exception ex)
            {
            }
        }

        public void NextPage()
        {

            try
            {
                HideHeaderFooter(true);
                this.DefaultReadAloud();

                DateTime EndTime = DateTime.Now;
                if (this.PageIndex.Count > 0)
                {
                    Utils.SetPageProgress(this.id, this.PageIndex, this.PageReadStart, EndTime);
                }

                this.PageReadStart = DateTime.Now;

                if (this.isCompleted && this.currentPage == this.PageCount + 1)
                {
                    Task.Run(() =>
                    {
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                        {
                            Utils.SetBookReadingTime(this.id, this.StartTime, EndTime);
                            this.ReadPage = 0;
                            Utils.SetBookCompleted(this.id);
                            Navigation.PushAsync(new BookCompleted(this.id, this.ReadFrom));
                            Navigation.RemovePage(this);
                        });
                    });
                    return;
                }

                if (this.displayPageCount == 1 && this.PageNames.ElementAt(this.currentPage).ToLower().Contains("blank"))
                {
                    this.currentPage++;
                }

                this.SetPageHeight(this.currentPage);

                this.SetDefaultPageAttribute();

                if (this.currentPage == this.PageCount)
                {
                    this.PageIndex.Add(this.currentPage + 1);

                    WebView view = this.AddPage(0, false);

                    if (this.isReadAloud)
                    {
                        this.AudioPage = 0;
                        BookDisplay.isLoading = true;
                        this.FileIndex = this.currentPage;
                        if (this.displayPageCount == 2 && string.IsNullOrEmpty(this.AudioFiles.ElementAt(this.FileIndex).FileName))
                        {
                            this.AudioPage = 1;
                            this.FileIndex = this.currentPage + 1;
                        }
                    }

                    this.WebViewContainer.Children.Add(view);

                    Label pager = new Label { Text = this.PageNumber.ElementAt(this.currentPage), WidthRequest = this.PageWidth, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 20 };
                    pager.SetDynamicResource(Label.StyleProperty, "LabelNormalRegular");
                    this.BookPager.Children.Add(pager, 0, 0);

                    this.currentPage++;
                    this.ReadPage = this.currentPage > this.ReadPage ? this.currentPage : this.ReadPage;

                    if (this.displayPageCount == 2)
                    {
                        this.WebViewContainer.Children.Add(new Grid { WidthRequest = this.PageWidth, HeightRequest = this.PageHeight, BackgroundColor = Color.Transparent });
                        Label Blankpager = new Label { WidthRequest = this.PageWidth, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 20 };
                        Blankpager.SetDynamicResource(Label.StyleProperty, "LabelNormalRegular");
                        this.BookPager.Children.Add(Blankpager, 1, 0);
                    }
                }
                else
                {
                    for (int i = 0; i < this.displayPageCount; i++)
                    {
                        if (this.currentPage != this.PageCount)
                        {
                            this.PageIndex.Add(this.currentPage + 1);

                            WebView view = this.AddPage(i, false);

                            if (this.isReadAloud && i == 0)
                            {
                                this.AudioPage = 0;
                                BookDisplay.isLoading = true;
                                this.FileIndex = this.currentPage;
                                if (this.displayPageCount == 2 && string.IsNullOrEmpty(this.AudioFiles.ElementAt(this.FileIndex).FileName))
                                {
                                    this.AudioPage = 1;
                                    this.FileIndex = this.currentPage + 1;
                                }
                            }

                            this.WebViewContainer.Children.Add(view);

                            Label pager = new Label { Text = this.PageNumber.ElementAt(this.currentPage), WidthRequest = this.PageWidth, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 20 };
                            pager.SetDynamicResource(Label.StyleProperty, "LabelNormalRegular");
                            this.BookPager.Children.Add(pager, i, 0);

                            this.currentPage++;
                            this.ReadPage = this.currentPage > this.ReadPage ? this.currentPage : this.ReadPage;
                        }
                        else
                        {
                            WebView view = this.AddPage(i, true);
                            this.WebViewContainer.Children.Add(view);
                            this.BookPager.Children.Add(new Label { WidthRequest = this.PageWidth, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 20 }, i, 0);
                        }
                    }
                }
                if (this.displayPageCount == 1 && this.currentPage - this.displayPageCount == 1 && this.PageNames.ElementAt(this.currentPage - this.displayPageCount - 1).ToLower().Contains("blank"))
                {
                    this.PreviousBtn.IsVisible = false;
                }
                else
                {
                    this.SetControlsVisibility(1);
                }
                if (this.currentPage == this.PageCount + 1)
                {
                    this.isCompleted = true;
                }
                HideHeaderFooter(false);
            }
            catch (Exception ex)
            {

            }
        }

        public void PreviousPage()
        {
            try
            {
                HideHeaderFooter(true);
                if (this.currentPage > this.displayPageCount && PreviousBtn.IsVisible == true)
                {

                    this.DefaultReadAloud();

                    if (this.currentPage == this.PageCount + 1)
                    {
                        this.isCompleted = false;
                        this.currentPage--;
                    }
                    else
                    {
                        if ((this.currentPage % this.displayPageCount) == 0)
                        {
                            this.currentPage -= this.displayPageCount;
                        }
                        else
                        {
                            this.currentPage -= (this.currentPage % this.displayPageCount);
                        }
                    }

                    if (this.PageIndex.Count > 0)
                    {
                        Utils.SetPageProgress(this.id, this.PageIndex, this.PageReadStart, DateTime.Now);
                    }

                    this.PageReadStart = DateTime.Now;

                    if (this.displayPageCount == 1 && this.currentPage > 0 && this.PageNames.ElementAt(this.currentPage - 1).ToLower().Contains("blank"))
                    {
                        this.currentPage--;
                    }

                    int valutobepassed = -1;
                    if (this.currentPage == this.PageCount && this.currentPage % 2 != 0)
                    {
                        valutobepassed = this.currentPage - 1;
                    }
                    else
                    {
                        valutobepassed = this.currentPage - this.displayPageCount;
                    }

                    this.SetPageHeight(valutobepassed);

                    this.SetDefaultPageAttribute();

                    List<WebView> Listview = new List<WebView>();

                    for (int i = this.displayPageCount - 1; i >= 0; i--)
                    {
                        if (i > 0 && this.currentPage == this.PageCount && this.currentPage % 2 != 0)
                        {
                            WebView view = this.AddPage(i, true);
                            Listview.Add(view);
                            this.BookPager.Children.Add(new Label { WidthRequest = this.PageWidth, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 20 }, i, 0);
                            continue;
                        }
                        if (this.currentPage > 0)
                        {
                            this.currentPage--;
                            this.PageIndex.Add(this.currentPage + 1);

                            WebView view = this.AddPage(i, false);
                            Listview.Add(view);

                            Label pager = new Label { Text = this.PageNumber.ElementAt(this.currentPage), WidthRequest = this.PageWidth, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 20 };
                            pager.SetDynamicResource(Label.StyleProperty, "LabelNormalRegular");
                            this.BookPager.Children.Add(pager, i, 0);
                        }
                    }
                    Listview.Reverse();

                    if (Xamarin.Forms.Device.OS == TargetPlatform.Android)
                    {
                        this.htmlSource.Reverse();
                    }

                    if (this.isReadAloud)
                    {
                        this.AudioPage = 0;
                        BookDisplay.isLoading = true;
                        this.FileIndex = this.currentPage;
                        if (this.displayPageCount == 2 && string.IsNullOrEmpty(this.AudioFiles.ElementAt(this.FileIndex).FileName))
                        {
                            this.AudioPage = 1;
                            this.FileIndex = this.currentPage + 1;
                        }
                    }

                    foreach (WebView v in Listview)
                    {
                        this.WebViewContainer.Children.Add(v);
                    }

                    if (this.displayPageCount == 1 && this.currentPage == 1 && this.PageNames.ElementAt(this.currentPage - 1).ToLower().Contains("blank"))
                    {
                        this.PreviousBtn.IsVisible = false;
                    }
                    else
                    {
                        this.SetControlsVisibility(2);
                    }
                    this.currentPage += this.displayPageCount;
                    if (this.currentPage == this.PageCount + 1)
                    {
                        this.currentPage--;
                    }
                }
                else
                {
                    if (this.isTouch)
                    {
                        NavigationControls.IsVisible = false;
                    }
                    else
                    {
                        NavigationControls.IsVisible = true;
                    }
                }
                HideHeaderFooter(false);
            }
            catch (Exception ex) { }
        }

        public void SetDefaultPageAttribute()
        {
            try
            {
                if (this.ContainerScale > 1)
                    MrScale = this.ContainerScale;
                else
                    MrScale = 1;
                this.scaleFactor = this.ContainerScale;
                this.WebViewContainer.Children.Clear();
                this.RepositionContainer();
                this.WebViewContainer.Scale = MrScale;
                BookPager.Children.Clear();
                this.PageIndex.Clear();
                this.htmlSource.Clear();
                MrTranslationX = 0;
                MrTranslationY = 0;
                //BookPager.WidthRequest = this.ContainerScaleWidth;
                BookPager.WidthRequest = this.PageWidth * 2;
                this.ZoomInBtn.IsEnabled = true;
                this.ZoomOutBtn.IsEnabled = false;
                this.ZoomInBtn.SetDynamicResource(Button.ImageProperty, "ZoomInImage");
                this.ZoomOutBtn.SetDynamicResource(Button.ImageProperty, "ZoomOutDeactiveImage");

                ScrollAnimation.Animation(false);
            }
            catch (Exception ex)
            {

            }
        }

        void SetContainer()
        {
            try
            {
                //Calculate Ratio
                double HeightRatio = this.InputAvgHeight / this.InputAvgWidth;

                //Set Container Width and Height
                this.ContainerWidth = this.displayPageCount == 1 ? this.InputWidth1 : this.InputWidth2;
                this.ContainerHeight = this.ContainerWidth * HeightRatio;

                //Update Container Width and Height according to Constant
                if (this.ContainerHeight > this.ConstantHeight)
                {
                    this.ContainerHeight = this.ConstantHeight;
                }

                //Update Container Width and Height according to Device
                if (this.ContainerHeight > this.DeviceHeight)
                {
                    this.ContainerHeight = this.DeviceHeight;
                    //this.ContainerWidth = this.ContainerHeight / HeightRatio;
                }

                this.PageWidth = this.ContainerWidth;
                this.ContainerWidth = this.displayPageCount == 1 ? this.ContainerWidth : this.ContainerWidth * 2;

                //Set Scale
                this.ContainerScale = 1;
                this.ContainerScaleWidth = this.ContainerWidth;
                this.ContainerScaleHeight = this.ContainerHeight;

                //----------------------------------Set Padding------------------------------                
                this.ContainerPadding = ((this.DeviceHeight - this.ContainerHeight) / 2) < 0 ? 15 : ((this.DeviceHeight - this.ContainerHeight) / 2) + 15;

                AbsoluteLayout.SetLayoutBounds(PaddingTop, new Rectangle(0, 0, 1, this.ContainerPadding));
                AbsoluteLayout.SetLayoutBounds(PaddingBottom, new Rectangle(0, 1, 1, this.ContainerPadding));
            }
            catch (Exception ex)
            {

            }
        }

        void SetPageHeight(int curPage)
        {
            try
            {
                //Set Page Height
                double bkPage1Height, bkPage2Height;
                if (this.displayPageCount == 2)
                {
                    bkPage1Height = this.InputHeight2[curPage];
                    if ((curPage % 2 == 0) && ((curPage + 1) == this.PageCount))
                    {
                        bkPage2Height = -1d;
                    }
                    else
                    {
                        if (curPage < this.PageCount)
                            bkPage2Height = this.InputHeight2[curPage + 1];
                        else
                            bkPage2Height = -1d;
                    }
                    this.PageHeight = Math.Max(bkPage1Height, bkPage2Height);
                }
                else
                {
                    bkPage1Height = this.InputHeight1[curPage];
                    this.PageHeight = bkPage1Height;
                }

                if (this.PageHeight <= this.ContainerHeight)
                {
                    this.PageHeight = this.ContainerHeight;
                    this.ContainerScroll = 0;
                }
                else
                {
                    this.ContainerScroll = (this.PageHeight - this.ContainerHeight) > 2 ? Math.Floor(this.PageHeight - this.ContainerHeight) : 0;
                }
            }
            catch (Exception ex)
            {

            }
        }

        public WebView AddPage(int i, bool isBlankPage)
        {
            try
            {
                //this.WebViewAbsolute.Swiped -= OnMrSwiped;
                giframe.Swiped -= OnMrSwiped;
            }
            catch (Exception ex)
            {

            }
            //this.PageLoader.IsVisible = true;
            if (this.WaitAnimation != null)
            {
                this.AbortAnimation("WaitAnimation");
                this.WaitAnimation = null;
                //this.PageLoader.IsVisible = false;
            }
            this.WaitAnimation = new Animation((v) =>
            {
                if (v == 1)
                {
                    this.PageLoader.IsVisible = true;
                }
            }, 0, 1);
            this.WaitAnimation.Commit(this, "WaitAnimation", 0, 500, Easing.Linear, null, () => false);
            try
            {
                WebView view = new WebView();
                if (isFirstLoading)
                {
                    view.IsVisible = false;
                }
                AbsoluteLayout.SetLayoutBounds(view, new Rectangle(i * this.PageWidth, 0, this.PageWidth, this.PageHeight));

                if (this.ContainerScroll > 0)
                {
                    isScrollVisible = true;
                    ScrollControls.IsVisible = false;
                    ScrollAnimationControls.IsVisible = true;
                    //ScrollAnimation.Animation(true);
                    //Task.Run(() =>
                    //{
                    //    Task.Delay(3000).Wait();
                    //    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                    //    {
                    //        ScrollControls.IsVisible = true;
                    //        ScrollAnimationControls.IsVisible = false;
                    //        ScrollAnimation.Animation(false);
                    //    });
                    //});
                    //ScrollControls.IsVisible = true;
                    UpBtn.Opacity = 0.5;
                    DownBtn.Opacity = 1.0;
                }
                else
                {
                    isScrollVisible = false;
                    ScrollControls.IsVisible = false;
                }

                HtmlWebViewSource HTML_Source = new HtmlWebViewSource();
                string HTML_Text;

                if (isBlankPage)
                {
                    HTML_Text = AppData.FileService.LoadText(Path.Combine(this.BasePagePath, "Blank.xhtml"));
                    view.Navigated += Blank_WebView_Navigated;
                }
                else
                {
                    HTML_Text = AppData.FileService.LoadEncryptedFile(Path.Combine(AppData.FileService.GetLocalLocalFolderPath(), this.BasePagePath, this.PageNames.ElementAt(this.currentPage)), Constant.UserCryptoKey);
                    view.Navigated += WebView_Navigated;
                }

                if (string.IsNullOrEmpty(HTML_Text))
                {
                    HTML_Text = "<?xml version='1.0' encoding='utf-8'?><html xmlns='http://www.w3.org/1999/xhtml' lang='en'><head> <meta name='viewport' content='width=device-width, initial-scale=1'/> <link rel='stylesheet' href='css/style.css'/> <script type='text/javascript' src='lib/jquery-1.11.3.js'></script> <script type='text/javascript' src='lib/control.js'></script></head><body></body></html>";
                }

                if (Xamarin.Forms.Device.OS == TargetPlatform.Android)
                {
                    HTML_Source.BaseUrl = "file://" + AppData.FileService.GetLocalLocalFolderPath() + "/" + this.BasePagePath + "/";
                    HTML_Text = HTML_Text.Replace("</head>", "<style> *{-webkit-touch-callout: none;-webkit-user-select: none;-khtml-user-select: none;-moz-user-select: none;-ms-user-select: none;user-select: none;} </style> </head>");
                    this.htmlSource.Add(HTML_Source);
                    HTML_Source.Html = HTML_Text;
                    view.Source = HTML_Source;
                }
                else if (Device.OS == TargetPlatform.iOS)
                {
                    HTML_Source.BaseUrl = AppData.FileService.GetLocalLocalFolderPath() + "/" + this.BasePagePath + "/";
                    HTML_Source.Html = HTML_Text;
                    view.Source = HTML_Source;
                }
                else if (Xamarin.Forms.Device.OS == TargetPlatform.Windows || Xamarin.Forms.Device.OS == TargetPlatform.WinPhone)
                {
                    AppData.FileService.SaveText(Path.Combine(this.BasePagePath, "test" + (i + 1) + ".xhtml"), HTML_Text);
                    view.Source = "ms-appdata:///local/" + Path.Combine(this.BasePagePath, "test" + (i + 1) + ".xhtml").Replace("\\", "/");
                }
                return view;
            }
            catch (Exception ex)
            {
                WebView view = new WebView();
                view.WidthRequest = this.PageWidth;
                view.HeightRequest = this.PageHeight;

                HtmlWebViewSource HTML_Source = new HtmlWebViewSource();
                HTML_Source.Html = "<?xml version='1.0' encoding='utf-8'?><html xmlns='http://www.w3.org/1999/xhtml' lang='en'><head> <meta name='viewport' content='width=device-width, initial-scale=1'/></head><body></body></html>";

                view.Source = HTML_Source;

                return view;
            }
        }

        private void Blank_WebView_Navigated(object s, WebNavigatedEventArgs e)
        {
            try
            {
                if (((WebView)s) == this.WebViewContainer.Children[this.WebViewContainer.Children.Count - 1] || this.WebViewContainer.Children.Where(x => x is WebView).Count() == 1)
                {
                    if (isFirstLoading)
                    {
                        Task.Run(() =>
                        {
                            Task.Delay(500).Wait();
                            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                            {
                                foreach (View _v in this.WebViewContainer.Children)
                                {
                                    _v.IsVisible = true;
                                }
                                if (this.WaitAnimation != null)
                                {
                                    this.AbortAnimation("WaitAnimation");
                                    this.WaitAnimation = null;
                                    this.PageLoader.IsVisible = false;
                                }
                                //this.PageLoader.IsVisible = false;
                                isFirstLoading = false;
                            });
                        });
                    }
                    else
                    {
                        if (this.WaitAnimation != null)
                        {
                            this.AbortAnimation("WaitAnimation");
                            this.WaitAnimation = null;
                            this.PageLoader.IsVisible = false;
                        }
                        //this.PageLoader.IsVisible = false;
                    }
                    if (ScrollAnimationControls.IsVisible)
                    {
                        ScrollAnimation.Animation(true);
                        Task.Run(() =>
                        {
                            Task.Delay(3000).Wait();
                            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                            {
                                if (isScrollVisible)
                                    ScrollControls.IsVisible = true;
                                ScrollAnimationControls.IsVisible = false;
                                ScrollAnimation.Animation(false);
                            });
                        });
                    }
                    try
                    {
                        //this.WebViewAbsolute.Swiped -= OnMrSwiped;
                        giframe.Swiped -= OnMrSwiped;
                    }
                    catch (Exception ex)
                    {

                    }
                    //this.WebViewAbsolute.Swiped += OnMrSwiped;
                    giframe.Swiped += OnMrSwiped;
                    if (this.isTouch)
                    {
                        NavigationControls.IsVisible = false;
                    }
                    else
                    {
                        NavigationControls.IsVisible = true;
                    }
                }
                rect = this.WebViewContainer.Bounds;
            }
            catch (Exception ex)
            {

            }
        }

        public void WebView_Navigated(object s, WebNavigatedEventArgs e)
        {
            try
            {
                if (s is WebView)
                {
                    //((WebView)s).Eval("ResizePage()");
                    //if (this.displayPageCount > 1)
                    //{
                    //    if ((i % 2) == 0)
                    //    {
                    //        ((WebView)s).Eval("AlignPage(0)");
                    //    }
                    //    else
                    //    {
                    //        ((WebView)s).Eval("AlignPage(1)");
                    //    }
                    //}
                    if (this.isReadAloud)
                    {
                        BookDisplay.isLoading = false;
                        ((WebView)s).Eval("LoadCCFile()");
                        ((WebView)s).Eval("StopTimer()");
                        ((WebView)s).Eval("ClearTimer()");
                        if (((WebView)s) == this.WebViewContainer.Children[this.AudioPage])
                        {
                            if (BookDisplay.isAutoPlay)
                            {
                                this.LoadMediaFile(true);
                            }
                            else
                            {
                                this.LoadMediaFile();
                            }
                        }
                    }
                }

                if (((WebView)s) == this.WebViewContainer.Children[this.WebViewContainer.Children.Count - 1] || this.WebViewContainer.Children.Where(x => x is WebView).Count() == 1)
                {
                    if (isFirstLoading)
                    {
                        Task.Run(() =>
                        {
                            Task.Delay(500).Wait();
                            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                            {
                                foreach (View _v in this.WebViewContainer.Children)
                                {
                                    _v.IsVisible = true;
                                }
                                if (this.WaitAnimation != null)
                                {
                                    this.AbortAnimation("WaitAnimation");
                                    this.WaitAnimation = null;
                                    this.PageLoader.IsVisible = false;
                                }
                                //this.PageLoader.IsVisible = false;
                                isFirstLoading = false;
                            });
                        });
                    }
                    else
                    {
                        if (this.WaitAnimation != null)
                        {
                            this.AbortAnimation("WaitAnimation");
                            this.WaitAnimation = null;
                            this.PageLoader.IsVisible = false;
                        }
                        //this.PageLoader.IsVisible = false;
                    }

                    if (ScrollAnimationControls.IsVisible)
                    {
                        ScrollAnimation.Animation(true);
                        Task.Run(() =>
                        {
                            Task.Delay(3000).Wait();
                            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                            {
                                if (isScrollVisible)
                                    ScrollControls.IsVisible = true;
                                ScrollAnimationControls.IsVisible = false;
                                ScrollAnimation.Animation(false);
                            });
                        });
                    }
                    try
                    {
                        //this.WebViewAbsolute.Swiped -= OnMrSwiped;
                        giframe.Swiped -= OnMrSwiped;
                    }
                    catch (Exception ex)
                    {

                    }
                    //this.WebViewAbsolute.Swiped += OnMrSwiped;
                    giframe.Swiped += OnMrSwiped;
                    if (this.isTouch)
                    {
                        NavigationControls.IsVisible = false;
                    }
                    else
                    {
                        NavigationControls.IsVisible = true;
                    }
                }
                rect = this.WebViewContainer.Bounds;
            }
            catch (Exception ex)
            {

            }
        }

        public void NextBtn_Clicked(object sender, EventArgs e)
        {
            if (pageChangedOperation == true)
                return;
            pageChangedOperation = true;
            Constant.UserActiveTime = DateTime.Now;
            if (this.isTouch)
            {
                NavigationControls.IsVisible = false;
            }
            else
            {
                NavigationControls.IsVisible = false;
            }
            this.NextPage();
            pageChangedOperation = false;
        }

        public void PreviousBtn_Clicked(object sender, EventArgs e)
        {
            if (pageChangedOperation == true)
                return;
            pageChangedOperation = true;
            Constant.UserActiveTime = DateTime.Now;
            if (this.isTouch)
            {
                NavigationControls.IsVisible = false;
            }
            else
            {
                NavigationControls.IsVisible = false;
            }
            this.PreviousPage();
            pageChangedOperation = false;
        }

        public void doublePagerBtn_Clicked(object sender, EventArgs e)
        {
            Constant.UserActiveTime = DateTime.Now;
            AutoHide();
            this.displayPageCount = 2;
            this.setPager(true);
        }

        public void singlePagerBtn_Clicked(object sender, EventArgs e)
        {
            Constant.UserActiveTime = DateTime.Now;
            AutoHide();
            this.displayPageCount = 1;
            this.setPager(true);
        }

        private void setPager(bool isPager = false)
        {
            if (WebViewContainer.Children.Count != this.displayPageCount)
            {
                isScrollVisible = false;
                ScrollControls.IsVisible = false;
                ScrollAnimationControls.IsVisible = false;
                if (isPager)
                {
                    this.DefaultReadAloud();
                    if (this.isCompleted)
                    {
                        this.currentPage--;
                    }
                    else
                    {
                        if (this.currentPage == this.PageCount && this.currentPage % 2 != 0)
                        {
                            this.currentPage--;
                        }
                        else
                        {
                            this.currentPage -= this.displayPageCount == 1 ? 2 : 1;
                            if (this.currentPage % 2 != 0 && this.displayPageCount == 2)
                            {
                                this.currentPage--;
                            }
                        }
                    }
                }

                if (this.displayPageCount == 1)
                {
                    singlePagerBtn.IsEnabled = false;
                    doublePagerBtn.IsEnabled = true;
                    singlePagerBtn.SetDynamicResource(Button.ImageProperty, "SinglePagerDeactiveImage");
                    doublePagerBtn.SetDynamicResource(Button.ImageProperty, "DoublePagerImage");
                }
                else
                {
                    singlePagerBtn.IsEnabled = true;
                    doublePagerBtn.IsEnabled = false;
                    singlePagerBtn.SetDynamicResource(Button.ImageProperty, "SinglePagerImage");
                    doublePagerBtn.SetDynamicResource(Button.ImageProperty, "DoublePagerDeactiveImage");
                }

                this.SetContainer();

                WebViewContainer.Children.Clear();
                BookPager.Children.Clear();

                if (isPager)
                {
                    this.LoadPage();
                }

                //this.AutoPlayBtn.SetDynamicResource(Button.ImageProperty, "ReadAloudMuteImage");
                //this.isAutoPlay = false;
            }
        }

        public void SetControlsVisibility(int n)
        {
            switch (n)
            {
                case 0:
                    if (this.PageCount <= this.displayPageCount)
                    {
                        //this.NextBtn.IsVisible = false;    
                    }
                    else
                    {
                        this.NextBtn.IsVisible = true;
                    }
                    this.PreviousBtn.IsVisible = false;
                    break;
                case 1:
                    if (this.currentPage == this.PageCount)
                    {
                        //this.NextBtn.IsVisible = false;
                    }
                    this.PreviousBtn.IsVisible = true;
                    break;
                case 2:
                    if (this.currentPage == 0)
                    {
                        this.PreviousBtn.IsVisible = false;
                    }
                    //this.NextBtn.IsVisible = true;
                    break;
                default:
                    this.NavigationControls.IsVisible = false;
                    //this.NextBtn.IsVisible = false;
                    //this.PreviousBtn.IsVisible = false;
                    break;
            }
            if (this.isTouch)
            {
                //gi.IsVisible = true;
                NavigationControls.IsVisible = false;
            }
            else
            {
                //gi.IsVisible = true;
                NavigationControls.IsVisible = true;
            }
        }

        public void RepositionContainer()
        {
            this.pos_x = (this.DeviceWidth - this.ContainerWidth) / 2 + 61;
            this.pos_y = this.ContainerPadding;
            AbsoluteLayout.SetLayoutBounds(this.WebViewContainer, new Rectangle(this.pos_x, this.pos_y, this.ContainerScaleWidth, this.PageHeight));
        }

        public void DefaultReadAloud()
        {
            BookDisplay.isPlaying = false;
            _audioPlayer?.Stop();

            foreach (View view in this.WebViewContainer.Children)
            {
                if (view is WebView)
                {
                    try
                    {
                        ((WebView)view).Navigated -= WebView_Navigated;
                    }
                    catch (Exception ex)
                    {

                    }
                    ((WebView)view).Source = "about:blank";
                }
            }
            //this.AutoPlayBtn.SetDynamicResource(Button.ImageProperty, "ReadAloudMuteImage");
            //this.isAutoPlay = false;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (Xamarin.Forms.Device.OS == TargetPlatform.Windows || Xamarin.Forms.Device.OS == TargetPlatform.WinPhone)
            {
                AppData.FileService.SaveText(Path.Combine(this.BasePagePath, "test1.xhtml"), "");
                AppData.FileService.SaveText(Path.Combine(this.BasePagePath, "test2.xhtml"), "");
            }
            this.DefaultReadAloud();
        }

        public void LoadMediaFile(bool isPager = false)
        {
            try
            {
                if (this.FileIndex < this.AudioFiles.Count && !string.IsNullOrEmpty(this.AudioFiles.ElementAt(this.FileIndex).FileName))
                {
                    string audioPath = Path.Combine(this.BasePagePath, "Audio", this.AudioFiles.ElementAt(this.FileIndex).FileName).Replace("\\", "/");
                    this.Position = 0;
                    _audioPlayer.Load(audioPath, isPager);
                    AudioDuration = this.AudioFiles.ElementAt(this.FileIndex).Duration;
                    if (BookDisplay.isAutoPlay && isPager && !BookDisplay.isLoading)
                    {
                        this.OperateMediaFile();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void OperateMediaFile()
        {
            try
            {
                if (this.FileIndex < this.AudioFiles.Count && !string.IsNullOrEmpty(this.AudioFiles.ElementAt(this.FileIndex).FileName))
                {
                    if (BookDisplay.isAutoPlay)
                    {
                        _audioPlayer?.Play();
                        BookDisplay.isPlaying = true;

                        if (Xamarin.Forms.Device.OS == TargetPlatform.Windows && this.WebViewContainer.Children.Count >= this.AudioPage && this.AudioPage < 2 && this.AudioFiles.ElementAt(this.FileIndex).IsCCFile)
                        {
                            ((WebView)this.WebViewContainer.Children[this.AudioPage]).Eval("StartTimer()");
                        }

                        Task.Run(() =>
                        {

                            AudioState = string.Empty;

                            if ((Xamarin.Forms.Device.OS == TargetPlatform.Android) || (Xamarin.Forms.Device.OS == TargetPlatform.iOS))
                            {
                                while (!(AudioState.Equals("Stopped") || AudioState.Equals("Paused")) && BookDisplay.isPlaying)
                                {
                                    this.Position += 0.05;
                                    Task.Delay(50).Wait();
                                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                                    {
                                        if (this.WebViewContainer.Children.Count > this.AudioPage)
                                        {
                                            ((WebView)this.WebViewContainer.Children[this.AudioPage]).Eval("timeUpdater1(" + this.Position + ")");
                                        }
                                        AudioState = _audioPlayer.GetMediaState();
                                    });
                                }
                            }
                            else
                            {
                                if (AppData.DeviceDetail.DeviceOS.Equals("Windows 8.1"))
                                {
                                    Task.Delay(50).Wait();
                                }
                                while (!(AudioState.Equals("Stopped") || AudioState.Equals("Paused")) && BookDisplay.isPlaying)
                                {
                                    this.Position += 0.05;
                                    Task.Delay(50).Wait();
                                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                                    {
                                        AudioState = _audioPlayer.GetMediaState();
                                    });
                                }
                            }

                            if (BookDisplay.isPlaying)
                            {
                                this.Position = 0;
                                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                                {
                                    ((WebView)this.WebViewContainer.Children[this.AudioPage]).Eval("StopTimer()");
                                    ((WebView)this.WebViewContainer.Children[this.AudioPage]).Eval("ClearTimer()");
                                });
                            }

                            if (BookDisplay.isAutoPlay && BookDisplay.isPlaying && this.displayPageCount == 2 && !BookDisplay.isLoading)
                            {
                                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                                {
                                    if (this.AudioPage == 0)
                                    {
                                        this.FileIndex = this.currentPage - 1;
                                        if (string.IsNullOrEmpty(this.AudioFiles.ElementAt(this.FileIndex).FileName))
                                        {
                                            this.FileIndex--;
                                            this.LoadMediaFile();
                                        }
                                        else
                                        {
                                            this.AudioPage++;
                                            this.LoadMediaFile(true);
                                        }
                                    }
                                    else if (this.AudioPage == 1)
                                    {
                                        this.AudioPage = 0;
                                        this.FileIndex = this.currentPage - 2;
                                        if (string.IsNullOrEmpty(this.AudioFiles.ElementAt(this.FileIndex).FileName))
                                        {
                                            this.AudioPage++;
                                            this.FileIndex++;
                                        }
                                        this.LoadMediaFile();
                                    }
                                });
                                return;
                            }
                            if (Xamarin.Forms.Device.OS == TargetPlatform.iOS)
                            {
                                if (BookDisplay.isAutoPlay && BookDisplay.isPlaying && this.displayPageCount == 1)
                                {
                                    if (this.AudioPage == 0)
                                        this.LoadMediaFile();
                                }
                            }
                        });
                    }
                    else
                    {
                        _audioPlayer?.Pause();
                        BookDisplay.isPlaying = false;
                        if (this.WebViewContainer.Children.Count >= this.AudioPage && this.AudioPage < 2)
                        {
                            ((WebView)this.WebViewContainer.Children[this.AudioPage]).Eval("StopTimer()");
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        #region MrProperties 

        protected double scale = 1;
        public double MrScale
        {
            get { return scale; }
            set { SetProperty(ref scale, value); }
        }

        protected double translationX = 0;
        public double MrTranslationX
        {
            get { return translationX; }
            set { SetProperty(ref translationX, value); }
        }

        protected double translationY = 0;
        public double MrTranslationY
        {
            get { return translationY; }
            set { SetProperty(ref translationY, value); }
        }

        protected double translationY1 = 0;

        protected void OnMrPinching(object sender, MR.Gestures.PinchEventArgs e)
        {
            isScrollVisible = false;
            ScrollControls.IsVisible = false;
            double pinchedThreshold = 1;
            if (scaleFactor > this.ContainerScale)
                pinchedThreshold = scaleFactor;
            //this.WebViewAbsolute.Panning -= OnMrPanning;
            giframe.Panning -= OnMrPanning;
            Constant.UserActiveTime = DateTime.Now;
            var newScale = MrScale * e.DeltaScale;
            //var scaleChange = newScale - MrScale;

            MrScale = Math.Min(2, Math.Max(pinchedThreshold, newScale));
            WebViewContainer.Scale = MrScale;

        }
        protected void OnMrPinched(object sender, MR.Gestures.PinchEventArgs e)
        {
            if (e.NumberOfTouches != 2)
                return;
            double pinchedThreshold = 1;
            if (scaleFactor > this.ContainerScale)
                pinchedThreshold = scaleFactor;
            //this.WebViewAbsolute.Panning += OnMrPanning;
            giframe.Panning += OnMrPanning;
            if (this.isTouch)
            {
                NavigationControls.IsVisible = true;
            }
            if (WebViewContainer.Scale > pinchedThreshold && WebViewContainer.Scale < 2)
            {
                ZoomOutBtn.IsEnabled = true;
                ZoomInBtn.IsEnabled = true;
                this.ZoomInBtn.SetDynamicResource(Button.ImageProperty, "ZoomInImage");
                this.ZoomOutBtn.SetDynamicResource(Button.ImageProperty, "ZoomOutImage");
            }
            else
            {
                if (WebViewContainer.Scale == 2)
                {
                    ZoomInBtn.IsEnabled = false;
                    ZoomOutBtn.IsEnabled = true;
                    this.ZoomInBtn.SetDynamicResource(Button.ImageProperty, "ZoomInDeactiveImage");
                    this.ZoomOutBtn.SetDynamicResource(Button.ImageProperty, "ZoomOutImage");
                }
                if (WebViewContainer.Scale <= pinchedThreshold)
                {
                    MrTranslationX = 0;
                    MrTranslationY = 0;

                    ZoomOutBtn.IsEnabled = false;
                    ZoomInBtn.IsEnabled = true;
                    this.ZoomInBtn.SetDynamicResource(Button.ImageProperty, "ZoomInImage");
                    this.ZoomOutBtn.SetDynamicResource(Button.ImageProperty, "ZoomOutDeactiveImage");
                    this.RepositionContainer();
                    if (this.ContainerScroll > 0)
                    {
                        isScrollVisible = true;
                        ScrollControls.IsVisible = true;
                    }
                    if (this.isTouch)
                    {
                        NavigationControls.IsVisible = false;
                    }
                }
            }
        }
        protected void OnMrSwiped(object sender, MR.Gestures.SwipeEventArgs e)
        {
            if (e.NumberOfTouches > 1)
                return;
            double swipeThreshold = 1;
            if (scaleFactor > this.ContainerScale)
                swipeThreshold = scaleFactor;
            if (MrScale == scaleFactor)
            {
                if (pageChangedOperation == true)
                    return;
                pageChangedOperation = true;
                if (e.Direction == MR.Gestures.Direction.Right)
                {
                    Constant.UserActiveTime = DateTime.Now;
                    this.PreviousPage();
                }
                else if (e.Direction == MR.Gestures.Direction.Left)
                {
                    Constant.UserActiveTime = DateTime.Now;
                    this.NextPage();
                }
                pageChangedOperation = false;
            }
        }
        protected void OnMrPanning(object sender, MR.Gestures.PanEventArgs e)
        {
            HideHeaderFooter(false);
            if (e.NumberOfTouches > 1)
                return;
            double panThreshold = 1;
            if (scaleFactor > this.ContainerScale)
                panThreshold = scaleFactor;
            Constant.UserActiveTime = DateTime.Now;
            if (MrScale > panThreshold)
            {
                if (Device.OS == TargetPlatform.WinPhone)
                {
                    MrTranslationX += e.DeltaDistance.X * MrScale;
                    MrTranslationY += e.DeltaDistance.Y * MrScale;
                }
                else
                {
                    MrTranslationX += e.DeltaDistance.X;
                    MrTranslationY += e.DeltaDistance.Y;
                }
                this.pos_x = this.pos_x + e.DeltaDistance.X;
                Point pt = new Point(this.pos_x, this.pos_y);
                if ((this.pos_x * MrScale > ((rect.X - (rect.Width / 2)) * MrScale)) && (this.pos_x * MrScale < ((rect.X + (rect.Width / 2)) * MrScale)))
                {
                    AbsoluteLayout.SetLayoutBounds(this.WebViewContainer, new Rectangle(this.pos_x, this.pos_y, this.ContainerScaleWidth, this.PageHeight));
                }
                else
                {
                    this.pos_x = this.pos_x - e.DeltaDistance.X;
                    //if (this.pos_x < rect.X) this.pos_x = rect.X;
                    //if (this.pos_x > rect.Width) this.pos_x = rect.Width;
                    //MrTranslationX -= e.DeltaDistance.X;
                }
                this.pos_y = this.pos_y + e.DeltaDistance.Y;
                if ((this.pos_y > (rect.Y - ((this.PageHeight / 2 + this.ContainerPadding) * MrScale))) && (this.pos_y < (rect.Y + this.PageHeight / 2)))
                {
                    AbsoluteLayout.SetLayoutBounds(this.WebViewContainer, new Rectangle(this.pos_x, this.pos_y, this.ContainerScaleWidth, this.PageHeight));
                }
                else
                {
                    this.pos_y = this.pos_y - e.DeltaDistance.Y;
                    //if (this.pos_y < rect.Y) this.pos_y = rect.Y;
                    //if (this.pos_y > rect.Height) this.pos_y = rect.Height;
                    //MrTranslationY -= e.DeltaDistance.Y;
                }
            }
            if (MrScale <= panThreshold)
            {
                if (this.ContainerScroll > 0)
                {
                    this.pos_y += e.DeltaDistance.Y;
                    if ((this.pos_y < this.ContainerPadding) && (this.pos_y > -(this.ContainerScroll - this.ContainerPadding)))
                    {
                        UpBtn.Opacity = 1.0;
                        DownBtn.Opacity = 1.0;
                        AbsoluteLayout.SetLayoutBounds(this.WebViewContainer, new Rectangle(this.pos_x, this.pos_y, this.ContainerScaleWidth, this.PageHeight));
                    }
                    else
                    {
                        if (e.DeltaDistance.Y > 0)
                        {
                            UpBtn.Opacity = 0.5;
                            DownBtn.Opacity = 1.0;
                        }
                        else
                        {
                            UpBtn.Opacity = 1.0;
                            DownBtn.Opacity = 0.5;
                        }
                        this.pos_y -= e.DeltaDistance.Y;
                    }
                }
            }
        }
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }
            storage = value;
            return true;
        }

        private void OnMrTapped(object sender, MR.Gestures.TapEventArgs e)
        {
            if (clickedFromActionTab)
            {
                clickedFromActionTab = false;
                return;
            }
            if (HeaderControls.IsVisible == false)
            {
                HideHeaderFooter(true);
                AutoHide();
            }
            else
            {
                HideHeaderFooter(false);
                timer.Stop();
            }
        }
        #endregion MrProperties

        private void HideHeaderFooter(bool IsVisible)
        {
            HeaderControls.IsVisible = IsVisible;
            BookPagerContainer.IsVisible = IsVisible;
        }

        private void AutoHide()
        {
            if (timer == null)
            {
                timer = new MyTimer(TimeSpan.FromSeconds(Constant.NumberOfSeconds), () =>
                {
                    HideHeaderFooter(false);
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
