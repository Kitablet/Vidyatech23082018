using Kitablet.Helpers;
using Kitablet.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Xamarin.Forms;

namespace Kitablet
{

    public partial class ActionTabPage : ContentPage
    {
        public static string currentGrade = String.Empty;
        public string currentTab = String.Empty;
        List<KeyValuePair<int, string>> subSections = new List<KeyValuePair<int, string>>();
        public bool isDropDownVisible;
        public static StackLayout PopUpFadeOut, HiUserPopup, CommonPopup, ParentPopupContainer;
        List<Grid> DropdownElements = new List<Grid>();
        public static List<int> OfflineBookIds;
        public static ActivityIndicatorLoader Loader;
        public static Image CurrentAvatarImageDashB;
        public static StackLayout categoriesStack1, categoriesStack2, specialsStack, languageStack, typeStack;
        public static Grid DoneButton;
        public static Popup CommonPopupActionTab;
        public static Label IsOfflineTextLabel;
        Image ArrowImage;
        public static object tabContent;
        bool isconnect = false;
        long doublePressInterval_ms = 3000;
        DateTime lastPressTime = DateTime.MinValue;
        public static bool isExpanded = false;
        public static Label NotificationLabel { get; set; }
        public static DateTime AppStartTime;
        public static ScrollView UAC;
        public static Image ExpandImg;
        public static StackLayout CollapseStack;
        public static Task UserSessionTask;
        public static CancellationTokenSource feedCancellationTokenSource;
        public static PageLoader page_Loader;

        public ActionTabPage()
        {
            OfflineBookIds = new List<int>();
            NavigationPage.SetHasNavigationBar(this, false);
            currentGrade = ThemeChanger.SubSection.ToString();

            InitializeComponent();

            ActionTabPage.AppStartTime = DateTime.Now;
            //this.ActionTabLayout.WidthRequest = Device.OnPlatform<double>(Constant.DeviceWidth, Constant.DeviceWidth / Constant.DeviceDensity, Constant.DeviceWidth);
            //this.ActionTabLayout.HeightRequest = Device.OnPlatform<double>(Constant.DeviceHeight, (Constant.DeviceHeight / Constant.DeviceDensity) - 25, Constant.DeviceHeight);

            UAC = CurrentBookDashboardScroll;
            ExpandImg = ExpandDashboard;
            CollapseStack = CollapseDashboard;
            categoriesStack1 = CategoriesPart1;
            categoriesStack2 = CategoriesPart2;
            specialsStack = Specials;
            languageStack = Languages;
            typeStack = BookTypes;
            DoneButton = OkBtn_CommonPopup1;
            CommonPopupActionTab = ActionTabPopup;
            NotificationLabel = this.NotificationText;

            if (ActionTabPage.page_Loader != null)
            {
                ActionTabPage.page_Loader = null;
            }
            ActionTabPage.page_Loader = this.Page_Loader;

            ActionTabPopup.SubmitTextMessage.Text = "OK";
            var okPopuptap = new TapGestureRecognizer();
            okPopuptap.Tapped += (s, e) => {
                Constant.UserActiveTime = DateTime.Now;
                this.ActionTabPopup.IsVisible = false;
                this.ActionTabPopup.TextMessage.Text = string.Empty;
            };
            ActionTabPopup.SubmitPopupBtn.GestureRecognizers.Add(okPopuptap);

            CurrentAvatarImageDashB = CurentAvatarImage;
            Loader = loader;
            PopUpFadeOut = PopWindowFadeout;
            HiUserPopup = hiUserPopup;
            CommonPopup = commonPopup;
            ParentPopupContainer = PopWindow;
            IsOfflineTextLabel = IsOfflineText;
            TabHeader.FontSize = Xamarin.Forms.Device.OnPlatform(
                16,
                18,
                18
            );
            isconnect = HelperFunctions.CheckInternetConnection();
            if (isconnect)
            {
                Task.Run(() => {
                    this.getEvent();
                });
                ActionTabPage.IsOfflineTextLabel.IsVisible = false;
                IsOfflineText.IsVisible = false;
            }
            else
            {
                ActionTabPage.IsOfflineTextLabel.IsVisible = true;
                IsOfflineText.IsVisible = true;
            }

            if (AppData.User != null)
            {
                userText.Text = "Hi " + AppData.User.FirstName + " " + AppData.User.LastName;
                UserNameDashboardPopup1.Text = HelperFunctions.UppercaseFirst(AppData.User.FirstName);
                UserNameDashboardPopup2.Text = HelperFunctions.UppercaseFirst(AppData.User.LastName);
                string uri = AppData.User.AvatarImage.Replace("#size#", Constant.AvatarMedium);
                CurentAvatarImage.Source = new UriImageSource
                {
                    Uri = new Uri(uri),
                    CachingEnabled = true,
                    CacheValidity = TimeSpan.MaxValue
                };
            }
            else
            {
                userText.Text = "Hi User";
            }

            if (isconnect)
            {
                GetBooks();
                GetUserDetailBooks();
            }

            Task.Run(() => {
                CheckIfBookOffline();
            });            

            paintSubSection();
            BookmarkRibbon.Margin = new Thickness(55, 0, 0, 0);

            ShowCount();           

            var tapnotifications1 = new TapGestureRecognizer();
            tapnotifications1.Tapped += (s, e) =>
            {
                Constant.UserActiveTime = DateTime.Now;
                HideDropdown();
                double screenHeight = Constant.DeviceHeight;
                if(Device.OS == TargetPlatform.Android)
                    screenHeight = Application.Current.MainPage.Height;
                var animation = new Animation(v => CurrentBookDashboardScroll.HeightRequest = v, 150, screenHeight - 80);
                animation.Commit(this, "SimpleAnimation", 500, 500, Easing.Linear, (v, c) => {
                    CurrentBookDashboardScroll.HeightRequest = screenHeight - 80;
                    CollapseDashboard.IsVisible = true;
                    ExpandDashboard.IsVisible = false;
                    isExpanded = true;
                }, () => false);
            };
            ExpandDashboard.GestureRecognizers.Add(tapnotifications1);

            var tapnotifications2 = new TapGestureRecognizer();
            tapnotifications2.Tapped += Tapnotifications2_Tapped;
            CollapseDashboard.GestureRecognizers.Add(tapnotifications2);

            //--------------------------------------------------------------------------------------------------------
            //-------------------------------------------User Session Logout------------------------------------------
            //--------------------------------------------------------------------------------------------------------

            ActionTabPage.UserSession(true);

            //--------------------------------------------------------------------------------------------------------
            //--------------------------------------------------------------------------------------------------------           
        }

