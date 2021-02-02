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

namespace FileExplorer
{
    public abstract class BaseListViewAdapter<T> : BaseAdapter<T>
    {

        public List<T> data { get; }
        public Activity ParentActivity { get; }

        public BaseListViewAdapter(Activity parent, List<T> data)
        {
            this.data = data;
            this.ParentActivity = parent;
        }

        public override T this[int position] => this.data[position];

        public override int Count => this.data.Count();

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            return GetView(this[position], convertView, parent);
        }

        public abstract View GetView(T item, View convertView, ViewGroup parent);
    }
}