using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Android;
using Android.App;
using Android.App.Usage;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Hardware.Usb;
using Android.OS;
using Android.OS.Storage;
using Android.Provider;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Xamarin.Essentials;
using static Android.Manifest;
using Permission = Android.Manifest.Permission;
using Uri = Android.Net.Uri;

namespace FileExplorer
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppThemeLight", MainLauncher = true, ConfigurationChanges = ConfigChanges.Keyboard | ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {

        Android.Support.V7.Widget.Toolbar toolbar;
        NavigationView navigationView;
        ListView listView;
        List<FileListViewItem> data = new List<FileListViewItem>();
        bool IsDisplayingFolderTree = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            #region LayoutSetup
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);
            navigationView.ItemIconTintList = null;

            listView = FindViewById<ListView>(Resource.Id.listView1);
            listView.ItemClick += listView1_ItemSelected;

            FindViewById<LinearLayout>(Resource.Id.linearLayout1).Visibility = ViewStates.Gone;
            #endregion

            RequestPermissions(new[] { Permission.ReadExternalStorage, Permission.WriteExternalStorage, Permission.ManageDocuments, "android.permission.ACCESS_ALL_DOWNLOADS" }, 12345);

            try
            {
                ShowStorageVolumes();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            if (newConfig.Orientation == Android.Content.Res.Orientation.Landscape)
            {
                Window.Attributes.LayoutInDisplayCutoutMode = LayoutInDisplayCutoutMode.Never;
            }
            else
            {
                Window.Attributes.LayoutInDisplayCutoutMode = LayoutInDisplayCutoutMode.Default;
            }
            Toast.MakeText(this, Window.Attributes.LayoutInDisplayCutoutMode.ToString(), ToastLength.Long).Show();
        }

        void ShowStorageVolumes()
        {
            IsDisplayingFolderTree = false;
            var manager = (StorageManager)GetSystemService(Context.StorageService);
            listView.Adapter = new StorageVolumeListViewAdapter(this, manager.StorageVolumes.ToList());
        }



        private void listView1_ItemSelected(object sender, AdapterView.ItemClickEventArgs e)
        {

            if (IsDisplayingFolderTree)
            {
                var item = data[e.Position];
                if (item.IsDirectory)
                {
                    DisplayFolderContent(item.Path + "/");
                }
                else
                {
                    try
                    {
                        StartActivity(Intent.CreateChooser(Utilities.CreateActionIntentFromFile(item.Path), "Öffnen mit ..."));
                    }
                    catch (Exception ex)
                    {
                        Snackbar.Make(FindViewById(Android.Resource.Id.Content), $"Fehler! {ex.Message}", Snackbar.LengthLong).Show();
                    }
                }
            }
            else
            {
                var item = ((StorageVolumeListViewAdapter)listView.Adapter).data[e.Position];
                if (string.IsNullOrEmpty(item.Uuid))
                {
                    DisplayFolderContent("/storage/emulated/0/");
                }
                else
                {
                    DisplayFolderContent($"/storage/{item.Uuid}/");
                }
            }

        }

        public string CurrentPath { get; private set; }

        Dictionary<string, string> ListDownloads()
        {
            var downloads = new Dictionary<string, string>();

            DownloadManager manager = (DownloadManager)this.GetSystemService(Context.DownloadService);

            var query = new DownloadManager.Query();
            query.SetFilterByStatus(DownloadStatus.Successful);

            using (var cursor = manager.InvokeQuery(query))
            {
                while (cursor.MoveToNext())
                {
                    var title = cursor.GetString(cursor.GetColumnIndexOrThrow(DownloadManager.ColumnTitle));
                    var uri = cursor.GetString(cursor.GetColumnIndexOrThrow(DownloadManager.ColumnLocalFilename));
                    downloads.Add(title, uri);
                }
            }

            return downloads;
        }
        void ShowDownload()
        {
            try
            {
                var str = "";
                int count = 0;
                foreach (var entry in ListDownloads())
                {
                    str += System.Environment.NewLine + $"{entry.Key}: {entry.Value}";
                    count++;
                }
                Toast.MakeText(this, $"{count}: {str}", ToastLength.Long).Show();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }

            StartActivity(new Intent(DownloadManager.ActionViewDownloads));

        }

        void DisplayFolderContent(string path)
        {
            IsDisplayingFolderTree = true;
            if (!Directory.Exists(path))
            {
                Snackbar.Make(FindViewById(Android.Resource.Id.Content), "Das ausgewählte Verzeichnis existiert nicht!", Snackbar.LengthLong).Show();
                return;
            }

            toolbar.Subtitle = path;

#if !DEBUG
            try
            {
#endif

            data.Clear();

            foreach (var dir in Directory.GetDirectories(path).OrderBy((x) => x))
            {
                var item = new FileListViewItem();
                item.IsDirectory = true;
                item.Path = dir;
                data.Add(item);
            }
            foreach (var dir in Directory.GetFiles(path).OrderBy((x) => x))
            {
                var item = new FileListViewItem();
                item.IsDirectory = false;
                item.Path = dir;
                item.Thumbnail = (new Java.IO.File(item.Path)).GetThumbnail();
                data.Add(item);
            }

            var linearLayout = FindViewById<LinearLayout>(Resource.Id.linearLayout1);
            if (data.Count == 0)
            {
                linearLayout.Visibility = ViewStates.Visible;
            }
            else
            {
                linearLayout.Visibility = ViewStates.Gone;
            }

            FindViewById<ListView>(Resource.Id.listView1).Adapter = new FileListViewAdapter(this, data);
#if !DEBUG
            }
            catch(Exception ex) {
                Snackbar.Make(FindViewById(Android.Resource.Id.Content), $"Fehler! {ex.Message}", Snackbar.LengthLong).Show();
            }
#endif
        }

        #region UI Behaviour
        public override void OnBackPressed()
        {
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            if (drawer.IsDrawerOpen(GravityCompat.Start))
            {
                drawer.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                base.OnBackPressed();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            int id = item.ItemId;

            switch (id)
            {
                case Resource.Id.nav_home:
                    ShowStorageVolumes();
                    break;
                case Resource.Id.nav_view_downloads:
                    ShowDownload();
                    break;
                case Resource.Id.nav_view_root:
                    DisplayFolderContent("/storage/");
                    break;
                case Resource.Id.nav_view_internal_storage:
                    DisplayFolderContent("/storage/emulated/0/");
                    break;
                case Resource.Id.nav_view_sd_card:

                    try
                    {
                        var manager = (StorageManager)GetSystemService(Context.StorageService);
                        var storageDevice = manager.StorageVolumes.Where((x) => !x.IsPrimary).ToList()[0];
                        DisplayFolderContent($"/storage/{storageDevice.Uuid}/");
                    }
                    catch { }

                    break;

                    try
                    {

                        var intent = new Intent(Intent.ActionOpenDocumentTree);
                        //this.StartActivity(intent);

                        const string externalstorage = "com.android.externalstorage.documents";
                        const string downloads = "com.android.downloads.documents";
                        string authority = downloads; // "com.android.providers.media.documents";

                        var providerInfo = PackageManager.ResolveContentProvider(authority, PackageInfoFlags.MetaData);
                        Toast.MakeText(this, providerInfo.PackageName, ToastLength.Long).Show();

                        var client = ContentResolver.AcquireUnstableContentProviderClient(authority);


                        //var cursor = ContentResolver.Query(rootsUri, (string[])null, (string)null, (string[])null, (string)null);
                        // var cursor = client.Query(rootsUri, (string[])null, (string)null, (string[])null, (string)null);
                        //client.Close();

                        Toast.MakeText(this, "Fertig!", ToastLength.Long).Show();
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                    }

                    // DisplayFolderContent();
                    break;
            }

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawer.CloseDrawer(GravityCompat.Start);
            return true;
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        #endregion
    }
}

