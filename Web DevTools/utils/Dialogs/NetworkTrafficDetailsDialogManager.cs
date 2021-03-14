using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Web_DevTools.utils.ListViewAdapters;

namespace Web_DevTools.utils.Dialogs
{
    public class NetworkTrafficDetailsDialogManager : Java.Lang.Object, IDialogInterfaceOnCancelListener
    {
        public NetworkTrafficDetailsDialogManager(MainActivity activity, IWebResourceRequest request)
        {
            this.BaseActivity = activity;
            this.Request = request;
        }

        #region Vars
        public MainActivity BaseActivity { get; private set; }
        public IWebResourceRequest Request { get; private set; }

        private AlertDialog dialog = null;
        #endregion

        public void ShowDialog()
        {
            dialog = new AlertDialog.Builder(BaseActivity, Resource.Style.Base_ThemeOverlay_MaterialComponents_Dialog_Alert)
                .SetView(Resource.Layout.content_network_traffic_details)
                .SetTitle("Request Details")
                .SetNeutralButton("Back", delegate { Close(); })
                .SetOnCancelListener(this)
                .Show();

            dialog.FillParent();

            dialog.FindViewById<TextView>(Resource.Id.urlTextView).TextFormatted = Android.Text.Html.FromHtml($"<b>{Request.Method}</b> {Request.Url}");

            new DynamicListView(dialog.FindViewById<LinearLayout>(Resource.Id.requestHeadersListView)).Adapter = new HeaderListViewAdapter(BaseActivity, Request.RequestHeaders);
        }

        #region Close
        public void Close()
        {
            if(dialog != null)
                dialog.Cancel();
            dialog = null;
        }
        public void OnCancel(IDialogInterface dialog)
        {
            Close();
        }
        #endregion
    }
}