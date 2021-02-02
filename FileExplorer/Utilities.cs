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
                var matches = pm.QueryIntentActivities(intent, 0).OrderBy((x) => x.PreferredOrder).ToList();
                var priorityMatches = matches.Where((x) => x.PreferredOrder > 0).ToList();
                if(priorityMatches.Count > 0)
                {
                    ret = ((BitmapDrawable)priorityMatches.Last().LoadIcon(pm))?.Bitmap;
                }
                else if (matches.Count > 0)
                {
                    ret = ((BitmapDrawable)matches.First().LoadIcon(pm))?.Bitmap;
                }
            }
            if(ret == null)
            {
                // ret = ((BitmapDrawable)Application.Context.Resources.GetDrawable(Resource.Drawable.baseline_insert_drive_file_24))?.Bitmap;
            }
            return ret;
        }
        #endregion

        public static string GetMimeType(this File file)
        {
            var extension = System.IO.Path.GetExtension(file.Path).Replace(".", "").ToLower();
            return MimeTypeMap.Singleton.GetMimeTypeFromExtension(extension)?.ToLower();
        }

        public static Uri GetFileProviderUri(this File file)
        {
            return FileProvider.GetUriForFile(Application.Context, "de.shortdevelopment.fileexplorer.documents", file);
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