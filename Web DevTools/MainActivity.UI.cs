using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Web_DevTools
{
    /// <summary>
    /// UI
    /// </summary>
    public partial class MainActivity
    {
        #region Events

        public void OnPageStarted(string url, Bitmap favicon)
        {
            ClearTrafficLog();

            ClearJSConsole();

            OnConsoleMessage(new ConsoleMessage("Navigation wurde ausgeführt.", "", 0, ConsoleMessage.MessageLevel.Tip));

            ShowFormattedURL(Android.Net.Uri.Parse(url));
        }
        #endregion

    }
}