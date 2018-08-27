using Kitablet.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Kitablet
{
    public partial class App : Application
    {
        public App()
        {
            AppData.InitailizeAppData();
            InitializeComponent();
            Constant.DeviceHeight = AppData.FileService.GetDeviceHeight();
            Constant.DeviceWidth = AppData.FileService.GetDeviceWidth();
            Constant.DeviceDensity = AppData.FileService.GetDeviceDensity();
            Constant.CurrentPlateform = AppData.FileService.setPlateform();
            if ((Xamarin.Forms.Device.OS == TargetPlatform.Android) || (Device.OS == TargetPlatform.iOS))
            {
                Constant.UniqueID = AppData.FileService.GetId();
                Constant.UserCryptoKey = AppData.FileService.GetId();
            }
            else
            {
                Constant.UniqueID = AppData.FileService.GetAshwid();
                Constant.UserCryptoKey = AppData.FileService.GetAshwid();
            }
            AppData.InitailizeDeviceDetail();
            ThemeChanger.SetGrade();
            ThemeChanger.changeTheme();
            MainPage = new NavigationPage(new LoginPage(false));
            if (HelperFunctions.checkCacheExist("DeviceEnvironment"))
            {
                LoginPage loginpage = ((LoginPage)MainPage.Navigation.NavigationStack.ElementAt(0));
                loginpage.EnvironmentVisible = false;
                loginpage.CheckLogin();
            }
        }
        protected override void OnStart()
        {

        }
        protected override void OnSleep()
        {
            try
            {
                object page = null;
                ActionTabPage.SendViewedEvents();
                if (MainPage.Navigation.NavigationStack.Count == 1)
                {
                    page = MainPage.Navigation.NavigationStack.ElementAt(0);
                    if (page is ActionTabPage)
                    {
                        ActionTabPage p = page as ActionTabPage;
                        Utils.SetBrowsingProgress(ActionTabPage.AppStartTime);
                        if (HelperFunctions.CheckInternetConnection())
                        {
                            Utils.UpdateUserDetails(Utils.UpdateMethod.Total);
                        }
                    }
                }
                else
                {
                    for (int i = MainPage.Navigation.NavigationStack.Count - 1; i >= 0; i--)
                    {
                        page = MainPage.Navigation.NavigationStack.ElementAt(i);
                        if (page != null)
                        {
                            if (page is ActionTabPage)
                            {
                                ActionTabPage p = page as ActionTabPage;
                                Utils.SetBrowsingProgress(ActionTabPage.AppStartTime);
                                if (HelperFunctions.CheckInternetConnection())
                                {
                                    Utils.UpdateUserDetails(Utils.UpdateMethod.Total);
                                }
                            }
                            else if (page is BookRead)
                            {
                                Utils.UpdateUserBookStatus();
                            }
                            else if (page is BookDisplay)
                            {
                                BookDisplay p = page as BookDisplay;
                                DateTime EndTime = DateTime.Now;
                                Utils.SetBookReadingTime(p.id, p.StartTime, EndTime);
                                Utils.SetBookCurrentPage(p.id, p.currentPage);
                                if (p.PageIndex.Count > 0)
                                {
                                    Utils.SetPageProgress(p.id, p.PageIndex, p.PageReadStart, EndTime);
                                }

                                if (Xamarin.Forms.Device.OS == TargetPlatform.Windows)
                                {
                                    ((View)p.FindByName<MyButton>("AutoPlayBtn")).SetDynamicResource(Button.ImageProperty, "ReadAloudMuteImageBlack");
                                    if (p.displayPageCount == 2)
                                    {
                                        ((View)p.FindByName<MyButton>("singlePagerBtn")).SetDynamicResource(Button.ImageProperty, "SinglePagerImage");
                                    }
                                    else if (p.displayPageCount == 1)
                                    {
                                        ((View)p.FindByName<MyButton>("doublePagerBtn")).SetDynamicResource(Button.ImageProperty, "DoublePagerImage");
                                    }
                                    ((View)p.FindByName<MyButton>("singlePagerBtn")).IsEnabled = true;
                                    ((View)p.FindByName<MyButton>("doublePagerBtn")).IsEnabled = true;
                                    BookDisplay.isAutoPlay = false;

                                    AbsoluteLayout view = ((View)p.FindByName<AbsoluteLayout>("WebViewContainer")) as AbsoluteLayout;
                                    foreach (View v in view.Children)
                                    {
                                        if (v is WebView)
                                        {
                                            if (p.isReadAloud)
                                            {
                                                ((WebView)v).Eval("LoadCCFile()");
                                                ((WebView)v).Eval("StopTimer()");
                                                ((WebView)v).Eval("ClearTimer()");
                                            }
                                        }
                                    }

                                    if (p.isReadAloud)
                                    {
                                        p.AudioPage = 0;
                                        p.FileIndex = p.currentPage - p.displayPageCount;
                                        if (p.displayPageCount == 2 && string.IsNullOrEmpty(p.AudioFiles.ElementAt(p.FileIndex).FileName))
                                        {
                                            p.AudioPage = 1;
                                            p.FileIndex++;
                                        }
                                        p.LoadMediaFile();
                                    }
                                }

                            }
                            else if (page is BookCompleted)
                            {
                                BookCompleted p = page as BookCompleted;
                                Utils.SetBookReviewTime(p.id, p.StartTime, DateTime.Now);
                            }
                            else if (page is BookActivity)
                            {
                                BookActivity p = page as BookActivity;
                                Utils.SetBookActivityTime(p.id, p.StartTime, DateTime.Now);
                            }
                        }
                    }
                }
                AppData.SaveAppData();
            }
            catch (Exception ex)
            {

            }
        }
        protected override void OnResume()
        {
            try
            {
                object page = null;
                DateTime ResumeTime = DateTime.Now;
                for (int i = MainPage.Navigation.NavigationStack.Count - 1; i >= 0; i--)
                {
                    page = MainPage.Navigation.NavigationStack.ElementAt(i);
                    if (page != null)
                    {
                        if (page is ActionTabPage)
                        {
                            ActionTabPage p = page as ActionTabPage;
                            ActionTabPage.AppStartTime = ResumeTime;
                        }
                        else if (page is BookDisplay)
                        {
                            BookDisplay p = page as BookDisplay;
                            p.StartTime = ResumeTime;
                            if (p.PageIndex.Count > 0)
                            {
                                p.PageReadStart = ResumeTime;
                            }

                            if (Xamarin.Forms.Device.OS == TargetPlatform.Android)
                            {
                                p.DefaultReadAloud();

                                ((View)p.FindByName<MyButton>("AutoPlayBtn")).SetDynamicResource(Button.ImageProperty, "ReadAloudMuteImageBlack");
                                if (p.displayPageCount == 2)
                                {
                                    ((View)p.FindByName<MyButton>("singlePagerBtn")).SetDynamicResource(Button.ImageProperty, "SinglePagerImage");
                                }
                                else if (p.displayPageCount == 1)
                                {
                                    ((View)p.FindByName<MyButton>("doublePagerBtn")).SetDynamicResource(Button.ImageProperty, "DoublePagerImage");
                                }
                                ((View)p.FindByName<MyButton>("singlePagerBtn")).IsEnabled = true;
                                ((View)p.FindByName<MyButton>("doublePagerBtn")).IsEnabled = true;
                                BookDisplay.isAutoPlay = false;

                                Grid pager = ((View)p.FindByName<Grid>("BookPager")) as Grid;
                                foreach (View v in pager.Children)
                                {
                                    if (v is Label)
                                    {
                                        ((Label)v).HorizontalTextAlignment = TextAlignment.Center;
                                    }
                                }

                                AbsoluteLayout view = ((View)p.FindByName<AbsoluteLayout>("WebViewContainer")) as AbsoluteLayout;
                                int n = 0;
                                foreach (View v in view.Children)
                                {
                                    if (v is WebView)
                                    {
                                        try
                                        {
                                            ((WebView)v).Navigated -= p.WebView_Navigated;
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                        ((WebView)v).Source = p.htmlSource.ElementAt(n);
                                        ((WebView)v).Navigated += p.WebView_Navigated;
                                    }
                                    n++;
                                }
                                if (p.isReadAloud)
                                {
                                    p.AudioPage = 0;
                                    p.FileIndex = p.currentPage - p.displayPageCount;
                                    if (p.displayPageCount == 2 && string.IsNullOrEmpty(p.AudioFiles.ElementAt(p.FileIndex).FileName))
                                    {
                                        p.AudioPage = 1;
                                        p.FileIndex++;
                                    }
                                    Task.Delay(500).Wait();
                                }
                            }
                        }
                        else if (page is BookCompleted)
                        {
                            BookCompleted p = page as BookCompleted;
                            p.StartTime = ResumeTime;
                        }
                        else if (page is BookActivity)
                        {
                            BookActivity p = page as BookActivity;
                            p.StartTime = ResumeTime;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
