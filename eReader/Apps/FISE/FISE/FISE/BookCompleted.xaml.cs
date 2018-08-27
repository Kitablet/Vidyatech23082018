using Kitablet.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Kitablet
{
    public partial class BookCompleted : ContentPage
    {
        public string id, currentStar, subSection;
        protected int ReadFrom, noOfQuestion, currentQuestion, userRating, subSectionId, noOfCharacter, maxWordCount, wordCountForLabel = 800;
        protected bool isCompleted, hasActivity, isOfflineReadAllowed, isRatingGiven, isForDownload, isForActivity;
        protected UserBookReview bookReview;
        protected Question question;
        protected List<Option> options;
        protected List<StackLayout> layouts;
        public DateTime StartTime;
        protected double editorWidth, editorHeight;
        protected UserBook _UserBook;
        protected Book _Book;
        private bool ShowLimitExceeded = false;
        public BookCompleted(string id, int ReadFrom)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

            Utils.AddUserSyncBook(id);
            //Uncoment for Android & IOS thease 2 lines 
            //this.BookCompletedLayout.WidthRequest = Xamarin.Forms.Device.OnPlatform<double>(Constant.DeviceWidth, Constant.DeviceWidth / Constant.DeviceDensity, Constant.DeviceWidth);
            //this.BookCompletedLayout.HeightRequest = Xamarin.Forms.Device.OnPlatform<double>(Constant.DeviceHeight, (Constant.DeviceHeight / Constant.DeviceDensity) - 25, Constant.DeviceHeight);

            this.editorWidth = Xamarin.Forms.Device.OnPlatform<double>((this.BookCompletedLayout.WidthRequest / 2) - 45, (this.BookCompletedLayout.WidthRequest / 2) - 45, (Constant.DeviceWidth / 2) - 45);
            this.editorHeight = 40;
            if (Xamarin.Forms.Device.OS == TargetPlatform.Android)
            {
                this.noOfCharacter = (int)(this.editorWidth / 9);
            }
            else
            {
                this.noOfCharacter = (int)(this.editorWidth / 7);
            }            

            TapGestureRecognizer backTab = new TapGestureRecognizer();
            backTab.Tapped += Back_Clicked;
            BackReview.GestureRecognizers.Add(backTab);

            TapGestureRecognizer nextTab = new TapGestureRecognizer();
            nextTab.Tapped += Next_Clicked;
            NextReview.GestureRecognizers.Add(nextTab);

            TapGestureRecognizer doneTab = new TapGestureRecognizer();
            doneTab.Tapped += Done_Clicked;
            DoneReview.GestureRecognizers.Add(doneTab);

            var releaseBookTab = new TapGestureRecognizer();
            releaseBookTab.Tapped += releaseBtn_Clicked;
            releaseBook.GestureRecognizers.Add(releaseBookTab);

            reviewPopup.SubmitTextMessage.IsVisible = true;
            reviewPopup.CancelTextMessage.IsVisible = true;

            reviewPopup.SubmitTextMessage.Text = "YES";
            reviewPopup.CancelTextMessage.Text = "NO";
            var reviewPopupOkTab = new TapGestureRecognizer();
            reviewPopupOkTab.Tapped += reviewPopupSubmitBtn_Clicked;
            reviewPopup.SubmitPopupBtn.GestureRecognizers.Add(reviewPopupOkTab);

            var reviewPopupCancelTab = new TapGestureRecognizer();
            reviewPopupCancelTab.Tapped += reviewPopupCancelBtn_Clicked;
            reviewPopup.CancelPopupBtn.GestureRecognizers.Add(reviewPopupCancelTab);

            if (Device.OS == TargetPlatform.Android)
            {
                AvgRatingofBook.Margin = new Thickness(1, 0, 1, 2);
            }
            else if (Device.OS == TargetPlatform.Windows)
            {
                AvgRatingofBook.Margin = new Thickness(1, 0, 1, 30);
            }
            if (Device.OS == TargetPlatform.iOS)
            {
                AvgRatingofBook.FontAttributes = FontAttributes.Bold;
            }
            this.id = id;
            this.ReadFrom = ReadFrom;
            //this.isReleaseBook = false;

            if (AppData.BooksDetail != null)
            {
                this._Book = AppData.BooksDetail.Books.Book.Where(x => x.BookId.Equals(this.id)).FirstOrDefault();
            }

            if (AppData.UserDetails != null)
            {
                this._UserBook = AppData.UserDetails.UserBooks.UserBook.Where(x => x.BookId.Equals(this.id)).FirstOrDefault();
            }

            this.GetAndSet();
        }

        public void GetAndSet()
        {
            if (this._Book != null && this._UserBook != null)
            {
                this.userRating = Int32.Parse(string.IsNullOrEmpty(this._UserBook.Rating) ? "0" : this._UserBook.Rating);

                if(this.userRating != 0)
                {
                    this.isRatingGiven = true;
                }
                else
                {
                    this.isRatingGiven = false;
                }

                this.isCompleted = Boolean.Parse(string.IsNullOrEmpty(this._UserBook.IsReviewDone?.Trim()) ? "false" : this._UserBook.IsReviewDone?.Trim());

                if (this.isCompleted)
                {
                    SkipReviewText.Text = "SKIP";
                }

                subSectionId = string.IsNullOrEmpty(this._Book.SubSections?.Trim()) ? 0 : Int32.Parse(this._Book.SubSections?.Trim());
                subSection = this._Book.Search.SubSection.ToUpper();

                if (this._Book.Rating != null && this._Book.Rating.AverageRating != null)
                {
                    int avgRating = string.IsNullOrEmpty(this._Book.Rating.AverageRating.Trim()) ? 0 : Int32.Parse(this._Book.Rating.AverageRating.Trim());
                    if (avgRating > 0)
                    {
                        RatingSmall.SetDynamicResource(Image.SourceProperty, "RatingGrayCompletedImage");
                        AvgRatingofBook.Text = avgRating.ToString();
                    }
                }                   

                if (Utils.IsBookDownloaded(this.id))
                {
                    isOfflineReadAllowed = true;
                }
                else
                {
                    isOfflineReadAllowed = false;
                }

                if (!isOfflineReadAllowed)
                {
                    this.releaseBook.IsVisible = false;                    
                }
                                       
                if (!string.IsNullOrEmpty(this._Book.Thumbnail3?.Trim()))
                {
                    if (!string.IsNullOrEmpty(this._Book.ViewMode))
                    {
                        NoBookThumbnailCover.WidthRequest = Constant.LargestBookWidth;
                        NoBookThumbnailBack.WidthRequest = Constant.LargestBookWidth;
                        DownloadedCoverImage.WidthRequest = Constant.LargestBookWidth;
                        DownloadedBackImage.WidthRequest = Constant.LargestBookWidth;
                        if (this._Book.ViewMode.ToLower().Equals("landscape"))
                        {
                            NoBookThumbnailCover.HeightRequest = Constant.LargestBookLandscapeHeight;
                            NoBookThumbnailBack.HeightRequest = Constant.LargestBookLandscapeHeight;
                            DownloadedCoverImage.HeightRequest = Constant.LargestBookLandscapeHeight;
                            DownloadedBackImage.HeightRequest = Constant.LargestBookLandscapeHeight;
                        }
                        else
                        {
                            if ((Application.Current.MainPage.Height - Constant.BookReadPadding) < Constant.MinimumHeightForThumbnail)
                            {
                                NoBookThumbnailCover.HeightRequest = Constant.LargestBookPortraitHeight_1024;
                                NoBookThumbnailBack.HeightRequest = Constant.LargestBookPortraitHeight_1024;
                                DownloadedCoverImage.HeightRequest = Constant.LargestBookPortraitHeight_1024;
                                DownloadedBackImage.HeightRequest = Constant.LargestBookPortraitHeight_1024;
                                NoBookThumbnailCover.WidthRequest = Constant.LargestBookWidth_1024;
                                NoBookThumbnailBack.WidthRequest = Constant.LargestBookWidth_1024;
                                DownloadedCoverImage.WidthRequest = Constant.LargestBookWidth_1024;
                                DownloadedBackImage.WidthRequest = Constant.LargestBookWidth_1024;
                            }
                            else
                            {
                                NoBookThumbnailCover.HeightRequest = Constant.LargestBookPortraitHeight;
                                NoBookThumbnailBack.HeightRequest = Constant.LargestBookPortraitHeight;
                                DownloadedCoverImage.HeightRequest = Constant.LargestBookPortraitHeight;
                                DownloadedBackImage.HeightRequest = Constant.LargestBookPortraitHeight;
                            }
                        }
                    }
                    DownloadedBackImage.Source = AppData.FileService.SetAndGetBackCover(this.id, Path.Combine(Constant.FileDirectoryStructure, Constant.FileNameInitials + this.id, "backcover.png").Replace("/", "\\"));
                    DownloadedBackImage.Aspect = Aspect.Fill;
                    DownloadedCoverImage.Source = new UriImageSource
                    {
                        Uri = new Uri(this._Book.Thumbnail3?.Trim()),
                        CacheValidity = TimeSpan.MaxValue,
                        CachingEnabled = true
                    };
                    DownloadedCoverImage.Aspect = Aspect.Fill;
                    DownloadedCoverImage.RotateYTo(-180, 0, Easing.Linear);

                }
                else
                {
                    if (!string.IsNullOrEmpty(this._Book.ViewMode))
                    {
                        NoBookThumbnailCover.WidthRequest = Constant.LargestBookWidth;
                        NoBookThumbnailBack.WidthRequest = Constant.LargestBookWidth;
                        if (this._Book.ViewMode.ToLower().Equals("landscape"))
                        {
                            NoBookThumbnailCover.HeightRequest = Constant.LargestBookLandscapeHeight;
                            NoBookThumbnailBack.HeightRequest = Constant.LargestBookLandscapeHeight;
                        }
                        else
                        {
                            if ((Application.Current.MainPage.Height - Constant.BookReadPadding) < Constant.MinimumHeightForThumbnail)
                            {
                                NoBookThumbnailCover.HeightRequest = Constant.LargestBookPortraitHeight_1024;
                                NoBookThumbnailBack.HeightRequest = Constant.LargestBookPortraitHeight_1024;
                                NoBookThumbnailCover.WidthRequest = Constant.LargestBookWidth_1024;
                                NoBookThumbnailBack.WidthRequest = Constant.LargestBookWidth_1024;
                            }
                            else
                            {
                                NoBookThumbnailCover.HeightRequest = Constant.LargestBookPortraitHeight;
                                NoBookThumbnailBack.HeightRequest = Constant.LargestBookPortraitHeight;
                            }
                        }
                    }                
                }
                FlipBookThumbnail();
                switch (subSectionId)
                {
                    case 1:
                        bookSubsectionImage.IsVisible = true;
                        bookSubsectionText.IsVisible = true;
                        App.Current.Resources["BookSubSectionLeft"] = "book_subsection_left_1.png";                            
                        SubsectionText.RotateTo(-90, 0, Easing.Linear);
                        SubsectionText.Text = subSection;

                        bookborder.IsVisible = true;
                        bookborder.BackgroundColor = Color.FromHex("#FC654C");
                        break;
                    case 2:
                        bookSubsectionImage.IsVisible = true;
                        bookSubsectionText.IsVisible = true;
                        App.Current.Resources["BookSubSectionLeft"] = "book_subsection_left_2.png";                         
                        SubsectionText.RotateTo(-90, 0, Easing.Linear);
                        SubsectionText.Text = subSection;

                        bookborder.IsVisible = true;
                        bookborder.BackgroundColor = Color.FromHex("#9DA503");
                        break;
                    case 3:
                        bookSubsectionImage.IsVisible = true;
                        bookSubsectionText.IsVisible = true;
                        App.Current.Resources["BookSubSectionLeft"] = "book_subsection_left_3.png";                           
                        SubsectionText.RotateTo(-90, 0, Easing.Linear);
                        SubsectionText.Text = subSection;

                        bookborder.IsVisible = true;
                        bookborder.BackgroundColor = Color.FromHex("#14B4B4");
                        break;
                    default:
                        bookSubsectionImage.IsVisible = true;
                        bookSubsectionText.IsVisible = true;
                        App.Current.Resources["BookSubSectionLeft"] = "book_subsection_left_3.png";
                        SubsectionText.RotateTo(-90, 0, Easing.Linear);
                        SubsectionText.Text = subSection;

                        bookborder.IsVisible = true;
                        bookborder.BackgroundColor = Color.FromHex("#14B4B4");
                        break;
                }
                this.hasActivity = string.IsNullOrEmpty(this._Book.HasActivity?.Trim()) ? false : Boolean.Parse(this._Book.HasActivity?.Trim());

                if (!this.hasActivity)
                {
                    //ActivitySmall.IsVisible = false;
                    ActivitySmall.Opacity = 0;
                }
                else
                {
                    if (Boolean.Parse(this._UserBook.IsActivityDone))
                    {
                        ActivitySmall.SetDynamicResource(Image.SourceProperty, "ActivityGrayCompletedImage");
                    }
                }

                if (string.IsNullOrEmpty(this._Book.HasReadAloud?.Trim()) ? true : !Boolean.Parse(this._Book.HasReadAloud?.Trim()))
                {
                   // ReadAloudSmall.IsVisible = false;
                    ReadAloudSmall.Opacity = 0;
                }
                if (string.IsNullOrEmpty(this._Book.HasAnimation?.Trim()) ? true : !Boolean.Parse(this._Book.HasAnimation?.Trim()))
                {
                    //AnimationSmall.IsVisible = false;
                    AnimationSmall.Opacity = 0;
                }
            }
        }

        public void FlipBookThumbnail()
        {
            try
            {
                Task.Run(async () =>
                {
                    await Task.Delay(0);
                    Task.Delay(2000).Wait();
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                    {
                        //book_cover.RotateYTo(0, 450, Easing.Linear);
                        //book_cover_back.RotateYTo(180, 450, Easing.Linear);
                        DownloadedCoverImage.RotateYTo(0, 450, Easing.Linear);
                        DownloadedBackImage.RotateYTo(180, 450, Easing.Linear);
                    });
                    Task.Delay(225).Wait();
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                    {
                        //book_cover_back.IsVisible = false;
                        NoBookThumbnailBack.IsVisible = false;
                    });
                    Task.Delay(225).Wait();
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                    {
                        if (isOfflineReadAllowed)
                        {
                            isOfflineBook.Opacity = 0;
                            isOfflineBook.IsVisible = true;
                            isOfflineBook.FadeTo(1, 450, Easing.SinIn);
                        }
                        InitailizeBookReview();
                    });
                });
            }
            catch(Exception ex)
            {

            }
        }

        public void releaseBtn_Clicked(object sender, EventArgs e)
        {
            Constant.UserActiveTime = DateTime.Now;
            //this.isReleaseBook = true;
            //reviewPopup.SubmitTextMessage.Text = "YES";
            //reviewPopup.CancelTextMessage.Text = "NO";
            //reviewPopup.TextMessage.Text = "Are you sure to release this book from this device?";
            //reviewPopup.IsVisible = true;            
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
                isOfflineReadAllowed = false;
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

                if (HelperFunctions.CheckInternetConnection())
                {
                    ActionTabPage.IsOfflineTextLabel.IsVisible = false;
                    string status = MyWebRequest.PostRequest("releaseuserbooks", null, new { UserId = AppData.User.UserId, BookIds = this.id, DeviceId = Constant.DeviceIdFromDB.ToString() }, null);
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

        public void reviewPopupSubmitBtn_Clicked(object sender, EventArgs e)
        {
            Constant.UserActiveTime = DateTime.Now;
            reviewPopup.IsVisible = false;
            reviewPopup.TextMessage.Text = string.Empty;

            if (isForActivity)
            {
                openActivity();
                isForActivity = false;
            }
            else if (isForDownload)
            {
                this.Page_Loader.IsVisible = true;
                Task.Run(() =>
                {
                    Task.Delay(100).Wait();
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                    {
                        Navigation.PushAsync(new BookRead(this.id, true, ReadFrom));
                        Navigation.RemovePage(this);
                    });
                });
                return;
            }
            else {
                if (this.BookRating.IsVisible)
                {
                    this.ReviewCompleted(true);
                }
                else
                {
                    this.nextQuestion(true);
                }
            }
            //if (this.isReleaseBook == true)
            //{
            //    this.isReleaseBook = false;
            //    if (this._UserBook != null)
            //    {
            //        string _device = this._UserBook.Devices.DeviceId.Where(x => x.Trim().Equals(Constant.DeviceIdFromDB.ToString()))?.FirstOrDefault();
            //        if (_device != null)
            //        {
            //            this._UserBook.Devices.DeviceId.Remove(_device);
            //        }
            //    }
            //    isOfflineReadAllowed = false;               
            //    releaseBook.IsVisible = false;
            //    isOfflineBook.IsVisible = false;                   
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
            //        string status = MyWebRequest.PostRequest("releaseuserbooks", null, new { UserId = AppData.User.UserId, BookIds = this.id, DeviceId = Constant.DeviceIdFromDB.ToString() }, null);
            //    }
            //    else
            //    {
            //        ActionTabPage.IsOfflineTextLabel.IsVisible = true;
            //    }
            //}
            //else
            //{
            //if (this.BookRating.IsVisible)
            //{
            //    this.ReviewCompleted(true);
            //}
            //else
            //{
            //    this.nextQuestion(true);
            //}
            //}         
        }

        public void reviewPopupCancelBtn_Clicked(object sender, EventArgs e)
        {
            Constant.UserActiveTime = DateTime.Now;
            //this.isReleaseBook = false;
            reviewPopup.IsVisible = false;
            reviewPopup.TextMessage.Text = string.Empty;
            if (isForDownload || isForActivity)
            {
                this.Page_Loader.IsVisible = true;
                Task.Run(() => {
                    Task.Delay(100).Wait();
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                        Navigation.PushAsync(new BookRead(this.id, false, ReadFrom));
                        Navigation.RemovePage(this);
                    });
                });
            }         
        }

        public void closeBtn_Clicked(object sender, EventArgs e)
        {
            Constant.UserActiveTime = DateTime.Now;
            DownloadedBackImage.Source = null;
            this.Page_Loader.IsVisible = true;
            Task.Run(() => {
                Task.Delay(100).Wait();
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                    Navigation.PushAsync(new BookRead(this.id, false, ReadFrom));
                    Navigation.RemovePage(this);
                });
            });
        }

        public void Back_Clicked(object sender, EventArgs e)
        {
            Constant.UserActiveTime = DateTime.Now;
            this.PreviousQuestion();
        }

        public void Next_Clicked(object sender, EventArgs e)
        {
            Constant.UserActiveTime = DateTime.Now;
            this.nextQuestion(false);
        }

        public void Done_Clicked(object sender, EventArgs e)
        {
            Constant.UserActiveTime = DateTime.Now;
            this.ReviewCompleted(false);
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Constant.UserActiveTime = DateTime.Now;
            DownloadedBackImage.Source = null;
            this.Page_Loader.IsVisible = true;
            Task.Run(() => {
                Task.Delay(100).Wait();
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                    Navigation.PushAsync(new BookRead(this.id, false, ReadFrom));
                    Navigation.RemovePage(this);
                });
            });
            return true;
        }

        public void InitailizeBookReview()
        {
            try
            {
                if (this._Book != null && this._UserBook != null)
                {
                    string UserBookReviewElement = this._UserBook.ReviewJson?.Trim();
                    this.bookReview = new UserBookReview();
                    if (UserBookReviewElement != null)
                    {
                        if (string.IsNullOrEmpty(UserBookReviewElement))
                        {
                            string s = AppData.BooksDetail.ReviewJson?.Trim();
                            if (s.Contains("\\\""))
                                s = s.Replace("\\\"", "\"").Trim('"');
                            var settings = new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore,
                                MissingMemberHandling = MissingMemberHandling.Ignore
                            };
                            JObject jsonModel = JObject.Parse(s);
                            this.bookReview.Questions = JsonConvert.DeserializeObject<Questions>(jsonModel["Questions"].ToString());
                        }
                        else
                        {
                            string s = UserBookReviewElement;
                            //if (s.Contains("\\\\\""))
                            //    s = s.Replace("\\\\\"", "\"").Trim('"');

                            if (s.Contains("\\\""))
                                s = s.Replace("\\\"", "\"").Trim('"');

                            if (s.Contains(Constant.quoteKey))
                                s = s.Replace(Constant.quoteKey, "\\\"");
                            if (s.Contains(Constant.slashKey))
                                s = s.Replace(Constant.slashKey, "\\\\");

                            var settings = new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore,
                                MissingMemberHandling = MissingMemberHandling.Ignore
                            };
                            JObject jsonModel = JObject.Parse(s);
                            this.bookReview.Questions = JsonConvert.DeserializeObject<Questions>(jsonModel["Questions"].ToString());
                        }
                    }
                    else
                    {
                        string s = AppData.BooksDetail.ReviewJson?.Trim();
                        if (s.Contains("\\\""))
                            s = s.Replace("\\\"", "\"").Trim('"');
                        var settings = new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                            MissingMemberHandling = MissingMemberHandling.Ignore
                        };
                        JObject jsonModel = JObject.Parse(s);
                        this.bookReview.Questions = JsonConvert.DeserializeObject<Questions>(jsonModel["Questions"].ToString());
                    }
                }
                              
                this.noOfQuestion = this.bookReview.Questions.Question.Count();
                this.currentQuestion = 0;
                if (this.noOfQuestion == 0)
                {
                    this.SetControlsVisibility(0, 0);
                    this.BookReviewSection.IsVisible = false;
                    this.BookRating.IsVisible = true;
                    this.UserRating();
                }
                else
                {
                    this.SetControlsVisibility(0, 1);
                    this.BookReviewSection.IsVisible = true;
                    this.BookRating.IsVisible = false;
                    this.LoadQuestion();
                }
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += ReviewSkiped;
                this.SkipReview.GestureRecognizers.Add(tapGestureRecognizer);          
            }
            catch (Exception ex)
            {

            }
        }

        public void LoadQuestion()
        {
            if (this.currentQuestion < this.noOfQuestion)
            {
                question = this.bookReview.Questions.Question.ElementAt(this.currentQuestion);
                paintQusetion();
                this.currentQuestion++;
            }           
        }

        public void nextQuestion(bool isSkipped)
        {
            bool isLast = false;
            if (this.currentQuestion == this.noOfQuestion && isSkipped)
            {
                this.SetControlsVisibility(1, 0);
                this.BookReviewSection.IsVisible = false;
                this.BookRating.IsVisible = true;
                this.UserRating();
            }
            else
            {
                if (this.currentQuestion == this.noOfQuestion)
                {
                    this.currentQuestion--;
                    isLast = true;
                }                   
            }
            if (this.currentQuestion < this.noOfQuestion)
            {
                switch (question.Type.ToUpper())
                {
                    case "TQ":
                        if (string.IsNullOrEmpty(question.Value) && !isSkipped)
                        {
                            reviewPopup.SubmitTextMessage.Text = "ANOTHER TIME";
                            reviewPopup.CancelTextMessage.Text = "OK";
                            reviewPopup.TextMessage.Text = "I would love your thoughts about the book - can you share?";
                            reviewPopup.SubmitPopupBtn.IsVisible = true;
                            reviewPopup.IsVisible = true;
                        }
                        else if (!string.IsNullOrEmpty(question.Value) && !isSkipped)
                        {
                            //string[] words = question.Value.Split(new string[] { " ", "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
                            if (question.Value.Length <= this.maxWordCount)
                            {
                                //if (question.Value.Contains("\""))
                                //    question.Value = question.Value.Replace("\"", "\\\"");

                                if (question.Value.Contains("\""))
                                    question.Value = question.Value.Replace("\"", Constant.quoteKey);
                                if (question.Value.Contains("\\"))
                                    question.Value = question.Value.Replace("\\", Constant.slashKey);

                                this.transferPage(isLast);
                            }
                        }
                        else
                        {
                            this.transferPage(isLast);
                        }
                        break;
                    case "MCQ":
                        if (question.Options != null && question.Options.Option != null)
                        {
                            bool isSelected = false;
                            foreach (Option option in question.Options.Option)
                            {
                                if (!string.IsNullOrEmpty(option.Value))
                                {
                                    isSelected = true;
                                    break;
                                }
                            }
                            if (!isSelected && !isSkipped)
                            {
                                reviewPopup.TextMessage.Text = "Are you sure you don't want to recommend this book to anyone?";
                                reviewPopup.SubmitTextMessage.Text = "YES";
                                reviewPopup.CancelTextMessage.Text = "NO";
                                reviewPopup.SubmitPopupBtn.IsVisible = true;
                                reviewPopup.IsVisible = true;                              
                            }
                            else
                            {
                                this.transferPage(isLast);
                            }                                                      
                        }
                        break;
                    default:
                        break;
                }
                if (isLast)
                {
                    this.currentQuestion++;
                }
            }
        }

        public void transferPage(bool isLast)
        {
            if (isLast)
            {
                this.SetControlsVisibility(1, 0);
                this.BookReviewSection.IsVisible = false;
                this.BookRating.IsVisible = true;
                this.UserRating();
            }
            else
            {
                this.SetControlsVisibility(1, 1);
                question = this.bookReview.Questions.Question.ElementAt(this.currentQuestion);
                paintQusetion();
                this.currentQuestion++;
            }
        }

        public void PreviousQuestion()
        {
            if (this.BookRating.IsVisible)
            {
                this.BookRating.IsVisible = false;
                this.BookReviewSection.IsVisible = true;
            }
            else
            {
                this.currentQuestion--;
            }                       
            if (this.currentQuestion > 0)
            {
                this.currentQuestion--;
                question = this.bookReview.Questions.Question.ElementAt(this.currentQuestion);
                paintQusetion();
                this.SetControlsVisibility(1, 1);
            }
            if (this.currentQuestion == 0)
            {
                this.SetControlsVisibility(0, 1);                
            }
            this.currentQuestion++;
        }

        public void paintQusetion()
        {
            if (question != null)
            {
                this.BookReview.Children.Clear();
                Label statement = new Label();
                statement.Text = string.IsNullOrEmpty(question.Text) ? "" : question.Text;
                statement.SetDynamicResource(Label.StyleProperty, "LabelNormalRegular");
                statement.FontSize = 24;
                statement.Margin = new Thickness(0, 0, 0, 20);
                this.BookReview.Children.Add(statement);
                switch (question.Type.ToUpper())
                {
                    case "TQ":
                        this.maxWordCount = 1000; //string.IsNullOrEmpty(question.MaxWord) ? 250 : Convert.ToInt32(question.MaxWord);
                        MyEditor editor = new MyEditor();
                        editor.BackgroundColor = Color.Transparent;
                        editor.VerticalOptions = LayoutOptions.Start;
                        editor.HorizontalOptions = LayoutOptions.Start;
                        editor.SetDynamicResource(MyEditor.StyleProperty, "EditorNormalRegular");
                        //editor.FontSize = 16;
                        editor.WidthRequest = this.editorWidth;
                        editor.HeightRequest = this.editorHeight;
                        editor.Focused += (s, e) => {
                            editor.BackgroundColor = Color.Transparent;
                        };
                        editor.Unfocused += (s, e) => {
                            editor.BackgroundColor = Color.Transparent;
                        };
                        editor.TextChanged += (s, e) =>
                        {
                            Constant.UserActiveTime = DateTime.Now;
                            question.Value = ((MyEditor)s).Text;
                            int enterKeyCount = question.Value.Split(new string[] { "\r\n", "\n", "\r"}, StringSplitOptions.None).Length - 1;                            
                            int characterHeight = ((int)question.Value.Length / this.noOfCharacter);
                            if (characterHeight > 0 || enterKeyCount > 0)
                            {
                                double h;
                                if (Xamarin.Forms.Device.OS == TargetPlatform.Android)
                                {
                                    h = 26 * (characterHeight == 0 ? enterKeyCount : (characterHeight + enterKeyCount));
                                }
                                else
                                {
                                    h = 20 * (characterHeight == 0 ? enterKeyCount : (characterHeight + enterKeyCount));                                  
                                }
                                editor.HeightRequest = this.editorHeight + h;
                            }
                            else
                            {
                                editor.HeightRequest = this.editorHeight;
                            }

                            View v = this.BookReview.Children.Where(x => x.StyleId != null && x.StyleId.Equals("wordlimitLayout"))?.FirstOrDefault();
                            if (v != null && v is Grid)
                            {
                                string[] words = question.Value.Split(new string[] { " ", "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
                                int totalChar = question.Value.Length - enterKeyCount;
                                if (totalChar >= 0)
                                {
                                    ((Label)((Grid)v).Children[1]).Text = String.Format("Current Letter Count: {0}", totalChar);
                                    if (question.Value.Length <= this.wordCountForLabel)
                                    {
                                        ((Label)((Grid)v).Children[1]).IsVisible = false;
                                        ((Label)((Grid)v).Children[0]).IsVisible = false;
                                    }
                                    else
                                    {
                                        ((Label)((Grid)v).Children[1]).IsVisible = true;
                                        ((Label)((Grid)v).Children[0]).IsVisible = true;
                                    }
                                    if (!(totalChar <= this.maxWordCount))
                                    {
                                        if(!ShowLimitExceeded)
                                        {
                                            reviewPopup.TextMessage.Text = "Maximum Letter count exceeded. Please consider updating the comment";
                                            reviewPopup.SubmitPopupBtn.IsVisible = false;
                                            reviewPopup.CancelTextMessage.Text = "OK";
                                            reviewPopup.IsVisible = true;
                                            ShowLimitExceeded = true;
                                            editor.Unfocus();
                                        }
                                        ((Label)((Grid)v).Children[1]).TextColor = Color.Red;
                                    }
                                    else
                                    {
                                        ((Label)((Grid)v).Children[1]).TextColor = Color.FromHex("#333333");
                                        ShowLimitExceeded = false;
                                    }
                                }
                                //if (words != null)
                                //{
                                //    ((Label)((Grid)v).Children[1]).Text = String.Format("Current Word Count: {0}", words.Length);
                                //    if (words.Length <= this.wordCountForLabel)
                                //    {
                                //        ((Label)((Grid)v).Children[1]).IsVisible = false;
                                //        ((Label)((Grid)v).Children[0]).IsVisible = false;
                                //    }
                                //    else 
                                //    {
                                //        ((Label)((Grid)v).Children[1]).IsVisible = true;
                                //        ((Label)((Grid)v).Children[0]).IsVisible = true;
                                //    }
                                //    if (!(words.Length <= this.maxWordCount))
                                //    {
                                //        ((Label)((Grid)v).Children[1]).TextColor = Color.Red;
                                //    }
                                //    else
                                //    {
                                //        ((Label)((Grid)v).Children[1]).TextColor = Color.FromHex("#333333");
                                //    }
                                //}
                            }
                        };
                        //if (question.Value.Contains("\\\""))
                        //    question.Value = question.Value.Replace("\\\"", "\"");

                        if (question.Value.Contains(Constant.quoteKey))
                            question.Value = question.Value.Replace(Constant.quoteKey, "\"");
                        if (question.Value.Contains(Constant.slashKey))
                            question.Value = question.Value.Replace(Constant.slashKey, "\\");

                        editor.Text = string.IsNullOrEmpty(question.Value) ? "" : question.Value;
                        this.BookReview.Children.Add(editor);
                        BoxView border = new BoxView();
                        border.HeightRequest = 2;
                        border.SetDynamicResource(BoxView.BackgroundColorProperty, "PrimaryColor");
                        border.Margin = new Thickness(0, -8, 0, 0);
                        this.BookReview.Children.Add(border);
                        //Error Msg
                        Grid wordlimitLayout = new Grid { RowSpacing = 0,  ColumnSpacing = 0};
                        wordlimitLayout.StyleId = "wordlimitLayout";
                        wordlimitLayout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto});
                        wordlimitLayout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                        wordlimitLayout.ColumnDefinitions.Add(new ColumnDefinition());
                        wordlimitLayout.RowSpacing = 5;
                        wordlimitLayout.Padding = new Thickness(0, 5, 0, 5);
                        Label maxrMsg = new Label { Text = String.Format("Maximum Letter Limit: {0}", this.maxWordCount), StyleId = "MaxWordCount", FontSize = 14 };
                        maxrMsg.SetDynamicResource(Label.StyleProperty, "LabelNormalRegular");
                        maxrMsg.TextColor = Color.FromHex("#333333");
                        maxrMsg.HorizontalOptions = LayoutOptions.Start;
                        maxrMsg.HorizontalTextAlignment = TextAlignment.Start;
                        wordlimitLayout.Children.Add(maxrMsg, 0, 1);
                        Label currentMsg = new Label { StyleId = "CurrentWordCount", FontSize = 14 };
                        string[] wordsCount = question.Value.Split(new string[] { " ", "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
                        //if (wordsCount != null)
                        //{
                        //    if (wordsCount.Length <= this.wordCountForLabel)
                        //    {
                        //        currentMsg.IsVisible = false;
                        //        maxrMsg.IsVisible = false;
                        //    }
                        //    else 
                        //    {
                        //        currentMsg.IsVisible = true;
                        //        maxrMsg.IsVisible = true;
                        //    }
                        //    currentMsg.Text = String.Format("Current Word Count: {0}", wordsCount.Length);
                        //}
                        if (question.Value.Length > 0)
                        {
                            if (question.Value.Length <= this.wordCountForLabel)
                            {
                                currentMsg.IsVisible = false;
                                maxrMsg.IsVisible = false;
                            }
                            else
                            {
                                currentMsg.IsVisible = true;
                                maxrMsg.IsVisible = true;
                            }
                            currentMsg.Text = String.Format("Current Letter Count: {0}", question.Value.Length);
                        }
                        else
                        {
                            currentMsg.Text = String.Format("Current Word Count: {0}", 0);
                            currentMsg.IsVisible = false;
                            maxrMsg.IsVisible = false;
                        }
                        currentMsg.SetDynamicResource(Label.StyleProperty, "LabelNormalRegular");
                        currentMsg.TextColor = Color.FromHex("#333333");
                        currentMsg.HorizontalOptions = LayoutOptions.Start;
                        currentMsg.HorizontalTextAlignment = TextAlignment.Start;
                        wordlimitLayout.Children.Add(currentMsg, 0, 0);
                        this.BookReview.Children.Add(wordlimitLayout);
                        break;
                    case "MCQ":
                        options = question.Options.Option;
                        if (options != null)
                        {
                            layouts = new List<StackLayout>();
                            foreach (Option option in options)
                            {
                                StackLayout layout = new StackLayout();
                                layout.Orientation = StackOrientation.Horizontal;
                                layout.Margin = new Thickness(0, 0, 10, 0);
                                layout.Spacing = 20;
                                Image img = new Image();
                                if (Boolean.Parse(string.IsNullOrEmpty(option.Value) ? "false" : option.Value))
                                {
                                    img.SetDynamicResource(Image.SourceProperty, "CheckBoxSelectedImage");
                                }
                                else
                                {
                                    img.SetDynamicResource(Image.SourceProperty, "CheckBoxImage");
                                }
                                img.WidthRequest = 18;
                                img.HeightRequest = 18;
                                img.VerticalOptions = LayoutOptions.Center;
                                img.HorizontalOptions = LayoutOptions.Start;
                                Label text = new Label();
                                text.SetDynamicResource(Label.StyleProperty, "LabelNormalRegular");
                                text.VerticalOptions = LayoutOptions.Start;
                                text.FontSize = 24;
                                text.Text = option.Text;

                                layout.Children.Add(img);
                                layout.Children.Add(text);

                                var tapGestureRecognizer = new TapGestureRecognizer();
                                tapGestureRecognizer.Tapped += (s, e) => {
                                    Constant.UserActiveTime = DateTime.Now;
                                    foreach (StackLayout sl in layouts)
                                    {
                                        if(sl == s)
                                        {
                                            ((Image)sl.Children.ElementAt(0)).SetDynamicResource(Image.SourceProperty, "CheckBoxSelectedImage");
                                            options.ElementAt(layouts.IndexOf(sl)).Value = "true";
                                        }
                                        else
                                        {
                                            ((Image)sl.Children.ElementAt(0)).SetDynamicResource(Image.SourceProperty, "CheckBoxImage");
                                            options.ElementAt(layouts.IndexOf(sl)).Value = "false";
                                        }
                                    }
                                };
                                layout.GestureRecognizers.Add(tapGestureRecognizer);
                                layouts.Add(layout);

                                this.BookReview.Children.Add(layout);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }            
        }

        public void UserRating()          
        {
            try
            {               
                this.currentStar = String.Empty;
                foreach (View StarLayout in BookRating.Children)
                {
                    if (StarLayout is Grid)
                    {
                        var tapGestureRecognizer = new TapGestureRecognizer();
                        tapGestureRecognizer.Tapped += SetStar;
                        StarLayout.GestureRecognizers.Add(tapGestureRecognizer);
                        StarLayout.Opacity = 0.5;
                    }
                }
                switch (this.userRating)
                {
                    case 1:
                        this.SetStar(Star1, new EventArgs());
                        break;
                    case 2:
                        this.SetStar(Star2, new EventArgs());
                        break;
                    case 3:
                        this.SetStar(Star3, new EventArgs());
                        break;
                    case 4:
                        this.SetStar(Star4, new EventArgs());
                        break;
                    case 5:
                        this.SetStar(Star5, new EventArgs());
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void SetStar(object sender, EventArgs e)
        {
            Constant.UserActiveTime = DateTime.Now;
            try
            {
                Grid starLayout = sender as Grid;
                if (!this.currentStar.Equals(starLayout.StyleId))
                {
                    foreach (View StarLayout in BookRating.Children)
                    {
                        if (StarLayout is Grid)
                        {
                            if (StarLayout as Grid == starLayout)
                            {
                                StarLayout.Opacity = 1;
                                this.currentStar = StarLayout.StyleId;
                            }
                            else
                            {
                                StarLayout.Opacity = 0.5;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void SetControlsVisibility(int n, int m)
        {
            if(n == 0)
            {
                this.SkipReview.IsVisible = true;
                this.BackReview.IsVisible = false;
            }
            else
            {
                this.SkipReview.IsVisible = false;
                this.BackReview.IsVisible = true;
            }
            if (m == 0)
            {
                this.NextReview.IsVisible = false;
                this.DoneReview.IsVisible = true;
            }
            else
            {
                this.NextReview.IsVisible = true;
                this.DoneReview.IsVisible = false;
            }
        }

        public void ReviewCompleted(bool isSkipped)
        {
            /////////////////////////////
            //Update Book XML for Review

            if (this._Book != null && this._UserBook != null)
            {
                //---------------------------------------------Rating Section------------------------------------------
                if (!string.IsNullOrEmpty(this.currentStar))
                {
                    if (Int32.Parse(this.currentStar) != 0)
                    {
                        if (string.IsNullOrEmpty(this._UserBook.Rating?.Trim()) || Int32.Parse(this._UserBook.Rating?.Trim()) == 0)
                        {
                            this._Book.Rating.TotalUserRatedThisBook = ((string.IsNullOrEmpty(this._Book.Rating.TotalUserRatedThisBook?.Trim()) ? 0 : Int32.Parse(this._Book.Rating.TotalUserRatedThisBook?.Trim())) + 1).ToString();
                        }
                        else
                        {
                            int previousStar = Int32.Parse(this._UserBook.Rating?.Trim());
                            switch (previousStar)
                            {
                                case 1:
                                    this._Book.Rating.OneStarRating = ((string.IsNullOrEmpty(this._Book.Rating.OneStarRating?.Trim()) ? 0 : Int32.Parse(this._Book.Rating.OneStarRating?.Trim())) - 1).ToString();
                                    break;
                                case 2:
                                    this._Book.Rating.TwoStarRating = ((string.IsNullOrEmpty(this._Book.Rating.TwoStarRating?.Trim()) ? 0 : Int32.Parse(this._Book.Rating.TwoStarRating?.Trim())) - 1).ToString();
                                    break;
                                case 3:
                                    this._Book.Rating.ThreeStarRating = ((string.IsNullOrEmpty(this._Book.Rating.ThreeStarRating?.Trim()) ? 0 : Int32.Parse(this._Book.Rating.ThreeStarRating?.Trim())) - 1).ToString();
                                    break;
                                case 4:
                                    this._Book.Rating.FourStarRating = ((string.IsNullOrEmpty(this._Book.Rating.FourStarRating?.Trim()) ? 0 : Int32.Parse(this._Book.Rating.FourStarRating?.Trim())) - 1).ToString();
                                    break;
                                case 5:
                                    this._Book.Rating.FiveStarRating = ((string.IsNullOrEmpty(this._Book.Rating.FiveStarRating?.Trim()) ? 0 : Int32.Parse(this._Book.Rating.FiveStarRating?.Trim())) - 1).ToString();
                                    break;
                            }                         
                        }

                        switch (Int32.Parse(this.currentStar))
                        {
                            case 1:
                                this._Book.Rating.OneStarRating = ((string.IsNullOrEmpty(this._Book.Rating.OneStarRating?.Trim()) ? 0 : Int32.Parse(this._Book.Rating.OneStarRating?.Trim())) + 1).ToString();
                                break;
                            case 2:
                                this._Book.Rating.TwoStarRating = ((string.IsNullOrEmpty(this._Book.Rating.TwoStarRating?.Trim()) ? 0 : Int32.Parse(this._Book.Rating.TwoStarRating?.Trim())) + 1).ToString();
                                break;
                            case 3:
                                this._Book.Rating.ThreeStarRating = ((string.IsNullOrEmpty(this._Book.Rating.ThreeStarRating?.Trim()) ? 0 : Int32.Parse(this._Book.Rating.ThreeStarRating?.Trim())) + 1).ToString();
                                break;
                            case 4:
                                this._Book.Rating.FourStarRating = ((string.IsNullOrEmpty(this._Book.Rating.FourStarRating?.Trim()) ? 0 : Int32.Parse(this._Book.Rating.FourStarRating?.Trim())) + 1).ToString();
                                break;
                            case 5:
                                this._Book.Rating.FiveStarRating = ((string.IsNullOrEmpty(this._Book.Rating.FiveStarRating?.Trim()) ? 0 : Int32.Parse(this._Book.Rating.FiveStarRating?.Trim())) + 1).ToString();
                                break;
                        }

                        long[] arrayOfCounts = {
                                string.IsNullOrEmpty(this._Book.Rating.FiveStarRating?.Trim()) ? 0 : Int32.Parse(this._Book.Rating.FiveStarRating?.Trim()),
                                string.IsNullOrEmpty(this._Book.Rating.FourStarRating?.Trim()) ? 0 : Int32.Parse(this._Book.Rating.FourStarRating?.Trim()),
                                string.IsNullOrEmpty(this._Book.Rating.ThreeStarRating?.Trim()) ? 0 : Int32.Parse(this._Book.Rating.ThreeStarRating?.Trim()),
                                string.IsNullOrEmpty(this._Book.Rating.TwoStarRating?.Trim()) ? 0 : Int32.Parse(this._Book.Rating.TwoStarRating?.Trim()),
                                string.IsNullOrEmpty(this._Book.Rating.OneStarRating?.Trim()) ? 0 : Int32.Parse(this._Book.Rating.OneStarRating?.Trim())
                            };

                        long avgtotal = (5 * arrayOfCounts[0]) + (4 * arrayOfCounts[1]) + (3 * arrayOfCounts[2]) + (2 * arrayOfCounts[3]) + (1 * arrayOfCounts[4]);
                        this._Book.Rating.AverageRating = ((long)(avgtotal / Double.Parse(this._Book.Rating.TotalUserRatedThisBook))).ToString();

                        this._UserBook.Rating = Int32.Parse(this.currentStar).ToString();

                        if (this.isRatingGiven)
                        {
                            if (Int32.Parse(this.currentStar) != this.userRating)
                            {
                                Utils.SetRatingLog(this.id, Int32.Parse(this.currentStar));
                            }                               
                        }
                        else
                        {
                            Utils.SetRatingLog(this.id, Int32.Parse(this.currentStar));
                        }
                    }
                }
            }

            /////////////////////////////
            if (isSkipped)
            {
                Utils.SetBookReviewCompleted(this.id, this.bookReview);
                if (this.hasActivity)
                {
                    if (Boolean.Parse(this._UserBook.IsActivityDone))
                    {
                        isForActivity = true;
                        //isForDownload = false;
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                        {
                            reviewPopup.TextMessage.Text = "Hey! You have already attempted this activity! Would you like to see how it went last time?";
                            reviewPopup.SubmitPopupBtn.IsVisible = true;
                            reviewPopup.SubmitTextMessage.Text = "YES";
                            reviewPopup.CancelTextMessage.Text = "NO";
                            reviewPopup.IsVisible = true;
                        });
                    }
                    else
                    {
                        openActivity();
                    }
                }
                else
                {
                    this.Page_Loader.IsVisible = true;
                    Task.Run(() => {
                        Task.Delay(100).Wait();
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                            Navigation.PushAsync(new BookRead(this.id, false, ReadFrom));
                            Navigation.RemovePage(this);
                        });
                    });
                }
            }
            else
            {
                if (string.IsNullOrEmpty(this.currentStar) && !isSkipped)
                {
                    reviewPopup.SubmitTextMessage.Text = "ANOTHER TIME";
                    reviewPopup.CancelTextMessage.Text = "OK";
                    reviewPopup.TextMessage.Text = "I would love your rating of the book - can you share?";
                    reviewPopup.SubmitPopupBtn.IsVisible = true;
                    reviewPopup.IsVisible = true;
                }
                else
                {
                    Utils.SetBookReviewCompleted(this.id, this.bookReview);
                    if (this.hasActivity)
                    {
                        if (Boolean.Parse(this._UserBook.IsActivityDone))
                        {
                            isForActivity = true;
                            //isForDownload = false;
                            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                            {
                                reviewPopup.TextMessage.Text = "Hey! You have already attempted this activity! Would you like to see how it went last time?";
                                reviewPopup.SubmitPopupBtn.IsVisible = true;
                                reviewPopup.SubmitTextMessage.Text = "YES";
                                reviewPopup.CancelTextMessage.Text = "NO";
                                reviewPopup.IsVisible = true;
                            });
                        }
                        else
                        {
                            openActivity();
                        }
                    }
                    else
                    {
                        this.Page_Loader.IsVisible = true;
                        Task.Run(() => {
                            Task.Delay(100).Wait();
                            Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                                Navigation.PushAsync(new BookRead(this.id, false, ReadFrom));
                                Navigation.RemovePage(this);
                            });
                        });
                    }
                }
            }           
        }

        private void openActivity()
        {
            if (isOfflineReadAllowed)
            {
                this.Page_Loader.IsVisible = true;
                Task.Run(() => {
                    Task.Delay(100).Wait();
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                        Navigation.PushAsync(new BookActivity(this.id, this.ReadFrom));
                        Navigation.RemovePage(this);
                    });
                });
            }
            else
            {
                isForDownload = true;
                isForActivity = false;
                reviewPopup.SubmitTextMessage.Text = "YES";
                reviewPopup.CancelTextMessage.Text = "NO";
                reviewPopup.SubmitPopupBtn.IsVisible = true;
                reviewPopup.TextMessage.Text = "You need to download the book to start the activity. Would you like to download the book?";
                reviewPopup.IsVisible = true;
            }
        }
        public void ReviewSkiped(object sender, EventArgs e)
        {
            Constant.UserActiveTime = DateTime.Now;
            if (this.hasActivity)
            {
                if (Boolean.Parse(this._UserBook.IsActivityDone))
                {
                    isForActivity = true;
                    //isForDownload = false;
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                    {
                        reviewPopup.TextMessage.Text = "Hey! You have already attempted this activity! Would you like to see how it went last time?";
                        reviewPopup.SubmitPopupBtn.IsVisible = true;
                        reviewPopup.SubmitTextMessage.Text = "YES";
                        reviewPopup.CancelTextMessage.Text = "NO";
                        reviewPopup.IsVisible = true;
                    });
                }
                else
                {
                    openActivity();
                }
            }
            else
            {
                this.Page_Loader.IsVisible = true;
                Task.Run(() => {
                    Task.Delay(100).Wait();
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                        Navigation.PushAsync(new BookRead(this.id, false, ReadFrom));
                        Navigation.RemovePage(this);
                    });
                });
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.StartTime = DateTime.Now;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            DownloadedBackImage.Source = null;
            Utils.SetBookReviewTime(this.id, this.StartTime, DateTime.Now);
        }
    }
}
