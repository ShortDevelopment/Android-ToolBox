using Android.Webkit;
using Android.Widget;
using System.Collections.Generic;
using Web_DevTools.utils;
using Web_DevTools.utils.ListViewAdapters;

namespace Web_DevTools
{
    /// <summary>
    /// JS Console
    /// </summary>
    public partial class MainActivity
    {
        ListView JSConsoleListView;
        List<ConsoleMessage> JSMessages = new List<ConsoleMessage>();

        #region Events
        public void OnConsoleMessage(ConsoleMessage consoleMessage)
        {
            JSMessages.Add(consoleMessage);
            JSConsoleListView.Adapter = new JSConsoleListViewAdapter(this, JSMessages);
        }

        public void ClearJSConsole()
        {
            JSMessages.Clear();
            JSConsoleListView.Adapter = new JSConsoleListViewAdapter(this, JSMessages);
        }

        #endregion
    }
}