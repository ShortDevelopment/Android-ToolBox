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
    public class WebChromeClientEx : WebChromeClient
    {

        public IWebChromeClientEventListener Listener { get; private set; }

        public WebChromeClientEx(IWebChromeClientEventListener listener)
        {
            this.Listener = listener;
        }

        public override bool OnConsoleMessage(ConsoleMessage consoleMessage)
        {
            Listener.OnConsoleMessage(consoleMessage);
            return base.OnConsoleMessage(consoleMessage);
        }

        public override void OnReceivedTitle(WebView view, string title)
        {
            Listener.OnReceivedTitle(title);
            base.OnReceivedTitle(view, title);
        }

        public override void OnReceivedIcon(WebView view, Bitmap icon)
        {
            Listener.OnReceivedIcon(icon);
            base.OnReceivedIcon(view, icon);
        }
    }

    public interface IWebChromeClientEventListener
    {
        void OnConsoleMessage(ConsoleMessage consoleMessage);
        void OnReceivedTitle(string title);
        void OnReceivedIcon(Bitmap icon);
    }
}