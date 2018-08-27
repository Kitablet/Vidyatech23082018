using Kitablet.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Kitablet
{
    public partial class Setting : Grid
    {
        string currentTab = string.Empty;
        int currentPageCount = 0, totalBookCount = 0;
        List<Book> selectedElements = new List<Book>();
        List<Image> BookList = new List<Image>();
        List<int> SelectedBookIds = new List<int>();
        bool ascending;
        bool bysize;
        bool dontTapNext, dontTapPrev;
        ActionTabPage actionTabPageObject;

        class ReleaseBooks
        {
            public int BookId { get; set; }
            public string Title { get; set; }
            public int BookSize { get; set; }
            public string DateAccessed { get; set; }
        }
        List<ReleaseBooks> Books = new List<ReleaseBooks>();
        public Setting(ActionTabPage action = null)
        {
            actionTabPageObject = action;
            InitializeComponent();
            HelpWebView.RegisterAction(data => SendContactUsQuestion(data));
            NavigationPage.SetHasNavigationBar(this, false);
            var tapGestureRecognizer1 = new TapGestureRecognizer();
            tapGestureRecognizer1.Tapped += GetTab;
            releasebook.GestureRecognizers.Add(tapGestureRecognizer1);
            var tapGestureRecognizer2 = new TapGestureRecognizer();
            tapGestureRecognizer2.Tapped += GetTab;
            help.GestureRecognizers.Add(tapGestureRecognizer2);
            var tapGestureRecognizer3 = new TapGestureRecognizer();
            tapGestureRecognizer3.Tapped += GetTab;
            privacy.GestureRecognizers.Add(tapGestureRecognizer3);
            GetTab(releasebook, new EventArgs());

            var tap = new TapGestureRecognizer();
            tap.Tapped += SelectAll_Clicked;
            SelectAllCheckbox.GestureRecognizers.Add(tap);

            var tap1 = new TapGestureRecognizer();
            tap1.Tapped += Sort_Clicked;
            DateLastAccessedStack.GestureRecognizers.Add(tap1);

            var tap2 = new TapGestureRecognizer();
            tap2.Tapped += Sort_Clicked;
            SizeStack.GestureRecognizers.Add(tap2);

            var tap5 = new TapGestureRecognizer();
            tap5.Tapped += ReleaseBtn_Clicked;
            ReleaseBtn.GestureRecognizers.Add(tap5);

            var tap4 = new TapGestureRecognizer();
            tap4.Tapped += NextBtn_Clicked;
            if (NextBtn.GestureRecognizers.Count == 0)
                NextBtn.GestureRecognizers.Add(tap4);
            var tap3 = new TapGestureRecognizer();
            tap3.Tapped += PrevBtn_Clicked;
            if (PrevBtn.GestureRecognizers.Count == 0)
                PrevBtn.GestureRecognizers.Add(tap3);

            if(Device.OS == TargetPlatform.Android)
            {
                releaseBookGrid.Padding = new Thickness(0, 25, 0, 0);
                noreleaseBookGrid.Padding = new Thickness(0, 25, 0, 0);
            }
            else
            {
                releaseBookGrid.Padding = new Thickness(0, 30, 0, 0);
                noreleaseBookGrid.Padding = new Thickness(0, 30, 0, 0);
            }
            ActionTabPage.CommonPopupActionTab.SubmitTextMessage.Text = "YES";
            ActionTabPage.CommonPopupActionTab.CancelTextMessage.Text = "NO";

            ActionTabPage.CommonPopupActionTab.SubmitTextMessage.IsVisible = true;
            ActionTabPage.CommonPopupActionTab.CancelTextMessage.IsVisible = true;

            if (AppData.User.SubSectionId == 1)
            {
                HelpWebView.Uri = "PrivacyPolicy/help_1.html";
                PrivacyWebView.Source = Path.Combine(AppData.FileService.getBasePath(), "PrivacyPolicy", "privacy_1.html");
            }
            else if (AppData.User.SubSectionId == 2)
            {
                HelpWebView.Uri = "PrivacyPolicy/help_2.html";
                PrivacyWebView.Source = Path.Combine(AppData.FileService.getBasePath(), "PrivacyPolicy", "privacy_2.html");
            }
            else
            {
                HelpWebView.Uri = "PrivacyPolicy/help_3.html";
                PrivacyWebView.Source = Path.Combine(AppData.FileService.getBasePath(), "PrivacyPolicy", "privacy_3.html");
            }
            try
            {

                if (ActionTabPage.CommonPopupActionTab.SubmitPopupBtn.GestureRecognizers.Count > 0)
                {
                    ActionTabPage.CommonPopupActionTab.SubmitPopupBtn.GestureRecognizers.Clear();
                }
                if (ActionTabPage.CommonPopupActionTab.CancelPopupBtn.GestureRecognizers.Count > 0)
                {
                    ActionTabPage.CommonPopupActionTab.CancelPopupBtn.GestureRecognizers.Clear();
                }
            }
            catch(Exception e)
            { }
            var reviewPopupOkTab = new TapGestureRecognizer();
            reviewPopupOkTab.Tapped += reviewPopupSubmitBtn1_Clicked;
            ActionTabPage.CommonPopupActionTab.SubmitPopupBtn.GestureRecognizers.Add(reviewPopupOkTab);

            var reviewPopupCancelTab = new TapGestureRecognizer();
            reviewPopupCancelTab.Tapped += reviewPopupCancelBtn1_Clicked;
            ActionTabPage.CommonPopupActionTab.CancelPopupBtn.GestureRecognizers.Add(reviewPopupCancelTab);

        }
        private void SendContactUsQuestion(string question)
        {
            if (!string.IsNullOrEmpty(question))
            {
                if (HelperFunctions.CheckInternetConnection())
                {
                    ActionTabPage.IsOfflineTextLabel.IsVisible = false;

                    string questionPostResult = MyWebRequest.PostRequest("createhelpitem", null, new { UserId = AppData.User.UserId, UserMessage = question }, null);
                    if (Int32.Parse(questionPostResult) == 1)
                    {
                        actionTabPageObject.WritePopupText("Thanks for your email - we will reply soon.", "nonetaccess", "YES", "OK", 1);
                        actionTabPageObject.ShowPopupCommon();
                    }
                    else
                    {
                        actionTabPageObject.WritePopupText("Something went wrong", "nonetaccess", "YES", "OK", 1);
                        actionTabPageObject.ShowPopupCommon();
                    }
                }
                else
                {
                    ActionTabPage.IsOfflineTextLabel.IsVisible = true;
                    actionTabPageObject.WritePopupText("Cannot connect to the Server. Check your Internet Connection.", "nonetaccess", "YES", "OK", 1);
                    actionTabPageObject.ShowPopupCommon();
                }
            }
        }
        private void reviewPopupCancelBtn1_Clicked(object sender, EventArgs e)
        {
            Constant.UserActiveTime = DateTime.Now;
            ActionTabPage.CheckInternetConnectivity();
            ActionTabPage.CommonPopupActionTab.IsVisible = false;
            ActionTabPage.CommonPopupActionTab.TextMessage.Text = string.Empty;
        }
        private void reviewPopupSubmitBtn1_Clicked(object sender, EventArgs e)
        {
            Constant.UserActiveTime = DateTime.Now;
            ActionTabPage.CheckInternetConnectivity();
            //try
            //{
            //    string listofBookIds = string.Empty;
            //    foreach (int id in SelectedBookIds)
            //    {
            //        listofBookIds += id.ToString() + ",";
            //        ActionTabPage.OfflineBookIds.Remove(id);
            //        if (AppData.UserDetails != null)
            //        {
            //            UserBook UserBookElement = AppData.UserDetails.UserBooks.UserBook.Where(x => x.BookId.Equals(id.ToString())).FirstOrDefault();
            //            if (UserBookElement != null)
            //            {
            //                string _device = UserBookElement.Devices.DeviceId.Where(x => x.Trim().Equals(Constant.DeviceIdFromDB.ToString()))?.FirstOrDefault();
            //                if (_device != null)
            //                {
            //                    UserBookElement.Devices.DeviceId.Remove(_device);
            //                }
            //            }
            //        }
            //        if (AppData.FileService.CheckDirectoryExistence(Path.Combine(Constant.FileDirectoryStructure, Constant.FileNameInitials + id)))
            //        {
            //            AppData.FileService.DeleteDirectory(Path.Combine(Constant.FileDirectoryStructure, Constant.FileNameInitials + id));
            //        }

            //        DownloadFile Download_File = new DownloadFile
            //        {
            //            BookID = id.ToString(),
            //            IsDownloaded = false.ToString(),
            //            IsUnZip = false.ToString(),
            //            IsDecrypted = false.ToString(),
            //            IsEncrypted = false.ToString(),
            //            IsProcessing = false.ToString(),
            //            BookFiles = new BookFiles
            //            {
            //                BookFile = new List<BookFile>()
            //            }
            //        };
            //        Utils.SetBookStatus(Download_File);

            //    }
            //    SelectedBookIds.Clear();
            //    listofBookIds = listofBookIds.TrimEnd(',');

            //    if (HelperFunctions.CheckInternetConnection())
            //    {
            //        ActionTabPage.IsOfflineTextLabel.IsVisible = false;
            //        string status = MyWebRequest.PostRequest("releaseuserbooks", null, new { UserId = AppData.User.UserId, BookIds = listofBookIds, DeviceId = Constant.DeviceIdFromDB }, null);
            //    }
            //    else
            //    {
            //        ActionTabPage.IsOfflineTextLabel.IsVisible = true;
            //    }
            //    releaseBookFunction();
            //    ActionTabPage tab = (ActionTabPage)Navigation.NavigationStack.ElementAt(0);
            //    tab.ShowCount();
            //    ReleaseBtn.Opacity = 0.5;
            //}
            //catch (Exception qqe)
            //{ }
            ActionTabPage.CommonPopupActionTab.IsVisible = false;
        }
        private void ReleaseBtn_Clicked(object sender, EventArgs e)
        {
            Constant.UserActiveTime = DateTime.Now;          
            if (SelectedBookIds.Count > 0)
            {
                //ActionTabPage.CommonPopupActionTab.TextMessage.Text = "Are you sure you want to release the book(s) from this device?";
                //ActionTabPage.CommonPopupActionTab.IsVisible = true;
                ActionTabPage.page_Loader.IsVisible = true;
                Task.Run(() => {
                    Task.Delay(100).Wait();
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                        try
                        {
                            ActionTabPage.CheckInternetConnectivity();
                            string listofBookIds = string.Empty;
                            foreach (int id in SelectedBookIds)
                            {
                                listofBookIds += id.ToString() + ",";
                                ActionTabPage.OfflineBookIds.Remove(id);
                                if (AppData.UserDetails != null)
                                {
                                    UserBook UserBookElement = AppData.UserDetails.UserBooks.UserBook.Where(x => x.BookId.Equals(id.ToString())).FirstOrDefault();
                                    if (UserBookElement != null)
                                    {
                                        string _device = UserBookElement.Devices.DeviceId.Where(x => x.Trim().Equals(Constant.DeviceIdFromDB.ToString()))?.FirstOrDefault();
                                        if (_device != null)
                                        {
                                            UserBookElement.Devices.DeviceId.Remove(_device);
                                        }
                                    }
                                }

                                DownloadFile Download_File = new DownloadFile
                                {
                                    BookID = id.ToString(),
                                    IsDownloaded = false.ToString(),
                                    IsUnZip = false.ToString(),
                                    IsDecrypted = false.ToString(),
                                    IsEncrypted = false.ToString(),
                                    IsProcessing = false.ToString(),
                                    BookFiles = new BookFiles
                                    {
                                        BookFile = new List<BookFile>()
                                    }
                                };
                                Utils.SetBookStatus(Download_File);

                                try
                                {
                                    if (AppData.FileService.CheckDirectoryExistence(Path.Combine(Constant.FileDirectoryStructure, Constant.FileNameInitials + id)))
                                    {
                                        AppData.FileService.DeleteDirectory(Path.Combine(Constant.FileDirectoryStructure, Constant.FileNameInitials + id));
                                    }
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                            SelectedBookIds.Clear();
                            listofBookIds = listofBookIds.TrimEnd(',');

                            if (HelperFunctions.CheckInternetConnection())
                            {
                                ActionTabPage.IsOfflineTextLabel.IsVisible = false;
                                string status = MyWebRequest.PostRequest("releaseuserbooks", null, new { UserId = AppData.User.UserId, BookIds = listofBookIds, DeviceId = Constant.DeviceIdFromDB }, null);
                            }
                            else
                            {
                                ActionTabPage.IsOfflineTextLabel.IsVisible = true;
                            }
                            releaseBookFunction();
                            ActionTabPage tab = (ActionTabPage)Navigation.NavigationStack.ElementAt(0);
                            tab.ShowCount();
                            ReleaseBtn.Opacity = 0.5;
                        }
                        catch (Exception qqe)
                        { }
                    });
                });
            }
        }
        private void Sort_Clicked(object sender, EventArgs e)
        {
            Constant.UserActiveTime = DateTime.Now;
            ActionTabPage.page_Loader.IsVisible = true;
            Task.Run(() => {
                Task.Delay(100).Wait();
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                    ActionTabPage.CheckInternetConnectivity();
                    StackLayout stack = (StackLayout)sender;
                    int skipCount = currentPageCount * Constant.ReleaseBookDisplayCount;
                    if (stack.ClassId.Equals("size"))
                    {
                        if (stack.StyleId == "0")
                        {
                            stack.StyleId = "1";
                            bysize = true;
                            ascending = true;

                        }
                        else
                        {
                            stack.StyleId = "0";
                            bysize = true;
                            ascending = false;
                        }
                    }
                    else
                    {
                        if (stack.StyleId == "0")
                        {
                            stack.StyleId = "1";
                            bysize = false;
                            ascending = true;
                        }
                        else
                        {
                            stack.StyleId = "0";
                            bysize = false;
                            ascending = false;
                        }
                    }
                    GetBooks(skipCount);
                });
            });           
        }
        protected void releaseBookFunction()
        {
            try
            {
                GetBookCount();
                resetTabs();
                if (totalBookCount == 0)
                {
                    noreleaseBookGrid.IsVisible = true;
                    ActionTabPage.page_Loader.IsVisible = false;
                }
                else
                {
                    int skipCount = currentPageCount * Constant.ReleaseBookDisplayCount;
                    if (totalBookCount - skipCount <= 0)
                    {
                        currentPageCount--;
                        skipCount = currentPageCount * Constant.ReleaseBookDisplayCount;
                    }            
                    bysize = true;
                    ascending = false;
                    GetBooks(skipCount);
                    releaseBookGrid.IsVisible = true;
                }
                releasebookl.SetDynamicResource(BoxView.ColorProperty, "ActiveBorderColor");
                releasebook.SetDynamicResource(Grid.StyleProperty, "selectedtabImage");
            }
            catch (Exception eop)
            {

            }

        }
        protected void helpSection()
        {
            resetTabs();
            releasebookl.SetDynamicResource(BoxView.ColorProperty, "TransparentBorderColor");
            helpl.SetDynamicResource(BoxView.ColorProperty, "ActiveBorderColor");
            help.SetDynamicResource(Grid.StyleProperty, "selectedtabImage");
            helpSectionGrid.IsVisible = true;
        }
        protected void privacySection()
        {
            resetTabs();
            releasebookl.SetDynamicResource(BoxView.ColorProperty, "TransparentBorderColor");
            privacyl.SetDynamicResource(BoxView.ColorProperty, "ActiveBorderColor");
            privacy.SetDynamicResource(Grid.StyleProperty, "selectedtabImage");
            privacyPolicySection.IsVisible = true;
        }

        protected void resetTabs()
        {
            //releasebookl.SetDynamicResource(BoxView.ColorProperty, "TransparentBorderColor");
            helpl.SetDynamicResource(BoxView.ColorProperty, "TransparentBorderColor");
            privacyl.SetDynamicResource(BoxView.ColorProperty, "TransparentBorderColor");

            releasebook.SetDynamicResource(Grid.StyleProperty, "unselectedtabImage");
            help.SetDynamicResource(Grid.StyleProperty, "unselectedtabImage");
            privacy.SetDynamicResource(Grid.StyleProperty, "unselectedtabImage");
            noreleaseBookGrid.IsVisible = false;
            releaseBookGrid.IsVisible = false;
            helpSectionGrid.IsVisible = false;
            privacyPolicySection.IsVisible = false;
        }
        public void GetTab(object sender, EventArgs e)
        {
            Constant.UserActiveTime = DateTime.Now;           
            Grid btn = (Grid)sender;
            try
            {
                if (!currentTab.Equals(btn.StyleId))
                {
                    ActionTabPage.page_Loader.IsVisible = true;
                    Task.Run(() => {
                        Task.Delay(100).Wait();
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                            ActionTabPage.CheckInternetConnectivity();
                            switch (btn.StyleId)
                            {
                                case "releasebook":
                                    releaseBookFunction();
                                    break;
                                case "help":
                                    helpSection();
                                    ActionTabPage.page_Loader.IsVisible = false;
                                    break;
                                case "privacy":
                                    privacySection();
                                    ActionTabPage.page_Loader.IsVisible = false;
                                    break;
                                default:
                                    releaseBookFunction();
                                    break;
                            }
                            currentTab = btn.StyleId;
                        });
                    });
                }
            }
            catch (Exception ex)
            { }
        }
        private void PrevBtn_Clicked(object sender, EventArgs e)
        {
            Constant.UserActiveTime = DateTime.Now;          
            try
            {
                if (!dontTapPrev)
                {
                    ActionTabPage.page_Loader.IsVisible = true;
                    Task.Run(() => {
                        Task.Delay(100).Wait();
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                            ActionTabPage.CheckInternetConnectivity();
                            currentPageCount--;
                            int skipCount = currentPageCount * Constant.ReleaseBookDisplayCount;
                            GetBooks(skipCount);
                        });
                    });
                }
            }
            catch (Exception ex)
            { }
        }
        private void NextBtn_Clicked(object sender, EventArgs e)
        {
            Constant.UserActiveTime = DateTime.Now;           
            try
            {
                if (!dontTapNext)
                {
                    ActionTabPage.page_Loader.IsVisible = true;
                    Task.Run(() => {
                        Task.Delay(100).Wait();
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                            ActionTabPage.CheckInternetConnectivity();
                            currentPageCount++;
                            int skipCount = currentPageCount * Constant.ReleaseBookDisplayCount;
                            GetBooks(skipCount);
                        });
                    });
                }
            }
            catch (Exception ex)
            { }
        }
        private void GetBooks(int skipCount)
        {
            try
            {
                List<int> ids = new Func<List<int>>( () => {
                    List<int> BookIds = new List<int>();
                    try
                    {
                        if (AppData.BooksStatus != null)
                        {
                            foreach (DownloadFile BookElement in AppData.BooksStatus.DownloadFile)
                            {
                                if (Utils.IsBookDownloaded(BookElement.BookID))
                                {
                                    BookIds.Add(Int32.Parse(BookElement.BookID));
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                    return BookIds;
                })();
                totalBookCount = ids.Count;
                selectedElements = AppData.BooksDetail.Books.Book.Where(x => ids.Contains(Int32.Parse(x.BookId))).OrderByDescending(element => element.BookSize.ToString()).ToList();
                Books.Clear();
                foreach (Book elem in selectedElements)
                {
                    ReleaseBooks book = new ReleaseBooks();
                    book.BookId = Int32.Parse(elem.BookId);
                    book.Title = elem.Search.Title;
                    book.BookSize = Int32.Parse(elem.BookSize);
                    book.DateAccessed = AppData.UserDetails.UserBooks.UserBook.Where(x => x.BookId.Equals(elem.BookId)).Count() > 0 ? AppData.UserDetails.UserBooks.UserBook.Where(x => x.BookId.Equals(elem.BookId)).FirstOrDefault().LastDateAccessed : "N/A";
                    Books.Add(book);
                }

                if (bysize)
                {
                    DateLastAccessedStackArrow.Opacity = 0;
                    SizeStackArrow.Opacity = 0;
                    Image img = (Image)SizeStack.Children[1];
                    if (ascending)
                    {
                        img.SetDynamicResource(Image.SourceProperty, "ReleaseArrowAscending");
                        Books = Books.OrderBy(x => x.BookSize).ToList();

                    }
                    else
                    {
                        img.SetDynamicResource(Image.SourceProperty, "ReleaseArrowDescending");
                        Books = Books.OrderByDescending(x => x.BookSize).ToList();
                    }
                    //LastDateAccessedList.Opacity = 0.5;
                    DateLastAccessedStack.Opacity = 0.5;
                    //SizeList.Opacity = 1;
                    SizeStack.Opacity = 1;
                    DateLastAccessedStackArrow.Opacity = 0;
                    SizeStackArrow.Opacity = 1;
                }
                else
                {
                    Image img = (Image)DateLastAccessedStack.Children[1];
                    List<ReleaseBooks> temp = new List<ReleaseBooks>();
                    if (ascending)
                    {
                        img.SetDynamicResource(Image.SourceProperty, "ReleaseArrowAscending");
                        temp = Books.Where(y => (!string.IsNullOrEmpty(y.DateAccessed) && !y.DateAccessed.Equals("N/A"))).OrderBy(x => Convert.ToDateTime(x.DateAccessed)).ToList();
                        temp.AddRange(Books.Where(y => (string.IsNullOrEmpty(y.DateAccessed) || y.DateAccessed.Equals("N/A"))).ToList());
                    }
                    else
                    {
                        img.SetDynamicResource(Image.SourceProperty, "ReleaseArrowDescending");
                        temp = Books.Where(y => (!string.IsNullOrEmpty(y.DateAccessed) && !y.DateAccessed.Equals("N/A"))).OrderByDescending(x => Convert.ToDateTime(x.DateAccessed)).ToList();
                        temp.AddRange(Books.Where(y => (string.IsNullOrEmpty(y.DateAccessed) || y.DateAccessed.Equals("N/A"))).ToList());
                        //Books = Books.OrderByDescending(x => Convert.ToDateTime(x.DateAccessed)).ToList();
                    }
                    Books.Clear();
                    Books = temp;
                    //LastDateAccessedList.Opacity = 1;
                    DateLastAccessedStack.Opacity = 1;
                    SizeStack.Opacity = 0.5;
                    //SizeList.Opacity = 0.5;
                    DateLastAccessedStackArrow.Opacity = 1;
                    SizeStackArrow.Opacity = 0;
                }

                if ((Constant.ReleaseBookDisplayCount + skipCount) >= totalBookCount)
                {
                    ShowCountLabel.Text = "Showing " + (skipCount + 1) + "-" + totalBookCount + " of " + totalBookCount + " books on this device";
                }
                else
                {
                    ShowCountLabel.Text = "Showing " + (skipCount + 1) + "-" + (skipCount + Constant.ReleaseBookDisplayCount) + " of " + totalBookCount + " books on this device";
                }
                if (((currentPageCount + 1) * Constant.ReleaseBookDisplayCount) >= totalBookCount)
                {
                    NextBtn.Opacity = 0.5;
                    dontTapNext = true;
                }
                else
                {
                    NextBtn.Opacity = 1;
                    dontTapNext = false;
                }
                if ((currentPageCount - 1) < 0)
                {
                    PrevBtn.Opacity = 0.5;
                    dontTapPrev = true;
                }
                else
                {
                    PrevBtn.Opacity = 1;
                    dontTapPrev = false;
                }
                paintListofBooks(skipCount);
            }
            catch (Exception ex) { }
        }
        private void GetBookCount()
        {
            try
            {
                totalBookCount = new Func<int>(() => {
                    int count = 0;
                    try
                    {
                        if (AppData.BooksStatus != null)
                        {
                            foreach (DownloadFile BookElement in AppData.BooksStatus.DownloadFile)
                            {
                                if (Utils.IsBookDownloaded(BookElement.BookID))
                                {
                                    count++;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                    return count;
                })(); ;
                int remainingHeight = Constant.DeviceHeight - Constant.PrimaryNavigationHeight - Constant.SecondaryNavigationHeight - 120 - 90;
                int rowCount = 0;
                if (Xamarin.Forms.Device.OS == TargetPlatform.Android)
                {
                    Double d = (double)(remainingHeight / Constant.DeviceDensity);
                    rowCount = (int)Math.Floor((double)(d / (Constant.RowHeightReleaseBook + 8)));
                }
                else
                {
                    rowCount = (int)Math.Floor((double)(remainingHeight / (Constant.RowHeightReleaseBook + 8)));
                }
                Constant.ReleaseBookDisplayCount = rowCount;
            }
            catch (Exception e) { }
        }
        private void paintListofBooks(int skipCount)
        {
            try
            {
                BookNameList.Children.Clear();
                LastDateAccessedList.Children.Clear();
                SizeList.Children.Clear();
                List<ReleaseBooks> paintBooks = Books.Skip(skipCount).Take(Constant.ReleaseBookDisplayCount).ToList();
                foreach (ReleaseBooks item in paintBooks)
                {
                    Image img = new Image { HeightRequest = 18, WidthRequest = 18, VerticalOptions = LayoutOptions.Center };
                    if (SelectedBookIds.Contains(item.BookId))
                    {
                        img.SetDynamicResource(Image.SourceProperty, "CheckBoxSelectedImage");
                        img.StyleId = "1";
                    }
                    else
                    {
                        img.SetDynamicResource(Image.SourceProperty, "CheckBoxImage");
                        img.StyleId = "0";
                    }
                    img.ClassId = item.BookId.ToString();
                    var tap = new TapGestureRecognizer();
                    tap.Tapped += CheckBox_Clicked;
                    img.GestureRecognizers.Add(tap);

                    if (!BookList.Contains(img))
                    {
                        BookList.Add(img);
                    }
                    StackLayout stack = new StackLayout();
                    stack.Orientation = StackOrientation.Horizontal;
                    stack.HeightRequest = 35;
                    stack.Children.Add(img);

                    if (Device.OS == TargetPlatform.Android)
                    {
                        BookTitle lbl1 = new BookTitle();
                        lbl1.VerticalTextAlignment = TextAlignment.Center;
                        lbl1.Text = item.Title;
                        lbl1.HeightRequest = 35;
                        lbl1.FontSize = 14;
                        lbl1.FontAttributes = FontAttributes.None;
                        lbl1.SetDynamicResource(Label.StyleProperty, "LabelNormalRegular");
                        stack.Children.Add(lbl1);
                    }
                    else
                    {
                        Label lbl1 = new Label();
                        lbl1.VerticalTextAlignment = TextAlignment.Center;
                        lbl1.Text = item.Title;
                        lbl1.HeightRequest = 35;
                        lbl1.FontSize = 14;
                        lbl1.FontAttributes = FontAttributes.Bold;
                        lbl1.SetDynamicResource(Label.StyleProperty, "LabelNormalBold");
                        stack.Children.Add(lbl1);
                    }

                    stack.HeightRequest = Constant.RowHeightReleaseBook;


                    BookNameList.Children.Add(stack);

                    Label lbl2 = new Label();
                    lbl2.VerticalTextAlignment = TextAlignment.Center;
                    lbl2.Text = item.BookSize.ToString() + " MB";
                    lbl2.HeightRequest = 40;
                    lbl2.FontSize = 14;
                    lbl2.FontAttributes = FontAttributes.Bold;
                    lbl2.SetDynamicResource(Label.StyleProperty, "LabelNormalBold");
                    SizeList.Children.Add(lbl2);
                    Label lbl3 = new Label();
                    lbl3.VerticalTextAlignment = TextAlignment.Center;
                    lbl3.Text = string.IsNullOrEmpty(item.DateAccessed) || item.DateAccessed.Equals("N/A") ? "N/A" : DateTime.Parse(item.DateAccessed).ToString("dd MMM yyyy");
                    lbl3.HeightRequest = 40;
                    lbl3.FontAttributes = FontAttributes.Bold;
                    lbl3.FontSize = 14;
                    lbl3.SetDynamicResource(Label.StyleProperty, "LabelNormalBold");
                    LastDateAccessedList.Children.Add(lbl3);
                }
                if (bysize)
                {
                    foreach (var item in SizeList.Children)
                        item.Opacity = 1;
                    foreach (var item in LastDateAccessedList.Children)
                        item.Opacity = 0.5;
                }
                else
                {
                    foreach (var item in SizeList.Children)
                        item.Opacity = 0.5;
                    foreach (var item in LastDateAccessedList.Children)
                        item.Opacity = 1;
                }
            }
            catch (Exception e)
            {

            }
            finally
            {
                ActionTabPage.page_Loader.IsVisible = false;
            }

        }
        private void CheckBox_Clicked(object sender, EventArgs e)
        {
            Constant.UserActiveTime = DateTime.Now;
            ActionTabPage.CheckInternetConnectivity();
            Image img = (Image)sender;
            if (img.StyleId == "0")
            {
                img.StyleId = "1";
                if (!SelectedBookIds.Contains(Int32.Parse(img.ClassId)))
                {
                    SelectedBookIds.Add(Int32.Parse(img.ClassId));
                }
                img.SetDynamicResource(Image.SourceProperty, "CheckBoxSelectedImage");
            }
            else
            {
                img.StyleId = "0";
                if (SelectedBookIds.Contains(Int32.Parse(img.ClassId)))
                {
                    SelectedBookIds.Remove(Int32.Parse(img.ClassId));
                }
                img.SetDynamicResource(Image.SourceProperty, "CheckBoxImage");
            }
            if (SelectedBookIds.Count == totalBookCount)
            {
                SelectAllCheckbox.SetDynamicResource(Image.SourceProperty, "CheckBoxSelectedImage");
                SelectAllCheckbox.StyleId = "1";
            }
            else
            {
                SelectAllCheckbox.SetDynamicResource(Image.SourceProperty, "CheckBoxImage");
                SelectAllCheckbox.StyleId = "0";
            }
            if (SelectedBookIds.Count > 0)
            {
                ReleaseBtn.Opacity = 1;
            }
            else
            {
                ReleaseBtn.Opacity = 0.5;
            }
        }
        private void SelectAll_Clicked(object sender, EventArgs e)
        {
            Constant.UserActiveTime = DateTime.Now;
            ActionTabPage.page_Loader.IsVisible = true;
            Task.Run(() => {
                Task.Delay(100).Wait();
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                    ActionTabPage.CheckInternetConnectivity();
                    List<int> ids = new Func<List<int>>(() => {
                        List<int> BookIds = new List<int>();
                        try
                        {
                            if (AppData.BooksStatus != null)
                            {
                                foreach (DownloadFile BookElement in AppData.BooksStatus.DownloadFile)
                                {
                                    if (Utils.IsBookDownloaded(BookElement.BookID))
                                    {
                                        BookIds.Add(Int32.Parse(BookElement.BookID));
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                        return BookIds;
                    })();
                    selectedElements = AppData.BooksDetail.Books.Book.Where(x => ids.Contains(Int32.Parse(x.BookId))).ToList();
                    Image img = (Image)sender;
                    if (img.StyleId == "0")
                    {
                        SelectedBookIds.Clear();
                        foreach (var item in selectedElements)
                        {
                            SelectedBookIds.Add(Int32.Parse(item.BookId));
                        }
                        img.StyleId = "1";
                        img.SetDynamicResource(Image.SourceProperty, "CheckBoxSelectedImage");
                    }
                    else
                    {
                        img.StyleId = "0";
                        img.SetDynamicResource(Image.SourceProperty, "CheckBoxImage");
                        SelectedBookIds.Clear();
                    }
                    if (SelectedBookIds.Count > 0)
                    {
                        ReleaseBtn.Opacity = 1;
                    }
                    else
                    {
                        ReleaseBtn.Opacity = 0.5;
                    }
                    int skipCount = currentPageCount * Constant.ReleaseBookDisplayCount;
                    GetBooks(skipCount);
                });
            });
        }
    }
}
