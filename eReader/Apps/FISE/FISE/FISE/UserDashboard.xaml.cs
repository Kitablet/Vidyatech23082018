using Kitablet.ViewModels;
using ImageCircle.Forms.Plugin.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Kitablet
{
    public partial class UserDashboard : Grid
    {
        bool flag = false, comFlag = false;
        double OldX = Constant.DeviceWidth / 3;
        string currentGrade = "0", fromWhere;
        public static string currentUserTab = string.Empty;
        private static string isRatingGiven = string.Empty, isActivityComplete = string.Empty, currentRatingtab = string.Empty, currentActivityTab = string.Empty;
        Avatar avatar = new Avatar();
        ActionTabPage actionTabPageObject;
        List<CircleImage> imagelist = new List<CircleImage>();
        List<Grid> RatingButtons = new List<Grid>();
        List<Grid> ActivitiesButtons = new List<Grid>();
        string action = string.Empty;
        List<AvatarDB> myavatars = null;
        List<int> bookreadids, bookreadlaterids, ratedbookids, ratingpendingbooks, bookswithactivity, activitydonebooks, activitypendingbooks;
        IEnumerator<Book> BookElements;
        int currentRow, bookCount;
        TapGestureRecognizer BookTapGestureRecognizer;

        public class Avatar
        {
            public string sourceName { get; set; }
            public int avatarId { get; set; }
        }
        public class AvatarDB
        {
            public int AvatarId { get; set; }
            public string Name { get; set; }
            public string ImagePath { get; set; }
        }

        public class BookDetails
        {
            public int BookId { get; set; }
            public string LastAccessdate { get; set; }
            public int Rating { get; set; }

        }
        public void BookReadAndBookReadLater()
        {
            try
            {
                bookreadlaterids = AppData.UserDetails.UserBooks.UserBook.Where(y => Boolean.Parse(y.IsReadLater) == true).OrderBy(x => Convert.ToDateTime(x.ReadLaterOn)).Select(r => Convert.ToInt32(r.BookId)).ToList();
                bookreadids = AppData.UserDetails.UserBooks.UserBook.Where(y => Boolean.Parse(y.IsRead) == true).OrderByDescending(x => Convert.ToDateTime(x.LastDateAccessed)).Select(r => Convert.ToInt32(r.BookId)).ToList();
                ratedbookids = AppData.UserDetails.UserBooks.UserBook.Where(y => Boolean.Parse(y.IsRead) == true && !string.IsNullOrEmpty(y.Rating) && Int32.Parse(y.Rating?.Trim()) > 0).OrderByDescending(x => Convert.ToInt32(x.Rating)).ThenByDescending(x => x.LastDateAccessed).Select(r => Convert.ToInt32(r.BookId)).ToList();
                ratingpendingbooks = AppData.UserDetails.UserBooks.UserBook.Where(y => Boolean.Parse(y.IsRead) == true && (string.IsNullOrEmpty(y.Rating?.Trim()) || Int32.Parse(y.Rating?.Trim()) == 0)).OrderByDescending(x => Convert.ToDateTime(x.LastDateAccessed)).Select(r => Convert.ToInt32(r.BookId)).ToList();
                bookswithactivity = AppData.BooksDetail.Books.Book.Where(x => Boolean.Parse(x.HasActivity) == true).Select(r => Convert.ToInt32(r.BookId)).ToList();
                activitydonebooks = AppData.UserDetails.UserBooks.UserBook.Where(y => Boolean.Parse(y.IsRead) == true && Boolean.Parse(y.IsActivityDone) == true && bookswithactivity.Contains(Int32.Parse(y.BookId))).OrderByDescending(x => Convert.ToDateTime(x.ActivityCompletedOn)).Select(r => Convert.ToInt32(r.BookId)).ToList();
                activitypendingbooks = AppData.UserDetails.UserBooks.UserBook.Where(y => Boolean.Parse(y.IsRead) == true && Boolean.Parse(y.IsActivityDone) == false && bookswithactivity.Contains(Int32.Parse(y.BookId))).OrderByDescending(x => Convert.ToDateTime(x.LastDateAccessed)).Select(r => Convert.ToInt32(r.BookId)).ToList();
            }
            catch(Exception ex) { }
        }

        public UserDashboard(ActionTabPage action = null)
        {
            BookReadAndBookReadLater();
            actionTabPageObject = action;
            InitializeComponent();

            try
            {
                if (AppData.User != null)
                {
                    currentGrade = AppData.User.SubSectionId.ToString();
                    
                    string uri = AppData.User.AvatarImage.Replace("#size#", Constant.AvatarLarge);
                    CurentAvatarImage.Source = new UriImageSource
                    {
                        Uri = new Uri(uri),
                        CacheValidity = TimeSpan.MaxValue,
                        CachingEnabled = true
                    };

                    var tapGestureRecognizer = new TapGestureRecognizer();
                    tapGestureRecognizer.Tapped += GetTab;
                    readlater.GestureRecognizers.Add(tapGestureRecognizer);

                    var tapbooknew = new TapGestureRecognizer();
                    tapbooknew.Tapped += GetTab;
                    booksread.GestureRecognizers.Add(tapbooknew);

                    var tapmyprofile = new TapGestureRecognizer();
                    tapmyprofile.Tapped += GetTab;
                    myprofile.GestureRecognizers.Add(tapmyprofile);

                    BookTapGestureRecognizer = new TapGestureRecognizer();
                    BookTapGestureRecognizer.Tapped += (s, e) =>
                    {
                        Constant.UserActiveTime = DateTime.Now;
                        if (!BookRead.isBookOpended)
                        {
                            ActionTabPage.page_Loader.IsVisible = true;
                            Task.Run(() => {
                                Task.Delay(100).Wait();
                                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                                    Grid img = (Grid)s;
                                    Navigation.PushAsync(new BookRead(img.StyleId, false, 1));
                                    ActionTabPage.CollapseUAC();
                                });
                            });
                        }
                    };

                    if (string.IsNullOrEmpty(currentUserTab))
                    {
                        GetTab(booksread, new EventArgs());
                    }
                    else
                    {
                        if (currentUserTab.Equals(booksread.StyleId))
                        {
                            currentUserTab = "";
                            GetTab(booksread, new EventArgs());
                        }
                        else if (currentUserTab.Equals(readlater.StyleId))
                        {
                            currentUserTab = "";
                            GetTab(readlater, new EventArgs());
                        }
                        else if (currentUserTab.Equals(myprofile.StyleId))
                        {
                            currentUserTab = "";
                            GetTab(myprofile, new EventArgs());
                        }
                    }
                }

                CurrentPassword.TextChanged += CurrentPassword_TextChanged;
                NewPassword.TextChanged += NewPassword_TextChanged;
                ReNewPassword.TextChanged += ReNewPassword_TextChanged;

                var tapGestureRecognizer1 = new TapGestureRecognizer();
                tapGestureRecognizer1.Tapped += ChangeAvatar_Clicked;
                ChangeAvatar_Show.GestureRecognizers.Add(tapGestureRecognizer1);

                var tapGestureRecognizer2 = new TapGestureRecognizer();
                tapGestureRecognizer2.Tapped += ChangePassword_Clicked;
                ChangePassword_Show.GestureRecognizers.Add(tapGestureRecognizer2);

                var tapGestureRecognizer3 = new TapGestureRecognizer();
                tapGestureRecognizer3.Tapped += SaveBtnAvatar_Clicked;
                SaveBtn_Avatar.GestureRecognizers.Add(tapGestureRecognizer3);

                var tapGestureRecognizer4 = new TapGestureRecognizer();
                tapGestureRecognizer4.Tapped += SaveBtnPassword_Clicked;
                SaveBtn_Password.GestureRecognizers.Add(tapGestureRecognizer4);

                var tapGestureRecognizer5 = new TapGestureRecognizer();
                tapGestureRecognizer5.Tapped += CancelBtn_Clicked;
                CancelBtn.GestureRecognizers.Add(tapGestureRecognizer5);

                ActionTabPage.CommonPopupActionTab.SubmitTextMessage.Text = "YES";
                ActionTabPage.CommonPopupActionTab.CancelTextMessage.Text = "NO";

                ActionTabPage.CommonPopupActionTab.SubmitTextMessage.IsVisible = true;
                ActionTabPage.CommonPopupActionTab.CancelTextMessage.IsVisible = true;

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
                catch (Exception e)
                { }
                var reviewPopupOkTab = new TapGestureRecognizer();
                reviewPopupOkTab.Tapped += reviewPopupSubmitBtn_Clicked;
                ActionTabPage.CommonPopupActionTab.SubmitPopupBtn.GestureRecognizers.Add(reviewPopupOkTab);

                var reviewPopupCancelTab = new TapGestureRecognizer();
                reviewPopupCancelTab.Tapped += reviewPopupCancelBtn_Clicked;
                ActionTabPage.CommonPopupActionTab.CancelPopupBtn.GestureRecognizers.Add(reviewPopupCancelTab);

                if (Xamarin.Forms.Device.OS == TargetPlatform.Android)
                {
                    errorNewPassword.Margin = new Thickness(0, 0, 5, -12);
                    errorCurrentPassword.Margin = new Thickness(0, 0, 5, -12);
                    errorReNewPassword.Margin = new Thickness(0, 0, 5, -12);
                }
                else //if (Xamarin.Forms.Device.OS == TargetPlatform.Windows)
                {
                    errorNewPassword.Margin = new Thickness(0, 0, 5, -16);
                    errorCurrentPassword.Margin = new Thickness(0, 0, 5, -16);
                    errorReNewPassword.Margin = new Thickness(0, 0, 5, -16);
                }

            }
            catch (Exception ex)
            { }

            CurrentPassword.Focused += (s, e) => {
                this.setFocus(1);
                this.setFocus(5);
                this.setFocus(6);
            };
            CurrentPassword.Unfocused += (s, e) => {
                this.setFocus(4);
            };

            ReNewPassword.Focused += (s, e) => {
                this.setFocus(3);
                this.setFocus(4);
                this.setFocus(5);
            };
            ReNewPassword.Unfocused += (s, e) => {
                this.setFocus(6);
            };
            NewPassword.Focused += (s, e) => {
                this.setFocus(2);
                this.setFocus(4);
                this.setFocus(6);
            };
            NewPassword.Unfocused += (s, e) => {
                this.setFocus(5);
            };

        }

        private void ReNewPassword_TextChanged(object sender, TextChangedEventArgs e)
        {
           if(AllFieldsFilled())
            {
                SaveBtn_Password.Opacity = 1;
            }
            else
            {
                SaveBtn_Password.Opacity = 0.5;
            }
        }

        private void NewPassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (AllFieldsFilled())
            {
                SaveBtn_Password.Opacity = 1;
            }
            else
            {
                SaveBtn_Password.Opacity = 0.5;
            }
        }

        private void CurrentPassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (AllFieldsFilled())
            {
                SaveBtn_Password.Opacity = 1;
            }
            else
            {
                SaveBtn_Password.Opacity = 0.5;
            }
        }

        public void DisplayBooksAgain()
        {
            try
            {
                BookReadAndBookReadLater();
                if (string.IsNullOrEmpty(currentUserTab))
                {
                    paintSideButtons();
                    GetBooks_BookRead();
                }
                else
                {
                    if (currentUserTab.Equals(booksread.StyleId))
                    {
                        paintSideButtons();
                        GetBooks_BookRead();
                    }
                    else if (currentUserTab.Equals(readlater.StyleId))
                    {
                        GetBooks_BookLater();
                    }
                }
            }
            catch(Exception ex) { }     
        }

        private void reviewPopupCancelBtn_Clicked(object sender, EventArgs e)
        {
            Constant.UserActiveTime = DateTime.Now;
            ActionTabPage.CheckInternetConnectivity();
            ActionTabPage.CommonPopupActionTab.IsVisible = false;
            ActionTabPage.CommonPopupActionTab.TextMessage.Text = string.Empty;
        }

        private void reviewPopupSubmitBtn_Clicked(object sender, EventArgs e)
        {
            Constant.UserActiveTime = DateTime.Now;
            ActionTabPage.CheckInternetConnectivity();
            if (!string.IsNullOrEmpty(action))
            {
                if (action.ToLower().Equals("avatar"))
                {
                   // HelperFunctions.AddEditAvatar(avatar.avatarId, AppData.User.UserId, null, this, "UserDashboard");
                    AppData.User.AvatarId = avatar.avatarId;
                    AppData.User.AvatarImage = avatar.sourceName;

                    string uri = AppData.User.AvatarImage.Replace("#size#", Constant.AvatarLarge);
                    CurentAvatarImage.Source = new UriImageSource
                    {
                        Uri = new Uri(uri),
                        CacheValidity = TimeSpan.MaxValue,
                        CachingEnabled = true
                    };
                }
                else if (action.ToLower().Equals("password"))
                {
                    //HelperFunctions.ChangePassword(AppData.User.UserId, NewPassword.Text, CurrentPassword.Text, this);
                }
                ActionTabPage.CommonPopupActionTab.IsVisible = false;
            }
        }

        public void GetTab(object sender, EventArgs e)
        {
            ActionTabPage.CollapseUAC();
            Constant.UserActiveTime = DateTime.Now;           
            Grid btn = (Grid)sender;
            try
            {
                if (!currentUserTab.Equals(btn.StyleId))
                {
                    ActionTabPage.page_Loader.IsVisible = true;
                    Task.Run(() => {
                        Task.Delay(100).Wait();
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                            ActionTabPage.CheckInternetConnectivity();
                            BooksContainer.IsVisible = false;
                            myprofiles.IsVisible = false;
                            switch (btn.StyleId)
                            {
                                case "readbook":
                                    //Clear Selection
                                    BooksContainer.IsVisible = true;
                                    booksreadl.SetDynamicResource(BoxView.BackgroundColorProperty, "ActiveBorderColor");
                                    readlaterl.SetDynamicResource(BoxView.BackgroundColorProperty, "TransparentBorderColor");
                                    myprofilel.SetDynamicResource(BoxView.BackgroundColorProperty, "TransparentBorderColor");
                                    paintSideButtons();
                                    booksread.SetDynamicResource(Grid.StyleProperty, "selectedtabImage");
                                    myprofile.SetDynamicResource(Grid.StyleProperty, "unselectedtabImage");
                                    readlater.SetDynamicResource(Grid.StyleProperty, "unselectedtabImage");
                                    GetBooks_BookRead();
                                    break;
                                case "readlater":
                                    BooksContainer.IsVisible = true;
                                    readlaterl.SetDynamicResource(BoxView.BackgroundColorProperty, "ActiveBorderColor");
                                    booksreadl.SetDynamicResource(BoxView.BackgroundColorProperty, "TransparentBorderColor");
                                    myprofilel.SetDynamicResource(BoxView.BackgroundColorProperty, "TransparentBorderColor");
                                    booksread.SetDynamicResource(Grid.StyleProperty, "unselectedtabImage");
                                    myprofile.SetDynamicResource(Grid.StyleProperty, "unselectedtabImage");
                                    readlater.SetDynamicResource(Grid.StyleProperty, "selectedtabImage");
                                    GetBooks_BookLater();
                                    break;
                                case "myprofile":
                                    BooksContainer.IsVisible = false;
                                    myprofiles.IsVisible = true;
                                    readlaterl.SetDynamicResource(BoxView.BackgroundColorProperty, "TransparentBorderColor");
                                    booksreadl.SetDynamicResource(BoxView.BackgroundColorProperty, "TransparentBorderColor");
                                    myprofilel.SetDynamicResource(BoxView.BackgroundColorProperty, "ActiveBorderColor");
                                    booksread.SetDynamicResource(Grid.StyleProperty, "unselectedtabImage");
                                    myprofile.SetDynamicResource(Grid.StyleProperty, "selectedtabImage");
                                    readlater.SetDynamicResource(Grid.StyleProperty, "unselectedtabImage");
                                    myprofiles.IsVisible = true;
                                    ButtonPanel.IsVisible = false;
                                    Column1.Width = 0;
                                    ShowProfile();
                                    break;
                                default:
                                    BooksContainer.IsVisible = true;
                                    booksreadl.SetDynamicResource(BoxView.BackgroundColorProperty, "ActiveBorderColor");
                                    readlaterl.SetDynamicResource(BoxView.BackgroundColorProperty, "TransparentBorderColor");
                                    myprofilel.SetDynamicResource(BoxView.BackgroundColorProperty, "TransparentBorderColor");
                                    booksread.SetDynamicResource(Grid.StyleProperty, "selectedtabImage");
                                    myprofile.SetDynamicResource(Grid.StyleProperty, "unselectedtabImage");
                                    readlater.SetDynamicResource(Grid.StyleProperty, "unselectedtabImage");
                                    GetBooks_BookRead();
                                    break;

                            }
                            currentUserTab = btn.StyleId;
                        });
                    });
                }
            }
            catch (Exception ex) { }
        }
        private void GetBooks_BookRead()
        {
            List<Book> _books = AppData.BooksDetail.Books.Book.Where(x => (bookreadids.Contains(Int32.Parse(x.BookId))) && (Boolean.Parse(x.IsTrashed) == false)).OrderBy(d => bookreadids.IndexOf(Convert.ToInt32(d.BookId))).ToList();
            fromWhere = "bookread";
            if (_books?.Count > 0)
            {
                bookCount = _books.Count;
                BookElements = _books.GetEnumerator();
            }
            else
            {
                bookCount = 0;
                BookElements = null;
            }
            InitializeBookContainer();
            PaintBooks();
        }

        private void GetBooks_BookLater()
        {
            List<Book> _books = AppData.BooksDetail.Books.Book.Where(x => (bookreadlaterids.Contains(Int32.Parse(x.BookId))) && (Boolean.Parse(x.IsTrashed) == false)).OrderBy(d => bookreadlaterids.IndexOf(Convert.ToInt32(d.BookId))).ToList();
            fromWhere = "readlater";
            if (_books?.Count > 0)
            {
                bookCount = _books.Count;
                BookElements = _books.GetEnumerator();
            }
            else
            {
                bookCount = 0;
                BookElements = null;
            }
            InitializeBookContainer();
            PaintBooks();
        }
        public void CategoryRating_Clicked(object sender, EventArgs e)
        {
            ActionTabPage.CollapseUAC();
            Constant.UserActiveTime = DateTime.Now;           
            ActionTabPage.page_Loader.IsVisible = true;
            Task.Run(() => {
                Task.Delay(100).Wait();
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                    ActionTabPage.CheckInternetConnectivity();
                    Grid grid = (Grid)sender;
                    if (!currentRatingtab.Equals(grid.StyleId))
                    {
                        foreach (var item in RatingButtons)
                        {
                            foreach (var elm in item.Children)
                            {
                                if (elm is Label)
                                {
                                    ((Label)elm).SetDynamicResource(Label.StyleProperty, "LabelStyle1");
                                }
                                else if (elm is Image)
                                {
                                    ((Image)elm).SetDynamicResource(Image.SourceProperty, "UnselectedFilledButton");
                                }
                            }
                        }
                        ((Image)grid.Children[0]).SetDynamicResource(Image.SourceProperty, "SelectedFilledButton");
                        ((Label)grid.Children[1]).SetDynamicResource(Label.StyleProperty, "LabelStyle1Selected");
                        currentRatingtab = grid.StyleId;
                        if (currentRatingtab.Equals("10"))
                        {
                            isRatingGiven = true.ToString().ToLower();
                        }
                        else if (currentRatingtab.Equals("11"))
                        {
                            isRatingGiven = false.ToString().ToLower();
                        }
                    }
                    else
                    {
                        ((Image)grid.Children[0]).SetDynamicResource(Image.SourceProperty, "UnselectedFilledButton");
                        ((Label)grid.Children[1]).SetDynamicResource(Label.StyleProperty, "LabelStyle1");
                        currentRatingtab = string.Empty;
                        isRatingGiven = string.Empty;
                    }
                    FilterResult();
                });
            });
        }
        public void CategoryActivities_Clicked(object sender, EventArgs e)
        {
            ActionTabPage.CollapseUAC();
            Constant.UserActiveTime = DateTime.Now;           
            ActionTabPage.page_Loader.IsVisible = true;
            Task.Run(() => {
                Task.Delay(100).Wait();
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                    ActionTabPage.CheckInternetConnectivity();
                    Grid grid = (Grid)sender;
                    if (!currentActivityTab.Equals(grid.StyleId))
                    {
                        foreach (var item in ActivitiesButtons)
                        {
                            foreach (var elm in item.Children)
                            {
                                if (elm is Label)
                                {
                                    ((Label)elm).SetDynamicResource(Label.StyleProperty, "LabelStyle1");
                                }
                                else if (elm is Image)
                                {
                                    ((Image)elm).SetDynamicResource(Image.SourceProperty, "UnselectedFilledButton");
                                }
                            }
                        }
                        ((Image)grid.Children[0]).SetDynamicResource(Image.SourceProperty, "SelectedFilledButton");
                        ((Label)grid.Children[1]).SetDynamicResource(Label.StyleProperty, "LabelStyle1Selected");
                        currentActivityTab = grid.StyleId;
                        if (currentActivityTab.Equals("10"))
                        {
                            isActivityComplete = true.ToString().ToLower();
                        }
                        else if (currentActivityTab.Equals("11"))
                        {
                            isActivityComplete = false.ToString().ToLower();
                        }
                    }
                    else
                    {
                        ((Image)grid.Children[0]).SetDynamicResource(Image.SourceProperty, "UnselectedFilledButton");
                        ((Label)grid.Children[1]).SetDynamicResource(Label.StyleProperty, "LabelStyle1");
                        currentActivityTab = string.Empty;
                        isActivityComplete = string.Empty;
                    }
                    FilterResult();
                });
            });
        }
        protected void FilterResult()
        {
            try
            {
                List<Book> _books = new List<Book>();
                if (string.IsNullOrEmpty(isRatingGiven))
                {
                    if (string.IsNullOrEmpty(isActivityComplete))
                    {
                        _books = AppData.BooksDetail.Books.Book.Where(x => (bookreadids.Contains(Int32.Parse(x.BookId))) && (Boolean.Parse(x.IsTrashed) == false)).OrderBy(d => bookreadids.IndexOf(Convert.ToInt32(d.BookId))).ToList();
                    }
                    else
                    {
                        if (isActivityComplete.ToLower().Equals("true".ToLower()))
                        {
                            _books = AppData.BooksDetail.Books.Book.Where(x => ((bookreadids.Contains(Int32.Parse(x.BookId))) && (activitydonebooks.Contains(Int32.Parse(x.BookId)))) && (Boolean.Parse(x.IsTrashed) == false)).OrderBy(d => activitydonebooks.IndexOf(Convert.ToInt32(d.BookId))).ToList();
                        }
                        else
                        {
                            _books = AppData.BooksDetail.Books.Book.Where(x => ((bookreadids.Contains(Int32.Parse(x.BookId))) && (activitypendingbooks.Contains(Int32.Parse(x.BookId)))) && (Boolean.Parse(x.IsTrashed) == false)).OrderBy(d => activitypendingbooks.IndexOf(Convert.ToInt32(d.BookId))).ToList();
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(isActivityComplete))
                    {
                        if (isRatingGiven.ToLower().Equals("true".ToLower()))
                        {
                            _books = AppData.BooksDetail.Books.Book.Where(x => ((bookreadids.Contains(Int32.Parse(x.BookId))) && (ratedbookids.Contains(Int32.Parse(x.BookId)))) && (Boolean.Parse(x.IsTrashed) == false)).OrderBy(d => ratedbookids.IndexOf(Convert.ToInt32(d.BookId))).ToList();
                        }
                        else
                        {
                            _books = AppData.BooksDetail.Books.Book.Where(x => ((bookreadids.Contains(Int32.Parse(x.BookId))) && (ratingpendingbooks.Contains(Int32.Parse(x.BookId)))) && (Boolean.Parse(x.IsTrashed) == false)).OrderBy(d => ratingpendingbooks.IndexOf(Convert.ToInt32(d.BookId))).ToList();
                        }
                    }
                    else
                    {
                        if (isRatingGiven.ToLower().Equals("true".ToLower()))
                        {
                            if (isActivityComplete.ToLower().Equals("true".ToLower()))
                            {
                                _books = AppData.BooksDetail.Books.Book.Where(x => ((bookreadids.Contains(Int32.Parse(x.BookId))) && (ratedbookids.Contains(Int32.Parse(x.BookId))) && (activitydonebooks.Contains(Int32.Parse(x.BookId)))) && (Boolean.Parse(x.IsTrashed) == false)).ToList();
                            }
                            else
                            {
                                _books = AppData.BooksDetail.Books.Book.Where(x => ((bookreadids.Contains(Int32.Parse(x.BookId))) && (ratedbookids.Contains(Int32.Parse(x.BookId))) && (activitypendingbooks.Contains(Int32.Parse(x.BookId)))) && (Boolean.Parse(x.IsTrashed) == false)).ToList();
                            }
                        }
                        else
                        {
                            if (isActivityComplete.ToLower().Equals("true".ToLower()))
                            {
                                _books = AppData.BooksDetail.Books.Book.Where(x => ((bookreadids.Contains(Int32.Parse(x.BookId))) && (ratingpendingbooks.Contains(Int32.Parse(x.BookId))) && (activitydonebooks.Contains(Int32.Parse(x.BookId)))) && (Boolean.Parse(x.IsTrashed) == false)).ToList();
                            }
                            else
                            {
                                _books = AppData.BooksDetail.Books.Book.Where(x => ((bookreadids.Contains(Int32.Parse(x.BookId))) && (ratingpendingbooks.Contains(Int32.Parse(x.BookId))) && (activitypendingbooks.Contains(Int32.Parse(x.BookId)))) && (Boolean.Parse(x.IsTrashed) == false)).ToList();
                            }
                        }
                    }
                }
                fromWhere = "bookread";
                if (_books?.Count > 0)
                {
                    bookCount = _books.Count;
                    BookElements = _books.GetEnumerator();
                }
                else
                {
                    bookCount = 0;
                    BookElements = null;
                }
                InitializeBookContainer();
                PaintBooks();
            }
            catch (Exception e)
            { }
        }
        #region New Paint Functions

        public void InitializeBookContainer()
        {
            OldX = Constant.DeviceWidth / 3;
            currentRow = 0;
            comFlag = false;
            if (BookElements != null)
            {
                NoBooksContainer.IsVisible = false;
                ScrollView scrollView = null;
                foreach (View v in BooksContainer.Children)
                {
                    if (v is ExtendedScrollView)
                    {
                        scrollView = v as ExtendedScrollView;
                        break;
                    }
                    if (v is ScrollView)
                    {
                        scrollView = v as ScrollView;
                        break;
                    }
                }
                if (scrollView != null)
                {
                    BooksContainer.Children.Remove(scrollView);
                    scrollView = null;
                }

                scrollView = new ExtendedScrollView();
                if (Device.OS == TargetPlatform.Android)
                    scrollView = new ScrollView();
                scrollView.Orientation = ScrollOrientation.Horizontal;
                scrollView.Scrolled += ParentScroll_Scrolled;

                Grid scrollContent = new Grid();
                scrollContent.RowSpacing = 30;
                scrollContent.ColumnSpacing = 0;
                scrollContent.Padding = new Thickness(0, 0, 20, 30);

                int remainingHeight = Constant.DeviceHeight - Constant.PrimaryNavigationHeight - Constant.SecondaryNavigationHeight;
                int rowCount = 0;
                if (Xamarin.Forms.Device.OS == TargetPlatform.Android)
                {
                    rowCount = (int)Math.Floor((double)((remainingHeight / Constant.DeviceDensity) / Constant.BookContainerHeight));
                }
                else
                {
                    rowCount = (int)Math.Floor((double)((remainingHeight) / Constant.BookContainerHeight));
                }
                if (rowCount < 1)
                    rowCount = 1;

                int remainingWidth = Constant.DeviceWidth;

                if (fromWhere.ToLower().Equals("bookread"))
                {
                    ButtonPanel.IsVisible = true;
                    Column1.Width = Constant.LeftButtonPanelWidth;
                    remainingWidth -= Constant.LeftButtonPanelWidth;
                    BooksContainer.Padding = new Thickness(0, 0, 0, 0);
                }
                else
                {
                    ButtonPanel.IsVisible = false;
                    Column1.Width = 0;
                    BooksContainer.Padding = new Thickness(22, 0, 0, 0);
                }
                int columnCount = (int)Math.Floor((double)(remainingWidth / (Constant.SmallestBookWidth + 50)));

                if ((Xamarin.Forms.Device.OS == TargetPlatform.Windows) || (Xamarin.Forms.Device.OS == TargetPlatform.iOS))
                {
                    Constant.ShownBookCount = (rowCount * (columnCount + 1));
                    Constant.ShowBookCount = Constant.ShownBookCount;
                }
                else
                {
                    Constant.ShownBookCount = (rowCount * (columnCount));
                    Constant.ShowBookCount = Constant.ShownBookCount;
                }
                for (int i = 0; i < rowCount; i++)
                {
                    scrollContent.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
                    StackLayout stack = new StackLayout();
                    stack.Orientation = StackOrientation.Horizontal;
                    stack.Spacing = 50;
                    scrollContent.Children.Add(stack, 0, i);
                }

                int val = (int)(bookCount / rowCount);
                if (bookCount % rowCount != 0)
                {
                    val++;
                }
                double BookRowGridWidth = (val > 0 ? (val * (Constant.SmallestBookWidth)) + ((val - 1) * 50) : (Constant.SmallestBookWidth)) + 20;
                scrollContent.WidthRequest = BookRowGridWidth;

                scrollView.Content = scrollContent;
                BooksContainer.Children.Add(scrollView);
            }
            else
            {
                if ((Xamarin.Forms.Device.OS == TargetPlatform.Android) || (Device.OS == TargetPlatform.iOS))
                {
                    NoBooksTextContainer.Padding = new Thickness(115, 0, 0, 0);
                }
                else
                {
                    NoBooksTextContainer.Padding = new Thickness(115, 20, 0, 0);
                }
                NoBooksText1Container.Text = "N o o o o o";
                NoBooksText2Container.Text = "No books here!";
                if (!fromWhere.ToLower().Equals("bookread"))
                {
                    ButtonPanel.IsVisible = false;
                    Column1.Width = 0;
                }
                else
                {
                    ButtonPanel.IsVisible = true;
                    Column1.Width = Constant.LeftButtonPanelWidth;
                }
                ScrollView scrollView = null;
                foreach (View v in BooksContainer.Children)
                {
                    if (v is ExtendedScrollView)
                    {
                        scrollView = v as ExtendedScrollView;
                        break;
                    }
                    if (v is ScrollView)
                    {
                        scrollView = v as ScrollView;
                        break;
                    }
                }
                if (scrollView != null)
                {
                    BooksContainer.Children.Remove(scrollView);
                    scrollView = null;
                }
                NoBooksContainer.IsVisible = true;
                NoBooksContainer.Opacity = 0;
                NoBooksContainer.FadeTo(1, 250, Easing.SinIn);
                ActionTabPage.page_Loader.IsVisible = false;
            }
        }

        protected async void PaintBooks()
        {
            try
            {
                int index = 0;
                if (BookElements != null)
                {
                    while (index < Constant.ShowBookCount && BookElements.MoveNext())
                    {
                        index++;

                        SmallBookView gridBook = new SmallBookView(BookElements.Current, BookTapGestureRecognizer, 1);

                        Grid v = ((Grid)((ScrollView)BooksContainer.Children.Last()).Content);
                        ((StackLayout)v.Children[currentRow]).Children.Add(gridBook);

                        currentRow++;
                        if (currentRow >= v.RowDefinitions.Count)
                        {
                            currentRow = 0;
                        }
                    }
                    if (index == Constant.ShowBookCount)
                    {
                        Constant.ShownBookCount += Constant.ShowBookCount;
                        flag = false;
                    }
                }
            }
            catch (Exception ex)
            { }
            finally
            {
                ActionTabPage.page_Loader.IsVisible = false;
            }
        }

        private void ParentScroll_Scrolled(object sender, ScrolledEventArgs e)
        {
            Constant.UserActiveTime = DateTime.Now;
            //ActionTabPage.CheckInternetConnectivity();
            ScrollView view = (ScrollView)sender;
            if (!flag)
            {
                if (view.ScrollX > OldX)
                {
                    OldX += Constant.DeviceWidth / 3;
                    flag = true;
                    PaintBooks();

                }
                else if ((!comFlag) && ((view.ScrollX < OldX) || (view.Width < OldX)))
                {
                    OldX += 230;
                    comFlag = true;
                    flag = true;
                    PaintBooks();
                }
            }
        }

        public void paintSideButtons()
        {
            ButtonPanel.Children.Clear();
            isActivityComplete = string.Empty;
            isRatingGiven = string.Empty;
            currentActivityTab = string.Empty;
            currentRatingtab = string.Empty;
            StackLayout stack = new StackLayout { Orientation = StackOrientation.Vertical };
            try
            {
                StackLayout stack1 = new StackLayout { Orientation = StackOrientation.Vertical, Padding = new Thickness(0, 10, 0, 10) };
                Label lbl1 = new Label { Text = "RATINGS", FontAttributes = FontAttributes.Bold, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center };
                lbl1.SetDynamicResource(Label.StyleProperty, "LabelNormalBold");

                stack1.Children.Add(lbl1);
                stack.Children.Add(stack1);
                StackLayout stack2 = new StackLayout { Orientation = StackOrientation.Vertical, Padding = new Thickness(0, 10, 0, 10) };
                Label lbl2 = new Label { Text = "ACTIVITIES", FontAttributes = FontAttributes.Bold, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center };
                lbl2.SetDynamicResource(Label.StyleProperty, "LabelNormalBold");
                stack2.Children.Add(lbl2);

                Grid ButtonGrid = new Grid();
                ButtonGrid.RowSpacing = 0;
                ButtonGrid.ColumnSpacing = 0;
                ButtonGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
                ButtonGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

                Image back = new Image { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, HeightRequest = 32, WidthRequest = 136 };
                back.SetDynamicResource(Image.SourceProperty, "UnselectedFilledButton");
                ButtonGrid.Children.Add(back, 0, 0);

                var subsectionBtn = new Label();
                subsectionBtn.Text = "GIVEN";
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += CategoryRating_Clicked;

                ButtonGrid.StyleId = "10";
                ButtonGrid.ClassId = "ratinggiven";
                subsectionBtn.SetDynamicResource(Image.StyleProperty, "LabelStyle1");
                subsectionBtn.HorizontalTextAlignment = TextAlignment.Center;
                subsectionBtn.VerticalTextAlignment = TextAlignment.Center;

                ButtonGrid.Children.Add(subsectionBtn, 0, 0);
                ButtonGrid.GestureRecognizers.Add(tapGestureRecognizer);
                stack.Children.Add(ButtonGrid);

                RatingButtons.Add(ButtonGrid);
                Grid ButtonGrid1 = new Grid();
                ButtonGrid1.RowSpacing = 0;
                ButtonGrid1.ColumnSpacing = 0;
                ButtonGrid1.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
                ButtonGrid1.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

                Image back1 = new Image { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, HeightRequest = 32, WidthRequest = 136 };
                back1.SetDynamicResource(Image.SourceProperty, "UnselectedFilledButton");
                ButtonGrid1.Children.Add(back1, 0, 0);

                var subsectionBtn1 = new Label();
                subsectionBtn1.Text = "PENDING";
                var tapGestureRecognizer1 = new TapGestureRecognizer();
                tapGestureRecognizer1.Tapped += CategoryRating_Clicked;

                ButtonGrid1.StyleId = "11";
                ButtonGrid1.ClassId = "ratingpending";
                subsectionBtn1.SetDynamicResource(Image.StyleProperty, "LabelStyle1");
                subsectionBtn1.HorizontalTextAlignment = TextAlignment.Center;
                subsectionBtn1.VerticalTextAlignment = TextAlignment.Center;

                ButtonGrid1.Children.Add(subsectionBtn1, 0, 0);
                ButtonGrid1.GestureRecognizers.Add(tapGestureRecognizer1);
                stack.Children.Add(ButtonGrid1);
                stack.Children.Add(stack2);
                RatingButtons.Add(ButtonGrid1);
                Grid ButtonGrid2 = new Grid();
                ButtonGrid2.RowSpacing = 0;
                ButtonGrid2.ColumnSpacing = 0;
                ButtonGrid2.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
                ButtonGrid2.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

                Image back2 = new Image { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, HeightRequest = 32, WidthRequest = 136 };
                back2.SetDynamicResource(Image.SourceProperty, "UnselectedFilledButton");
                ButtonGrid2.Children.Add(back2, 0, 0);

                var subsectionBtn2 = new Label();
                subsectionBtn2.Text = "COMPLETED";
                var tapGestureRecognizer2 = new TapGestureRecognizer();
                tapGestureRecognizer2.Tapped += CategoryActivities_Clicked;
                ButtonGrid2.ClassId = "activitiescompleted";
                ButtonGrid2.StyleId = "10";
                subsectionBtn2.SetDynamicResource(Image.StyleProperty, "LabelStyle1");
                subsectionBtn2.HorizontalTextAlignment = TextAlignment.Center;
                subsectionBtn2.VerticalTextAlignment = TextAlignment.Center;

                ButtonGrid2.Children.Add(subsectionBtn2, 0, 0);
                ButtonGrid2.GestureRecognizers.Add(tapGestureRecognizer2);
                stack.Children.Add(ButtonGrid2);
                ActivitiesButtons.Add(ButtonGrid2);
                Grid ButtonGrid3 = new Grid();
                ButtonGrid3.RowSpacing = 0;
                ButtonGrid3.ColumnSpacing = 0;
                ButtonGrid3.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
                ButtonGrid3.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                Image back3 = new Image { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, HeightRequest = 32, WidthRequest = 136 };
                back3.SetDynamicResource(Image.SourceProperty, "UnselectedFilledButton");
                ButtonGrid3.Children.Add(back3, 0, 0);
                var subsectionBtn3 = new Label();
                subsectionBtn3.Text = "PENDING";
                var tapGestureRecognizer3 = new TapGestureRecognizer();
                tapGestureRecognizer3.Tapped += CategoryActivities_Clicked;
                ButtonGrid3.ClassId = "activitiespending";
                ButtonGrid3.StyleId = "11";
                subsectionBtn3.SetDynamicResource(Image.StyleProperty, "LabelStyle1");
                subsectionBtn3.HorizontalTextAlignment = TextAlignment.Center;
                subsectionBtn3.VerticalTextAlignment = TextAlignment.Center;
                ButtonGrid3.Children.Add(subsectionBtn3, 0, 0);
                ButtonGrid3.GestureRecognizers.Add(tapGestureRecognizer3);
                stack.Children.Add(ButtonGrid3);
                ActivitiesButtons.Add(ButtonGrid3);
            }
            catch (Exception ex)
            { }
            if (Device.OS == TargetPlatform.iOS)
            {
                ButtonPanel.IsVisible = true;
            }
            ButtonPanel.Children.Add(stack);
        }
        #endregion
        protected void ShowProfile()
        {
            CurrentAvatarImageStack.IsVisible = true;
            CurentAvatarLabel.Opacity = 0;
            ChangePasswordStack.IsVisible = false;
            ChangeAvatarImageStack.IsVisible = false;
            SaveBtn_Avatar.IsVisible = false;
            SaveBtn_Password.IsVisible = false;
            CancelBtn.IsVisible = false;
            UserDetailStack.IsVisible = true;
            CurentAvatarImage.IsVisible = true;
            ChangeAvatar_Show.IsVisible = true;
            ChangePassword_Show.IsVisible = true;

            

            CurrentPassword.Text = "";
            NewPassword.Text = "";
            ReNewPassword.Text = "";
            avatar.avatarId = 0;
            this.setFocus(4);

            if (AppData.User != null)
            {

                if (AppData.User.Role.ToLower() != "student")
                {
                    ChangePassword_Show.IsVisible = false;
                }

                currentGrade = AppData.User.SubSectionId.ToString();

                userName.Text = AppData.User.FirstName + " " + AppData.User.LastName;
                gradeName.Text = AppData.User.Grade;
                subsectionName.Text = AppData.User.SubSection.ToUpper();
                schoolName.Text = AppData.User.SchoolName;
            }
            ActionTabPage.page_Loader.IsVisible = false;
        }
        public void setFocus(int i)
        {
            switch (i)
            {
                case 1:
                    ((AbsoluteLayout)CurrentPassword.Parent).Opacity = 1;
                    CurrentPasswordBoxView.Opacity = 1;
                    break;
                case 2:
                    ((AbsoluteLayout)NewPassword.Parent).Opacity = 1;
                    NewPasswordBoxView.Opacity = 1;
                    break;
                case 3:
                    ((AbsoluteLayout)ReNewPassword.Parent).Opacity = 1;
                    ReNewPasswordBoxView.Opacity = 1;
                    break;
                case 4:
                    ((AbsoluteLayout)CurrentPassword.Parent).Opacity = 0.5;
                    CurrentPasswordBoxView.Opacity = 0.5;
                    break;
                case 5:
                    ((AbsoluteLayout)NewPassword.Parent).Opacity = 0.5;
                    NewPasswordBoxView.Opacity = 0.5;
                    break;
                case 6:
                    ((AbsoluteLayout)ReNewPassword.Parent).Opacity = 0.5;
                    ReNewPasswordBoxView.Opacity = 0.5;
                    break;
                default:
                    ((AbsoluteLayout)CurrentPassword.Parent).Opacity = 1;
                    CurrentPasswordBoxView.Opacity = 1;
                    break;
            }
        }
        protected void ShowChangeAvatar()
        {
            CurentAvatarLabel.Opacity = 1;
            CurentAvatarImage.IsVisible = true;
            ChangePasswordStack.IsVisible = false;
            ChangeAvatarImageStack.IsVisible = true;
            SaveBtn_Avatar.IsVisible = true;
            SaveBtn_Password.IsVisible = false;
            CancelBtn.IsVisible = true;
            UserDetailStack.IsVisible = false;
            ChangeAvatar_Show.IsVisible = false;
            ChangePassword_Show.IsVisible = false;
        }
        protected void ShowChangePassword()
        {
            SaveBtn_Password.Opacity = 0.5;
            CurrentAvatarImageStack.IsVisible = false;
            ChangePasswordStack.IsVisible = true;
            ChangeAvatarImageStack.IsVisible = false;
            SaveBtn_Avatar.IsVisible = false;
            SaveBtn_Password.IsVisible = true;
            CancelBtn.IsVisible = true;
            UserDetailStack.IsVisible = false;
            ChangeAvatar_Show.IsVisible = false;
            ChangePassword_Show.IsVisible = false;
        }
        protected void ChangePassword_Clicked(object sender, EventArgs e)
        {
            ActionTabPage.CollapseUAC();
            Constant.UserActiveTime = DateTime.Now;
            bool isconnect = true;
            isconnect = HelperFunctions.CheckInternetConnection();
            if (isconnect)
            {
                ActionTabPage.IsOfflineTextLabel.IsVisible = false;
                this.errorCurrentPassword.IsVisible = false;
                this.errorNewPassword.IsVisible = false;
                this.errorReNewPassword.IsVisible = false;
                ShowChangePassword();
            }
            else
            {
                ActionTabPage.IsOfflineTextLabel.IsVisible = true;
                actionTabPageObject.WritePopupText("Cannot connect to the Server. Check your Internet Connection.", "nonetaccess", "YES", "OK", 1);
                actionTabPageObject.ShowPopupCommon();
            }
        }
        protected void ChangeAvatar_Clicked(object sender, EventArgs e)
        {
            ActionTabPage.CollapseUAC();
            Constant.UserActiveTime = DateTime.Now;
            bool isconnect = true;
            isconnect = HelperFunctions.CheckInternetConnection();
            if (isconnect)
            {
                ActionTabPage.IsOfflineTextLabel.IsVisible = false;
                //HelperFunctions.GetAvatar(AppData.User.UserId, null, this, "userdashboard");
                string response = MyWebRequest.GetRequest("GetAvatar?UserId=" + AppData.User.UserId, null, null);
                paintGrid(response);
            }
            else
            {
                ActionTabPage.IsOfflineTextLabel.IsVisible = true;
                actionTabPageObject.WritePopupText("Cannot connect to the Server. Check your Internet Connection.", "nonetaccess", "YES", "OK", 1);
                actionTabPageObject.ShowPopupCommon();
            }
        }
        public void paintGrid(string jsonString)
        {
            imagelist.Clear();
            try
            {
                myavatars = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AvatarDB>>(jsonString);
                paintAvatars();
                ShowChangeAvatar();
            }
            catch (Exception e)
            { }
        }
        public void SelectOne(CircleImage image)
        {
            // PrevImage = Int32.Parse(image.StyleId);
            paintAvatars(Int32.Parse(image.StyleId));
        }
        private void paintAvatars(int PrevImage = -1)
        {
            ChangeAvatarImages.Children.Clear();
            var imagetapped = new TapGestureRecognizer();
            imagetapped.Tapped += (s, e) =>
            {
                Constant.UserActiveTime = DateTime.Now;
                CircleImage img = (CircleImage)s;
                avatar.avatarId = int.Parse(img.StyleId);
                avatar.sourceName = img.ClassId.ToString();
                SelectOne(img);
            };
            int count = 1;
            foreach (AvatarDB item in myavatars)
            {
                if (item.AvatarId != AppData.User.AvatarId && count <= 4)
                {
                    count++;
                    var image = new CircleImage();
                    image.ClassId = item.ImagePath;
                    // item.ImagePath = item.ImagePath.Replace("#size#", Constant.AvatarSmall);
                    image.Aspect = Aspect.Fill;
                    image.HeightRequest = 70;
                    image.WidthRequest = 70;
                    if (PrevImage != -1 && PrevImage.Equals(item.AvatarId))
                    {
                        string ptrm = Constant.CurrentPlateform;
                        if (ptrm == "Win8.1")
                        {
                            image.SetDynamicResource(CircleImage.BackgroundColorProperty, "Secondary2Color");
                        }
                        else
                        {
                            image.SetDynamicResource(CircleImage.BorderColorProperty, "Secondary2Color");
                        }
                    }
                    else
                    {
                        image.SetDynamicResource(CircleImage.BorderColorProperty, "PrimaryColor");
                    }
                    image.SetDynamicResource(CircleImage.SourceProperty, "AvatarSmallImage");

                    string temp = image.ClassId;
                    temp = temp.Replace("#size#", Constant.AvatarSmall);


                    image.BorderThickness = 3;
                    image.StyleId = item.AvatarId.ToString();
                    image.GestureRecognizers.Add(imagetapped);
                    imagelist.Add(image);
                    image.Source = new UriImageSource
                    {
                        Uri = new Uri(temp),
                        CacheValidity = TimeSpan.MaxValue,
                        CachingEnabled = true
                    };
                    ChangeAvatarImages.Children.Add(image);
                }
            }

            //  ReplaceImage();
        }
        //Not in use
        protected void CancelBtn_Clicked(object sender, EventArgs e)
        {
            ActionTabPage.CollapseUAC();
            Constant.UserActiveTime = DateTime.Now;
            ActionTabPage.CheckInternetConnectivity();
            ShowProfile();
        }
        protected void SaveBtnAvatar_Clicked(object sender, EventArgs e)
        {
            ActionTabPage.CollapseUAC();
            Constant.UserActiveTime = DateTime.Now;
            bool isconnect = true;
            isconnect = HelperFunctions.CheckInternetConnection();
            if (isconnect)
            {
                if (avatar.avatarId == 0)
                {
                    actionTabPageObject.WritePopupText("Please select an Avatar", "SelectAvatar", "YES", "OK", 1);
                    actionTabPageObject.ShowPopupCommon();
                }
                else
                {
                    //HelperFunctions.AddEditAvatar(avatar.avatarId, AppData.User.UserId, null, this, "UserDashboard");
                    string status1 = MyWebRequest.PostRequest("AddEditAvatar", null, new { AvatarId = Convert.ToString(avatar.avatarId), UserId = Convert.ToString(AppData.User.UserId) }, null);
                    AppData.User.AvatarId = avatar.avatarId;
                    AppData.User.AvatarImage = avatar.sourceName;
                    string uri = AppData.User.AvatarImage.Replace("#size#", Constant.AvatarLarge);
                    CurentAvatarImage.Source = new UriImageSource
                    {
                        Uri = new Uri(uri),
                        CacheValidity = TimeSpan.MaxValue,
                        CachingEnabled = true
                    };
                    GotoNext(status1);
                    action = "avatar";
                }

            }
            else
            {
                ActionTabPage.IsOfflineTextLabel.IsVisible = true;
                actionTabPageObject.WritePopupText("Cannot connect to the Server. Check your Internet Connection.", "nonetaccess", "YES", "OK", 1);
                actionTabPageObject.ShowPopupCommon();
            }

        }
        public void GotoNext(string response)
        {
            ShowProfile();
            ActionTabPage.ChangeCurrentAvatarImageDashboard();
            if (response == "false")
            {
                actionTabPageObject.WritePopupText("Unable to change avatar - please try again after some time", "SelectAvatar", "YES", "OK", 1);
                ActionTabPage.PopUpFadeOut.IsVisible = true;
                ActionTabPage.CommonPopup.IsVisible = true;
                ActionTabPage.ParentPopupContainer.IsVisible = true;
            }
            else
            {
                actionTabPageObject.WritePopupText("Avatar changed!", "SelectAvatar", "YES", "OK", 1);
                ActionTabPage.PopUpFadeOut.IsVisible = true;
                ActionTabPage.CommonPopup.IsVisible = true;
                ActionTabPage.ParentPopupContainer.IsVisible = true;
            }
        }
        protected void SaveBtnPassword_Clicked(object sender, EventArgs e)
        {
            ActionTabPage.CollapseUAC();
            Constant.UserActiveTime = DateTime.Now;
            bool IsValid = false;
            if(AllFieldsFilled())
            {
                if (!NewPassword.Text.Equals(CurrentPassword.Text))
                {
                    if (ReNewPassword.Text.Equals(NewPassword.Text))
                    {
                        if (AppData.User.Role.ToLower().Equals("student"))
                        {
                            IsValid = NewPassword.Text.Length >= 6 ? true : false;
                        }
                        else
                        {
                            IsValid = (Regex.IsMatch(NewPassword.Text, Constant.passwordRegex));
                        }
                        if (IsValid)
                        {
                            bool isconnect = true;
                            isconnect = HelperFunctions.CheckInternetConnection();
                            if (isconnect)
                            {
                                ActionTabPage.IsOfflineTextLabel.IsVisible = false;
                                //ActionTabPage.CommonPopupActionTab.TextMessage.Text = "Are you sure you want to change the password?";
                                //ActionTabPage.CommonPopupActionTab.IsVisible = true;
                                string status1 = MyWebRequest.PostRequest("changepassword", null, new { NewPassword = NewPassword.Text, OldPassword = CurrentPassword.Text, UserId = AppData.User.UserId.ToString() }, null);
                                changePasswordCallback(status1);
                                //HelperFunctions.ChangePassword(AppData.User.UserId, NewPassword.Text, CurrentPassword.Text, this);
                                action = "password";
                            }
                            else
                            {
                                ActionTabPage.IsOfflineTextLabel.IsVisible = true;
                                actionTabPageObject.WritePopupText("Cannot connect to the Server. Check your Internet Connection.", "nonetaccess", "YES", "OK", 1);
                                actionTabPageObject.ShowPopupCommon();
                            }
                        }
                        else
                        {
                            errorNewPassword.Text = "Password must have atleast 6 characters.";
                            errorNewPassword.IsVisible = true;
                            this.setFocus(4);
                            this.setFocus(6);
                            this.setFocus(2);
                        }
                    }
                    else
                    {

                        errorReNewPassword.IsVisible = true;
                        errorReNewPassword.Text = "The two secret codes do not match - try again.";
                        this.setFocus(3);
                        this.setFocus(4);
                        this.setFocus(5);
                    }
                }
                else
                {
                        errorNewPassword.IsVisible = true;
                        errorNewPassword.Text = "Current & New Password cannot be same.";
                        this.setFocus(2);
                        this.setFocus(4);
                        this.setFocus(6);
                }

            }

            //if (!string.IsNullOrEmpty(CurrentPassword.Text))
            //{
            //    if (!string.IsNullOrEmpty(NewPassword.Text))
            //    {
            //        if (!NewPassword.Text.Equals(CurrentPassword.Text))
            //        {
            //            if (!string.IsNullOrEmpty(ReNewPassword.Text))
            //            {
            //                if (ReNewPassword.Text.Equals(NewPassword.Text))
            //                {
            //                    User userObj = new User();
            //                    if (Xamarin.Forms.Device.OS == TargetPlatform.Android)
            //                    {
            //                        Helpers.CacheSettings.SettingsKey = "UserDetails";
            //                        if (Helpers.CacheSettings.GeneralSettings != "")
            //                        {
            //                            userObj = JsonConvert.DeserializeObject<User>(Helpers.CacheSettings.GeneralSettings);
            //                        }
            //                    }
            //                    else
            //                    {
            //                        if (fileService.getUserDetails("UserDetails") != null)
            //                        {
            //                            userObj = fileService.getUserDetails("UserDetails");
            //                        }
            //                    }

            //                    if (userObj.Role.ToLower().Equals("student"))
            //                    {
            //                        IsValid = NewPassword.Text.Length >= 6 ? true : false;
            //                    }
            //                    else
            //                    {
            //                        IsValid = (Regex.IsMatch(NewPassword.Text, Constant.passwordRegex));
            //                    }
            //                    if (IsValid)
            //                    {
            //                        bool isconnect = true;
            //                        isconnect = HelperFunctions.CheckInternetConnection().Result;
            //                        if (isconnect)
            //                        {
            //                            ActionTabPage.IsOfflineTextLabel.IsVisible = false;
            //                            ActionTabPage.CommonPopupActionTab.TextMessage.Text = "Are you sure you want to change the password?";
            //                            ActionTabPage.CommonPopupActionTab.IsVisible = true;
            //                            action = "password";


            //                        }
            //                        else
            //                        {
            //                            ActionTabPage.IsOfflineTextLabel.IsVisible = true;
            //                            actionTabPageObject.WritePopupText("Cannot connect to the Server. Check your Internet Connection.", "nonetaccess", "YES", "OK", 1);
            //                            actionTabPageObject.ShowPopupCommon();
            //                        }
            //                    }
            //                    else
            //                    {
            //                        errorNewPassword.Text = "Weak Password";
            //                        errorNewPassword.IsVisible = true;
            //                        this.setFocus(4);
            //                        this.setFocus(6);
            //                        this.setFocus(2);
            //                    }
            //                }
            //                else
            //                {

            //                    errorReNewPassword.IsVisible = true;
            //                    errorReNewPassword.Text = "Entries don’t match.";
            //                    this.setFocus(3);
            //                    this.setFocus(4);
            //                    this.setFocus(5);
            //                }
            //            }
            //            else
            //            {

            //                errorReNewPassword.IsVisible = true;
            //                errorReNewPassword.Text = "Please enter text.";
            //                this.setFocus(3);
            //                this.setFocus(4);
            //                this.setFocus(5);
            //            }
            //        }
            //        else
            //        {
            //            errorNewPassword.IsVisible = true;

            //            errorNewPassword.Text = "Current & New Password cannot be same.";
            //            this.setFocus(2);
            //            this.setFocus(4);
            //            this.setFocus(6);
            //        }
            //    }
            //    else
            //    {

            //        errorNewPassword.IsVisible = true;
            //        errorNewPassword.Text = "Please enter text.";
            //        this.setFocus(2);
            //        this.setFocus(4);
            //        this.setFocus(6);
            //    }
            //}
            //else
            //{

            //    errorCurrentPassword.IsVisible = true;
            //    errorCurrentPassword.Text = "Please enter text.";
            //    this.setFocus(1);
            //    this.setFocus(5);
            //    this.setFocus(6);
            //}
        }


        private bool AllFieldsFilled()
        {
            if(!(string.IsNullOrEmpty(CurrentPassword.Text) && string.IsNullOrWhiteSpace(CurrentPassword.Text))
                && !(string.IsNullOrEmpty(NewPassword.Text) && string.IsNullOrWhiteSpace(NewPassword.Text))
                 && !(string.IsNullOrEmpty(ReNewPassword.Text) && string.IsNullOrWhiteSpace(ReNewPassword.Text)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void changePasswordCallback(string result)
        {
            if (result.Equals("1"))
            {
                if (actionTabPageObject != null)
                {
                    actionTabPageObject.WritePopupText("Password changed!", "ChangePassword", "YES", "OK", 1);
                    actionTabPageObject.ShowPopupCommon();
                }
                ShowProfile();
            }
            else if (result.Equals("4"))
            {
                if (actionTabPageObject != null)
                {
                    errorCurrentPassword.IsVisible = true;
                    //errorCurrentPassword.Text = "Incorrect Password.";
                    errorCurrentPassword.Text = "Doesn’t seem like your secret code - try again";
                }
            }
            else
            {
                if (actionTabPageObject != null)
                {
                    actionTabPageObject.WritePopupText("Unable to change password - please try again after sometime", "ChangePassword", "YES", "OK", 1);
                }
                ShowProfile();
            }

        }
        public void OnTextChanged(object sender, EventArgs e)
        {
            Constant.UserActiveTime = DateTime.Now;
            ActionTabPage.CheckInternetConnectivity();
            try
            {
                Entry box = ((Entry)sender);
                if (box == CurrentPassword)
                {
                    this.errorCurrentPassword.IsVisible = false;
                }
                else if (box == NewPassword)
                {
                    this.errorNewPassword.IsVisible = false;
                }
                else if (box == ReNewPassword)
                {
                    this.errorReNewPassword.IsVisible = false;
                }
                else
                {
                    this.errorCurrentPassword.IsVisible = false;
                    this.errorNewPassword.IsVisible = false;
                    this.errorReNewPassword.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                this.errorCurrentPassword.IsVisible = false;
                this.errorNewPassword.IsVisible = false;
                this.errorReNewPassword.IsVisible = false;
            }
        }
    }
}
