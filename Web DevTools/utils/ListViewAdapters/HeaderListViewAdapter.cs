using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Debug = System.Diagnostics.Debug;

namespace Web_DevTools.utils.ListViewAdapters
{
    public class HeaderListViewAdapter : ArrayAdapter<KeyValuePair<string, string>>
    {
        public HeaderListViewAdapter(Context context, IDictionary<string, string> data) : base(context, Android.Resource.Layout.SimpleListItem1, data.Select((x) => new KeyValuePair<string, string>(x.Key, x.Value)).ToList()) { }
        
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = this.GetItem(position);

            TextView view = (TextView)base.GetView(position, convertView, parent);
            view.TextFormatted = Android.Text.Html.FromHtml($"<font color=\"#0074E8\">{item.Key}</font>: {item.Value}");
            view.SetTextIsSelectable(true);
            return view;
        }
    }
}