        public void Tapnotifications2_Tapped(object sender, EventArgs e)
        {
            Constant.UserActiveTime = DateTime.Now;
            HideDropdown();
            double screenHeight = Constant.DeviceHeight;
            if (Device.OS == TargetPlatform.Android)
                screenHeight = Application.Current.MainPage.Height;
            var animation = new Animation(v => CurrentBookDashboardScroll.HeightRequest = v, screenHeight - 80, 150);
            animation.Commit(this, "SimpleAnimation", 500, 500, Easing.Linear, (v, c) => CurrentBookDashboardScroll.HeightRequest = 150, () => false);
            ExpandDashboard.IsVisible = true;
            CollapseDashboard.IsVisible = false;
            isExpanded = false;
        }

        public static async void UserSession(bool IsStarted)
        {
            Constant.UserActiveTime = DateTime.Now;
            if (ActionTabPage.UserSessionTask != null)
            {
                GC.SuppressFinalize(ActionTabPage.UserSessionTask);
                ActionTabPage.feedCancellationTokenSource.Cancel();
            }
            if (!Constant.IsRememberMe && IsStarted)
            {
                ActionTabPage.feedCancellationTokenSource = new CancellationTokenSource();
                ActionTabPage.UserSessionTask = Task.Run(async () =>
                {
                    await Task.Delay(0);
                    bool isLogout = true;
                    while ((int)DateTime.Now.Subtract(Constant.UserActiveTime).TotalMinutes < Constant.UserSessionTimeOut)
                    {
                        if (feedCancellationTokenSource.IsCancellationRequested)
                        {
                            isLogout = false;
                            break;
                        }
                    };
                    if (isLogout)
                    {
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                        {
                            BookRead.isBookOpended = false;
                            HelperFunctions._HelperFunctions.Logout();
                            Application.Current.MainPage.Navigation.PushAsync(new LoginPage(true));
                            while (Application.Current.MainPage.Navigation.NavigationStack.Count != 1)
                            {
                                Application.Current.MainPage.Navigation.RemovePage(Application.Current.MainPage.Navigation.NavigationStack.ElementAt(0));
                            }
                        });
                    }
                });
            }
        }

