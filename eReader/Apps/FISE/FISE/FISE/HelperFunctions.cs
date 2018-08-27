using Kitablet.ViewModels;
using System;
using Xamarin.Forms;

namespace Kitablet
{
    public class HelperFunctions
    {
        public static HelperFunctions _HelperFunctions { set; get; }
        static HelperFunctions()
        {
            _HelperFunctions = new HelperFunctions();
        }
        public static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            return char.ToUpper(s[0]) + s.Substring(1);
        }
        public void Logout()
        {
            ActionTabPage.SendViewedEvents();
            Notification.UserEvents = null;
            Notification.NewEventID = null;

            if ((Xamarin.Forms.Device.OS == TargetPlatform.Android) || (Device.OS == TargetPlatform.iOS))
            {
                Helpers.CacheSettings.SettingsKey = "UserLoginId";
                if (Helpers.CacheSettings.GeneralSettings != "")
                {
                    Helpers.CacheSettings.SettingsKey = "UserLoginId";
                    Helpers.CacheSettings.GeneralSettings = "";
                    Helpers.CacheSettings.SettingsKey = "UserLoginPassword";
                    Helpers.CacheSettings.GeneralSettings = "";
                    Helpers.CacheSettings.SettingsKey = "LastLoginDate";
                    Helpers.CacheSettings.GeneralSettings = "";
                    Helpers.CacheSettings.SettingsKey = "UserDetails";
                    Helpers.CacheSettings.GeneralSettings = "";
                }
            }
            else
            {
                if (AppData.FileService.getCacheDetails("UserLoginId") != null)
                {
                    AppData.FileService.deleteKey("UserLoginId");
                    AppData.FileService.deleteKey("LastLoginDate");
                    AppData.FileService.deleteKey("UserDetails");
                    AppData.FileService.deleteKey("UserLoginPassword");
                }
            }
            Utils.SetBrowsingProgress(ActionTabPage.AppStartTime);
            if (HelperFunctions.CheckInternetConnection())
            {
                Utils.UpdateUserDetails(Utils.UpdateMethod.Total);
            }
            AppData.UserDetails = null;
            AppData.UserProgress = null;
            AppData.User = null;
            AppData.SaveAppData();
            ThemeChanger.SetGrade();
            ThemeChanger.changeTheme();
            if (AppData.DeviceDetail.Environment != null && AppData.DeviceDetail.Environment == "School")
            {
                Constant.IsRememberMe = false;
            }
            else
            {
                Constant.IsRememberMe = true;
            }                 
        }
        public static string getCacheData(string id)
        {
            string value = string.Empty;

            if ((Xamarin.Forms.Device.OS == TargetPlatform.Android) || (Device.OS == TargetPlatform.iOS))
            {
                Kitablet.Helpers.CacheSettings.SettingsKey = id;
                return Kitablet.Helpers.CacheSettings.GeneralSettings;
            }
            else
            {
                if (!string.IsNullOrEmpty(AppData.FileService.getCacheDetails(id)))
                {
                    value = AppData.FileService.getCacheDetails(id);
                }

            }
            return value;
        }
        public static void saveCacheData(string id, string value)
        {
            if ((Xamarin.Forms.Device.OS == TargetPlatform.Android) || (Device.OS == TargetPlatform.iOS))
            {
                Kitablet.Helpers.CacheSettings.SettingsKey = id;
                Kitablet.Helpers.CacheSettings.GeneralSettings = value;
            }
            else
            {
                if (AppData.FileService.getCacheDetails(id) != null)
                {
                    AppData.FileService.saveCacheDetails(id, value);
                }
                else
                {
                    AppData.FileService.saveCacheDetails(id, value);
                }
            }
        }
        public static bool checkCacheExist(string id)
        {
            bool value = false;

            if ((Xamarin.Forms.Device.OS == TargetPlatform.Android) || (Device.OS == TargetPlatform.iOS))
            {
                Kitablet.Helpers.CacheSettings.SettingsKey = id;
                if (Kitablet.Helpers.CacheSettings.GeneralSettings != "")
                    value = true;
            }
            else
            {
                if (!string.IsNullOrEmpty(AppData.FileService.getCacheDetails(id)))
                {
                        value = true;
                }
            }
            return value;
        }
        public static void removeCacheData(string id)
        {
            if ((Xamarin.Forms.Device.OS == TargetPlatform.Android) || (Device.OS == TargetPlatform.iOS))
            {
                
                if(checkCacheExist(id))
                {
                    Kitablet.Helpers.CacheSettings.SettingsKey = id;
                    Kitablet.Helpers.CacheSettings.GeneralSettings = "";
                }
            }
            else
            {
                if (AppData.FileService.getCacheDetails(id) != null)
                {
                    AppData.FileService.deleteKey(id);
                }
            }

        }
        public static bool CheckInternetConnection()
        {
            bool isconnected = false;
            try
            {
                string result = MyWebRequest.GetRequest("connectserver", null, null);
                isconnected = Boolean.Parse(result);
            }
            catch (Exception ex)
            {
                isconnected = false;
            }
            return isconnected;
        }
    }

}
