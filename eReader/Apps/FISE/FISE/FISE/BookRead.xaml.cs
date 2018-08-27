using Kitablet.Helpers;
using Kitablet.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Kitablet
{
    public partial class BookRead : ContentPage
    {
        public static bool isBookOpended = false;
        protected bool isOfflineReadAllowed, isReadAloud, isRead, isReadLater, IsLandscape, IsComingSoon, isForDownload, isHasActivity;
        protected string id, EPUB_Name, subSection, bookCover = string.Empty;
        protected int ReadFrom, subSectionId, currentPage, PageDisplay = 1;
        public static List<BookProgress> bookfiles = new List<BookProgress>();
        protected UserBook _UserBook;
        protected Book _Book;
        [System.ComponentModel.DefaultValue(true)]
        protected bool isPagerAllowed { get; set; }


        public class BookProgress
        {
            public string BookID { get; set; }
            public ActivityIndicatorLoader ProgressLoader { get; set; }
        }

        #region Rating Control
        private Label ReviewsLabel { get; set; }
        private List<Image> StarImages { get; set; }
        #endregion

        public BookRead(string id, bool isForDownload, int ReadFrom = 0)
        {
            isBookOpended = true;
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            if (Device.OS == TargetPlatform.Android)
            {
                bookTitle_Android.IsVisible = true;
                bookTitle.IsVisible = false;
            }
            else
            {
                bookTitle_Android.IsVisible = false;
                bookTitle.IsVisible = true;
            }
            //this.BookReadLayout.WidthRequest = Xamarin.Forms.Device.OnPlatform<double>(Constant.DeviceWidth, Constant.DeviceWidth / Constant.DeviceDensity, Constant.DeviceWidth);
            //this.BookReadLayout.HeightRequest = Xamarin.Forms.Device.OnPlatform<double>(Constant.DeviceHeight, (Constant.DeviceHeight / Constant.DeviceDensity) - 25, Constant.DeviceHeight);

            var tapReadLater = new TapGestureRecognizer();
            tapReadLater.Tapped += readLaterBtn_Clicked;
            readLaterBtn.GestureRecognizers.Add(tapReadLater);

            var releaseBookTab = new TapGestureRecognizer();
            releaseBookTab.Tapped += releaseBtn_Clicked;
            releaseBook.GestureRecognizers.Add(releaseBookTab);

            var tapsetting2 = new TapGestureRecognizer();
            tapsetting2.Tapped += (s, e) =>
            {
                Constant.UserActiveTime = DateTime.Now;
                if (!PopWindow.IsVisible)
                {
                    PopWindow.IsVisible = true;
                    PopArrowImg.IsVisible = true;
                }
                else
                {
                    PopWindow.IsVisible = false;
                    PopArrowImg.IsVisible = false;
                }
            };
            ratingLabel.GestureRecognizers.Add(tapsetting2);

            if (Device.OS == TargetPlatform.Android)
            {
                AvgRatingofBook.Margin = new Thickness(1, 0, 1, 2);
            }
            else if (Device.OS == TargetPlatform.Windows)
            {
                AvgRatingofBook.Margin = new Thickness(1, 0, 1, 30);
            }
            if (Xamarin.Forms.Device.OS == TargetPlatform.Android)
            {
                bigRatingText.Margin = new Thickness(1, 0, 0, 0);
            }
            else if (Xamarin.Forms.Device.OS == TargetPlatform.Windows)
            {
                bigRatingText.Margin = new Thickness(2, 0, 0, 0);
            }
            if (Device.OS == TargetPlatform.iOS)
            {
                AvgRatingofBook.FontAttributes = FontAttributes.Bold;
            }
            var ratingtap = new TapGestureRecognizer();
            ratingtap.Tapped += (s, e) =>
            {
                Constant.UserActiveTime = DateTime.Now;
                PopWindow.IsVisible = false;
                PopArrowImg.IsVisible = false;
                if (this.isRead)
                {
                    this.Page_Loader.IsVisible = true;
                    Task.Run(() =>
                    {
                        Task.Delay(100).Wait();
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                        {
                            Navigation.PushAsync(new BookCompleted(this.id, this.ReadFrom));
                            Navigation.RemovePage(this);
                        });
                    });
                }
            };
            ((AbsoluteLayout)ratingBigBtn.Parent).GestureRecognizers.Add(ratingtap);

            var activitytap = new TapGestureRecognizer();
            activitytap.Tapped += (s, e) =>
            {
                Constant.UserActiveTime = DateTime.Now;
                PopWindow.IsVisible = false;
                PopArrowImg.IsVisible = false;
                if (this.isRead && this.isOfflineReadAllowed)
                {
                    if (Boolean.Parse(this._UserBook.IsActivityDone))
                    {
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                        {
                            BookPagePopUp.TextMessage.Text = "Hey! You have already attempted this activity! Would you like to see how it went last time?";
                            BookPagePopUp.SubmitPopupBtn.IsVisible = true;
                            BookPagePopUp.SubmitTextMessage.Text = "YES";
                            BookPagePopUp.CancelTextMessage.Text = "NO";
                            BookPagePopUp.IsVisible = true;
                        });
                    }
                    else
                    {
                        this.Page_Loader.IsVisible = true;
                        Task.Run(() =>
                        {
                            Task.Delay(100).Wait();
                            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                            {
                                Navigation.PushAsync(new BookActivity(this.id, this.ReadFrom));
                                Navigation.RemovePage(this);
                            });
                        });
                    }
                }
            };
            activityBigBtn.GestureRecognizers.Add(activitytap);

            var okPopuptap = new TapGestureRecognizer();
            okPopuptap.Tapped += (s, e) =>
            {
                Constant.UserActiveTime = DateTime.Now;
                BookPagePopUp.IsVisible = false;
                BookPagePopUp.TextMessage.Text = string.Empty;
                BookPagePopUp.SubmitTextMessage.Text = string.Empty;
                BookPagePopUp.CancelTextMessage.Text = string.Empty;

                this.Page_Loader.IsVisible = true;
                Task.Run(() =>
                {
                    Task.Delay(100).Wait();
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                    {
                        Navigation.PushAsync(new BookActivity(this.id, this.ReadFrom));
                        Navigation.RemovePage(this);
                    });
                });
                //try
                //{
                //    if (this._UserBook != null)
                //    {
                //        string _device = this._UserBook.Devices.DeviceId.Where(x => x.Trim().Equals(Constant.DeviceIdFromDB.ToString()))?.FirstOrDefault();
                //        if (_device != null)
                //        {
                //            this._UserBook.Devices.DeviceId.Remove(_device);
                //        }
                //    }
                //    releaseBook.IsVisible = false;
                //    isOfflineBook.IsVisible = false;
                //    this.isOfflineReadAllowed = false;
                //    activityBigBtn.Opacity = 0.5;
                //    this.EPUB_Name = string.Empty;

                //    if (AppData.FileService.CheckDirectoryExistence(Path.Combine(Constant.FileDirectoryStructure, Constant.FileNameInitials + this.id)))
                //    {
                //        AppData.FileService.DeleteDirectory(Path.Combine(Constant.FileDirectoryStructure, Constant.FileNameInitials + this.id));
                //    }

                //    DownloadFile Download_File = new DownloadFile
                //    {
                //        BookID = this.id,
                //        IsDownloaded = false.ToString(),
                //        IsUnZip = false.ToString(),
                //        IsDecrypted = false.ToString(),
                //        IsEncrypted = false.ToString(),
                //        IsProcessing = false.ToString(),
                //        BookFiles = new BookFiles
                //        {
                //            BookFile = new List<BookFile>()
                //        }
                //    };
                //    Utils.SetBookStatus(Download_File);

                //    if (HelperFunctions.CheckInternetConnection())
                //    {
                //        ActionTabPage.IsOfflineTextLabel.IsVisible = false;
                //        string status = MyWebRequest.PostRequest("releaseuserbooks", null, new { UserId = AppData.User.UserId, BookIds = this.id, DeviceId = Constant.DeviceIdFromDB }, null);
                //    }
                //    else
                //    {
                //        ActionTabPage.IsOfflineTextLabel.IsVisible = true;
                //    }
                //}
                //catch (Exception ex)
                //{
                //}
            };
            BookPagePopUp.SubmitPopupBtn.GestureRecognizers.Add(okPopuptap);

            var cancelPopuptap = new TapGestureRecognizer();
            cancelPopuptap.Tapped += (s, e) =>
            {
                Constant.UserActiveTime = DateTime.Now;
                BookPagePopUp.IsVisible = false;
                BookPagePopUp.TextMessage.Text = string.Empty;
                BookPagePopUp.SubmitTextMessage.Text = string.Empty;
                BookPagePopUp.CancelTextMessage.Text = string.Empty;
            };
            BookPagePopUp.CancelPopupBtn.GestureRecognizers.Add(cancelPopuptap);


            this.id = id;
            this.isForDownload = isForDownload;
            this.ReadFrom = ReadFrom;

            if (AppData.BooksDetail != null)
            {
                this._Book = AppData.BooksDetail.Books.Book.Where(x => x.BookId.Equals(this.id)).FirstOrDefault();
            }

            if (AppData.UserDetails != null)
            {
                this._UserBook = AppData.UserDetails.UserBooks.UserBook.Where(x => x.BookId.Equals(this.id)).FirstOrDefault();
            }

            paintBookDetail();

            checkBookProgress();

            Task.Run(() =>
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    ActionTabPage.page_Loader.IsVisible = false;
                });
            });
        }

        public void BackBtn_Clicked(object sender, EventArgs e)
        {
            Constant.UserActiveTime = DateTime.Now;

            this.Page_Loader.IsVisible = true;
            Task.Run(() =>
            {
                Task.Delay(100).Wait();
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        Utils.UpdateUserBookStatus();
                        if (HelperFunctions.CheckInternetConnection())
                        {
                            Utils.UpdateUserDetails(Utils.UpdateMethod.Books);
                        }

                        ActionTabPage.CheckInternetConnectivity();
                        isBookOpended = false;
                        ActionTabPage tab = (ActionTabPage)Application.Current.MainPage.Navigation.NavigationStack.ElementAt(0);
                        tab.CheckIfBookOffline();
                        tab.ShowCount();
                        ActionTabPage.currentGrade = ThemeChanger.SubSection.ToString();
                        tab.paintSubSection();
                        if (this.ReadFrom == 1)
                        {
                            UserDashboard user_dashboard = (UserDashboard)ActionTabPage.tabContent;
                            if (user_dashboard != null)
                            {
                                user_dashboard.DisplayBooksAgain();
                            }
                        }
                        else if (this.ReadFrom == 2)
                        {
                            HomePage homePage = (HomePage)ActionTabPage.tabContent;
                            if (homePage != null)
                            {
                                homePage.currentGrade = ActionTabPage.currentGrade;
                                homePage.RefreshContent();
                            }
                        }
                        Navigation.RemovePage(this);
                    }
                    catch (Exception ex)
                    {

                    }
                });
            });
        }

        private void closeBtn1_Clicked(object sender, EventArgs e)
        {
            Constant.UserActiveTime = DateTime.Now;
            PopWindow.IsVisible = false;
            PopArrowImg.IsVisible = false;
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Constant.UserActiveTime = DateTime.Now;

            this.Page_Loader.IsVisible = true;

            Task.Run(() =>
            {
                Task.Delay(100).Wait();
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        Utils.UpdateUserBookStatus();
                        if (HelperFunctions.CheckInternetConnection())
                        {
                            Utils.UpdateUserDetails(Utils.UpdateMethod.Books);
                        }

                        ActionTabPage.CheckInternetConnectivity();
                        isBookOpended = false;
                        //if (ReadFrom == 0)
                        //{
                        //    return false;
                        //}
                        ActionTabPage tab = (ActionTabPage)Application.Current.MainPage.Navigation.NavigationStack.ElementAt(0);
                        tab.CheckIfBookOffline();
                        tab.ShowCount();
                        ActionTabPage.currentGrade = ThemeChanger.SubSection.ToString();
                        tab.paintSubSection();
                        if (this.ReadFrom == 1)
                        {
                            UserDashboard user_dashboard = (UserDashboard)ActionTabPage.tabContent;
                            if (user_dashboard != null)
                            {
                                user_dashboard.DisplayBooksAgain();
                            }
                        }
                        else if (this.ReadFrom == 2)
                        {
                            HomePage homePage = (HomePage)ActionTabPage.tabContent;
                            if (homePage != null)
                            {
                                homePage.currentGrade = ActionTabPage.currentGrade;
                                homePage.RefreshContent();
                            }
                        }
                        //Navigation.PushAsync(new ActionTabPage(this.ReadFrom));
                        Navigation.RemovePage(this);
                    }
                    catch (Exception ex)
                    {

                    }
                });
            });
            //return true;
            return true;
        }

        public async void paintBookDetail()
        {
            try
            {
                if (this._Book.Search != null)
                {
                    bookTitle_Android.Text = string.IsNullOrEmpty(this._Book.Search.Title?.Trim()) ? "" : this._Book.Search.Title?.Trim().ToUpper();
                    bookTitle.Text = string.IsNullOrEmpty(this._Book.Search.Title?.Trim()) ? "" : this._Book.Search.Title?.Trim().ToUpper();
                    author_Name.Text = string.IsNullOrEmpty(this._Book.Search.Author?.Trim()) ? "" : this._Book.Search.Author?.Trim().ToUpper();
                    illustrator_Name.Text = string.IsNullOrEmpty(this._Book.Search.Illustrator?.Trim()) ? "" : this._Book.Search.Illustrator?.Trim().ToUpper();
                    translator_Name.Text = string.IsNullOrEmpty(this._Book.Search.Translator?.Trim()) ? "" : this._Book.Search.Translator?.Trim().ToUpper();
                    publisher_Name.Text = string.IsNullOrEmpty(this._Book.Search.Publisher?.Trim()) ? "" : this._Book.Search.Publisher?.Trim().ToUpper();
                }

                if (string.IsNullOrEmpty(author_Name.Text))
                {
                    ((Grid)author_Name.Parent).IsVisible = false;
                }
                if (string.IsNullOrEmpty(illustrator_Name.Text))
                {
                    ((Grid)illustrator_Name.Parent).IsVisible = false;
                }
                if (string.IsNullOrEmpty(translator_Name.Text))
                {
                    ((Grid)translator_Name.Parent).IsVisible = false;
                }
                if (string.IsNullOrEmpty(publisher_Name.Text))
                {
                    ((Grid)publisher_Name.Parent).IsVisible = false;
                }
                foreach (View layout in bookDetailSection.Children)
                {
                    if (layout is Grid)
                    {
                        if (layout.IsVisible)
                        {
                            UpperBox.IsVisible = true;
                            LowerBox.IsVisible = true;
                            break;
                        }
                    }
                }

                subSection = string.IsNullOrEmpty(this._Book.Search.SubSection?.Trim()) ? "" : this._Book.Search.SubSection?.Trim().ToUpper();
                subSectionId = string.IsNullOrEmpty(this._Book.SubSections?.Trim()) ? 0 : Int32.Parse(this._Book.SubSections?.Trim());
                string description = string.IsNullOrEmpty(this._Book.Search.ShortDescription?.Trim()) ? "" : this._Book.Search.ShortDescription?.Trim();
                description = Regex.Replace(description, @"\\r\\n?|\n", "\n");
                book_description.Text = description;

                IsComingSoon = string.IsNullOrEmpty(this._Book.ComingSoon?.Trim()) ? false : Boolean.Parse(this._Book.ComingSoon?.Trim());

                if (this._UserBook != null)
                {
                    isRead = Boolean.Parse(string.IsNullOrEmpty(this._UserBook.IsRead?.Trim()) ? "false" : this._UserBook.IsRead?.Trim());
                    isReadLater = Boolean.Parse(string.IsNullOrEmpty(this._UserBook.IsReadLater?.Trim()) ? "false" : this._UserBook.IsReadLater?.Trim());
                    currentPage = Int32.Parse(string.IsNullOrEmpty(this._UserBook.Bookmark.CurrentPage?.Trim()) ? "0" : this._UserBook.Bookmark.CurrentPage?.Trim());
                }
                else
                {
                    isRead = false;
                    isReadLater = false;
                    currentPage = 0;
                }
                isReadAloud = string.IsNullOrEmpty(this._Book.HasReadAloud?.Trim()) ? false : Boolean.Parse(this._Book.HasReadAloud?.Trim());
                isHasActivity = string.IsNullOrEmpty(this._Book.HasActivity?.Trim()) ? false : Boolean.Parse(this._Book.HasActivity?.Trim());
                bookCover = string.IsNullOrEmpty(this._Book.Thumbnail3?.Trim()) ? "" : this._Book.Thumbnail3?.Trim();
                isPagerAllowed = string.IsNullOrEmpty(this._Book.IsPagerAllowed?.Trim()) ? true : Boolean.Parse(this._Book.IsPagerAllowed?.Trim());

                if (!string.IsNullOrEmpty(this._Book.PageDisplay?.Trim()))
                {
                    PageDisplay = this._Book.PageDisplay.Trim().ToLower().Equals("single") ? 1 : 2;
                }
                if (!string.IsNullOrEmpty(this._Book.ViewMode?.Trim()))
                {
                    IsLandscape = this._Book.ViewMode.Trim().ToLower().Equals("landscape");
                }

                if (Utils.IsBookDownloaded(this.id))
                {
                    isOfflineReadAllowed = true;
                }
                else
                {
                    isOfflineReadAllowed = false;
                }

                if (isOfflineReadAllowed)
                {
                    releaseBook.IsVisible = true;
                    this.EPUB_Name = Constant.FileNameInitials + this.id;
                }

                if (!isHasActivity)
                {
                    activityBigBtn.IsVisible = false;
                    // ActivitySmall.IsVisible = false;
                    ActivitySmall.Opacity = 0;
                }

                if (!isReadAloud)
                {
                    ReadAloudSmall.Opacity = 0;
                    // ReadAloudSmall.IsVisible = false;
                }
                if (string.IsNullOrEmpty(this._Book.HasAnimation?.Trim()) ? true : !Boolean.Parse(this._Book.HasAnimation?.Trim()))
                {
                    AnimationSmall.Opacity = 0;
                    //   AnimationSmall.IsVisible = false;
                }
                if (isRead)
                {
                    ((AbsoluteLayout)ratingBigBtn.Parent).Opacity = 1;
                    if (!string.IsNullOrEmpty(this._Book.HasActivity?.Trim()))
                    {
                        if (isOfflineReadAllowed)
                            activityBigBtn.Opacity = 1;
                        if (Boolean.Parse(this._UserBook.IsActivityDone))
                        {
                            activityBigBtn.SetDynamicResource(Image.SourceProperty, "ActivityBigCompletedImage");
                            ActivitySmall.SetDynamicResource(Image.SourceProperty, "ActivityGrayCompletedImage");
                        }
                        else
                        {
                            activityBigBtn.SetDynamicResource(Image.SourceProperty, "ActivityBigIncompletedImage");
                        }
                    }
                    if (string.IsNullOrEmpty(this._UserBook.Rating))
                    {
                        bigRatingText.Text = string.Empty;
                        ratingBigBtn.SetDynamicResource(Image.SourceProperty, "RatingBigIncompletedImage");
                    }
                    else
                    {
                        if (Int32.Parse(this._UserBook.Rating?.Trim()) != 0)
                        {
                            bigRatingText.Text = this._UserBook.Rating?.Trim();
                            ratingBigBtn.SetDynamicResource(Image.SourceProperty, "RatingBigCompletedImage");
                        }
                        else
                        {
                            bigRatingText.Text = string.Empty;
                            ratingBigBtn.SetDynamicResource(Image.SourceProperty, "RatingBigIncompletedImage");
                        }
                    }
                }
                else
                {
                    ratingBigBtn.SetDynamicResource(Image.SourceProperty, "RatingBigIncompletedImage");
                    activityBigBtn.SetDynamicResource(Image.SourceProperty, "ActivityBigIncompletedImage");
                }
                setRating();
                Label lbl = new Label();
                Image img = new Image();

                string classid = string.Empty;
                foreach (var item in readLaterBtn.Children)
                {
                    try
                    {
                        if (item is Label)
                        {
                            lbl = (Label)item;
                        }
                        else if (item is Image)
                        {
                            img = (Image)item;
                        }
                    }
                    catch (Exception ex) { }
                }
                //Full disable
                if (isRead || currentPage > 0)
                {
                    img.SetDynamicResource(Image.SourceProperty, "SelectedFilledButton");
                    lbl.SetDynamicResource(Label.StyleProperty, "LabelNormal312");
                    lbl.TextColor = Color.FromHex("#d5d5d5");
                    readLaterBtn.StyleId = "2";
                    //readLaterBtn.Opacity = 0; 
                    readLaterBtn.IsVisible = false;
                }
                else if (isReadLater) //Toggle
                {
                    img.SetDynamicResource(Image.SourceProperty, "SelectedFilledButton");
                    lbl.SetDynamicResource(Label.StyleProperty, "LabelNormal312");
                    lbl.TextColor = Color.FromHex("#d5d5d5");
                    readLaterBtn.StyleId = "1";
                }
                if (this.IsComingSoon)
                {
                    img.SetDynamicResource(Image.SourceProperty, "SelectedFilledButton");
                    lbl.SetDynamicResource(Label.StyleProperty, "LabelNormal312");
                    lbl.TextColor = Color.FromHex("#d5d5d5");
                    readLaterBtn.StyleId = "1";
                    releaseBook.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                //await DisplayAlert("Error", "Something went wrong!", "Ok");
            }

            if (!string.IsNullOrEmpty(bookCover))
            {
                NoBookThumbnail.WidthRequest = Constant.LargestBookWidth;
                DownloadedImage.WidthRequest = Constant.LargestBookWidth;
                if (this.IsLandscape)
                {
                    DownloadedImage.HeightRequest = Constant.LargestBookLandscapeHeight;
                    NoBookThumbnail.HeightRequest = Constant.LargestBookLandscapeHeight;
                }
                else
                {
                    if ((Application.Current.MainPage.Height - Constant.BookReadPadding) < Constant.MinimumHeightForThumbnail)
                    {
                        DownloadedImage.HeightRequest = Constant.LargestBookPortraitHeight_1024;
                        NoBookThumbnail.HeightRequest = Constant.LargestBookPortraitHeight_1024;
                        NoBookThumbnail.WidthRequest = Constant.LargestBookWidth_1024;
                        DownloadedImage.WidthRequest = Constant.LargestBookWidth_1024;
                    }
                    else
                    {
                        DownloadedImage.HeightRequest = Constant.LargestBookPortraitHeight;
                        NoBookThumbnail.HeightRequest = Constant.LargestBookPortraitHeight;
                    }
                }
                //  DownloadedImage.Source = bookCover;
                Task.Run(() =>
                {
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                    {
                        try
                        {
                            DownloadedImage.Source = new UriImageSource
                            {
                                Uri = new Uri(bookCover),
                                CachingEnabled = true,
                                CacheValidity = TimeSpan.MaxValue
                            };
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                    });
                });
                DownloadedImage.Aspect = Aspect.Fill;
                if (isOfflineReadAllowed)
                {
                    isOfflineBook.IsVisible = true;
                }
                var tapsetting = new TapGestureRecognizer();
                tapsetting.Tapped += (s, e) =>
                {
                    Constant.UserActiveTime = DateTime.Now;
                    PopWindow.IsVisible = false;
                    PopArrowImg.IsVisible = false;
                    if (!this.IsComingSoon)
                    {
                        this.checkBookStatus();
                    }
                };
                NoBookThumbnail.GestureRecognizers.Add(tapsetting);
            }
            else
            {
                NoBookThumbnail.WidthRequest = Constant.LargestBookWidth;
                if (this.IsLandscape)
                {
                    NoBookThumbnail.HeightRequest = Constant.LargestBookLandscapeHeight;
                }
                else
                {
                    if ((Application.Current.MainPage.Height - Constant.BookReadPadding) < Constant.MinimumHeightForThumbnail)
                    {
                        NoBookThumbnail.HeightRequest = Constant.LargestBookPortraitHeight_1024;
                        NoBookThumbnail.WidthRequest = Constant.LargestBookWidth_1024;
                    }
                    else
                    {
                        NoBookThumbnail.HeightRequest = Constant.LargestBookPortraitHeight;
                    }
                }
                var tapsetting = new TapGestureRecognizer();
                tapsetting.Tapped += (s, e) =>
                {
                    Constant.UserActiveTime = DateTime.Now;
                    PopWindow.IsVisible = false;
                    PopArrowImg.IsVisible = false;
                    if (!this.IsComingSoon)
                    {
                        this.checkBookStatus();
                    }
                };
                NoBookThumbnail.GestureRecognizers.Add(tapsetting);
            }
            switch (subSectionId)
            {
                case 1:
                    bookSubsectionImage.IsVisible = true;
                    bookSubsectionText.IsVisible = true;
                    App.Current.Resources["BookSubSectionLeft"] = "book_subsection_left_1.png";
                    await SubsectionText.RotateTo(-90, 0, Easing.Linear);
                    SubsectionText.Text = subSection;

                    bookborder.IsVisible = true;
                    bookborder.BackgroundColor = Color.FromHex("#FC654C");
                    break;
                case 2:
                    bookSubsectionImage.IsVisible = true;
                    bookSubsectionText.IsVisible = true;
                    App.Current.Resources["BookSubSectionLeft"] = "book_subsection_left_2.png";
                    await SubsectionText.RotateTo(-90, 0, Easing.Linear);
                    SubsectionText.Text = subSection;

                    bookborder.IsVisible = true;
                    bookborder.BackgroundColor = Color.FromHex("#9DA503");
                    break;
                case 3:
                    bookSubsectionImage.IsVisible = true;
                    bookSubsectionText.IsVisible = true;
                    App.Current.Resources["BookSubSectionLeft"] = "book_subsection_left_3.png";
                    await SubsectionText.RotateTo(-90, 0, Easing.Linear);
                    SubsectionText.Text = subSection;

                    bookborder.IsVisible = true;
                    bookborder.BackgroundColor = Color.FromHex("#14B4B4");
                    break;
                default:
                    bookSubsectionImage.IsVisible = true;
                    bookSubsectionText.IsVisible = true;
                    App.Current.Resources["BookSubSectionLeft"] = "book_subsection_left_3.png";
                    await SubsectionText.RotateTo(-90, 0, Easing.Linear);
                    SubsectionText.Text = subSection;

                    bookborder.IsVisible = true;
                    bookborder.BackgroundColor = Color.FromHex("#14B4B4");
                    break;
            }
        }

        public void checkBookStatus()
        {
            bool isNewBook = false;
            UserBook UserBookElement = null;
            UserProgressBook UserProgressBookElement = null;
            if (AppData.UserDetails != null && AppData.UserProgress != null)
            {
                UserBookElement = AppData.UserDetails.UserBooks.UserBook.Where(x => x.BookId.Equals(this.id)).FirstOrDefault();
                UserProgressBookElement = AppData.UserProgress.UserProgressBooks.UserProgressBook.Where(x => x.BookId.Equals(this.id)).FirstOrDefault();
            }
            if (UserBookElement == null && UserProgressBookElement == null)
            {
                this.EPUB_Name = string.Empty;
            }
            if (UserBookElement == null || UserProgressBookElement == null)
            {
                if (UserBookElement == null)
                {
                    isNewBook = true;
                    AppData.UserDetails.UserBooks.UserBook.Add(Utils.AddNewBookElement(this.id, false.ToString(), new List<string>()));
                    UserBookElement = AppData.UserDetails.UserBooks.UserBook.Where(x => x.BookId.Equals(this.id)).FirstOrDefault();
                }
                if (UserProgressBookElement == null)
                {
                    Utils.AddUserSyncBook(this.id);
                    UserProgressBookElement = AppData.UserProgress.UserProgressBooks.UserProgressBook.Where(x => x.BookId.Equals(this.id)).FirstOrDefault();
                }
            }

            string isDownloadedOnLocal = string.Empty;

            if (!String.IsNullOrEmpty(this.EPUB_Name))
            {
                string filepath = Path.Combine(Constant.FileDirectoryStructure, Constant.FileNameInitials + this.id + Constant.FileExtension);
                if (AppData.FileService.CheckDirectoryExistence(filepath.Split('.')[0]))
                {
                    isDownloadedOnLocal = "OnLocal";
                    this.Page_Loader.IsVisible = true;
                    Task.Run(() =>
                    {
                        Task.Delay(100).Wait();
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                        {
                            Navigation.PushAsync(new BookDisplay(this.id, this.EPUB_Name, this.isReadAloud, this.IsLandscape, this.ReadFrom, this.PageDisplay, this.isPagerAllowed));
                            Navigation.RemovePage(this);
                        });
                    });
                }
                else
                {
                    isDownloadedOnLocal = "NotOnLocal";
                    this.EPUB_Name = string.Empty;
                    try
                    {
                        string _device = UserBookElement.Devices.DeviceId.Where(x => x.Trim().Equals(Constant.DeviceIdFromDB.ToString()))?.FirstOrDefault();
                        if (_device != null)
                        {
                            UserBookElement.Devices.DeviceId.Remove(_device);
                        }
                    }
                    catch (Exception e)
                    {
                    }
                    releaseBook.IsVisible = false;
                    isOfflineBook.IsVisible = false;

                    DownloadFile Download_File = new DownloadFile
                    {
                        BookID = this.id,
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
                }
            }
            else
            {
                isDownloadedOnLocal = "NotOnLocal";
            }
            if (isDownloadedOnLocal.Equals("NotOnLocal"))
            {
                Task.Run(async () =>
                {
                    await Task.Delay(0);

                    foreach (BookProgress book in BookRead.bookfiles)
                    {
                        if (book.BookID.Contains(this.id))
                        {
                            return;
                        }
                    }

                    //Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                    //    ProgressLoader.IsVisible = true;
                    //});

                    DownloadFile Download_File = null;
                    if (AppData.BooksStatus != null)
                    {
                        Download_File = AppData.BooksStatus.DownloadFile.Where(x => x.BookID.Equals(this.id)).FirstOrDefault();
                    }

                    string filepath = Path.Combine(Constant.FileDirectoryStructure, Constant.FileNameInitials + this.id + Constant.FileExtension);

                    int Status = 0;
                    if (Download_File != null)
                    {
                        Status = Utils.GetBookStatus(Download_File);
                        if (Status == 2 || Status == 3)
                        {
                            Download_File.IsDecrypted = false.ToString();
                            Download_File.IsEncrypted = false.ToString();
                            Status = 1;
                        }
                    }
                    else
                    {
                        Download_File = new DownloadFile
                        {
                            BookID = this.id,
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
                    }

                    try
                    {
                        BookProgress _bookProgress = new BookProgress
                        {
                            BookID = this.id,
                            ProgressLoader = new ActivityIndicatorLoader
                            {
                                HorizontalOptions = LayoutOptions.Fill,
                                VerticalOptions = LayoutOptions.Fill,
                                Margin = new Thickness(36, 0, 0, 0),
                                IsVisible = true
                            }
                        };

                        BookRead.bookfiles.Add(_bookProgress);

                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                        {
                            BookDetailArea.Children.Add(_bookProgress.ProgressLoader, 0, 0);
                        });

                        if (isNewBook)
                        {
                            if (Status == 4)
                            {
                                if (!AppData.FileService.CheckDirectoryExistence(filepath.Split('.')[0]))
                                {
                                    Status = 0;
                                }
                            }
                        }
                        switch (Status)
                        {
                            case 0:
                                Download_File.IsProcessing = true.ToString();
                                if (AppData.FileService.CheckDirectoryExistence(filepath.Split('.')[0]))
                                {
                                    AppData.FileService.DeleteDirectory(filepath.Split('.')[0]);
                                }
                                if (!AppData.FileService.CheckDirectoryExistence(filepath.Split('.')[0]))
                                {
                                    if (!HelperFunctions.CheckInternetConnection())
                                    {
                                        //ActionTabPage.IsOfflineTextLabel.IsVisible = true;
                                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                                        {
                                            BookPagePopUp.TextMessage.Text = "Cannot connect to the Server. Check your Internet Connection.";
                                            BookPagePopUp.SubmitPopupBtn.IsVisible = false;
                                            BookPagePopUp.CancelTextMessage.Text = "OK";
                                            BookPagePopUp.IsVisible = true;
                                        });
                                    }
                                    else
                                    {
                                        //ActionTabPage.IsOfflineTextLabel.IsVisible = false;
                                        using (var client = new HttpClient())
                                        {
                                            client.BaseAddress = new Uri(Constant.baseUri);
                                            if (AppData.User != null && AppData.User.UserId != 0)
                                            {
                                                client.DefaultRequestHeaders.Add("CurrentUserId", AppData.User.UserId.ToString());
                                            }
                                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                            string address = Constant.baseUri + Constant.relativePath + "getbookdownloadurl?BookId=" + this.id;
                                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("amx", ComputeHash._ComputeHash.GetHashCode(null, address, "GET"));
                                            HttpResponseMessage response = await client.GetAsync(Constant.relativePath + "getbookdownloadurl?BookId=" + this.id);
                                            //HttpResponseMessage response = await client.GetAsync(@"https://kitaablet.blob.core.windows.net/kitaabletcn/temp/manifest.xml");
                                            if (response.IsSuccessStatusCode)
                                            {
                                                using (HttpContent requestResponse = response.Content)
                                                {
                                                    string result = await requestResponse.ReadAsStringAsync();
                                                    if (result != null && !result.ToLower().Equals("null"))
                                                    {
                                                        //------------------------------------------------------------

                                                        //XmlSerializer serializer = new XmlSerializer(typeof(DownloadFile));
                                                        //DownloadFile file = (DownloadFile)serializer.Deserialize(new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(result))));
                                                        //if (file != null)
                                                        //{
                                                        //    if (Utils.StartDownloading(file))
                                                        //    { 
                                                        //    }
                                                        //}

                                                        //------------------------------------------------------------
                                                        result = result.Substring(1, result.Length - 2);

                                                        ActivityIndicatorLoader progress = new ActivityIndicatorLoader();
                                                        progress.ProgressControl.PropertyChanging += (sender, e) =>
                                                        {
                                                            CircularProgressControl cir_progress = (CircularProgressControl)sender;
                                                            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                                                            {
                                                                _bookProgress.ProgressLoader.ProgressValue.Text = ((int)(cir_progress.Progress * 90)).ToString() + "%";
                                                                _bookProgress.ProgressLoader.ProgressControl.Progress = ((cir_progress.Progress * 90 / 100));
                                                            });
                                                        };

                                                        bool isSaved = false;
                                                        using (MemoryStream data_response = Utils.DownloadFileAsync(result, progress, (new CancellationTokenSource()).Token))
                                                        {
                                                            if (data_response != null)
                                                            {
                                                                if (AppData.FileService.SaveBookDownload(this.id, Constant.FileNameInitials + this.id + Constant.FileExtension, data_response))
                                                                {
                                                                    isSaved = true;
                                                                }
                                                            }
                                                        }
                                                        if (isSaved)
                                                        {
                                                            Download_File.IsDownloaded = true.ToString();
                                                            Utils.SetBookStatus(Download_File);
                                                            goto case 1;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                goto default;
                            case 1:
                                if (Boolean.Parse(Download_File.IsDownloaded))
                                {
                                    if (AppData.FileService.FileUnzip(filepath))
                                    {
                                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                                        {
                                            _bookProgress.ProgressLoader.ProgressValue.Text = (95).ToString() + "%";
                                            _bookProgress.ProgressLoader.ProgressControl.Progress = 0.95;
                                        });
                                        Download_File.IsUnZip = true.ToString();
                                        Utils.SetBookStatus(Download_File);
                                        goto case 2;
                                    }
                                }
                                goto default;
                            case 2:
                                if (Boolean.Parse(Download_File.IsUnZip))
                                {
                                    if (AppData.FileService.BookXmlFilesOperation(filepath.Split('.')[0], "decrypt", Constant.DecryptionKey))
                                    {
                                        //try
                                        //{
                                        //    if (this.isHasActivity)
                                        //    {
                                        //        AppData.FileService.DecryptFile(Path.Combine(AppData.FileService.GetLocalLocalFolderPath(), filepath.Split('.')[0], "BookActivity", "ActivityData.txt"), Constant.DecryptionKey);
                                        //    }
                                        //}
                                        //catch (Exception ex)
                                        //{

                                        //}

                                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                                        {
                                            _bookProgress.ProgressLoader.ProgressValue.Text = (98).ToString() + "%";
                                            _bookProgress.ProgressLoader.ProgressControl.Progress = 0.98;
                                        });
                                        Download_File.IsDecrypted = true.ToString();
                                        Utils.SetBookStatus(Download_File);
                                        goto case 3;
                                    }
                                }
                                goto default;
                            case 3:
                                if (Boolean.Parse(Download_File.IsDecrypted))
                                {
                                    if (AppData.FileService.BookXmlFilesOperation(filepath.Split('.')[0], "encrypt", Constant.UserCryptoKey))
                                    {
                                        //try
                                        //{
                                        //    if (this.isHasActivity)
                                        //    {
                                        //        AppData.FileService.EncryptFile(Path.Combine(AppData.FileService.GetLocalLocalFolderPath(), filepath.Split('.')[0], "BookActivity", "ActivityData.txt"), Constant.UserCryptoKey);
                                        //    }
                                        //}
                                        //catch (Exception ex)
                                        //{

                                        //}

                                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                                        {
                                            _bookProgress.ProgressLoader.ProgressValue.Text = (100).ToString() + "%";
                                            _bookProgress.ProgressLoader.ProgressControl.Progress = 1.00;
                                        });
                                        Download_File.IsEncrypted = true.ToString();
                                        Download_File.IsProcessing = false.ToString();

                                        Utils.SetBookStatus(Download_File);
                                        AppData.FileService.DeleteFile(filepath);
                                        goto default;
                                    }
                                }
                                goto default;
                            default:
                                BookProgress _book = BookRead.bookfiles.Where(x => x.BookID.Equals(this.id)).FirstOrDefault();
                                if (_book != null)
                                {
                                    BookRead.bookfiles.Remove(_book);
                                }

                                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                                {
                                    if (BookDetailArea.Children[BookDetailArea.Children.Count - 1] is ActivityIndicatorLoader)
                                    {
                                        BookDetailArea.Children.RemoveAt(BookDetailArea.Children.Count - 1);
                                    }
                                });
                                if (!Boolean.Parse(Download_File.IsProcessing))
                                {
                                    if (HelperFunctions.CheckInternetConnection())
                                    {
                                        string status = MyWebRequest.PostRequest("userdownloadbook", null, new { UserId = AppData.User.UserId, BookId = this.id, DeviceId = Constant.DeviceIdFromDB }, null);
                                    }
                                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                                    {
                                        //_bookProgress.ProgressLoader.IsVisible = false;
                                        if (UserBookElement != null && UserProgressBookElement != null)
                                        {
                                            this.EPUB_Name = Constant.FileNameInitials + this.id;
                                            if (UserBookElement.Devices.DeviceId == null)
                                            {
                                                UserBookElement.Devices.DeviceId = new List<string>();
                                            }
                                            if (!UserBookElement.Devices.DeviceId.Contains(Constant.DeviceIdFromDB.ToString()))
                                            {
                                                UserBookElement.Devices.DeviceId.Add(Constant.DeviceIdFromDB.ToString());
                                            }
                                            isOfflineReadAllowed = true;
                                            isOfflineBook.IsVisible = true;
                                            releaseBook.IsVisible = true;
                                        }
                                        if (Status != 4)
                                        {
                                            XFToast.ShortMessage("Hurray - I downloaded the book for you!");
                                        }
                                        if (!String.IsNullOrEmpty(this.EPUB_Name))
                                        {
                                            if (Application.Current.MainPage.Navigation.NavigationStack.ElementAt(Application.Current.MainPage.Navigation.NavigationStack.Count - 1) is BookRead)
                                            {
                                                BookRead temp = Application.Current.MainPage.Navigation.NavigationStack.ElementAt(Application.Current.MainPage.Navigation.NavigationStack.Count - 1) as BookRead;
                                                if (temp == this)
                                                {
                                                    Application.Current.MainPage.Navigation.PushAsync(new BookDisplay(this.id, this.EPUB_Name, this.isReadAloud, this.IsLandscape, this.ReadFrom, this.PageDisplay, this.isPagerAllowed));
                                                    Application.Current.MainPage.Navigation.RemovePage(this);
                                                }
                                            }
                                        }
                                    });
                                }
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        BookProgress _book = BookRead.bookfiles.Where(x => x.BookID.Equals(this.id)).FirstOrDefault();
                        if (_book != null)
                        {
                            BookRead.bookfiles.Remove(_book);
                        }

                        DownloadFile Temp_File = new DownloadFile
                        {
                            BookID = this.id,
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
                        Utils.SetBookStatus(Temp_File);

                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                        {
                            this.EPUB_Name = string.Empty;
                            try
                            {
                                string _device = UserBookElement.Devices.DeviceId.Where(x => x.Trim().Equals(Constant.DeviceIdFromDB.ToString()))?.FirstOrDefault();
                                if (_device != null)
                                {
                                    UserBookElement.Devices.DeviceId.Remove(_device);
                                }
                            }
                            catch (Exception e)
                            {
                            }
                            releaseBook.IsVisible = false;
                            isOfflineBook.IsVisible = false;

                            if (BookDetailArea.Children[BookDetailArea.Children.Count - 1] is ActivityIndicatorLoader)
                            {
                                BookDetailArea.Children.RemoveAt(BookDetailArea.Children.Count - 1);
                            }
                        });
                    }
                });
            }
        }

        public void checkBookProgress()
        {
            try
            {
                BookProgress _book = BookRead.bookfiles.Where(x => x.BookID.Equals(this.id)).FirstOrDefault();
                if (_book != null)
                {
                    BookDetailArea.Children.Add(_book.ProgressLoader, 0, 0);
                    _book.ProgressLoader.ProgressControl.PropertyChanged += ProgressControl_PropertyChanged;
                }
                else if (this.isForDownload)
                {
                    if (!this.IsComingSoon)
                    {
                        this.checkBookStatus();
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void ProgressControl_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            CircularProgressControl cir_progress = (CircularProgressControl)sender;
            if ((int)cir_progress.Progress >= 1)
            {
                BookDetailArea.Children[BookDetailArea.Children.Count - 1].IsVisible = false;
                isOfflineBook.IsVisible = true;
                releaseBook.IsVisible = true;
            }
        }

        public void releaseBtn_Clicked(object sender, EventArgs e)
        {
            Constant.UserActiveTime = DateTime.Now;
            PopWindow.IsVisible = false;
            PopArrowImg.IsVisible = false;
            //BookPagePopUp.TextMessage.Text = "Are you sure to release this book from this device?";
            //BookPagePopUp.SubmitPopupBtn.IsVisible = true;
            //BookPagePopUp.CancelTextMessage.Text = "NO";
            //BookPagePopUp.SubmitTextMessage.Text = "YES";
            //BookPagePopUp.IsVisible = true;
            try
            {
                if (this._UserBook != null)
                {
                    string _device = this._UserBook.Devices.DeviceId.Where(x => x.Trim().Equals(Constant.DeviceIdFromDB.ToString()))?.FirstOrDefault();
                    if (_device != null)
                    {
                        this._UserBook.Devices.DeviceId.Remove(_device);
                    }
                }
                releaseBook.IsVisible = false;
                isOfflineBook.IsVisible = false;
                this.isOfflineReadAllowed = false;
                activityBigBtn.Opacity = 0.5;
                this.EPUB_Name = string.Empty;

                DownloadFile Download_File = new DownloadFile
                {
                    BookID = this.id,
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

                if (HelperFunctions.CheckInternetConnection())
                {
                    ActionTabPage.IsOfflineTextLabel.IsVisible = false;
                    string status = MyWebRequest.PostRequest("releaseuserbooks", null, new { UserId = AppData.User.UserId, BookIds = this.id, DeviceId = Constant.DeviceIdFromDB }, null);
                }
                else
                {
                    ActionTabPage.IsOfflineTextLabel.IsVisible = true;
                }

                if (AppData.FileService.CheckDirectoryExistence(Path.Combine(Constant.FileDirectoryStructure, Constant.FileNameInitials + this.id)))
                {
                    AppData.FileService.DeleteDirectory(Path.Combine(Constant.FileDirectoryStructure, Constant.FileNameInitials + this.id));
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void readLaterBtn_Clicked(object sender, EventArgs e)
        {
            Constant.UserActiveTime = DateTime.Now;
            PopWindow.IsVisible = false;
            PopArrowImg.IsVisible = false;
            Grid gridbtn = (Grid)sender;
            Label lbl = new Label();
            Image img = new Image();

            string classid = string.Empty;
            foreach (var item in gridbtn.Children)
            {
                try
                {
                    if (item is Label)
                    {
                        lbl = (Label)item;
                    }
                    else if (item is Image)
                    {
                        img = (Image)item;
                    }
                }
                catch (Exception ex) { }
            }
            if (readLaterBtn.StyleId == "0")
            {
                UserBook UserBookElement = null;
                UserProgressBook UserProgressElement = null;
                if (AppData.UserDetails != null && AppData.UserProgress != null)
                {
                    UserBookElement = AppData.UserDetails.UserBooks.UserBook.Where(x => x.BookId.Equals(this.id)).FirstOrDefault();
                    UserProgressElement = AppData.UserProgress.UserProgressBooks.UserProgressBook.Where(x => x.BookId.Equals(this.id)).FirstOrDefault();
                }
                AppData.UserDetails.LastReadLaterBookId = this.id;
                string readlater_date = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture);
                if (UserBookElement == null)
                {
                    AppData.UserDetails.UserBooks.UserBook.Add(Utils.AddNewBookElement(this.id, true.ToString(), new List<string>()));
                }
                else
                {
                    UserBookElement.IsReadLater = "true";
                    UserBookElement.ReadLaterOn = readlater_date;
                }
                if (UserProgressElement == null)
                {
                    Utils.AddUserSyncBook(this.id);
                }
                else
                {
                    UserProgressElement.IsReadLater = "true";
                    UserProgressElement.ReadLaterOn = readlater_date;
                }
                //readLaterBtn.Opacity = 0.5;
                img.SetDynamicResource(Image.SourceProperty, "SelectedFilledButton");
                lbl.SetDynamicResource(Label.StyleProperty, "LabelNormal312");
                lbl.TextColor = Color.FromHex("#d5d5d5");
                readLaterBtn.StyleId = "1";
            }
            //else if (!isRead && !(currentPage > 0))
            else if (readLaterBtn.StyleId == "1")
            {
                UserBook UserBookElement = null;
                UserProgressBook UserProgressElement = null;
                if (AppData.UserDetails != null && AppData.UserProgress != null)
                {
                    UserBookElement = AppData.UserDetails.UserBooks.UserBook.Where(x => x.BookId.Equals(this.id)).FirstOrDefault();
                    UserProgressElement = AppData.UserProgress.UserProgressBooks.UserProgressBook.Where(x => x.BookId.Equals(this.id)).FirstOrDefault();
                }
                AppData.UserDetails.LastReadLaterBookId = string.Empty;
                if (UserBookElement == null)
                {
                    AppData.UserDetails.UserBooks.UserBook.Add(Utils.AddNewBookElement(this.id, false.ToString(), new List<string>()));
                }
                else
                {
                    UserBookElement.IsReadLater = "false";
                    UserBookElement.ReadLaterOn = string.Empty;
                }
                if (UserProgressElement == null)
                {
                    Utils.AddUserSyncBook(this.id);
                }
                else
                {
                    UserProgressElement.IsReadLater = "false";
                    UserProgressElement.ReadLaterOn = string.Empty;
                }
                //readLaterBtn.Opacity = 1;
                readLaterBtn.IsVisible = true;
                img.SetDynamicResource(Image.SourceProperty, "RoundedTransparentButton");
                lbl.SetDynamicResource(Label.StyleProperty, "LabelNormal3");
                lbl.SetDynamicResource(Label.TextColorProperty, "PrimaryColor");
                readLaterBtn.StyleId = "0";
            }
        }

        private void setRating()
        {
            try
            {
                int avgRating = string.IsNullOrEmpty(this._Book.Rating.AverageRating?.Trim()) ? 0 : Int32.Parse(this._Book.Rating.AverageRating?.Trim());
                if (avgRating > 0)
                {
                    RatingSmall.SetDynamicResource(Image.SourceProperty, "RatingGrayCompletedImage");
                    AvgRatingofBook.Text = avgRating.ToString();
                }
                foreach (var starImage in AverageRating.Children)
                {
                    if (avgRating <= 0)
                    {
                        break;
                    }
                    ((Image)starImage).SetDynamicResource(Image.SourceProperty, "RatingGrayCompletedImage");
                    avgRating--;
                }
                long[] arrayOfCounts = {
                    string.IsNullOrEmpty(this._Book.Rating.FiveStarRating?.Trim()) ? 0 : Int32.Parse(this._Book.Rating.FiveStarRating?.Trim()),
                    string.IsNullOrEmpty(this._Book.Rating.FourStarRating?.Trim()) ? 0 : Int32.Parse(this._Book.Rating.FourStarRating?.Trim()),
                    string.IsNullOrEmpty(this._Book.Rating.ThreeStarRating?.Trim()) ? 0 : Int32.Parse(this._Book.Rating.ThreeStarRating?.Trim()),
                    string.IsNullOrEmpty(this._Book.Rating.TwoStarRating?.Trim()) ? 0 : Int32.Parse(this._Book.Rating.TwoStarRating?.Trim()),
                    string.IsNullOrEmpty(this._Book.Rating.OneStarRating?.Trim()) ? 0 : Int32.Parse(this._Book.Rating.OneStarRating?.Trim())
                };
                var total = arrayOfCounts.Sum() <= 0 ? 1 : arrayOfCounts.Sum();
                fiveStarRatingCount.Text = arrayOfCounts[0].ToString();
                fourStarRatingCount.Text = arrayOfCounts[1].ToString();
                threeStarRatingCount.Text = arrayOfCounts[2].ToString();
                twoStarRatingCount.Text = arrayOfCounts[3].ToString();
                oneStarRatingCount.Text = arrayOfCounts[4].ToString();
                fiveStarRating.Progress = (double)arrayOfCounts[0] / total;
                fourStarRating.Progress = (double)arrayOfCounts[1] / total;
                threeStarRating.Progress = (double)arrayOfCounts[2] / total;
                twoStarRating.Progress = (double)arrayOfCounts[3] / total;
                oneStarRating.Progress = (double)arrayOfCounts[4] / total;
            }
            catch (Exception e)
            { }
        }

    }
}
