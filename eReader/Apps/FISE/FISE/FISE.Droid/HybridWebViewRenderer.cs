using Android.Webkit;
using Kitablet;
using Kitablet.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using static Android.Webkit.WebSettings;
using System.Linq;
using System;
using System.Threading.Tasks;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace Kitablet.Droid
{
    public class HybridWebViewRenderer : ViewRenderer<HybridWebView, Android.Webkit.WebView>
    {
       const string JavaScriptFunction = "function invokeCSharpAction(data){jsBridge.invokeAction(data);}";

        protected override void OnElementChanged(ElementChangedEventArgs<HybridWebView> e)
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
                var hybridWebView = e.OldElement as HybridWebView;              
                hybridWebView.Cleanup();
            }
            if (e.NewElement != null)
            {
                Control.AddJavascriptInterface(new JSBridge(this), "jsBridge");
                Control.LoadUrl(string.Format("file:///android_asset/Content/{0}", Element.Uri));

                Task.Run(() => {
                    Task.WaitAny(Task.Delay(2000));
                    InjectJS(JavaScriptFunction);
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
       
    }
}
