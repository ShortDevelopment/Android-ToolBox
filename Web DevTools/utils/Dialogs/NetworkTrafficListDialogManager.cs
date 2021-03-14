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

namespace Web_DevTools.utils.Dialogs
{
    public class NetworkTrafficListDialogManager : Java.Lang.Object, IDialogInterfaceOnCancelListener
    {
        public NetworkTrafficListDialogManager(MainActivity activity, List<IWebResourceRequest> requests)
        {
            this.BaseActivity = activity;
            this.Requests = requests;
        }

        #region Vars
        public MainActivity BaseActivity { get; private set; }
        public List<IWebResourceRequest> Requests { get; private set; }

        private AlertDialog dialog = null;
        private DynamicListView listView = null;
        #endregion

        public void ShowDialog()
        {
            dialog = new AlertDialog.Builder(BaseActivity, Resource.Style.Base_ThemeOverlay_MaterialComponents_Dialog_Alert)
                .SetView(Resource.Layout.content_network_traffic)
                .SetTitle("Network Traffic")
                .SetPositiveButton("Close", delegate { Close(); })
                .SetOnCancelListener(this)
                .Show();

            dialog.FillParent();

            listView = new DynamicListView(dialog.FindViewById<LinearLayout>(Resource.Id.trafficUrlsListView));
            listView.IsSelectable = true;
            listView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
            {
                var detailsDialog = new NetworkTrafficDetailsDialogManager(BaseActivity, Requests[e.Position]);
                detailsDialog.ShowDialog();
            };

            UpdateData(Requests);
        }

        public void UpdateData(List<IWebResourceRequest> data)
        {
            Requests = data;
            if (listView != null)
            {
                BaseActivity.RunOnUiThread(() =>
                {
                    listView.Adapter = new ArrayAdapter(BaseActivity, Android.Resource.Layout.SimpleListItem1, Requests.Select((x) => System.IO.Path.GetFileName(x.Url.Path)).ToArray());
                });
            }
        }

        #region Close
        public void Close()
        {
            listView = null;
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