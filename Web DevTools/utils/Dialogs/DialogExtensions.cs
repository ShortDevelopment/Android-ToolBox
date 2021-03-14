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

namespace Web_DevTools.utils.Dialogs
{
    public static class DialogExtensions
    {
        public static void FillParent(this AlertDialog dialog)
        {
            WindowManagerLayoutParams lp = new WindowManagerLayoutParams();
            lp.CopyFrom(dialog.Window.Attributes);
            lp.Width = ViewGroup.LayoutParams.MatchParent;
            lp.Height = ViewGroup.LayoutParams.MatchParent;
            dialog.Window.Attributes = lp;
        }
    }
}