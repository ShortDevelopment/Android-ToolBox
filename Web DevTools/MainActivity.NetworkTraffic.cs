using Android.Graphics;
using Android.Webkit;
using Android.Widget;
using System.Collections.Generic;
using Web_DevTools.utils;
using Web_DevTools.utils.Dialogs;

namespace Web_DevTools
{
    /// <summary>
    /// Network Traffic
    /// </summary>
    public partial class MainActivity
    {
        List<IWebResourceRequest> Requests = new List<IWebResourceRequest>();
        NetworkTrafficListDialogManager networkTrafficListDialogManager = null;

        #region Events
        public void OnRequest(IWebResourceRequest request)
        {
            Requests.Add(request);

            if (networkTrafficListDialogManager != null)
                networkTrafficListDialogManager.UpdateData(Requests);
        }

        public void ClearTrafficLog()
        {
            Requests.Clear();
            if (networkTrafficListDialogManager != null)
                networkTrafficListDialogManager.UpdateData(Requests);
        }
        
        #endregion
    }
}