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

namespace Web_DevTools.utils.ListViewAdapters
{
    public class GenericListViewAdapter<T> : BaseAdapter<T>
    {

        public GenericListViewAdapter(Activity baseActivity, List<T> data)
        {
            this.BaseActivity = baseActivity;
            this.Data = data;
        }

        public List<T> Data { get; private set; }
        public Activity BaseActivity { get; private set; }
        public delegate View OnGetViewDelegate(Activity baseActivity, int position, T item, View convertView, ViewGroup parent);

        public event OnGetViewDelegate OnGetView;


        public override T this[int position] => Data[position];

        public override int Count => Data.Count();

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            return GetView(BaseActivity, position, Data[position], convertView, parent);
        }

        public virtual View GetView(Activity baseActivity, int position, T item, View convertView, ViewGroup parent)
        {
            if (OnGetView == null)
                throw new NotImplementedException("No handler for event \"OnGetView\"!");
            return OnGetView?.Invoke(baseActivity, position, item, convertView, parent);
        }
    }
}