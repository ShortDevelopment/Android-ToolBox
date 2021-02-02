using Android.App;
using Android.App.Usage;
using Android.Content;
using Android.Graphics;
using Android.Hardware.Usb;
using Android.OS;
using Android.OS.Storage;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileExplorer
{
    public class StorageVolumeListViewAdapter : BaseListViewAdapter<StorageVolume>
    {

        public StorageVolumeListViewAdapter(Activity parent, List<StorageVolume> data) : base(parent, data)
        {

        }

        public override bool IsEnabled(int position)
        {
            return data[position].State == "mounted";
        }

        public override View GetView(StorageVolume item, View convertView, ViewGroup parent)
        {
            if (convertView == null)
                convertView = ParentActivity.LayoutInflater.Inflate(Resource.Layout.storage_listview_item, null);

            var usbDevices = ((UsbManager)ParentActivity.GetSystemService(Context.UsbService)).DeviceList.Values.Select((x) => x.DeviceName);
            
            var text = convertView.FindViewById<TextView>(Resource.Id.textView1);
            text.Text = item.GetDescription(ParentActivity);

            var iconImageView = convertView.FindViewById<ImageView>(Resource.Id.imageView1);
            if (item.IsRemovable)
            {
                
                if (usbDevices.Contains(text.Text))
                {
                    iconImageView.SetImageResource(Resource.Drawable.baseline_usb_24);
                }
                else
                {
                    iconImageView.SetImageResource(Resource.Drawable.baseline_sd_storage_24);
                }
                iconImageView.Drawable.SetColorFilter(Color.ParseColor("#e53935"), PorterDuff.Mode.SrcIn);
            }
            else
            {                
                iconImageView.SetImageResource(Resource.Drawable.baseline_smartphone_24);
                iconImageView.Drawable.SetColorFilter(Color.ParseColor("#1976D2"), PorterDuff.Mode.SrcIn);
            }
            if (item.State != "mounted")
            {
                iconImageView.Drawable.SetColorFilter(Color.ParseColor("#CFD8DC"), PorterDuff.Mode.SrcIn);
            }

            var StatsManager = (StorageStatsManager)ParentActivity.GetSystemService(Context.StorageStatsService);
            // StorageStatsManager.QueryStatsForUid()

            UUID uuid = null;

            if (item.IsPrimary)
            {
                uuid = StorageManager.UuidDefault;
            }else if (!string.IsNullOrEmpty(item.Uuid))
            {
                // uuid = UUID.NameUUIDFromBytes(System.Text.Encoding.Default.GetBytes(item.Uuid));
                try
                {
                    uuid = UUID.FromString(item.Uuid);
                }
                catch (Exception)
                {

                }
            }

            if(uuid != null)
            {
                try
                {
                    var freeBytes = StatsManager.GetFreeBytes(uuid);
                    var totalBytes = StatsManager.GetTotalBytes(uuid);
                    var percent = (double)(totalBytes - freeBytes) / (double)totalBytes;

                    var progressBar = convertView.FindViewById<ProgressBar>(Resource.Id.progressBar1);
                    progressBar.SetProgress((int)(percent * 100), true);
                    if (percent < 0.9)
                    {
                        progressBar.ProgressDrawable.SetColorFilter(Color.ParseColor("#1565C0"), PorterDuff.Mode.SrcIn);
                    }
                    else
                    {
                        progressBar.ProgressDrawable.SetColorFilter(Color.ParseColor("#e53935"), PorterDuff.Mode.SrcIn);
                    }
                }
                catch { }
            }

            return convertView;
        }
    }
}