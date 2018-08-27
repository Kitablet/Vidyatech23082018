using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using ImageCircle.Forms.Plugin.Droid;

namespace Kitablet.Droid
{
    [Activity(Label = "Kitablet", Icon = "@drawable/icon", Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.SensorLandscape )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);
            global::Xamarin.Forms.Forms.Init(this, bundle);
            this.Window.SetFlags(WindowManagerFlags.Secure, WindowManagerFlags.Secure);
            MR.Gestures.Android.Settings.LicenseKey = "JHDP-JUJQ-HFXM-6EBP-J9JR-CYVX-M9X7-9LCX-ATUP-3AH8-S5EZ-8WS4-KLMF";
            ImageCircleRenderer.Init();
            LoadApplication(new App());
        }
    }
}

