using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Web_DevTools.utils.Clients
{
    public class WebViewClientEx : WebViewClient
    {

        public IWebViewClientEventListener Listener { get; private set; }

        public WebViewClientEx(IWebViewClientEventListener listener)
        {
            this.Listener = listener;
        }

        public override WebResourceResponse ShouldInterceptRequest(WebView view, IWebResourceRequest request)
        {
            Listener.OnRequest(request);
            return base.ShouldInterceptRequest(view, request);
        }

        public override void OnPageStarted(WebView view, string url, Bitmap favicon)
        {
            Listener.OnPageStarted(url, favicon);
            base.OnPageStarted(view, url, favicon);
        }
    }
    public interface IWebViewClientEventListener
    {
        void OnRequest(IWebResourceRequest request);
        void OnPageStarted(string url, Bitmap favicon);
    }
}