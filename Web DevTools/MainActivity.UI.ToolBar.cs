using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Web_DevTools
{
    public partial class MainActivity : PopupMenu.IOnMenuItemClickListener
    {
        ImageButton triggerOptionsMenuImageButton;
        EditText UrlTextView;

        #region Events
        public void OnReceivedTitle(string title)
        {
            //SupportActionBar.SubtitleFormatted = Android.Text.Html.FromHtml(title);
        }

        public void OnReceivedIcon(Bitmap icon)
        {
            FindViewById<ImageView>(Resource.Id.faviconImageView).SetImageBitmap(icon);
        }

        public bool OnMenuItemClick(IMenuItem item)
        {
            return OnOptionsItemSelected(item);
        }
        #endregion

        public void PrepareCustomToolBar()
        {
            UrlTextView = FindViewById<EditText>(Resource.Id.urlTextView);
            UrlTextView.EditorAction += (object sender, TextView.EditorActionEventArgs e) =>
            {
                try
                {
                    Android.Net.Uri uri = Android.Net.Uri.Parse(UrlTextView.Text);

                    webView.LoadUrl(uri.ToString());
                    webView.RequestFocus();

                    ShowFormattedURL(uri);
                }
                catch (Exception ex)
                {
                    Snackbar.Make(webView, ex.Message, Snackbar.LengthLong).Show();
                }
            };
            UrlTextView.FocusChange += delegate
            {
                if (UrlTextView.IsFocused)
                {
                    // Clear Color
                    UrlTextView.Text = UrlTextView.Text;                    
                    UrlTextView.SelectAll();
                    UrlTextView.ScrollX = 0;
                    InputMethodManager.ShowSoftInput(UrlTextView, ShowFlags.Forced);
                }
                else
                {
                    try
                    {
                        Android.Net.Uri uri = Android.Net.Uri.Parse(UrlTextView.Text);
                        ShowFormattedURL(uri);
                    }
                    catch { }
                }
            };

            triggerOptionsMenuImageButton = FindViewById<ImageButton>(Resource.Id.triggerOptionsMenuImageButton);
            triggerOptionsMenuImageButton.Click += delegate
            {
                OpenOptionsMenu();
            };
        }

        public override void OpenOptionsMenu()
        {
            PopupMenu popup = new PopupMenu(this, triggerOptionsMenuImageButton);
            popup.SetOnMenuItemClickListener(this);
            popup.Inflate(Resource.Menu.menu_main);
            popup.Show();
        }

        public void ShowFormattedURL(Android.Net.Uri uri)
        {
            UrlTextView.TextFormatted = Android.Text.Html.FromHtml($"<font color=\"{(uri.Scheme == "https" ? "#009900" : "#f44336")}\">{uri.Scheme}</font><font color=\"#A4A4A4\">://</font><font color=\"#000;\">{uri.Host}</font><font color=\"#A4A4A4\">{uri.Path}{uri.Query}</font>");
        }
    }
}