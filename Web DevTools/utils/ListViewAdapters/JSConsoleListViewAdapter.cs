using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text.Method;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Web_DevTools.utils.ListViewAdapters
{
    public class JSConsoleListViewAdapter : GenericListViewAdapter<ConsoleMessage>
    {
        public JSConsoleListViewAdapter(Activity baseActivity, List<ConsoleMessage> data) : base(baseActivity, data) { }

        public override View GetView(Activity baseActivity, int position, ConsoleMessage consoleMessage, View convertView, ViewGroup parent)
        {
            if (convertView == null)
                convertView = baseActivity.LayoutInflater.Inflate(Resource.Layout.js_console_item, null);

            ImageView imageView = convertView.FindViewById<ImageView>(Resource.Id.msgTypeImageView);
            imageView.Visibility = ViewStates.Visible;

            ConsoleMessage.MessageLevel level = consoleMessage.InvokeMessageLevel();
            string msg = consoleMessage.Message();
            string file = consoleMessage.SourceId();
            int line = consoleMessage.LineNumber();
            if (level == ConsoleMessage.MessageLevel.Error)
            {
                imageView.SetImageResource(Resource.Drawable.critical);
            }
            else if (level == ConsoleMessage.MessageLevel.Warning)
            {
                imageView.SetImageResource(Resource.Drawable.i_alerterror);
            }
            else if (level == ConsoleMessage.MessageLevel.Tip)
            {
                imageView.SetImageResource(Resource.Drawable.i_alertinfo);
            }
            else
            {
                imageView.Visibility = ViewStates.Invisible;
            }

            TextView msgTextView = convertView.FindViewById<TextView>(Resource.Id.msgTextView);
            //msgTextView.MovementMethod = LinkMovementMethod.Instance;
            msgTextView.Text = msg;            

            if(!String.IsNullOrEmpty(file))
                convertView.FindViewById<TextView>(Resource.Id.linkTextView).TextFormatted = Android.Text.Html.FromHtml($"<u>{System.IO.Path.GetFileName(file)} ({line})</u>");

            return convertView;
        }
    }
}