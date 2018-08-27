using Kitablet.Helpers;
using Kitablet.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Kitablet
{
    public partial class LoginPage : ContentPage
    {
        public bool EnvironmentVisible = true;
        public static Popup login_Popup;
        public static StackLayout PopupFadeout;
        public static ForgotPopup Popup;
        public static ReleaseDevicePopup ReleasePopup;
        long doublePressInterval_ms = 300;
        DateTime lastPressTime = DateTime.MinValue;
        Dictionary<string, int> dictionary = new Dictionary<string, int>();
        public static PageLoader page_Loader;

        public LoginPage(bool isLogout)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

            //this.LoginPageLayout.WidthRequest = Xamarin.Forms.Device.OnPlatform<double>(Constant.DeviceWidth, Constant.DeviceWidth / Constant.DeviceDensity, Constant.DeviceWidth);
            //this.LoginPageLayout.HeightRequest = Xamarin.Forms.Device.OnPlatform<double>(Constant.DeviceHeight, (Constant.DeviceHeight / Constant.DeviceDensity) - 25, Constant.DeviceHeight);

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += Environment_Click;
            schoolEnv.GestureRecognizers.Add(tapGestureRecognizer);
            homeEnv.GestureRecognizers.Add(tapGestureRecognizer);

            LoginPopUp.CancelPopupBtn.IsVisible = false;
            LoginPopUp.SubmitTextMessage.Text = "OK";
            var okPopuptap = new TapGestureRecognizer();
            okPopuptap.Tapped += (s, e) => {
                this.LoginPopUp.IsVisible = false;
                this.LoginPopUp.TextMessage.Text = string.Empty;
            };
            LoginPopUp.SubmitPopupBtn.GestureRecognizers.Add(okPopuptap);
            login_Popup = this.LoginPopUp;
            Popup = this.ForgotPopUp;
            //For Asus Tab
            if(Device.OS == TargetPlatform.Android)
            {
                LoginContainer.Padding = new Thickness(30, 55, 30, 20);
            }
            else
            {
                LoginContainer.Padding = new Thickness(30, 80, 30, 20);
            }
            ReleasePopup = this.ReleaseDevicePopup;
            if (LoginPage.page_Loader != null)
            {
                LoginPage.page_Loader = null;
            }           
            LoginPage.page_Loader = this.Page_Loader;
            //Release Popup cancel
            var tapset5 = new TapGestureRecognizer();
            tapset5.Tapped += (s, e) =>
            {
                LoginPage.ReleasePopup.IsVisible = false;
                LoginPage.ReleasePopup.DeviceIds.Clear();
                HelperFunctions._HelperFunctions.Logout();
            };
            LoginPage.ReleasePopup.CancelPopupBtn.GestureRecognizers.Add(tapset5);
            //Release Popup release
            var tapset6 = new TapGestureRecognizer();            
            tapset6.Tapped += (s, e) =>
            {
                string status = string.Empty;
                if (LoginPage.ReleasePopup.DeviceIds.Count != 0)
                {
                    LoginPage.page_Loader.IsVisible = true;
                    Task.Run(() => {
                        Task.Delay(100).Wait();
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                            string ids = string.Empty;
                            foreach (string id in LoginPage.ReleasePopup.DeviceIds)
                            {
                                ids += id + ",";
                            }
                            ids = ids.TrimEnd(',');
                            if (HelperFunctions.CheckInternetConnection())
                            {
                                LoginPage.ReleasePopup.SubmitPopupBtn.Opacity = 0.5;
                                status = MyWebRequest.PostRequest("releaseuserdevices", null, new { UserId = LoginPage.ReleasePopup.UserId, DeviceIds = ids }, null);
                                if (Boolean.Parse(status))
                                {
                                    AppData.DeviceDetail.Environment = "Home";
                                    AppData.DeviceDetail.UserId = LoginPage.ReleasePopup.UserId;
                                    status = MyWebRequest.PostRequest("AddDeviceInfo", null, AppData.DeviceDetail, null);
                                    AddDeviceModel deviceModel = JsonConvert.DeserializeObject<AddDeviceModel>(status);
                                    Constant.DeviceIdFromDB = deviceModel.DeviceId;
                                    if ((Xamarin.Forms.Device.OS == TargetPlatform.Android) || (Device.OS == TargetPlatform.iOS))
                                    {
                                        Helpers.CacheSettings.SettingsKey = "DeviceIdFromDB";
                                        Helpers.CacheSettings.GeneralSettings = deviceModel.DeviceId.ToString();
                                    }
                                    else
                                    {
                                        AppData.FileService.saveCacheDetails("DeviceIdFromDB", deviceModel.DeviceId.ToString());
                                    }
                                    UserLogin userLogin = (UserLogin)this.UserPage;
                                    if (AppData.User != null)
                                    {
                                        AppData.User.UserId = LoginPage.ReleasePopup.UserId;
                                    }
                                    userLogin.AddDeviceInfoFunction(deviceModel);
                                    LoginPage.ReleasePopup.SubmitPopupBtn.Opacity = 1;
                                }
                                else
                                {
                                    LoginPage.ReleasePopup.IsVisible = false;
                                    LoginPage.login_Popup.TextMessage.Text = "Release Device failed due to technical reasons.";
                                    LoginPage.login_Popup.IsVisible = true;
                                }
                            }
                            else
                            {
                                LoginPage.ReleasePopup.IsVisible = false;
                                LoginPage.login_Popup.TextMessage.Text = "Cannot connect to the Server. Check your Internet Connection.";
                                LoginPage.login_Popup.IsVisible = true;
                            }
                            LoginPage.page_Loader.IsVisible = false;
                        });
                    });                 
                }
            };
            LoginPage.ReleasePopup.SubmitPopupBtn.GestureRecognizers.Add(tapset6);
            //CANCEL BUTTON OF FORGOT POPUP
            var tapset3 = new TapGestureRecognizer();
            tapset3.Tapped += (s, e) =>
            {
                LoginPage.Popup.IsVisible = false;
                LoginPage.Popup.Action = 0;
            };
            LoginPage.Popup.CancelPopupBtn.GestureRecognizers.Add(tapset3);
            LoginPage.Popup.Cancelsend.GestureRecognizers.Add(tapset3);

            var tapset66 = new TapGestureRecognizer();
            tapset66.Tapped += (s, e) =>
            {
                Xamarin.Forms.Device.OpenUri(new Uri(Constant.justexploringlink));
            };
            AppLink.GestureRecognizers.Add(tapset66);

            //Send btn forgot popup
            var tapset7 = new TapGestureRecognizer();
            tapset7.Tapped += (s, e) =>
            {
               if(LoginPage.Popup.EmailIdsPicker.SelectedIndex != 0)
                {
                    string name = LoginPage.Popup.EmailIdsPicker.Items[LoginPage.Popup.EmailIdsPicker.SelectedIndex];
                    int userid = dictionary[name];
                    string username = LoginPage.Popup.EntryControl.Text;
                    string status = string.Empty;
                    status = MyWebRequest.PostRequest("sendschoolstudentpasswordrecovery", null, new { UserName = username, ToId = userid }, null);
                    UserPasswordRecovery userpassword = new UserPasswordRecovery();
                    userpassword = JsonConvert.DeserializeObject<UserPasswordRecovery>(status);
                    switch (userpassword.Status)
                    {
                        case 0:
                            LoginPage.Popup.IsVisible = false;
                            this.LoginPopUp.IsVisible = true;
                            this.LoginPopUp.TextMessage.Text = "Login failed due to technical reasons.";
                            break;
                        case 1:
                            LoginPage.Popup.IsVisible = false;
                            this.LoginPopUp.IsVisible = true;
                            this.LoginPopUp.TextMessage.Text = "Check your email to get the username.";
                            break;
                    }
                }
                else
                {
                    LoginPage.Popup.SendBtn.Opacity = 0.5;
                }
            };
            LoginPage.Popup.SendBtn.GestureRecognizers.Add(tapset7);

            //Submitbtn forgot popup
            var tapset4 = new TapGestureRecognizer();
            tapset4.Tapped += (s, e) =>
            {
                try
                {
                    string Input = LoginPage.Popup.EntryControl.Text;
                    string status = string.Empty;
                    if (string.IsNullOrEmpty(Input))
                    {
                        LoginPage.Popup.ErrorMessage.Text = "Please enter text";
                    }
                    else
                    {
                        LoginPage.Popup.SubmitPopupBtn.Opacity = 0.5;
                        if (HelperFunctions.CheckInternetConnection())
                        {
                            switch (LoginPage.Popup.Action)
                            {
                                case 1:
                                    status = MyWebRequest.PostRequest("usernamerecovery", null, new { Email = Input }, null);
                                    switch (Int32.Parse(status))
                                    {
                                        case 0:
                                            LoginPage.Popup.ErrorMessage.Text = "Login failed due to technical reasons.";
                                            break;
                                        case 1:
                                            LoginPage.Popup.IsVisible = false;
                                            this.LoginPopUp.IsVisible = true;
                                            this.LoginPopUp.TextMessage.Text = "Check your email to get the username.";
                                            break;
                                        case 2:
                                            LoginPage.Popup.ErrorMessage.Text = "This user account does not exist.";
                                            break;
                                        case 3:
                                            LoginPage.Popup.ErrorMessage.Text = "This user account is not active.";
                                            break;
                                        case 4:
                                            LoginPage.Popup.ErrorMessage.Text = "This user account has been disabled.";
                                            break;
                                        case 5:
                                            break;
                                    };
                                    break;
                                case 2:
                                    string env = string.Empty;
                                    if(HelperFunctions.getCacheData("DeviceEnvironment")=="School")
                                    {
                                        env = "School";
                                    }
                                    else
                                    {
                                        env = "Home";
                                    }
                                    status = MyWebRequest.PostRequest("sendpasswordrecoveryreader", null, new { Username = Input, Url = string.Empty, Environment = env }, null);
                                    UserPasswordRecovery userpassword = new UserPasswordRecovery();
                                    userpassword = JsonConvert.DeserializeObject<UserPasswordRecovery>(status);
                                    switch (userpassword.Status)
                                    {
                                        case 0:
                                            LoginPage.Popup.ErrorMessage.Text = "Login failed due to technical reasons.";
                                            break;
                                        case 1:
                                            LoginPage.Popup.IsVisible = false;
                                            this.LoginPopUp.IsVisible = true;
                                            this.LoginPopUp.TextMessage.Text = "Check your email to get the link to reset password.";
                                            break;
                                        case 3:
                                            LoginPage.Popup.ErrorMessage.Text = "This user account does not exist.";
                                            break;
                                        case 4:
                                            // LoginPage.Popup.ErrorMessage.Text = "Wrong Credentials. Try Again.";
                                            LoginPage.Popup.ErrorMessage.Text = "Doesn’t seem like your secret code - try again";
                                            break;
                                        case 6:
                                            LoginPage.Popup.ErrorMessage.Text = "This user account is not active.";
                                            break;
                                        case 7:
                                            LoginPage.Popup.ErrorMessage.Text = "This user account has been disabled.";
                                            break;
                                        case 8:
                                            LoginPage.Popup.ErrorMessage.Text = "Sorry, you cannot login. Try Again.";
                                            break;
                                        case 9:
                                            LoginPage.Popup.ErrorMessage.Text = "Subscription expired";
                                            break;
                                        case 10:
                                            LoginPage.Popup.InputStackParent.IsVisible = false;
                                            getEmailIds(userpassword);
                                            LoginPage.Popup.EmailIdStackParent.IsVisible = true;
                                            break;
                                    };
                                    break;
                                case 3: //Admin username
                                    status = MyWebRequest.PostRequest("usernamerecovery", null, new { Email = Input }, null);
                                    switch (Int32.Parse(status))
                                    {
                                        case 0:
                                            LoginPage.Popup.ErrorMessage.Text = "Login failed due to technical reasons.";
                                            break;
                                        case 1:
                                            LoginPage.Popup.IsVisible = false;
                                            this.LoginPopUp.IsVisible = true;
                                            this.LoginPopUp.TextMessage.Text = "Check your email to get the username.";
                                            break;
                                        case 2:
                                            LoginPage.Popup.ErrorMessage.Text = "This user account does not exist.";
                                            break;
                                        case 3:
                                            LoginPage.Popup.ErrorMessage.Text = "This user account is not active.";
                                            break;
                                        case 4:
                                            LoginPage.Popup.ErrorMessage.Text = "This user account has been disabled.";
                                            break;
                                        case 5:
                                            break;
                                    };
                                    break;
                                case 4: //Admin pwd
                                    status = MyWebRequest.PostRequest("sendpasswordrecovery", null, new { Username = Input, Url = string.Empty }, null);
                                    status = status.Replace("\"", "");
                                    switch (Int32.Parse(status))
                                    {
                                        case 0:
                                            LoginPage.Popup.ErrorMessage.Text = "Login failed due to technical reasons.";
                                            break;
                                        case 1:
                                            LoginPage.Popup.IsVisible = false;
                                            this.LoginPopUp.IsVisible = true;
                                            this.LoginPopUp.TextMessage.Text = "Check your email to get the link to reset password.";
                                            break;
                                        case 3:
                                            LoginPage.Popup.ErrorMessage.Text = "This user account does not exist.";
                                            break;
                                        case 4:
                                            //LoginPage.Popup.ErrorMessage.Text = "Wrong Credentials. Try Again.";
                                            LoginPage.Popup.ErrorMessage.Text = "Doesn’t seem like your secret code - try again";
                                            break;
                                        case 6:
                                            LoginPage.Popup.ErrorMessage.Text = "This user account is not active.";
                                            break;
                                        case 7:
                                            LoginPage.Popup.ErrorMessage.Text = "This user account has been disabled.";
                                            break;
                                        case 8:
                                            LoginPage.Popup.ErrorMessage.Text = "Sorry, you cannot login. Try Again.";
                                            break;
                                        case 9:
                                            LoginPage.Popup.ErrorMessage.Text = "Subscription expired";
                                            break;
                                    };
                                    //   HelperFunctions.PasswordRecovery(Input);
                                    break;
                            };
                        }
                        else
                        {
                            LoginPage.Popup.IsVisible = false;
                            LoginPage.login_Popup.TextMessage.Text = "Cannot connect to the Server. Check your Internet Connection.";
                            LoginPage.login_Popup.IsVisible = true;
                        }
                        LoginPage.Popup.SubmitPopupBtn.Opacity = 1;
                    }
                }
                catch (Exception e1) { }
            };
            LoginPage.Popup.SubmitPopupBtn.GestureRecognizers.Add(tapset4);

            if (isLogout)
            {
                if (HelperFunctions.checkCacheExist("DeviceEnvironment"))
                {
                    this.EnvironmentVisible = false;
                    this.CheckLogin();
                }
            }
        }

        public void Environment_Click(object s, EventArgs e)
        {
            StackLayout layout = ((StackLayout)s);
            if (layout != null)
            {
                if (EnvironmentContainer.Margin.Left != 0)
                {
                    var animation = new Animation(v => EnvironmentContainer.Margin = new Thickness(v, 0, 0, 0), 230, 0);
                    animation.Commit(this, "MarginAnimation", 0, 300, Easing.Linear, (v, c) => EnvironmentContainer.Margin = new Thickness(0), () => false);
                }
                switch (layout.StyleId)
                {
                    case "0":
                        layout.Opacity = 1;
                        ((BoxView)layout.Children[1]).SetDynamicResource(BoxView.BackgroundColorProperty, "ActiveBorderColor");
                        ((Image)((Grid)layout.Children[0]).Children[2]).IsVisible = true;
                        homeEnv.Opacity = 0.5;
                        ((BoxView)homeEnv.Children[1]).SetDynamicResource(BoxView.BackgroundColorProperty, "TransparentBorderColor");
                        ((Image)((Grid)homeEnv.Children[0]).Children[2]).IsVisible = false;

                        UserPage.IsVisible = false;
                        AdminPage.Opacity = 0;
                        ClearAdminFields();
                        AdminPage.IsVisible = true;
                        AdminPage.FadeTo(1, 300, Easing.SinIn);
                        break;
                    case "1":
                        layout.Opacity = 1;
                        ((BoxView)layout.Children[1]).SetDynamicResource(BoxView.BackgroundColorProperty, "ActiveBorderColor");
                        ((Image)((Grid)layout.Children[0]).Children[2]).IsVisible = true;
                        schoolEnv.Opacity = 0.5;
                        ((BoxView)schoolEnv.Children[1]).SetDynamicResource(BoxView.BackgroundColorProperty, "TransparentBorderColor");
                        ((Image)((Grid)schoolEnv.Children[0]).Children[2]).IsVisible = false;

                        AdminPage.IsVisible = false;
                        UserPage.Opacity = 0;
                        ClearUserFields();
                        UserPage.IsVisible = true;
                        UserPage.FadeTo(1, 300, Easing.SinIn);
                        if (!this.EnvironmentVisible)
                        {
                            if (HelperFunctions.getCacheData("DeviceEnvironment") == "School")
                            {
                                UserPage.Children[0].FindByName<StackLayout>("rememberMeControlUser").IsVisible = false;
                                UserPage.Children[0].FindByName<Label>("justExploreUser").IsVisible = false;
                            }
                        }
                        break;
                    case "2":
                        UserPage.IsVisible = false;
                        AdminPage.IsVisible = false;
                        ChangeAvatar obj = new ChangeAvatar();
                        if (Xamarin.Forms.Device.OS == TargetPlatform.Android)
                        {
                            obj.Margin = new Thickness(150, 0, 0, 0);
                        }
                        else
                        {
                            obj.Margin = new Thickness(230, 0, 0, 0);
                        }
                        obj.Opacity = 0;
                        Grid.SetRow(obj, 0);
                        Grid.SetColumn(obj, 0);
                        this.LoginContainer.Children.Add(obj);
                        obj.FadeTo(1, 300, Easing.SinIn);
                        break;
                    default:
                        break;
                }
                if (!this.EnvironmentVisible)
                {
                    ((StackLayout)layout.Parent).IsVisible = false;
                }
            }
        }

        private void ClearAdminFields()
        {
            try
            {
                AdminPage.Children[0].FindByName<MyEntry>("usernameEntry").Text = string.Empty;
                AdminPage.Children[0].FindByName<Label>("errorAdminName").Text = string.Empty;
                AdminPage.Children[0].FindByName<MyEntry>("passwordEntry").Text = string.Empty;
                AdminPage.Children[0].FindByName<Label>("errorAdminPassword").Text = string.Empty;
            }
            catch(Exception ex) { }
        }
        private void ClearUserFields()
        {
            try
            {
                UserPage.Children[0].FindByName<MyEntry>("usernameEntry").Text = string.Empty;
                UserPage.Children[0].FindByName<Label>("errorUserName").Text = string.Empty;
                UserPage.Children[0].FindByName<MyEntry>("passwordEntry").Text = string.Empty;
                UserPage.Children[0].FindByName<Label>("errorUserPassword").Text = string.Empty;
            }
            catch(Exception ex) { }
        }

        public void CheckLogin()
        {
            bool isconnect = true;
            try
            {
                if (HelperFunctions.getCacheData("DeviceEnvironment") == "School")
                {
                    Constant.IsRememberMe = false;
                    Constant.DeviceIdFromDB = Int32.Parse(HelperFunctions.getCacheData("DeviceIdFromDB"));
                    isconnect = HelperFunctions.CheckInternetConnection();
                    if (isconnect)
                    {

                        this.Environment_Click(homeEnv, new EventArgs());
                    }
                    else
                    {

                        if (HelperFunctions.checkCacheExist("UserLoginId"))
                        {
                            if (!this.isAvatarSet())
                            {
                                this.EnvironmentVisible = false;
                                this.Environment_Click(changeAvatar, new EventArgs());
                            }
                            else
                            {
                                ThemeChanger.SetGrade();
                                ThemeChanger.changeTheme();
                                AppData.InitailizeUserDetails();
                                AppData.InitailizeUserProgress();
                                Navigation.PushAsync(new ActionTabPage());
                                Navigation.RemovePage(this);
                            }
                        }
                        else
                        {
                            this.Environment_Click(homeEnv, new EventArgs());
                        }
                    }
                }
                else
                {
                    Constant.IsRememberMe = true;
                    if (HelperFunctions.checkCacheExist("UserLoginId"))
                    {                       
                        Constant.DeviceIdFromDB = Int32.Parse(HelperFunctions.getCacheData("DeviceIdFromDB"));
                        isconnect = HelperFunctions.CheckInternetConnection();
                        if (isconnect)
                        {
                            HelperFunctions.saveCacheData("LastLoginDate", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture));
                            if (!this.isAvatarSet())
                            {
                                this.EnvironmentVisible = false;
                                this.Environment_Click(changeAvatar, new EventArgs());
                            }
                            else
                            {
                                ThemeChanger.SetGrade();
                                ThemeChanger.changeTheme();
                                AppData.InitailizeUserDetails();
                                AppData.InitailizeUserProgress();
                                Navigation.PushAsync(new ActionTabPage());
                                Navigation.RemovePage(this);
                            }
                        }
                        else
                        {
                            DateTime lastLoginDate = Convert.ToDateTime(HelperFunctions.getCacheData("LastLoginDate"));
                            DateTime currentDate = DateTime.Now;
                            double count = Math.Abs((currentDate - lastLoginDate).TotalDays);                            
                            if (count > Constant.numberOfDaysSinceLastLogin)
                            {
                                HelperFunctions._HelperFunctions.Logout();
                                this.Environment_Click(homeEnv, new EventArgs());
                                this.LoginPopUp.TextMessage.Text = "It's been 15 days since I saw you online - you now need to connect to the internet & login again";
                                this.LoginPopUp.IsVisible = true;
                            }
                            else
                            {
                                if (!this.isAvatarSet())
                                {
                                    this.EnvironmentVisible = false;
                                    this.Environment_Click(changeAvatar, new EventArgs());
                                }
                                else
                                {
                                    ThemeChanger.SetGrade();
                                    ThemeChanger.changeTheme();
                                    AppData.InitailizeUserDetails();
                                    AppData.InitailizeUserProgress();
                                    Navigation.PushAsync(new ActionTabPage());
                                    Navigation.RemovePage(this);
                                }
                            }
                        }
                    }
                    else
                    {
                        this.Environment_Click(homeEnv, new EventArgs());
                    }
                }
            }
            catch (Exception ex)
            { }
        }

        public void getEmailIds(UserPasswordRecovery userpassword)
        {
            dictionary.Clear();
            LoginPage.Popup.EmailIdsPicker.Items.Clear();
            foreach (PasswordRecoveryUsers user in userpassword.Users)
            {
                dictionary.Add(user.FirstName +" "+ user.LastName,user.UserId);
            }
            LoginPage.Popup.EmailIdsPicker.Items.Add("Select");
            foreach (string name in dictionary.Keys)
            {
                LoginPage.Popup.EmailIdsPicker.Items.Add(name);
            }
            LoginPage.Popup.EmailIdsPicker.SelectedIndex = 0;
            LoginPage.Popup.SendBtn.Opacity = 0.5;
            LoginPage.Popup.EmailIdsPicker.SelectedIndexChanged += (sender, args) =>
            {
                if (LoginPage.Popup.EmailIdsPicker.SelectedIndex == 0)
                {
                    LoginPage.Popup.SendBtn.Opacity = 0.5 ;
                }
                else
                {
                    LoginPage.Popup.SendBtn.Opacity = 1;
                }
            };
        }

        public bool isAvatarSet()
        {
            try
            {
                if(AppData.User != null)
                {
                    if (AppData.User.AvatarId != 0)
                    {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        protected override bool OnBackButtonPressed()
        {
            bool isBack = false;
            try
            {
                DateTime pressTime = DateTime.Now;
               
                if ((pressTime - lastPressTime).TotalMilliseconds <= doublePressInterval_ms)
                {
                    if (Device.OS == TargetPlatform.Android)
                    {
                        isBack = false;
                    }
                }
                else
                {
                    if (Device.OS == TargetPlatform.Android)
                    {
                        XFToast.ShortMessage("Press again to exit");
                        isBack = true;
                    }
                }
                lastPressTime = pressTime;
            }
            catch(Exception ex) { }
            return isBack;
        }
    }
}
