using Kitablet.ViewModels;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Kitablet
{
    public partial class UserLogin : Grid
    {
        public UserLogin()
        {
            InitializeComponent();

            usernameEntry.Focused += (s, e) =>
            {
                this.setFocus(1);
                this.setFocus(4);
            };
            usernameEntry.Unfocused += (s, e) =>
            {
                this.setFocus(2);
            };

            passwordEntry.Focused += (s, e) =>
            {
                this.setFocus(2);
                this.setFocus(3);
            };
            passwordEntry.Unfocused += (s, e) =>
            {
                this.setFocus(4);
            };

            usernameEntry.Completed += (sender, e) =>
            {
                OnLoginButtonClicked(buttonLogin, new EventArgs());
            };

            passwordEntry.Completed += (sender, e) =>
            {
                OnLoginButtonClicked(buttonLogin, new EventArgs());
            };

            var tapEnter = new TapGestureRecognizer();
            tapEnter.Tapped += OnLoginButtonClicked;
            buttonLogin.GestureRecognizers.Add(tapEnter);

            var tapset = new TapGestureRecognizer();
            tapset.Tapped += (s, e) =>
            {
                Xamarin.Forms.Device.OpenUri(new Uri(Constant.justexploringlink));
            };
            justExploreUser.GestureRecognizers.Add(tapset);

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) =>
            {
                Image img = (Image)s;
                if (img.StyleId == "0")
                {
                    img.SetDynamicResource(Image.SourceProperty, "RememberMeSelectedImage");
                    img.StyleId = "1";
                }
                else
                {
                    img.SetDynamicResource(Image.SourceProperty, "RememberMeImage");
                    img.StyleId = "0";
                }
            };
            rememberMe.GestureRecognizers.Add(tapGestureRecognizer);

            var tapset2 = new TapGestureRecognizer();
            tapset2.Tapped += (s, e) =>
            {
                LoginPage.Popup.InputStackParent.IsVisible = true;
                LoginPage.Popup.EmailIdStackParent.IsVisible = false;
                LoginPage.Popup.IsVisible = true;
                LoginPage.Popup.CancelTextMessage.Text = "CANCEL";
                LoginPage.Popup.SubmitTextMessage.Text = "RECOVER";
                LoginPage.Popup.EntryControl.Text = string.Empty;
                LoginPage.Popup.ErrorMessage.Text = string.Empty;
                LoginPage.Popup.EntryControl.Placeholder = "Enter Email ID";
                LoginPage.Popup.Action = 1;
            };
            forgotId.GestureRecognizers.Add(tapset2);

            var tapset3 = new TapGestureRecognizer();
            tapset3.Tapped += (s, e) =>
            {
                LoginPage.Popup.InputStackParent.IsVisible = true;
                LoginPage.Popup.EmailIdStackParent.IsVisible = false;
                LoginPage.Popup.IsVisible = true;
                LoginPage.Popup.CancelTextMessage.Text = "CANCEL";
                LoginPage.Popup.SubmitTextMessage.Text = "RECOVER";
                LoginPage.Popup.EntryControl.Text = string.Empty;
                LoginPage.Popup.ErrorMessage.Text = string.Empty;
                LoginPage.Popup.EntryControl.Placeholder = "Enter Username";
                LoginPage.Popup.Action = 2;
            };
            forgotSecretCode.GestureRecognizers.Add(tapset3);

            if ((Xamarin.Forms.Device.OS == TargetPlatform.Android) || (Xamarin.Forms.Device.OS == TargetPlatform.iOS))
            {
                errorUserPassword.Margin = new Thickness(0, 0, 5, -15);
                errorUserName.Margin = new Thickness(0, 0, 5, -15);
            }
            else if (Xamarin.Forms.Device.OS == TargetPlatform.Windows)
            {
                errorUserPassword.Margin = new Thickness(0, 0, 5, -18);
                errorUserName.Margin = new Thickness(0, 0, 5, -18);
            }

        }
        public void setFocus(int i)
        {
            switch (i)
            {
                case 1:
                    ((AbsoluteLayout)usernameEntry.Parent).Opacity = 1;
                    usernameEntry.Focus();
                    borderUserName.Opacity = 1;

                    break;
                case 2:
                    ((AbsoluteLayout)usernameEntry.Parent).Opacity = 0.5;
                    borderUserName.Opacity = 0.5;
                    break;
                case 3:
                    ((AbsoluteLayout)passwordEntry.Parent).Opacity = 1;
                    passwordEntry.Focus();
                    borderUserPass.Opacity = 1;
                    break;
                case 4:
                    ((AbsoluteLayout)passwordEntry.Parent).Opacity = 0.5;
                    borderUserPass.Opacity = 0.5;
                    break;
                default:
                    ((AbsoluteLayout)usernameEntry.Parent).Opacity = 1;
                    usernameEntry.Focus();
                    borderUserName.Opacity = 1;
                    break;
            }
        }
        public void OnLoginButtonClicked(object sender, EventArgs e)
        {
            this.OnTextChanged(new object(), new EventArgs());
            bool isconnect = false;
            if (string.IsNullOrEmpty(usernameEntry.Text))
            {
                errorUserName.IsVisible = true;
                errorUserName.Text = "Enter Username";
                this.setFocus(1);
            }
            else if (string.IsNullOrEmpty(passwordEntry.Text))
            {
                errorUserPassword.IsVisible = true;
                errorUserPassword.Text = "Enter Secret Code";
                this.setFocus(3);
            }
            else
            {
                try
                {
                    LoginPage.page_Loader.IsVisible = true;
                    Task.Run(() =>
                    {
                        Task.Delay(100).Wait();
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                        {
                            isconnect = HelperFunctions.CheckInternetConnection();
                            if (isconnect)
                            {
                                buttonLogin.Opacity = 0.5;
                                LoadingText.IsVisible = true;
                                string status1 = MyWebRequest.PostRequest("login", null, new { Username = usernameEntry.Text, Password = passwordEntry.Text }, null);
                                string message = string.Empty;
                                LoginObject user = JsonConvert.DeserializeObject<LoginObject>(status1);
                                if (user.Status == 1)
                                {
                                    message = "";
                                }
                                else if (user.Status == 3)
                                {
                                    message = "Wrong user name or password - try again.";
                                }
                                else if (user.Status == 4)
                                {
                                    // message = "Wrong Credentials. Try Again.";
                                    message = "Doesn’t seem like your secret code - try again";
                                }
                                else if (user.Status == 6)
                                {
                                    message = "Registration pending – complete that first";
                                }
                                else if (user.Status == 7)
                                {
                                    message = "This user account has been disabled.";
                                }
                                else if (user.Status == 8)
                                {
                                    message = "Parent log-in don’t work on Reader App";
                                }
                                else if (user.Status == 9)
                                {
                                    message = "Subscription expired.";

                                }
                                else
                                {
                                    message = "Oops! I was unable to log you in - can you try again?";
                                }
                                UserLoginFunction(user, message);
                                //HelperFunctions.login(usernameEntry.Text, passwordEntry.Text,this);
                            }
                            else
                            {
                                LoginPage.login_Popup.TextMessage.Text = "Cannot connect to the Server. Check your Internet Connection.";
                                LoginPage.login_Popup.IsVisible = true;
                                LoginPage.page_Loader.IsVisible = false;
                            }
                        });
                    });
                }
                catch (Exception ex) { }
            }
        }
        public void UserLoginFunction(LoginObject login, string message)
        {
            if (login.Status == 1)
            {
                message = "";
                AppData.User = login.User != null ? login.User : AppData.User;

                if (HelperFunctions.getCacheData("DeviceEnvironment") == "School")
                {
                    Constant.IsRememberMe = false;
                    if ((Xamarin.Forms.Device.OS == TargetPlatform.Android) || (Device.OS == TargetPlatform.iOS))
                    {
                        string userString = JsonConvert.SerializeObject(AppData.User);
                        Helpers.CacheSettings.SettingsKey = "UserDetails";
                        Helpers.CacheSettings.GeneralSettings = userString;
                        Helpers.CacheSettings.SettingsKey = "LastLoginDate";
                        Helpers.CacheSettings.GeneralSettings = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture);
                        Helpers.CacheSettings.SettingsKey = "UserLoginId";
                        Helpers.CacheSettings.GeneralSettings = AppData.User.Username;
                        Helpers.CacheSettings.SettingsKey = "UserLoginPassword";
                        Helpers.CacheSettings.GeneralSettings = AppData.User.Password;
                    }
                    else
                    {
                        AppData.FileService.saveUserDetails("UserDetails", AppData.User);
                        AppData.FileService.saveCacheDetails("LastLoginDate", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture));
                        AppData.FileService.saveCacheDetails("UserLoginId", AppData.User.Username);
                        AppData.FileService.saveCacheDetails("UserLoginPassword", AppData.User.Password);
                    }
                    if (AppData.User.AvatarId == 0)
                    {
                        LoginPage loginpage = ((LoginPage)Navigation.NavigationStack.ElementAt(0));
                        loginpage.EnvironmentVisible = false;
                        loginpage.Environment_Click(loginpage.FindByName<StackLayout>("changeAvatar"), new EventArgs());
                    }
                    else
                    {
                        ThemeChanger.SetGrade();
                        ThemeChanger.changeTheme();
                        AppData.InitailizeUserDetails();
                        AppData.InitailizeUserProgress();
                        Navigation.PushAsync(new ActionTabPage());
                        Navigation.RemovePage(Navigation.NavigationStack.ElementAt(0));
                    }
                }
                else
                {
                    if (rememberMe.StyleId == "1")
                    {
                        Constant.IsRememberMe = true;
                        if ((Xamarin.Forms.Device.OS == TargetPlatform.Android) || (Device.OS == TargetPlatform.iOS))
                        {
                            Helpers.CacheSettings.SettingsKey = "UserLoginId";
                            Helpers.CacheSettings.GeneralSettings = AppData.User.Username;
                            Helpers.CacheSettings.SettingsKey = "UserLoginPassword";
                            Helpers.CacheSettings.GeneralSettings = AppData.User.Password;
                        }
                        else
                        {
                            AppData.FileService.saveCacheDetails("UserLoginId", AppData.User.Username);
                            AppData.FileService.saveCacheDetails("UserLoginPassword", AppData.User.Password);
                        }
                    }
                    else
                    {
                        Constant.IsRememberMe = false;
                    }

                    AppData.DeviceDetail.Environment = "Home";
                    AppData.DeviceDetail.UserId = AppData.User.UserId;
                    string status = MyWebRequest.PostRequest("AddDeviceInfo", null, AppData.DeviceDetail, null);
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
                    if (AppData.User != null)
                    {
                        AppData.User.UserId = AppData.User.UserId;
                    }
                    AddDeviceInfoFunction(deviceModel);
                }
            }
            else if (login.Status == 3)
            {
                errorUserName.IsVisible = true;
                errorUserName.Text = message;
                this.setFocus(1);
            }
            else if (login.Status == 4)
            {
                errorUserPassword.IsVisible = true;
                errorUserPassword.Text = message;
                this.setFocus(3);
            }
            else
            {
                errorUserName.IsVisible = true;
                errorUserName.Text = message;
                this.setFocus(1);
            }
            LoadingText.IsVisible = false;
            buttonLogin.Opacity = 1;
            LoginPage.page_Loader.IsVisible = false;
        }
        public void AddDeviceInfoFunction(AddDeviceModel deviceModel)
        {
            switch (deviceModel.Status)
            {
                case DeviceStatus.Error:
                    break;
                case DeviceStatus.Sucess:
                    if ((Xamarin.Forms.Device.OS == TargetPlatform.Android) || (Device.OS == TargetPlatform.iOS))
                    {
                        Helpers.CacheSettings.SettingsKey = "DeviceEnvironment";
                        if (string.IsNullOrEmpty(Helpers.CacheSettings.GeneralSettings))
                        {
                            Helpers.CacheSettings.GeneralSettings = "Home";
                        }
                        string userString = JsonConvert.SerializeObject(AppData.User);
                        Helpers.CacheSettings.SettingsKey = "UserDetails";
                        Helpers.CacheSettings.GeneralSettings = userString;
                        Helpers.CacheSettings.SettingsKey = "LastLoginDate";
                        Helpers.CacheSettings.GeneralSettings = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(HelperFunctions.getCacheData("DeviceEnvironment")))
                        {
                            AppData.FileService.saveCacheDetails("DeviceEnvironment", "Home");
                        }
                        AppData.FileService.saveUserDetails("UserDetails", AppData.User);
                        AppData.FileService.saveCacheDetails("LastLoginDate", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture));
                    }
                    if (AppData.User.AvatarId == 0)
                    {
                        LoginPage loginpage = ((LoginPage)Navigation.NavigationStack.ElementAt(0));
                        loginpage.EnvironmentVisible = false;
                        loginpage.Environment_Click(loginpage.FindByName<StackLayout>("changeAvatar"), new EventArgs());
                    }
                    else
                    {
                        ThemeChanger.SetGrade();
                        ThemeChanger.changeTheme();
                        AppData.InitailizeUserDetails();
                        AppData.InitailizeUserProgress();
                        Navigation.PushAsync(new ActionTabPage());
                        Navigation.RemovePage(Navigation.NavigationStack.ElementAt(0));
                    }
                    break;
                case DeviceStatus.DeviceAlreadyHave:
                    PaintDevices(deviceModel, AppData.User.UserId);
                    LoginPage.ReleasePopup.IsVisible = true;
                    break;
            }
            LoginPage.ReleasePopup.SubmitPopupBtn.Opacity = 1;
        }
        private void PaintDevices(AddDeviceModel deviceModel, int id)
        {
            LoginPage.ReleasePopup.Devicestorelease.Children.Clear();
            LoginPage.ReleasePopup.UserId = id;
            LoginPage.ReleasePopup.DeviceIds.Clear();
            foreach (DeviceDetail device in deviceModel.Devices)
            {
                StackLayout stack = new StackLayout { HorizontalOptions = LayoutOptions.Start, Orientation = StackOrientation.Horizontal, Spacing = 10 };
                Image img = new Image { HeightRequest = 18, WidthRequest = 18, Margin = new Thickness(5, 2, 5, 2), VerticalOptions = LayoutOptions.Center };
                img.SetDynamicResource(Image.SourceProperty, "RememberMeImage");
                img.StyleId = "0";
                img.ClassId = device.DeviceId.ToString();
                var tapEnter = new TapGestureRecognizer();
                tapEnter.Tapped += TapEnter_Tapped;
                img.GestureRecognizers.Add(tapEnter);
                Label lbl = new Label { Text = device.DeviceName, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Start, Margin = new Thickness(2, 0, 0, 0), LineBreakMode = LineBreakMode.TailTruncation, WidthRequest = 500 };
                lbl.SetDynamicResource(Label.StyleProperty, "LabelNormalRegular");
                stack.Children.Add(img);
                stack.Children.Add(lbl);
                LoginPage.ReleasePopup.Devicestorelease.Children.Add(stack);
            }
        }
        public void DeviceReleased(bool isReleased, int UserId)
        {
            if (isReleased)
            {
                AppData.DeviceDetail.Environment = "Home";
                AppData.DeviceDetail.UserId = UserId;
                string status = MyWebRequest.PostRequest("AddDeviceInfo", null, AppData.DeviceDetail, null);
                AddDeviceModel deviceModel = JsonConvert.DeserializeObject<AddDeviceModel>(status);
                AddDeviceInfoFunction(deviceModel);
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
            }
            else
            {
                LoginPage.ReleasePopup.IsVisible = false;
            }
        }
        private void TapEnter_Tapped(object sender, EventArgs e)
        {
            Image img = (Image)sender;
            if (img.StyleId == "0")
            {
                img.StyleId = "1";
                img.SetDynamicResource(Image.SourceProperty, "RememberMeSelectedImage");
                LoginPage.ReleasePopup.DeviceIds.Add(img.ClassId);
            }
            else
            {
                img.StyleId = "0";
                img.SetDynamicResource(Image.SourceProperty, "RememberMeImage");
                LoginPage.ReleasePopup.DeviceIds.Remove(img.ClassId);
            }
        }
        public void CallBackDevice(bool isvalid, LoginObject login)
        {
            if (isvalid)
            {
            }
        }
        public void OnTextChanged(object sender, EventArgs e)
        {
            try
            {
                Entry box = ((Entry)sender);
                if (box == usernameEntry)
                {
                    this.errorUserName.IsVisible = false;
                }
                else if (box == passwordEntry)
                {
                    this.errorUserPassword.IsVisible = false;
                }
                else
                {
                    this.errorUserName.IsVisible = false;
                    this.errorUserPassword.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                this.errorUserName.IsVisible = false;
                this.errorUserPassword.IsVisible = false;
            }
        }

    }
}
