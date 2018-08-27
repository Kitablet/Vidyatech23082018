
using FISE;
using FISE.iOS;
using Foundation;
using System;
using System.IO;
using System.Threading.Tasks;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ActivityHybridWebView), typeof(ActivityHybridWebViewRenderer))]
namespace FISE.iOS
{
    public class ActivityHybridWebViewRenderer : ViewRenderer<ActivityHybridWebView, WKWebView>, IWKScriptMessageHandler
    {
        const string JavaScriptFunction = "function invokeCSharpAction(data){window.webkit.messageHandlers.invokeAction.postMessage(data);}";
        WKUserContentController userController;

        protected override void OnElementChanged(ElementChangedEventArgs<ActivityHybridWebView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                userController = new WKUserContentController();
                var script = new WKUserScript(new NSString(JavaScriptFunction), WKUserScriptInjectionTime.AtDocumentEnd, false);
                userController.AddUserScript(script);
                userController.AddScriptMessageHandler(this, "invokeAction");

                var config = new WKWebViewConfiguration { UserContentController = userController };
                var webView = new WKWebView(Frame, config);
                SetNativeControl(webView);
            }
            if (e.OldElement != null)
            {
                userController.RemoveAllUserScripts();
                userController.RemoveScriptMessageHandler("invokeAction");
                var hybridWebView = e.OldElement as ActivityHybridWebView;
                hybridWebView.Cleanup();
            }
            if (e.NewElement != null)
            {
                string localpath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                string url = Path.Combine(localpath, Element.Uri);
                Control.LoadFileUrl(new NSUrl("file://" + url), new NSUrl("file://" + Path.GetDirectoryName(url)));
                Task.Run(() =>
                {
                    Task.WaitAny(Task.Delay(2000));
                    InjectJS(JavaScriptFunction);
                    CallJS();
                });

            }
        }
        void InjectJS(string script)
        {
            try
            {
                if (Control != null)
                {
                    var scriptstring = new WKUserScript(new NSString(JavaScriptFunction), WKUserScriptInjectionTime.AtDocumentEnd, false);
                    userController.AddUserScript(scriptstring);
                }
            }
            catch (Exception ex)
            { }

        }

        void CallJS()
        {
            if (Control != null)
            {
                try
                {
                    if (!string.IsNullOrEmpty(BookActivity.strJson))
                    {
                        Control.EvaluateJavaScript(string.Format("Javascript: ActivityStart('{0}')", BookActivity.strJson), null);
                        BookActivity.strJson = string.Empty;
                    }
                }
                catch (Exception ex)
                { }
            }
        }


        public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
        {
            Element.InvokeAction(message.Body.ToString());
        }
    }
}