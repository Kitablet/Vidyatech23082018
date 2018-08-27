using Kitablet.ViewModels;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Kitablet
{
    public partial class AdminLogin : Grid
    {
        public AdminLogin()
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
            justExplore.GestureRecognizers.Add(tapset);
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
                LoginPage.Popup.Action = 3;
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
                LoginPage.Popup.Action = 4;
            };
            forgotSecretCode.GestureRecognizers.Add(tapset3);

            if ((Xamarin.Forms.Device.OS == TargetPlatform.Android) || (Device.OS == TargetPlatform.iOS))
            {
                errorAdminPassword.Margin = new Thickness(0, 0, 5, -15);
                errorAdminName.Margin = new Thickness(0, 0, 5, -15);
            }
            else if (Xamarin.Forms.Device.OS == TargetPlatform.Windows)
            {
                errorAdminPassword.Margin = new Thickness(0, 0, 5, -18);
                errorAdminName.Margin = new Thickness(0, 0, 5, -18);
            }
        }
        public void setFocus(int i)
        {
            switch (i)
            {
                case 1:
                    ((AbsoluteLayout)usernameEntry.Parent).Opacity = 1;
                    usernameEntry.Focus();
                    borderAdminName.Opacity = 1;
                    break;
                case 2:
                    ((AbsoluteLayout)usernameEntry.Parent).Opacity = 0.5;
                    borderAdminName.Opacity = 0.5;
                    break;
                case 3:
                    ((AbsoluteLayout)passwordEntry.Parent).Opacity = 1;
                    passwordEntry.Focus();
                    borderAdminPass.Opacity = 1;
                    break;
                case 4:
                    ((AbsoluteLayout)passwordEntry.Parent).Opacity = 0.5;
                    borderAdminPass.Opacity = 0.5;
                    break;
                default:
                    ((AbsoluteLayout)usernameEntry.Parent).Opacity = 1;
                    usernameEntry.Focus();
                    borderAdminName.Opacity = 1;
                    break;
            }
        }
        public void OnLoginButtonClicked(object sender, EventArgs e)
        {
            this.OnTextChanged(new object(), new EventArgs());
            bool isconnect = false;
            if (string.IsNullOrEmpty(usernameEntry.Text))
            {
                errorAdminName.IsVisible = true;
                errorAdminName.Text = "Enter Admin Id";
                this.setFocus(1);
            }
            else if (string.IsNullOrEmpty(passwordEntry.Text))
            {
                errorAdminPassword.IsVisible = true;
                errorAdminPassword.Text = "Enter Password";
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
                            bool isValid = false;
                            string message = string.Empty;
                            int resint = 0;
                            if (isconnect)
                            {
                                buttonLogin.Opacity = 0.5;
                                LoadingText.IsVisible = true;
                                string result = MyWebRequest.PostRequest("validateschooladmin", null, new { Username = usernameEntry.Text, Password = passwordEntry.Text }, null);
                                if (result != null)
                                {
                                    resint = Int32.Parse(result);
                                    if (resint == 1)
                                    {
                                        isValid = true;

                                    }
                                    else if (resint == 3)
                                    {
                                        message = "Wrong user name or password - try again.";

                                    }
                                    else if (resint == 4)
                                    {
                                        // message = "Wrong Credentials. Try Again.";
                                        message = "Doesn’t seem like your secret code - try again";
                                    }
                                    else if (resint == 6)
                                    {
                                        message = "Registration pending – complete that first";

                                    }
                                    else if (resint == 7)
                                    {
                                        message = "This user account has been disabled.";

                                    }
                                    else if (resint == 8)
                                    {
                                        message = "Sorry, you cannot login. Try Again.";

                                    }
                                    else if (resint == 9)
                                    {
                                        message = "Subscription expired.";

                                    }
                                    else
                                    {
                                        message = "Oops! I was unable to log you in - can you try again?";

                                    }
                                }
                                AdminFunctions(isValid, message, resint);
                                //HelperFunctions.CheckAdminValid(usernameEntry.Text, passwordEntry.Text, this);
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
                catch (Exception ex)
                {
                }
            }
        }
        public void AdminFunctions(bool isvalid, string message, int status)
        {
            if (isvalid)
            {
                LoginObject login = new LoginObject();
                AppData.DeviceDetail.Environment = "School";
                AppData.DeviceDetail.UserId = -1;
                string status1 = MyWebRequest.PostRequest("AddDeviceInfo", null, AppData.DeviceDetail, null);
                AddDeviceModel deviceModel = JsonConvert.DeserializeObject<AddDeviceModel>(status1);
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
                CallBackDevice(true);
            }
            else
            {
                if (status == 3)
                {
                    errorAdminName.IsVisible = true;
                    errorAdminName.Text = message;
                    this.setFocus(1);
                }
                else if (status == 4)
                {
                    errorAdminPassword.IsVisible = true;
                    errorAdminPassword.Text = message;
                    this.setFocus(1);
                }
                else if (status == 6)
                {
                    errorAdminName.IsVisible = true;
                    errorAdminName.Text = message;
                    this.setFocus(1);
                }
                else if (status == 7)
                {
                    errorAdminPassword.IsVisible = true;
                    errorAdminPassword.Text = message;
                    this.setFocus(3);
                }
                else if (status == 8)
                {
                    errorAdminPassword.IsVisible = true;
                    errorAdminPassword.Text = message;
                    this.setFocus(1);
                }
                else
                {
                    errorAdminName.IsVisible = true;
                    errorAdminName.Text = message;
                    this.setFocus(1);
                }
            }
            LoadingText.IsVisible = false;
            buttonLogin.Opacity = 1;
            LoginPage.page_Loader.IsVisible = false;
        }
        public void CallBackDevice(bool isvalid)
        {
            if (isvalid)
            {
                if ((Xamarin.Forms.Device.OS == TargetPlatform.Android) || (Device.OS == TargetPlatform.iOS))
                {
                    Helpers.CacheSettings.SettingsKey = "DeviceEnvironment";
                    Helpers.CacheSettings.GeneralSettings = "School";
                }
                else
                {
                    AppData.FileService.saveCacheDetails("DeviceEnvironment", "School");
                }
                LoginPage login = ((LoginPage)Navigation.NavigationStack.ElementAt(0));
                login.EnvironmentVisible = false;
                login.Environment_Click(login.FindByName<StackLayout>("homeEnv"), new EventArgs());
            }
            else
            { }
        }
        public void OnTextChanged(object sender, EventArgs e)
        {
            try
            {
                Entry box = ((Entry)sender);
                if (box == usernameEntry)
                {
                    this.errorAdminName.IsVisible = false;
                }
                else if (box == passwordEntry)
                {
                    this.errorAdminPassword.IsVisible = false;
                }
                else
                {
                    this.errorAdminName.IsVisible = false;
                    this.errorAdminPassword.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                this.errorAdminName.IsVisible = false;
                this.errorAdminPassword.IsVisible = false;
            }
        }
    }
}
