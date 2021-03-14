using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Views.InputMethods;
using Android.Webkit;
using Android.Widget;
using Web_DevTools.utils;
using Web_DevTools.utils.Clients;
using Web_DevTools.utils.Dialogs;
using AlertDialog = Android.App.AlertDialog;
using Debug = System.Diagnostics.Debug;

namespace Web_DevTools
{
    /// <summary>
    /// Main Activity
    /// </summary>
    [IntentFilter(new[] { Intent.ActionView }, Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable }, Label = "Dev Tools", DataSchemes = new[] { "https", "http" })]
    [IntentFilter(new[] { Intent.ActionSend }, Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable }, Label = "Dev Tools", DataMimeTypes = new[] { "text/plain" })]
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true, ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.Keyboard | Android.Content.PM.ConfigChanges.ScreenLayout)]
    public partial class MainActivity : AppCompatActivity, IWebChromeClientEventListener, IWebViewClientEventListener, PopupMenu.IOnMenuItemClickListener
    {

        BottomSheetBehavior BottomSheet;
        WebView webView;
        InputMethodManager InputMethodManager { get; set; }

        private const string HOME_URL = "https://bing.com/"; //"https://shortdevelopment.github.io/";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            InputMethodManager = (InputMethodManager)GetSystemService(Context.InputMethodService);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            BottomSheet = BottomSheetBehavior.From(FindViewById(Resource.Id.bottom_sheet_console));
            //mBottomSheetBehavior.State = BottomSheetBehavior.StateHalfExpanded;

            webView = FindViewById<WebView>(Resource.Id.webView1);
            webView.Settings.JavaScriptEnabled = true;
            webView.Settings.BuiltInZoomControls = true;
            webView.Settings.DisplayZoomControls = false;
            webView.Settings.LoadWithOverviewMode = false;
            webView.Settings.DomStorageEnabled = true;
            webView.SetWebChromeClient(new WebChromeClientEx(this));
            webView.SetWebViewClient(new WebViewClientEx(this));
            
            if(Intent.Action == Intent.ActionView || Intent.Action == Intent.ActionSend)
            {
                string url = Intent.Extras.GetString(Intent.ExtraText);
                Toast.MakeText(this, $"Try to open \"{url}\"", ToastLength.Long).Show();
                webView.LoadUrl(url);
            }
            else
            {
                webView.LoadUrl(HOME_URL);
            }

            JSConsoleListView = FindViewById<ListView>(Resource.Id.JSConsole_ListView);

            FindViewById<ImageButton>(Resource.Id.clearJSConsoleButton).Click += delegate
            {
                ClearJSConsole();
            };

            PrepareCustomToolBar();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;

            if (id == Resource.Id.menu_console)
            {
                BottomSheet.State = BottomSheetBehavior.StateHalfExpanded;
                return true;
            }
            else if (id == Resource.Id.menu_traffic)
            {
                networkTrafficListDialogManager = new NetworkTrafficListDialogManager(this, Requests);
                networkTrafficListDialogManager.ShowDialog();
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            webView.LoadUrl(HOME_URL);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

    }

}
