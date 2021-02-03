using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileExplorer
{
    /// <summary>
    /// https://github.com/aosp-mirror/platform_frameworks_base/blob/c5d02da0f6553a00da6b0d833b67d3bbe87341e0/media/java/android/media/ThumbnailUtils.java
    /// </summary>
    public class ThumbnailUtilsCompat
    {
        public static Bitmap CreateAudioThumbnail(File file, Size size, CancellationSignal signal)
        {
            try
            {
                if (Utilities.IsApiLevel(BuildVersionCodes.Q))
                {
                    return ThumbnailUtils.CreateAudioThumbnail(file, size, signal);
                }
            }
            catch { }
            try
            {
                MediaMetadataRetriever retriever = new MediaMetadataRetriever();
                retriever.SetDataSource(file.AbsolutePath);
                byte[] raw = retriever.GetEmbeddedPicture();

                if (raw != null)
                {
                    return BitmapFactory.DecodeByteArray(raw, 0, raw.Length);
                }
            }
            catch {}
            return null;
        }
        public static Bitmap CreateImageThumbnail(File file, Size size, CancellationSignal signal)
        {
            try
            {
                if (Utilities.IsApiLevel(BuildVersionCodes.Q))
                {
                    return ThumbnailUtils.CreateImageThumbnail(file, size, signal);
                }
            }
            catch { }
            try
            {
                return BitmapFactory.DecodeFile(file.AbsolutePath);
            }
            catch { }
            return null;
        }
        public static Bitmap CreateVideoThumbnail(File file, Size size, CancellationSignal signal)
        {
            if (Utilities.IsApiLevel(BuildVersionCodes.Q))
            {
                return ThumbnailUtils.CreateVideoThumbnail(file, size, signal);
            }
            return ThumbnailUtils.CreateVideoThumbnail(file.AbsolutePath, Android.Provider.ThumbnailKind.MiniKind);
        }
    }
}