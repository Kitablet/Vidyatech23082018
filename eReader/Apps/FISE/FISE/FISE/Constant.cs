using System;

namespace Kitablet
{
    public class Constant
    {
        public static readonly int numberOfDaysSinceLastLogin = 15;

        public static readonly string forgotlink = "http://loginstagging.azurewebsites.net/passwordrecovery";

        public static readonly string justexploringlink = "http://www.kitablet.in/";

        public static readonly string baseUri = "http://loginapistagging.azurewebsites.net/";
        //public static readonly string baseUri = "https://loginapi.Kitablet.in/";
        //public static readonly string baseUri = "http://192.168.16.51:8989/api/";
        //public static readonly string baseUri = "http://192.168.16.106:8989/";
        //public static readonly string baseUri = "http://192.168.16.106:7419/";

        public static readonly string relativePath = "api/FISEAPI/";

        public static int totalRatingCount = 500;

        public static int ReleaseBookDisplayCount = 4;
        public static int DeviceIdFromDB { get; set; }

        public static string UniqueID { get; set; }

        public static string DeviceDetails = string.Empty;
        public static int DeviceHeight { get; set; }
        public static int DeviceWidth { get; set; }
        public static float DeviceDensity { get; set; }

        public readonly static int PrimaryNavigationHeight = 80;

        public readonly static int SecondaryNavigationHeight = 160;

        public readonly static int SmallestBookHeight = 173;

        public readonly static int SmallestBookWidth = 136;

        public readonly static int MediumBookHeight = 260;

        public readonly static int MediumBookWidth = 198;

        public readonly static int IconsOnBookBottomHeight = 25;

        public readonly static int ButtonHeight = 45;

        public readonly static int BookContainerHeight = SmallestBookHeight + 10 + IconsOnBookBottomHeight + 30;

        public readonly static int LeftButtonPanelWidth = 180;

        public readonly static int RowHeightReleaseBook = 40;

        public readonly static string AvatarSmall = "63X63";

        public readonly static string AvatarMedium = "90X90";

        public readonly static string AvatarLarge = "180X180";

        public const string passwordRegex = @"^(?=.*\d)(?=.*[$@$!%*#?&])[A-Za-z\d$@$!%*#?&]{8,}$";

        [System.ComponentModel.DefaultValue(true)]
        public static bool IsRememberMe { get; set; }

        public static DateTime LastInteractionTime = DateTime.MinValue;

        public readonly static string FileDirectoryStructure = "Books/";

        public readonly static string FileNameInitials = "Book_";

        public readonly static string FileExtension = ".zip";

        public readonly static string DecryptionKey = "!TEL%$BATIK&^VT*";

        [System.ComponentModel.DefaultValue(0)]
        public static int ShownBookCount { get; set; }

        public static int ShowBookCount { get; set; }

        public static string UserCryptoKey { get; set; }

        public readonly static string CryptoSalt = "�,j�6�%<��j1��";

        public static string Primarycolor = "#14B4B4";

        public static string Secondary1Color = "#D5D5D5";
        public static DateTime UserActiveTime { get; set; }
        public static string CurrentPlateform { get; set; }

        public readonly static string APPId = "4d53bce03ec34c0a911182d4c228ee6c";
        public readonly static string APIKey = "A93reRTUJHsCuQSHR+L3GxqOJyDmQpCgps102ciuabc=";

        public readonly static int NumberOfSeconds = 5;

        public readonly static int UserSessionTimeOut = 10;
        public readonly static int LargestBookPortraitHeight = 585, LargestBookLandscapeHeight = 230, LargestBookWidth = 445;
        public readonly static int LargestBookPortraitHeight_1024 = 490, LargestBookWidth_1024 = 376;
        public readonly static int BookReadPadding = 210;
        public readonly static int MinimumHeightForThumbnail = 620;

        public readonly static string quoteKey = "!**!quot!**!";
        public readonly static string slashKey = "!**!slash!**!";
    }
}
