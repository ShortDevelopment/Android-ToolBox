using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Java.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Essentials;
using Uri = Android.Net.Uri;
using Debug = System.Diagnostics.Debug;
using Android.OS.Storage;
using Google.Android.Material.Shadow;

namespace FileExplorer
{

    public static class Extensions
    {
        #region File Thumbnail
        public static Bitmap GetThumbnail(this File file)
        {
            Bitmap ret = null;
            string MimeType = file.GetMimeType();
            MimeType = (MimeType == null) ? "" : MimeType;
            Debug.Print(MimeType);
            if (MimeType.Contains("image"))
            {
                // ret = ThumbnailUtilsCompat.CreateImageThumbnail(file, new Android.Util.Size(60, 60), null);
            }
            if (MimeType.Contains("audio"))
            {
                ret = ThumbnailUtilsCompat.CreateAudioThumbnail(file, new Android.Util.Size(60, 60), null);
            }
            if (MimeType.Contains("video"))
            {
                ret = ThumbnailUtilsCompat.CreateVideoThumbnail(file, new Android.Util.Size(60, 60), null);
            }
            if(ret == null)
            {
                var intent = Utilities.CreateActionIntentFromFile(file.AbsolutePath);
                var pm = Application.Context.PackageManager;
                var matches = pm.QueryIntentActivities(intent, 0).Where((x) => x.ActivityInfo.Enabled && x.ActivityInfo.IsEnabled).OrderBy((x) => x.PreferredOrder).ToList();
                var priorityMatches = matches.Where((x) => x.PreferredOrder > 0).ToList();
                Debug.Print(file.AbsolutePath);
                Debug.Print(string.Join("\r\n", matches.Select((x) => x.ActivityInfo.ApplicationInfo.PackageName)));
                if(priorityMatches.Count > 0)
                {
                    var iconRes = priorityMatches.Last()?.ActivityInfo.LoadIcon(pm);
                    ret = ((BitmapDrawable)iconRes)?.Bitmap;
                }
                else if (matches.Count > 0)
                {
                    var iconRes = matches.First()?.ActivityInfo.LoadIcon(pm);
                    ret = iconRes.GetBitmap();
                }
            }
            if(ret == null)
            {
                // ret = ((BitmapDrawable)Application.Context.Resources.GetDrawable(Resource.Drawable.baseline_insert_drive_file_24))?.Bitmap;
            }
            return ret;
        }
        #endregion

        /// <summary>
        /// https://stackoverflow.com/a/46018816
        /// </summary>
        /// <param name="drawable"></param>
        /// <returns></returns>
        public static Bitmap GetBitmap(this Drawable drawable)
        {
            Bitmap bmp = Bitmap.CreateBitmap(drawable.IntrinsicWidth, drawable.IntrinsicHeight, Bitmap.Config.Argb8888);
            using (Canvas canvas = new Canvas(bmp))
            {
                drawable.SetBounds(0, 0, canvas.Width, canvas.Height);
                drawable.Draw(canvas);
                return bmp;
            }
        }

        public static string GetMimeType(this File file)
        {
            var extension = System.IO.Path.GetExtension(file.Path).Replace(".", "").ToLower();
            return MimeTypeMap.Singleton.GetMimeTypeFromExtension(extension)?.ToLower();
        }

        public static Uri GetFileProviderUri(this File file)
        {
            return FileProvider.GetUriForFile(Application.Context, "de.shortdevelopment.fileexplorer.documents", file);
        }

        public static string GetPath(this StorageVolume storageVolume)
        {
            if (storageVolume.IsPrimary)
            {
                return "/storage/emulated/0";
            }
            else
            {
                return $"/storage/{storageVolume.Uuid}/";
            }
        }
    }

    public class Utilities
    {
        public static string ParsePath(string path)
        {
            if (!path.ToLower().Contains("/storage/emulated/0"))
            {
                path = path.Replace("/storage", "/mnt/media_rw");
            }
            return path;
        }

        public static bool IsApiLevel(int level) => IsApiLevel(level);
        public static bool IsApiLevel(BuildVersionCodes level) => Build.VERSION.SdkInt >= level;

        public static Intent CreateActionIntentFromFile(string path, string action = Intent.ActionView)
        {
            path = ParsePath(path);
            var file = new File(path);
            var intent = new Intent(action);
            if (IsApiLevel(BuildVersionCodes.N))
            {
                Uri uri = file.GetFileProviderUri();
                intent.SetData(uri);
                intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.GrantReadUriPermission);
            }
            else
            {
                Uri uri = Uri.FromFile(file);
                intent.SetDataAndType(uri, file.GetMimeType());
                intent.SetFlags(ActivityFlags.NewTask);
            }
            return intent;
        }

    }
}