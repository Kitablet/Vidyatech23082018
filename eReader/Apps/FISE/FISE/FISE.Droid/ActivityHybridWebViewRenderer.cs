using Android.Webkit;
using Kitablet;
using Kitablet.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using static Android.Webkit.WebSettings;
using System.Linq;
using System;
using System.Threading.Tasks;
using Java.Interop;

[assembly: ExportRenderer(typeof(ActivityHybridWebView), typeof(ActivityHybridWebViewRenderer))]
namespace Kitablet.Droid
{
    public class ActivityHybridWebViewRenderer : ViewRenderer<ActivityHybridWebView, Android.Webkit.WebView>
    {
        const string JavaScriptFunction = "function invokeCSharpAction(data){jsBridge.invokeAction(data);}";

        protected override void OnElementChanged(ElementChangedEventArgs<ActivityHybridWebView> e)
        {
            base.OnElementChanged(e);
            Android.Webkit.WebView.SetWebContentsDebuggingEnabled(true);

            if (Control == null)
            {
                var webView = new Android.Webkit.WebView(Forms.Context);
                webView.Settings.JavaScriptEnabled = true;

                SetNativeControl(webView);
            }
            if (e.OldElement != null)
            {
                Control.RemoveJavascriptInterface("jsBridge");
                var hybridWebView = e.OldElement as ActivityHybridWebView;
                hybridWebView.Cleanup();
            }
            if (e.NewElement != null)
            {
                Control.AddJavascriptInterface(new ActivityJSBridge(this), "jsBridge");
                string localpath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                string url = System.IO.Path.Combine(localpath, Element.Uri);
                Control.LoadUrl(string.Format("file://{0}", url));

                Task.Run(() => {
                    Task.WaitAny(Task.Delay(2000));
                    InjectJS(JavaScriptFunction);
                    CallJS();
                });

            }
        }

        void InjectJS(string script)
        {
            if (Control != null)
            {
                Control.LoadUrl(string.Format("Javascript: {0}", script));
            }
        }

        void CallJS()
        {
            if (Control != null)
            {
                try
                {
                    if (!string.IsNullOrEmpty(BookActivity.strJson))
                    {
                        //string htmlData = BookActivity.activityHTML.Replace("\"", "'");
                        //string str = "{\"htmlContent\": {\"data\":\"" + htmlData + "\"},";
                        //string temp = BookActivity.strJson.Substring(1);
                        //temp = str + temp;
                        Control.LoadUrl(string.Format("Javascript: ActivityStart('{0}')", BookActivity.strJson));
                        BookActivity.strJson = string.Empty;
                        //BookActivity.activityHTML = string.Empty;
                    }
                }
                catch (Exception ex)
                { }

            }
        }
    }

    public class ActivityJSBridge : Java.Lang.Object
    {
        readonly WeakReference<ActivityHybridWebViewRenderer> hybridWebViewRenderer;

        public ActivityJSBridge(ActivityHybridWebViewRenderer hybridRenderer)
        {
            hybridWebViewRenderer = new WeakReference<ActivityHybridWebViewRenderer>(hybridRenderer);
        }

        [JavascriptInterface]
        [Export("invokeAction")]
        public void InvokeAction(String data)
        {
            ActivityHybridWebViewRenderer hybridRenderer;
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
            catch (Exception ex) { }
        }
    }
}