        public static async void CollapseUAC()
        {
            if (isExpanded)
            {
                Task.Run(async () =>
                {
                    await Task.Delay(0);
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                    {
                        Constant.UserActiveTime = DateTime.Now;
                        double screenHeight = Constant.DeviceHeight;
                        if (Device.OS == TargetPlatform.Android)
                            screenHeight = Application.Current.MainPage.Height;
                        var animation = new Animation(v => UAC.HeightRequest = v, screenHeight - 80, 150);
                        animation.Commit(UAC, "SimpleAnimation", 500, 500, Easing.Linear, (v, c) => UAC.HeightRequest = 150, () => false);
                        ExpandImg.IsVisible = true;
                        CollapseStack.IsVisible = false;
                        isExpanded = false;
                    });
                });
            }
        }

        protected void getEvent()
        {
            try
            {
                string response = MyWebRequest.GetRequest("GetEvents?UserId=" + AppData.User.UserId, null, null);

                if (!string.IsNullOrEmpty(response) && !response.ToLower().Equals("null"))
                {
                    List<UserEvent> UserEvent1 = JsonConvert.DeserializeObject<List<UserEvent>>(response);
                    Notification.NewEventID = new List<int>();

                    if (Notification.UserEvents != null)
                    {
                        foreach (UserEvent item in UserEvent1)
                        {
                            bool flag = false;

                            foreach (UserEvent events in Notification.UserEvents)
                            {
                                if (events.EventId == item.EventId)
                                {
                                    flag = true;
                                }
                            }
                            if (!flag)
                            {
                                Notification.UserEvents.Add(item);
                            }
                        }
                    }
                    else
                    {
                        Notification.UserEvents = UserEvent1;
                    }


                    if (Notification.UserEvents != null)
                    {
                        int count = Notification.UserEvents.Where(x => x.IsView == false).Count();
                        NotificationLabel.Text = count > 0 ? count.ToString() : string.Empty;
                        foreach (UserEvent _event in Notification.UserEvents)
                        {
                            if (!_event.IsView)
                            {
                                Notification.NewEventID.Add(_event.EventId);
                            }
                        }
                    }
                }      
            }
            catch (Exception ex)
            {

            }
        }

        public static void SendViewedEvents()
        {
            if (Notification.UserEvents != null)
            {
                string viewEvents = string.Empty;
                foreach (UserEvent _event in Notification.UserEvents)
                {
                    if (_event.IsView && Notification.NewEventID.Contains(_event.EventId))
                    {
                        viewEvents += _event.EventId + ",";
                    }
                }
                if (!string.IsNullOrEmpty(viewEvents))
                {
                    viewEvents = viewEvents.TrimEnd(',');
                    if (HelperFunctions.CheckInternetConnection())
                    {
                        string status = MyWebRequest.PostRequest("UpdateViewEvents", null, new { UserId = AppData.User.UserId, EventIds = viewEvents }, null);
                        if (!string.IsNullOrEmpty(status) && !status.ToLower().Equals("null") && Convert.ToInt32(status.Trim()) == 1)
                        {
                            foreach(string value in viewEvents.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                Notification.NewEventID.Remove(Convert.ToInt32(value));
                            }
                        }
                    }
                }
            }
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            DateTime pressTime = DateTime.Now;
            bool isBack = false;
            if (Device.OS == TargetPlatform.Android)
            {
                if (pressTime.Subtract(lastPressTime).TotalMilliseconds <= doublePressInterval_ms)
                {
                    isBack = false;
                }
                else
                {
                    XFToast.ShortMessage("Press again to exit");
                    isBack = true;
                }
            }           
            lastPressTime = pressTime;
            return isBack;
        }

        public void BackBtn_Clicked(object sender, EventArgs e)
        {
            Constant.UserActiveTime = DateTime.Now;
            CheckInternetConnectivity();
            if(ActionTabPage.tabContent is HomePage)
            {
                HomePage obj = (HomePage)ActionTabPage.tabContent;
                obj.ClearGenreSelection();
            }
            //OkBtn_CommonPopup1.Opacity = 0.5;
            selectMore.IsVisible = false;
            PopWindowFadeout.IsVisible = false;
        }
        public void CheckIfBookOffline()
        {
            try
            {
                OfflineBookIds.Clear();
                OfflineBookIds = new Func<List<int>>(() => {
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
            }
            catch (Exception ec) { }
        }
        public static string ReturnUserRating(string BookId)
        {
            string value = string.Empty;
            try
            {
                value = AppData.UserDetails.UserBooks.UserBook.Where(x => x.BookId.Equals(BookId)).FirstOrDefault()?.Rating?.Trim();
            }
            catch (Exception e) { }
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            else
            {
                if (Int32.Parse(value) == 0)
                {
                    return string.Empty;
                }
                else
                {
                    return value;
                }
            }
        }
        public static bool ActivityDone(string BookId)
        {
            try
            {
                return Boolean.Parse(AppData.UserDetails.UserBooks.UserBook.Where(x => x.BookId.Equals(BookId)).FirstOrDefault()?.IsActivityDone?.Trim());
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private void TabClicked(object sender, EventArgs e)
        {
            Constant.UserActiveTime = DateTime.Now;           
            Button btn = (Button)sender;
            try
            {
                if (!currentTab.Equals(btn.StyleId))
                {
                    ActionTabPage.page_Loader.IsVisible = true;
                    Task.Run(() => {
                        Task.Delay(100).Wait();
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                            CheckInternetConnectivity();
                            switch (btn.StyleId)
                            {
                                case "userhome":
                                    DropDown.IsVisible = true;
                                    paintSubSection();
                                    TabHeader.Text = "";
                                    TabHeader.IsVisible = false;
                                    CurrentBookDashboardScroll.HeightRequest = 150;
                                    ExpandDashboard.IsVisible = true;
                                    CollapseDashboard.IsVisible = false;
                                    CurrentBookDashboard.IsVisible = true;
                                    tabContent = new HomePage();
                                    UserDashboard.currentUserTab = string.Empty;
                                    break;
                                case "booksearch":
                                    currentGrade = ThemeChanger.SubSection.ToString();
                                    DropDown.IsVisible = false;
                                    //ThemeSeletor.IsVisible = false;
                                    TabHeader.IsVisible = true;
                                    HideDropdown();
                                    TabHeader.Text = "SEARCH";
                                    CurrentBookDashboard.IsVisible = false;
                                    ExpandDashboard.IsVisible = false;
                                    CollapseDashboard.IsVisible = false;
                                    tabContent = new BookSearch();
                                    UserDashboard.currentUserTab = string.Empty;
                                    break;
                                case "dashboard":
                                    currentGrade = ThemeChanger.SubSection.ToString();
                                    DropDown.IsVisible = false;
                                    //ThemeSeletor.IsVisible = false;
                                    TabHeader.IsVisible = true;
                                    HideDropdown();
                                    TabHeader.Text = "PROFILE";
                                    CurrentBookDashboardScroll.HeightRequest = 150;
                                    ExpandDashboard.IsVisible = true;
                                    CollapseDashboard.IsVisible = false;
                                    CurrentBookDashboard.IsVisible = true;
                                    tabContent = new UserDashboard(this);
                                    break;
                                case "notifications":
                                    currentGrade = ThemeChanger.SubSection.ToString();
                                    DropDown.IsVisible = false;
                                    //ThemeSeletor.IsVisible = false;
                                    HideDropdown();
                                    TabHeader.IsVisible = true;
                                    TabHeader.Text = "ANNOUNCEMENTS";
                                    isconnect = HelperFunctions.CheckInternetConnection();
                                    if (isconnect)
                                    {
                                        this.getEvent();
                                    }
                                    tabContent = new Notification();
                                    CurrentBookDashboard.IsVisible = false;
                                    ExpandDashboard.IsVisible = false;
                                    CollapseDashboard.IsVisible = false;
                                    UserDashboard.currentUserTab = string.Empty;
                                    break;
                                case "setting":
                                    currentGrade = ThemeChanger.SubSection.ToString();
                                    DropDown.IsVisible = false;
                                    //ThemeSeletor.IsVisible = false;
                                    HideDropdown();
                                    TabHeader.IsVisible = true;
                                    TabHeader.Text = "SETTINGS | INFO";
                                    tabContent = new Setting(this);
                                    CurrentBookDashboard.IsVisible = false;
                                    ExpandDashboard.IsVisible = false;
                                    CollapseDashboard.IsVisible = false;
                                    UserDashboard.currentUserTab = string.Empty;
                                    break;
                                case "poweroff":
                                    tabContent = null;
                                    userhome.SetDynamicResource(Button.ImageProperty, "HomeImage");
                                    booksearch.SetDynamicResource(Button.ImageProperty, "SearchImage");
                                    dashboard.SetDynamicResource(Button.ImageProperty, "ProfileImage");
                                    notifications.SetDynamicResource(Button.ImageProperty, "AnnouncementImage");
                                    setting.SetDynamicResource(Button.ImageProperty, "SettingImage");
                                    poweroff.SetDynamicResource(Button.ImageProperty, "LogoutSelectedImage");
                                    break;
                                default:
                                    DropDown.IsVisible = true;
                                    //ThemeSeletor.IsVisible = true;
                                    HideDropdown();
                                    tabContent = new HomePage();
                                    CurrentBookDashboardScroll.HeightRequest = 150;
                                    ExpandDashboard.IsVisible = true;
                                    CollapseDashboard.IsVisible = false;
                                    CurrentBookDashboard.IsVisible = true;
                                    UserDashboard.currentUserTab = string.Empty;
                                    break;
                            }
                            if (tabContent != null)
                            {
                                try
                                {
                                    ActionTabContentLayout.Children.Clear();
                                }
                                catch (Exception ex)
                                {
                                    ActionTabContentLayout.Children.Clear();
                                }
                                ActionTabContentLayout.Children.Add(tabContent as View);
                                Grid.SetRow(tabContent as View, 0);
                                Grid.SetColumn(tabContent as View, 0);
                                currentTab = btn.StyleId;
                                setTabSelection();
                                if ((tabContent is Notification) || (tabContent is BookSearch))
                                {
                                    ActionTabPage.page_Loader.IsVisible = false;
                                }
                            }
                            else
                            {
                                //WritePopupText("Are you sure you want to logout?", "Logout", "YES", "NO", 2);
                                //ShowPopupCommon();
                                HelperFunctions._HelperFunctions.Logout();
                                ActionTabPage.UserSession(false);
                                Navigation.PushAsync(new LoginPage(true));
                                Navigation.RemovePage(this);
                                ActionTabPage.page_Loader.IsVisible = false;
                                ActionTabPage.page_Loader = null;
                            }
                        });
                    });
                }
            }
            catch (Exception ex)
            {

            }
        }
        #region Subsection Dropdown
        public void paintSubSection()
        {
            DropdownElements.Clear();
            DropDownContent.Children.Clear();
            DropDownOutline.Opacity = 0;

            var tapGestureRecognizer1 = new TapGestureRecognizer();
            tapGestureRecognizer1.Tapped += DropDownClicked;

            List<SubSection> currentGradeElement = AppData.BooksDetail.SubSections.SubSection.Where(x => x.Id.Equals(currentGrade)).ToList();
            AddSubSection(currentGradeElement, true, tapGestureRecognizer1);
            List<SubSection> GradeElements = AppData.BooksDetail.SubSections.SubSection.Where(x => !x.Id.Equals(currentGrade)).ToList();
            AddSubSection(GradeElements, false, tapGestureRecognizer1);
        }
        private async void AddSubSection(List<SubSection> Elements, bool isSelected, TapGestureRecognizer tapGestureRecognizer1)
        {
            try
            {
                for (int i = 0; i < Elements.Count; i++)
                {
                    subSections.Add(new KeyValuePair<int, string>(Int32.Parse(Elements[i].Id), Elements[i].ShortForm));
                    Grid stackP = new Grid();
                    stackP.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    stackP.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                    stackP.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                    Image img = new Image();
                    img.SetDynamicResource(Image.SourceProperty, "DownArrowImage");
                    img.HorizontalOptions = LayoutOptions.Center;
                    img.VerticalOptions = LayoutOptions.Center;
                    StackLayout stack = new StackLayout { Orientation = StackOrientation.Vertical, Padding = new Thickness(0, 0, 10, 0), HeightRequest = 30, VerticalOptions = LayoutOptions.Start };
                    stack.StyleId = Elements[i].Id;
                    StackLayout librarytext = new StackLayout { Orientation = StackOrientation.Horizontal, Padding = new Thickness(0), Spacing = 0 };

                    Label lbl1 = new Label();
                    lbl1.Text = Elements[i].ShortForm.Trim().Substring(0, 1);
                    lbl1.FontSize = 16;
                    lbl1.FontAttributes = FontAttributes.Bold;
                    lbl1.SetDynamicResource(Label.StyleProperty, "LabelNormalBold");
                    lbl1.VerticalTextAlignment = TextAlignment.Center;
                    lbl1.SetDynamicResource(Label.TextColorProperty, "TextNormalColor");


                    Label lbl2 = new Label();
                    if (Elements[i].ShortForm.Trim().Length > 1)
                    {
                        lbl2.Text = Elements[i].ShortForm.Trim().Substring(1, 1);
                    }
                    lbl2.FontSize = 10;
                    lbl2.FontAttributes = FontAttributes.Bold;
                    lbl2.SetDynamicResource(Label.StyleProperty, "LabelNormalBold");
                    lbl2.VerticalTextAlignment = TextAlignment.Start;
                    lbl2.SetDynamicResource(Label.TextColorProperty, "TextNormalColor");
                    Label lbl = new Label();
                    lbl.Text = " L I B R A R Y";
                    lbl.FontSize = 16;
                    lbl.FontAttributes = FontAttributes.Bold;
                    lbl.SetDynamicResource(Label.StyleProperty, "LabelNormalBold");
                    lbl.VerticalTextAlignment = TextAlignment.Center;
                    lbl.SetDynamicResource(Label.TextColorProperty, "TextNormalColor");
                    librarytext.Children.Add(lbl1);
                    librarytext.Children.Add(lbl2);
                    librarytext.Children.Add(lbl);
                    stackP.IsVisible = false;
                    img.Opacity = 0;
                    if (isSelected)
                    {
                        stackP.IsVisible = true;
                        img.Opacity = 0.5;
                        ArrowImage = img;
                        lbl.SetDynamicResource(Label.TextColorProperty, "TextWhiteColor");
                        lbl2.SetDynamicResource(Label.TextColorProperty, "TextWhiteColor");
                        lbl1.SetDynamicResource(Label.TextColorProperty, "TextWhiteColor");
                    }
                    BoxView box = new BoxView
                    {
                        Color = Color.FromHex("#FFFFFF"),
                        HeightRequest = 2,
                        VerticalOptions = LayoutOptions.End
                    };
                    box.Opacity = 0.4;
                    if ((!isSelected) && (i == Elements.Count - 1))
                    {
                        box.Opacity = 0;
                    }
                    stack.Margin = new Thickness(0, 5, 0, 5);
                    stack.Children.Add(librarytext);
                    stack.Children.Add(box);
                    stackP.StyleId = Elements[i].Id;
                    //stack.GestureRecognizers.Add(tapGestureRecognizer1);
                    //stackP.GestureRecognizers.Add(tapGestureRecognizer1);

                    Grid.SetRow(stack, 0);
                    Grid.SetColumn(stack, 0);
                    Grid.SetRow(img, 0);
                    Grid.SetColumn(img, 1);
                    stackP.Children.Add(stack);
                    stackP.Children.Add(img);

                    Label lBtn = new Label();
                    lBtn.VerticalOptions = LayoutOptions.Fill;
                    lBtn.HorizontalOptions = LayoutOptions.Fill;
                    lBtn.BackgroundColor = Color.Transparent;
                    Grid.SetRow(lBtn, 0);
                    Grid.SetColumn(lBtn, 0);
                    Grid.SetColumnSpan(lBtn, 2);
                    lBtn.GestureRecognizers.Add(tapGestureRecognizer1);

                    stackP.Children.Add(lBtn);

                    if (!DropdownElements.Contains(stackP))
                    {
                        DropdownElements.Add(stackP);
                        DropDownContent.Children.Add(stackP);
                    }

                }
            }
            catch (Exception ex)
            {

            }
        }
        private void DropDownClicked(object sender, EventArgs e)
        {
            try
            {
                Constant.UserActiveTime = DateTime.Now;
                CheckInternetConnectivity();
                ActionTabPage.CollapseUAC();
                Grid currentStack = (Grid)((Label)sender).Parent;
                bool isSame = false;
                if (currentGrade == currentStack.StyleId)
                {
                    isSame = true;
                }
                if (isDropDownVisible)
                {

                    if (!isSame)
                    {
                        currentGrade = currentStack.StyleId;
                        paintSubSection();
                        currentTab = String.Empty;
                        TabClicked(userhome, new EventArgs());
                    }

                    HideDropdown();
                }
                else
                {
                    DropDownOutline.Opacity = 0.5;
                    ArrowImage.Opacity = 1;
                    isDropDownVisible = true;
                    for (int i = 0; i < DropdownElements.Count; i++)
                    {
                        DropdownElements[i].IsVisible = true;
                    }
                }
            }
            catch (Exception ex)
            {
            }            
        }
        public async void HideDropdown()
        {
            try
            {
                if (isDropDownVisible)
                {
                    DropDownOutline.Opacity = 0;

                    isDropDownVisible = false;

                    for (int i = 0; i < DropdownElements.Count; i++)
                    {
                        DropdownElements[i].IsVisible = false;
                    }
                    DropdownElements[0].IsVisible = true;
                }
            }
            catch (Exception e)
            {

            }
        }
        public void setTabSelection()
        {
            userhome.SetDynamicResource(Button.ImageProperty, "HomeImage");
            booksearch.SetDynamicResource(Button.ImageProperty, "SearchImage");
            dashboard.SetDynamicResource(Button.ImageProperty, "ProfileImage");
            notifications.SetDynamicResource(Button.ImageProperty, "AnnouncementImage");
            setting.SetDynamicResource(Button.ImageProperty, "SettingImage");
            poweroff.SetDynamicResource(Button.ImageProperty, "LogoutImage");
            switch (currentTab)
            {
                case "userhome":
                    userhome.SetDynamicResource(Button.ImageProperty, "HomeSelectedImage");
                    break;
                case "booksearch":
                    booksearch.SetDynamicResource(Button.ImageProperty, "SearchSelectedImage");
                    break;
                case "dashboard":
                    dashboard.SetDynamicResource(Button.ImageProperty, "ProfileSelectedImage");
                    break;
                case "notifications":
                    notifications.SetDynamicResource(Button.ImageProperty, "AnnouncementSelectedImage");
                    break;
                case "setting":
                    setting.SetDynamicResource(Button.ImageProperty, "SettingSelectedImage");
                    break;
                case "poweroff":
                    poweroff.SetDynamicResource(Button.ImageProperty, "LogoutSelectedImage");
                    break;
                default:
                    userhome.SetDynamicResource(Button.ImageProperty, "HomeSelectedImage");
                    break;
            }
        }
        #endregion
        #region Popup
        private void closeBtn_Clicked(object sender, EventArgs e)
        {
            Constant.UserActiveTime = DateTime.Now;
            CheckInternetConnectivity();
            PopWindowFadeout.IsVisible = false;
            PopWindow.IsVisible = false;
            hiUserPopup.IsVisible = false;
            setTabSelection();
        }
        private void commoncloseBtn_Clicked(object sender, EventArgs e)
        {
            if (Device.OS == TargetPlatform.iOS)
            {
                CommonTextMessage.Text = string.Empty;
            }
            Constant.UserActiveTime = DateTime.Now;
            CheckInternetConnectivity();
            PopWindowFadeout.IsVisible = false;
            PopWindow.IsVisible = false;
            commonPopup.IsVisible = false;
            setTabSelection();
        }
        public void ShowButtonPopup()
        {
            PopWindowFadeout.IsVisible = true;
            selectMore.IsVisible = true;
        }
        public void HideButtonPopup()
        {
            PopWindowFadeout.IsVisible = false;
            selectMore.IsVisible = false;
        }
        public static void OpenUserPopup()
        {
            PopUpFadeOut.IsVisible = true;
            HiUserPopup.IsVisible = true;
            ParentPopupContainer.IsVisible = true;
        }
        public static void OpenCommonPopup(string Message, string Action, string OKBtnText, string CancelBtnText)
        {
            PopUpFadeOut.IsVisible = true;
            CommonPopup.IsVisible = true;
            ParentPopupContainer.IsVisible = true;

        }

        //Message = text to show
        //Action if two buttons then specify action to be performed on yes
        //OKBtnText 
        //CancelBtnText
        //button count
        public void WritePopupText(string Message, string Action, string OKBtnText, string CancelBtnText, int buttonCount)
        {
            CommonTextMessage.Text = Message;
            OkBtn_CommonPopupTxt.Text = OKBtnText;
            CancelBtn_CommonPopupTxt.Text = CancelBtnText;
            if (buttonCount == 1)
            {
                OkBtn_CommonPopup.IsVisible = false;
                setTabSelection();
            }
            else
            {
                OkBtn_CommonPopup.IsVisible = true;
                CancelBtn_CommonPopup.IsVisible = true;
            }
            var tapGestureRecognizer1 = new TapGestureRecognizer();
            tapGestureRecognizer1.Tapped += LogoutOnYes;
            OkBtn_CommonPopup.GestureRecognizers.Add(tapGestureRecognizer1);

            var tapGestureRecognizer2 = new TapGestureRecognizer();
            tapGestureRecognizer2.Tapped += commoncloseBtn_Clicked;
            CancelBtn_CommonPopup.GestureRecognizers.Add(tapGestureRecognizer2);
        }
        public void ShowPopupCommon()
        {
            PopUpFadeOut.IsVisible = true;
            CommonPopup.IsVisible = true;
            ParentPopupContainer.IsVisible = true;
        }
        public void HidePopupCommon()
        {
            PopUpFadeOut.IsVisible = false;
            CommonPopup.IsVisible = false;
            ParentPopupContainer.IsVisible = false;
        }
        protected void LogoutOnYes(object sender, EventArgs e)
        {
            HelperFunctions._HelperFunctions.Logout();
            ActionTabPage.UserSession(false);
            Navigation.PushAsync(new LoginPage(true));
            Navigation.RemovePage(this);
        }
        #endregion
        public async void ShowCount()
        {
            try
            {
                int TotalBookReadCount = Int32.Parse(AppData.UserDetails.TotalBookRead);
                int TotalBookRatedCount = Int32.Parse(AppData.UserDetails.TotalBookRated);
                int TotalActivitiesCompletedCount = Int32.Parse(AppData.UserDetails.TotalActivitiesCompleted);
                string totalhour = AppData.UserDetails.TotalHourSpentOnReading?.Trim();
                if (!string.IsNullOrEmpty(totalhour))
                {
                    string[] Time = totalhour.Split(':');
                    if (Time.Length == 3)
                    {
                        HoursSpentReading.Text = String.Format("{0:00}", Time[0]) + ":" + String.Format("{0:00}", Time[1]);
                        // TotalHourSpentCount = (float) new TimeSpan(Convert.ToInt32(Time[0]), Convert.ToInt32(Time[1]), Convert.ToInt32(Time[2])).TotalHours;
                    }
                    else
                    {
                        if (Time.Length == 1)
                        {
                            int minutes = Int32.Parse(Time[0]);
                            HoursSpentReading.Text = String.Format("{0:00}", ((int)minutes / 60)) + ":" + String.Format("{0:00}", ((int)minutes % 60));
                        }
                        else
                        {
                            HoursSpentReading.Text = "00:00";
                        }                       
                    }
                }

                int currentReadingBook = 0;
                int LastAccessedBookId = Int32.Parse(AppData.UserDetails.LastAccessedBookId);
                int LastReadLaterBookId = Int32.Parse(AppData.UserDetails.LastReadLaterBookId);
                if (LastAccessedBookId == 0)
                {
                    if (LastReadLaterBookId != 0)
                    {
                        currentReadingBook = LastReadLaterBookId;
                    }
                }
                else
                {
                    currentReadingBook = LastAccessedBookId;
                }

                if (currentReadingBook != 0)
                {
                    BookmarkRow.Margin = new Thickness(0,-10, 0, 0);
                    DefaultText.Text = "";
                    Book elm = AppData.BooksDetail.Books.Book.Where(x => x.BookId.Equals(currentReadingBook.ToString())).FirstOrDefault();
                    if(elm != null)
                    {
                        string bookCover = elm.Thumbnail2?.Trim();
                        NoBookThumbnail.WidthRequest = Constant.MediumBookWidth;
                        DownloadedImage.WidthRequest = Constant.MediumBookWidth;
                        if (!string.IsNullOrEmpty(bookCover))
                        {
                            if (elm.ViewMode.Equals("Landscape"))
                            {
                                NoBookThumbnail.HeightRequest = Constant.MediumBookHeight / 2;
                                DownloadedImage.HeightRequest = Constant.MediumBookHeight / 2;
                            }
                            else
                            {
                                NoBookThumbnail.HeightRequest = Constant.MediumBookHeight;
                                DownloadedImage.HeightRequest = Constant.MediumBookHeight;
                            }
                            DownloadedImage.Source = new UriImageSource
                            {
                                Uri = new Uri(bookCover),
                                CacheValidity = TimeSpan.MaxValue,
                                CachingEnabled = true
                            };
                            DownloadedImage.Aspect = Aspect.Fill;

                            if (Utils.IsBookDownloaded(currentReadingBook.ToString()))
                            {
                                //isOfflineBook.IsVisible = true;
                                isOfflineBook.Opacity = 1;
                            }
                            else
                            {
                                //isOfflineBook.IsVisible = false;
                                isOfflineBook.Opacity = 0;
                            }
                        }
                        else
                        {
                            if (elm.ViewMode.Equals("Landscape"))
                            {
                                NoBookThumbnail.HeightRequest = Constant.MediumBookHeight / 2;
                            }
                            else
                            {
                                NoBookThumbnail.HeightRequest = Constant.MediumBookHeight;
                            }
                        }
                        var tapsetting = new TapGestureRecognizer();
                        tapsetting.Tapped += (s, e) => {
                            Constant.UserActiveTime = DateTime.Now;
                            if (!BookRead.isBookOpended)
                            {
                                ActionTabPage.page_Loader.IsVisible = true;
                                Task.Run(() => {
                                    Task.Delay(100).Wait();
                                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                                        Grid img = (Grid)s;
                                        if (this.currentTab.Equals("userhome"))
                                        {
                                            Navigation.PushAsync(new BookRead(img.StyleId, false, 2));
                                        }
                                        else if (this.currentTab.Equals("dashboard"))
                                        {
                                            Navigation.PushAsync(new BookRead(img.StyleId, false, 1));
                                        }
                                        HideDropdown();
                                        ActionTabPage.CollapseUAC();
                                    });
                                });
                            }

                        };
                        switch (Int32.Parse(elm.SubSections))
                        {
                            case 1:
                                BookmarkRibbon.IsVisible = true;
                                BookmarkRibbon.Source = "bookmark_1.png";
                                bookborder.IsVisible = true;
                                bookborder.BackgroundColor = Color.FromHex("#FC654C");
                                break;
                            case 2:
                                BookmarkRibbon.IsVisible = true;
                                BookmarkRibbon.Source = "bookmark_2.png";
                                bookborder.IsVisible = true;
                                bookborder.BackgroundColor = Color.FromHex("#9DA503");
                                break;
                            case 3:
                                BookmarkRibbon.IsVisible = true;
                                BookmarkRibbon.Source = "bookmark_3.png";
                                bookborder.IsVisible = true;
                                bookborder.BackgroundColor = Color.FromHex("#14B4B4");
                                break;
                            default:
                                BookmarkRibbon.IsVisible = true;
                                BookmarkRibbon.Source = "bookmark_3.png";
                                bookborder.IsVisible = true;
                                bookborder.BackgroundColor = Color.FromHex("#14B4B4");
                                break;
                        }
                        NoBookThumbnail.StyleId = elm.BookId;
                        if (NoBookThumbnail.GestureRecognizers.Count > 0)
                        {
                            NoBookThumbnail.GestureRecognizers.Clear();
                        }
                        NoBookThumbnail.GestureRecognizers.Add(tapsetting);

                        var value = ActionTabPage.ReturnUserRating(elm.BookId);
                        if (!Boolean.Parse(elm.HasAnimation))
                        {
                            // AnimationSmall.IsVisible = false;
                            AnimationSmall.Opacity = 0;
                        }
                        else
                        {
                            // AnimationSmall.IsVisible = true;
                            AnimationSmall.Opacity = 1;
                        }
                        if (!Boolean.Parse(elm.HasReadAloud))
                        {
                            // ReadAloudSmall.IsVisible = false;
                            ReadAloudSmall.Opacity = 0;
                        }
                        else
                        {
                            //ReadAloudSmall.IsVisible = true;
                            ReadAloudSmall.Opacity = 1;
                        }
                        if (!Boolean.Parse(elm.HasActivity))
                        {
                            // ActivitySmall.IsVisible = false;
                            ActivitySmall.Opacity = 0;
                        }
                        else
                        {
                            if (ActivityDone(currentReadingBook.ToString()))
                            {
                                ActivitySmall.Opacity = 1;
                                // ActivitySmall.IsVisible = true;
                                ActivitySmall.SetDynamicResource(Image.SourceProperty, "ActivityCompletedImage");
                            }
                            else
                            {
                                ActivitySmall.Opacity = 1;
                                // ActivitySmall.IsVisible = true;
                                ActivitySmall.SetDynamicResource(Image.SourceProperty, "ActivityImage");
                            }
                        }
                        if (!string.IsNullOrEmpty(value) && Int32.Parse(value) != 0)
                        {
                            RatingSmall.SetDynamicResource(Image.SourceProperty, "RatingCompletedImage");
                            RatingofBook.Text = value;
                            // RatingSmall.IsVisible = true;
                            RatingSmall.Opacity = 1;
                        }
                        else
                        {
                            // RatingSmall.IsVisible = true;
                            RatingSmall.Opacity = 1;
                            RatingofBook.Text = string.Empty;
                            RatingSmall.SetDynamicResource(Image.SourceProperty, "RatingImage");
                        }
                    }
                    else
                    {
                        ShowDefaultCover();
                    }
                }
                else
                {
                    ShowDefaultCover();
                }
                BooksReadCount.Text = TotalBookReadCount.ToString();
                BooksRatedCount.Text = TotalBookRatedCount.ToString();
                ActivitiesCompletedCount.Text = TotalActivitiesCompletedCount.ToString();

            }
            catch (Exception e)
            {
            }
        }

        private void ShowDefaultCover()
        {
            NoBookThumbnail.WidthRequest = Constant.MediumBookWidth;
            NoBookThumbnail.HeightRequest = Constant.MediumBookHeight;

            bookborder.IsVisible = false;
            BookmarkRibbon.IsVisible = false;
            BookmarkRow.Margin = new Thickness(0, 10, 0, 0);
            AnimationSmall.Opacity = 0;
            ActivitySmall.Opacity = 0;
            ReadAloudSmall.Opacity = 0;
            RatingSmall.Opacity = 0;
            DefaultText.Text = "No books to show here - select a book to read!";
            isOfflineBook.Opacity = 0;
        }
        //Common Function called on every event
        public static void CheckInternetConnectivity()
        {
            if (HelperFunctions.CheckInternetConnection())
            {
                ActionTabPage.IsOfflineTextLabel.IsVisible = false;
            }
            else
            {
                ActionTabPage.IsOfflineTextLabel.IsVisible = true;
            }
        }

        public static void ChangeCurrentAvatarImageDashboard()
        {
            if (AppData.User != null)
            {
                string uri = AppData.User.AvatarImage.Replace("#size#", Constant.AvatarMedium);
                CurrentAvatarImageDashB.Source = new UriImageSource
                    {
                        Uri = new Uri(uri),
                        CacheValidity = TimeSpan.MaxValue,
                        CachingEnabled = true
                    };
            }

            //if ((Xamarin.Forms.Device.OS == TargetPlatform.Android) || (Device.OS == TargetPlatform.iOS))
            //{
            //    Helpers.CacheSettings.SettingsKey = "UserDetails";
            //    if (Helpers.CacheSettings.GeneralSettings != "")
            //    {
            //        User userObj = JsonConvert.DeserializeObject<User>(Helpers.CacheSettings.GeneralSettings);
            //        string uri = userObj.AvatarImage.Replace("#size#", Constant.AvatarMedium);
            //        CurrentAvatarImageDashB.Source = new UriImageSource
            //        {
            //            Uri = new Uri(uri),
            //            CacheValidity = TimeSpan.MaxValue,
            //            CachingEnabled = true
            //        };
            //    }
            //}
            //else
            //{
            //    if (fileService.getUserDetails("UserDetails") != null)
            //    {
            //        User userObj = fileService.getUserDetails("UserDetails");

            //        string uri = userObj.AvatarImage.Replace("#size#", Constant.AvatarMedium);
            //        CurrentAvatarImageDashB.Source = new UriImageSource
            //        {
            //            Uri = new Uri(uri),
            //            CacheValidity = TimeSpan.MaxValue,
            //            CachingEnabled = true
            //        };
            //    }
            //}
        }
        #region Get Book Catalogue and User Books
        public void GetBooks()
        {
            try
            {
                string bookxml = MyWebRequest.GetRequest("getbookscatlog", null, null);
                if (!string.IsNullOrEmpty(bookxml) && bookxml != "null")
                {
                    if (bookxml.Contains("\\\""))
                        bookxml = bookxml.Replace("\\\"", "\"").Trim('"');
                    XmlSerializer serializer = new XmlSerializer(typeof(BooksDetail));
                    BooksDetail _books = (BooksDetail)serializer.Deserialize(new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(bookxml.ToString()))));
                    AppData.BooksDetail = _books != null ? _books : AppData.BooksDetail;
                }
            }
            catch (Exception ex)
            {
                ActionTabPopup.CancelPopupBtn.IsVisible = false;
                this.ActionTabPopup.TextMessage.Text = "Something went wrong. Please try again later!";
                this.ActionTabPopup.IsVisible = true;
            }
        }
        private void GetUserDetailBooks()
        {
            if (!Utils.UpdateUserDetails(Utils.UpdateMethod.New))
            {
                ActionTabPopup.CancelPopupBtn.IsVisible = false;
                this.ActionTabPopup.TextMessage.Text = "Something went wrong. Please try again later!";
                this.ActionTabPopup.IsVisible = true;
            }
        }
        #endregion

        double width, height;
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (this.width == 0d && this.height == 0d && this.width != width && this.height != height && width >= height)
            {
                this.width = width;
                this.height = height;
                Constant.DeviceHeight = AppData.FileService.GetDeviceHeight();
                Constant.DeviceWidth = AppData.FileService.GetDeviceWidth();
                Constant.DeviceDensity = AppData.FileService.GetDeviceDensity();
                TabClicked(userhome, new EventArgs());
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ActionTabPage.SendViewedEvents();
            if (Application.Current.MainPage.Navigation.NavigationStack.Count == 1)
            {
                Utils.SetBrowsingProgress(ActionTabPage.AppStartTime);
                if (HelperFunctions.CheckInternetConnection())
                {
                    Utils.UpdateUserDetails(Utils.UpdateMethod.Total);
                }
            }
        }
    }
}
