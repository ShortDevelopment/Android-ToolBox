using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileExplorer
{

    public class FileListViewItem
    {
        public bool IsDirectory { get; set; } = false;
        public string Path { get; set; } = null;
        public Bitmap Thumbnail { get; set; } = null;
    }

    public class FileListViewAdapter : BaseAdapter<FileListViewItem>
    {

        List<FileListViewItem> data = null;
        Activity parent = null;

        public FileListViewAdapter(Activity parent, List<FileListViewItem> data)
        {
            this.data = data;
            this.parent = parent;
        }

        public override FileListViewItem this[int position] => this.data[position];

        public override int Count => this.data.Count();

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (convertView == null)
                convertView = this.parent.LayoutInflater.Inflate(Resource.Layout.file_listview_item, null);

            var item = this[position];

            convertView.FindViewById<TextView>(Resource.Id.textView1).Text = System.IO.Path.GetFileName(item.Path);

            var imageView = convertView.FindViewById<ImageView>(Resource.Id.imageView1);
            if (item.Thumbnail == null)
            {
                if (item.IsDirectory)
                {
                    imageView.SetImageResource(Resource.Drawable.baseline_folder_open_24);
                }
                else
                {
                    imageView.SetImageResource(Resource.Drawable.baseline_insert_drive_file_24);
                }
                imageView.SetScaleType(ImageView.ScaleType.CenterCrop);
            }
            else
            {
                imageView.SetImageBitmap(item.Thumbnail);
                imageView.SetScaleType(ImageView.ScaleType.Center);
            }

            return convertView;
        }
    }
}