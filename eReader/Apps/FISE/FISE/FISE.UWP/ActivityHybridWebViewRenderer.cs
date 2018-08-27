using System;
using Kitablet;
using Kitablet.UWP;
using Xamarin.Forms.Platform.UWP;
using Windows.UI.Xaml.Controls;
using Windows.Storage;
using Windows.Web;
using Windows.Foundation;
using Windows.Storage.Streams;
using System.Threading.Tasks;

[assembly: ExportRenderer(typeof(ActivityHybridWebView), typeof(ActivityHybridWebViewRenderer))]
namespace Kitablet.UWP
{
    public class ActivityHybridWebViewRenderer : ViewRenderer<ActivityHybridWebView, Windows.UI.Xaml.Controls.WebView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ActivityHybridWebView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                SetNativeControl(new WebView());
            }
            if (e.OldElement != null)
            {
                Control.NavigationCompleted -= OnWebViewNavigationCompleted;
                Control.ScriptNotify -= OnWebViewScriptNotify;
                Control.NavigationStarting -= Control_NavigationStarting;
            }
            if (e.NewElement != null)
            {
                Control.NavigationCompleted += OnWebViewNavigationCompleted;
                Control.ScriptNotify += OnWebViewScriptNotify;
                Control.NavigationStarting += Control_NavigationStarting;

                Uri url = Control.BuildLocalStreamUri("MyTag", "/local/" + Element.Uri);
                StreamUriWinRTResolver myResolver = new StreamUriWinRTResolver();

                // Pass the resolver object to the navigate call.
                Control.NavigateToLocalStreamUri(url, myResolver);
            }
        }
        private void Control_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            if (Control != null)
            {
                Control.Opacity = 0;

            }
        }
        async void OnWebViewNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            if (args.IsSuccess)
            {
                try
                {
                    if (!string.IsNullOrEmpty(BookActivity.strJson))
                    {
                        //string htmlData = BookActivity.activityHTML.Replace("\"", "'");
                        //string str = "{\"htmlContent\": {\"data\":\"" + htmlData + "\"},";
                        //string temp = BookActivity.strJson.Substring(1);
                        //temp = str + temp;
                        Task.Delay(500).Wait();
                        Control.Opacity = 1;
                        await Control.InvokeScriptAsync("ActivityStart", new[] { BookActivity.strJson });
                        BookActivity.strJson = string.Empty;
                        //BookActivity.activityHTML = string.Empty;
                    }
                }
                catch (Exception ex)
                { }
            }
        }

        void OnWebViewScriptNotify(object sender, NotifyEventArgs e)
        {
            Element.InvokeAction(e.Value);
        }

        public sealed class StreamUriWinRTResolver : IUriToStreamResolver
        {
            /// <summary>
            /// The entry point for resolving a Uri to a stream.
            /// </summary>
            /// <param name="uri"></param>
            /// <returns></returns>
            public IAsyncOperation<IInputStream> UriToStreamAsync(Uri uri)
            {
                if (uri == null)
                {
                    throw new Exception();
                }
                string path = uri.AbsolutePath;
                // Because of the signature of this method, it can't use await, so we 
                // call into a separate helper method that can use the C# await pattern.
                return getContent(path).AsAsyncOperation();
            }
            /// <summary>
            /// Helper that maps the path to package content and resolves the Uri
            /// Uses the C# await pattern to coordinate async operations
            /// </summary>
            private async Task<IInputStream> getContent(string path)
            {
                // We use a package folder as the source, but the same principle should apply
                // when supplying content from other locations
                try
                {
                    // Don't use "ms-appdata:///" on the scheme string, because inside the path
                    // will contain "/local/MyFolderOnLocal/index.html"
                    string scheme = "ms-appdata://" + path;

                    Uri localUri = new Uri(scheme);
                    StorageFile f = await StorageFile.GetFileFromApplicationUriAsync(localUri);
                    IRandomAccessStream stream = await f.OpenAsync(FileAccessMode.Read);
                    return stream.GetInputStreamAt(0);
                }
                catch (Exception) { throw new Exception("Invalid path"); }
            }
        }
    }
}
