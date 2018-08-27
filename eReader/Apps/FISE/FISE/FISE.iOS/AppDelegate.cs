using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using ImageCircle.Forms.Plugin.iOS;

namespace FISE.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            try
            {
                    global::Xamarin.Forms.Forms.Init();
                    ImageCircleRenderer.Init();
                    MR.Gestures.iOS.Settings.LicenseKey = "T2C8-JF4C-2ZJJ-G76H-7N6E-Y7HT-Q5ER-Y3HW-76NW-CZSE-FW5N-C8N3-5HNL";

                    LoadApplication(new App());

                    return base.FinishedLaunching(app, options);
            }
            catch(Exception ex) { return false; }

        }
    }
}
