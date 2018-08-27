using System;
using Android.Webkit;
using Kitablet.Droid;
using Java.Interop;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Kitablet.Droid
{
    public class JSBridge : Java.Lang.Object
    {
        readonly WeakReference<HybridWebViewRenderer> hybridWebViewRenderer;

        public JSBridge(HybridWebViewRenderer hybridRenderer)
        {
            hybridWebViewRenderer = new WeakReference<HybridWebViewRenderer>(hybridRenderer);
        }

        [JavascriptInterface]
        [Export("invokeAction")]
        public void InvokeAction(String data)
        {
            HybridWebViewRenderer hybridRenderer;
            try
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    if (hybridWebViewRenderer != null && hybridWebViewRenderer.TryGetTarget(out hybridRenderer))
                    {
                        hybridRenderer.Element.InvokeAction(data);
                    }
                });
            }
            catch(Exception ex) { }       
        }
    }
}

