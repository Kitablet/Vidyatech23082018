using Kitablet.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Kitablet
{
    public partial class BookActivity : ContentPage
    {
        public string id;
        protected int ReadFrom;
        public DateTime StartTime;
        public static string strJson = string.Empty;
        //public static string activityHTML = string.Empty;

        public BookActivity(string id, int ReadFrom)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            ActivityPageLayout.BackgroundColor = Color.FromHex(Constant.Primarycolor);
            //this.ActivityPageLayout.WidthRequest = Device.OnPlatform<double>(Constant.DeviceWidth, Constant.DeviceWidth / Constant.DeviceDensity, Constant.DeviceWidth);
            //this.ActivityPageLayout.HeightRequest = Device.OnPlatform<double>(Constant.DeviceHeight, (Constant.DeviceHeight / Constant.DeviceDensity) - 25, Constant.DeviceHeight);

            Utils.AddUserSyncBook(id);

            this.id = id;
            this.ReadFrom = ReadFrom;

            string filePath = Path.Combine(Constant.FileDirectoryStructure, Constant.FileNameInitials + this.id, "BookActivity", "BookActivity.html");
            //try
            //{
            //    activityHTML = AppData.FileService.LoadEncryptedFile(Path.Combine(AppData.FileService.GetLocalLocalFolderPath(), Path.GetDirectoryName(filePath), "ActivityData.txt"), Constant.UserCryptoKey);
            //    //activityHTML = AppData.FileService.LoadText(Path.Combine(Path.GetDirectoryName(filePath), "ActivityData.txt"));
            //}
            //catch (Exception ex)
            //{

            //}

            //if (activityHTML == null)
            //{
            //    activityHTML = string.Empty;
            //}

            IsActivityDone();
            HelpWebView.Uri = filePath;
            HelpWebView.RegisterAction(
                data => ActivityJsonFromWebView(data)
            );
        }

        private void IsActivityDone()
        {
            try
            {
                bool isdone = false;
                string ActivityJSON = string.Empty;

                if (AppData.BooksDetail != null)
                {
                    UserBook userBook = null;
                    if (AppData.UserDetails != null)
                    {
                        userBook = AppData.UserDetails.UserBooks.UserBook.Where(x => x.BookId.Equals(this.id)).FirstOrDefault();
                    }
                    if (userBook == null)
                    {
                        if (AppData.UserDetails.UserBooks.UserBook == null)
                        {
                            AppData.UserDetails.UserBooks.UserBook = new List<UserBook>();
                        }
                        AppData.UserDetails.UserBooks.UserBook.Add(Utils.AddNewBookElement(this.id, false.ToString(), new List<string>()));
                        userBook = AppData.UserDetails.UserBooks.UserBook.Where(x => x.BookId.Equals(this.id)).FirstOrDefault();
                    }
                    if (userBook != null)
                    {
                        Book _book = AppData.BooksDetail.Books.Book.Where(x => x.BookId.Equals(this.id)).FirstOrDefault();

                        bool bookReadAloud = false, bookAnimation = false;
                        int rating = 0;
                        string orientation = string.Empty;

                        if (_book != null)
                        {
                            bookReadAloud = string.IsNullOrEmpty(_book.HasReadAloud) ? false : Boolean.Parse(_book.HasReadAloud);
                            bookAnimation = string.IsNullOrEmpty(_book.HasAnimation) ? false : Boolean.Parse(_book.HasAnimation);
                            orientation = string.IsNullOrEmpty(_book.ViewMode.Trim()) ? string.Empty : _book.ViewMode.Trim();
                        }
                        
                        if (!string.IsNullOrEmpty(userBook.Rating))
                            rating = int.Parse(userBook.Rating);                       

                        isdone = Boolean.Parse(string.IsNullOrEmpty(userBook.IsActivityDone?.Trim()) ? "false" : userBook.IsActivityDone?.Trim());
                        if (isdone)
                        {
                            // ActivityJSON = userBook.ActivityJson?.Trim().Replace("\\n","#").Replace("\n","\\n");
                            // ActivityJSON = ActivityJSON.Replace("#", "\\\\n");
                            if(Device.OS == TargetPlatform.Android || Device.OS == TargetPlatform.iOS)
                                ActivityJSON = userBook.ActivityJson?.Trim().Replace("#13#", "\\\\n");
                            else
                                ActivityJSON = userBook.ActivityJson?.Trim().Replace("#13#", "\\n");
                        }
                        else
                        {
                            ActivityJSON = AppData.BooksDetail.Books.Book.Where(x => x.BookId.Equals(this.id)).FirstOrDefault().ActivityJson?.Trim();
                        }                       
                        if (!string.IsNullOrEmpty(ActivityJSON))
                        {
                            
                            string str = "{\"ActivityAttr\": {\"id\":" + this.id + ",\"readaloud\":" + bookReadAloud.ToString().ToLower() + ",\"activity\":" + isdone.ToString().ToLower() + ",\"animation\":" + bookAnimation.ToString().ToLower() + ",\"rating\":" + rating.ToString() + ",\"device\":\""+ AppData.DeviceDetail.Platform.ToLower() + "\",\"subsection\":\"" + ThemeChanger.SubSection + "\",\"orientation\":\"" + orientation.ToLower() + "\",\"SessionTimeOut\":\"" + (Constant.IsRememberMe ? "0" : Constant.UserSessionTimeOut.ToString()) + "\"},";                            
                            string temp = ActivityJSON.Substring(1);
                            temp = str + temp;
                            strJson = temp;
                        }

                    }
                }
            }
            catch (Exception ex) { }
        }

        private void ActivityJsonFromWebView(string data)
        {
            this.Page_Loader.IsVisible = true;
            if (!string.IsNullOrEmpty(data))
            {
                if (data.Trim().ToLower().Equals("logout"))
                {
                    Task.Run(() => 
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
                    });
                    return;
                }

                data = data.Replace("\\n", "#13#");
                if (data.Contains("\\\""))
                    data = data.Replace("\\\"", "\"").Trim('"');

                if (!string.IsNullOrEmpty(data))
                {
                    Utils.SetBookActivityCompleted(this.id, data);
                }           
            }          
            Task.Run(() => {
                Task.Delay(100).Wait();
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                    Navigation.PushAsync(new BookRead(this.id, false, ReadFrom));
                    Navigation.RemovePage(this);
                });
            });           
        }

        public void closeBtn_Clicked(object sender, EventArgs e)
        {
            Constant.UserActiveTime = DateTime.Now;
            this.Page_Loader.IsVisible = true;
            Task.Run(() => {
                Task.Delay(100).Wait();
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                    Navigation.PushAsync(new BookRead(this.id, false, ReadFrom));
                    Navigation.RemovePage(this);
                });
            });
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Constant.UserActiveTime = DateTime.Now;
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

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ActionTabPage.UserSession(false);
            this.StartTime = DateTime.Now;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Utils.SetBookActivityTime(this.id, this.StartTime, DateTime.Now);
            ActionTabPage.UserSession(true);
        }

    }
}
